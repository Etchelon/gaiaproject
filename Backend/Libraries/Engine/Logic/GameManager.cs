using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GaiaProject.Common.Extensions;
using GaiaProject.Common.Reflection;
using GaiaProject.Core.Logic;
using GaiaProject.Engine.Commands;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Board;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;
using GaiaProject.Engine.Model.Players;
using Microsoft.Extensions.Logging;

namespace GaiaProject.Engine.Logic
{
	public class GameManager
	{
		private readonly ILogger<GameManager> _logger;
		private readonly IProvideGameData _gameDataProvider;
		private readonly UserManager _userManager;

		public GameManager(ILogger<GameManager> logger, IProvideGameData gameDataProvider, UserManager userManager)
		{
			_logger = logger;
			_gameDataProvider = gameDataProvider;
			_userManager = userManager;
		}

		public async Task<GaiaProjectGame[]> GetUserGames(string userId)
		{
			var games = await _gameDataProvider.GetUserGames(userId, true);
			return games;
		}

		public async Task<GaiaProjectGame[]> GetUserFinishedGames(string userId)
		{
			var games = await _gameDataProvider.GetUserGames(userId, false);
			return games.Where(g => g.Ended.HasValue).ToArray();
		}

		public async Task<(GaiaProjectGame[] games, bool hasMore)> GetAllGames(bool active, int page, int pageSize)
		{
			var games = await _gameDataProvider.GetAllGames(active, page, pageSize);
			var totalCount = await _gameDataProvider.CountAllGames(active);
			var nFetched = page * pageSize + games.Length;
			var hasMore = nFetched < totalCount;
			return (games, hasMore);
		}

		public async Task<GaiaProjectGame> GetGame(string id)
		{
			var game = await _gameDataProvider.GetGame(id);
			return game;
		}

		public async Task<GaiaProjectGame> GetGameAtAction(string id, int actionId, bool persist = false)
		{
			var currentState = await _gameDataProvider.GetGame(id);
			if (currentState.Ended.HasValue)
			{
				throw new Exception("You can only backtrack active games");
			}

			var currentActionId = 0;
			try
			{
				var initialState = await _gameDataProvider.GetInitialGameState(id);
				var ret = new GaiaProjectGame
				{
					Id = currentState.Id,
					Name = currentState.Name,
					CreatedBy = currentState.CreatedBy,
					Created = currentState.Created,
					Ended = null,
					Options = currentState.Options,
					CurrentPhaseId = GamePhase.Setup,
					BoardState = initialState.BoardState,
					Players = initialState.Players,
					Setup = initialState.Setup
				};

				var actions = currentState.Actions
					.Where(a => a.Id <= actionId)
					.OrderBy(a => a.Id)
					.ToList();
				foreach (var action in actions)
				{
					currentActionId = action.Id;
					ret = HandleAction(ret, action, false);
				}

				var nextAction = currentState.Actions.FirstOrDefault(a => a.Id == actionId + 1);
				// If the player has then passed
				if (nextAction?.Type == ActionType.PassTurn ||
					// Or if the next action is a conversion before passing the turn
					(nextAction?.Type == ActionType.Conversions && currentState.Actions.FirstOrDefault(a => a.Id == actionId + 2)?.PlayerId != nextAction!.PlayerId))
				{
					ret = HandleAction(ret, nextAction, false);
				}

				if (persist)
				{
					await _gameDataProvider.SaveGame(ret);
				}
				return ret;
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Error trying to rollback game {Id} to action {ActionId}", id, actionId);
				throw ex;
			}
		}

		private async Task<GaiaProjectGame> LoadGame(string id)
		{
			var game = await GetGame(id);
			if (!IsGameActive(game))
			{
				throw new Exception($"Game with id {id} is not active anymore.");
			}
			return game;
		}

		private (bool valid, string errorMessage) ValidateCreateGameCommand(CreateGameCommand command)
		{
			var valid = true;
			string errorMessage = null;
			var playerNo = command.PlayerIds?.Length ?? 0;
			if (playerNo == 0)
			{
				valid = false;
				errorMessage = "Solo mode not yet supported.";
			}
			if (playerNo > 3)
			{
				valid = false;
				errorMessage = "You can only play Gaia Project with at most 3 other players.";
			}
			return (valid, errorMessage);
		}

		public async Task<GaiaProjectGame> CreateGame(CreateGameCommand command, string createdBy)
		{
			var (valid, errorMessage) = ValidateCreateGameCommand(command);
			if (!valid)
			{
				throw new Exception($"Could not create a new game. Error: {errorMessage}");
			}

			var index = 0;
			var creatorUsername = await _userManager.GetUsername(createdBy);
			var players = new List<PlayerInGame>
			{
				new PlayerInGame
				{
					Id = createdBy,
					IsWinner = false,
					Username = creatorUsername,
					InitialOrder = ++index,
					Actions = new ActionState(),
					State = null
				}
			};
			foreach (var id in command.PlayerIds)
			{
				var username = await _userManager.GetUsername(id);
				var playerInGame = new PlayerInGame
				{
					Id = id,
					IsWinner = false,
					Username = username,
					InitialOrder = ++index,
					Actions = new ActionState(),
					State = null
				};
				players.Add(playerInGame);
			}

			if (command.Options.TurnOrderSelectionMode == TurnOrderSelectionMode.Random)
			{
				RandomizePlayersOrder(players);
			}
			if (command.Options.FactionSelectionMode == RaceSelectionMode.Random && !command.Options.Auction)
			{
				players = RandomizeRaces(players);
			}

			var boardRandomizer = new BoardRandomizer(players.Select(p => p.Id).ToArray());
			boardRandomizer.Randomize();
			var newGame = new GaiaProjectGame
			{
				Id = null,
				Name = command.Options.GameName,
				CreatedBy = createdBy,
				Created = DateTime.Now,
				CurrentPhaseId = GamePhase.Setup,
				BoardState = boardRandomizer.Board,
				Players = players.ToList(),
				Setup = Factory.InitialSetupPhase(players.First().Id, players, command.Options),
				Options = command.Options,
				GameLogs = new List<GameLog>()
			};
			newGame.LogSystemMessage(newGame.Setup.SubPhase.ToDescription(), true);

			var i = 0;
			foreach (var player in newGame.Players)
			{
				if (!player.RaceId.HasValue)
				{
					continue;
				}
				player.State = Factory.InitialPlayerState(player.RaceId.Value, ++i);
			}
			await _gameDataProvider.CreateGame(newGame);
			return newGame;
		}

		private void RandomizePlayersOrder(List<PlayerInGame> players)
		{
			var index = 0;
			players.Shuffle();
			players.ForEach(p => p.InitialOrder = ++index);
		}

		private List<PlayerInGame> RandomizeRaces(List<PlayerInGame> players)
		{
			return players
				.Zip(RaceUtils.RandomizeRaces(players.Count), (player, race) =>
				{
					player.RaceId = race;
					return player;
				})
				.ToList();
		}

		public async Task DeleteGame(string id)
		{
			var game = await GetGame(id);
			if (game.Ended.HasValue)
			{
				throw new Exception("You can only delete active games");
			}
			await _gameDataProvider.DeleteGame(id);
		}

		public async Task<HandleActionResult> HandleAction(string gameId, PlayerAction action)
		{
			var game = await LoadGame(gameId);
			try
			{
				var newState = HandleAction(game, action);
				await _gameDataProvider.SaveGame(newState);
				return HandleActionResult.Ok(newState);
			}
			catch (InvalidActionException ex)
			{
				return HandleActionResult.Failure(ex.Message);
			}
		}

		private GaiaProjectGame HandleAction(GaiaProjectGame game, PlayerAction action, bool doValidation = true)
		{
			try
			{
				// Generic validation
				if (game.ActivePlayerId != action.PlayerId)
				{
					throw new InvalidActionException("You are not the active player");
				}
				if (action.Type.HasAttributeOfType<RaceActionAttribute>())
				{
					var attr = action.Type.GetAttributeOfType<RaceActionAttribute>();
					var player = game.GetPlayer(action.PlayerId);
					if (attr.Race != player.RaceId)
					{
						throw new InvalidActionException($"You cannot take action {action.Type} as it is reserved for race {attr.Race}");
					}
				}

				HydrateActionId(action, game);
				var handler = GetActionHandler(action);
				handler.ToggleActionValidation(doValidation);
				var effects = handler.Handle(game, action);
				// Itars pick tiles in GaiaDecisions subphase, and they must not decide whether to pass or convert
				if (game.CurrentPhaseId == GamePhase.Rounds && game.Rounds.SubPhase != RoundSubPhase.Actions)
				{
					effects.RemoveAll(eff =>
						(eff as PendingDecisionEffect)?.Decision?.Type == PendingDecisionType.PerformConversionOrPassTurn);
				}
				var effectsApplier = new ActionEffectsApplier();
				var newState = effectsApplier.ApplyActionEffects(action, game, effects);
				PerformUpkeep(newState);
				newState.Actions.Add(action);
				return newState;
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Error trying to execute action {ActionId} in game {Id}", action.Id, game.Id);
				throw ex;
			}
		}

		private static void HydrateActionId(PlayerAction action, GaiaProjectGame game)
		{
			var actionId = game.Actions.Select(a => a.Id).DefaultIfEmpty(0).Max() + 1;
			action.Id = actionId;
		}

		private IHandleAction GetActionHandler(PlayerAction action)
		{
			var handlerType = GetHandlerType(action);
			var handler = Activator.CreateInstance(handlerType);
			return handler as IHandleAction;
		}

		private Type GetHandlerType(PlayerAction action)
		{
			var actionType = action.GetType();
			var assembly = GetType().Assembly;
			var handlerTypes = assembly
				.GetTypes()
				.Where(t =>
					(t.BaseType?.IsGenericType ?? false)
					&& typeof(ActionHandlerBase<>).IsAssignableFrom(t.BaseType.GetGenericTypeDefinition())
				);
			var handlerType = handlerTypes.FirstOrDefault(t =>
				t.BaseType.GenericTypeArguments.Length == 1
				&& t.BaseType.GenericTypeArguments[0] == actionType
			);
			return handlerType;
		}

		private void PerformUpkeep(GaiaProjectGame game)
		{
			if (game.CurrentPhaseId == GamePhase.Setup)
			{
				UpdateFinalScorings(game);
				return;
			}

			var roundsPhase = game.Rounds;
			var currentRound = roundsPhase.CurrentRound;
			Debug.Assert(roundsPhase != null, nameof(roundsPhase) + " != null");

			void GotoActions()
			{
				game.LogSystemMessage("Action Phase", true);
				roundsPhase.SubPhase = RoundSubPhase.Actions;
				game.Players.OrderBy(p => p.TurnOrder).First().Actions.MustTakeMainAction();
				// Player logs generated in the upkeep will be grouped by their own turn
				++game.CurrentTurn;
			}

			switch (roundsPhase.SubPhase)
			{
				default:
					throw new ArgumentOutOfRangeException($"Subphase {roundsPhase.SubPhase} not handled.");
				case RoundSubPhase.Income:
					{
						game.LogSystemMessage("Income Phase", true);
						var nDecisions = 0;
						game.Players.OrderBy(p => p.TurnOrder).ToList().ForEach(p =>
						{
							p.Actions.ResetForNewRound();

							var resourceIncomes = p.State.Incomes.OfType<ResourceIncome>();
							var resources = resourceIncomes.Aggregate(new Resources(), (acc, income) =>
							{
								acc.Credits += income.Credits;
								acc.Ores += income.Ores;
								acc.Knowledge += income.Knowledge;
								acc.Qic += income.Qic;
								return acc;
							});
							var resourceGain = new ResourcesGain(resources) { PlayerId = p.Id };
							p.State = ResourceUtils.ApplyGain(resourceGain,
								new ActionContext(new NullAction { PlayerId = p.Id }, game));
							game.LogGain(resourceGain);
							var powerIncomes = p.State.Incomes.OfType<PowerIncome>().ToList();
							var powerTokenIncomes = p.State.Incomes.OfType<PowerTokenIncome>().ToList();
							var mustDecideOrder = powerIncomes.Any() && powerTokenIncomes.Any();
							if (mustDecideOrder)
							{
								var decision = SortIncomesDecision
									.FromIncomes(powerIncomes, powerTokenIncomes)
									.ForPlayer(p.Id);
								p.Actions.MustTakeDecision(decision);
								++nDecisions;
							}
							else
							{
								if (powerIncomes.Any())
								{
									var power = powerIncomes.Aggregate(0, (acc, income) => acc + income.Power);
									var powerGain = new PowerGain(power) { PlayerId = p.Id };
									p.State = ResourceUtils.ApplyGain(powerGain,
										new ActionContext(new NullAction { PlayerId = p.Id }, game));
									game.LogGain(powerGain);
								}

								if (powerTokenIncomes.Any())
								{
									var powerTokens = powerTokenIncomes.Aggregate(0, (acc, income) => acc + income.PowerTokens);
									var powerTokensGain = new ResourcesGain(new Resources { PowerTokens = powerTokens }) { PlayerId = p.Id };
									p.State = ResourceUtils.ApplyGain(powerTokensGain,
										new ActionContext(new NullAction { PlayerId = p.Id }, game));
									game.LogGain(powerTokensGain);
								}
							}
						});

						if (nDecisions == 0)
						{
							roundsPhase.SubPhase = RoundSubPhase.Gaia;
							PerformUpkeep(game);
						}
						else
						{
							roundsPhase.SubPhase = RoundSubPhase.IncomeDecisions;
						}
						return;
					}
				case RoundSubPhase.IncomeDecisions:
					{
						if (game.PendingDecisions.Any())
						{
							return;
						}
						roundsPhase.SubPhase = RoundSubPhase.Gaia;
						PerformUpkeep(game);
						return;
					}
				case RoundSubPhase.Gaia:
					{
						if (currentRound == 1)
						{
							GotoActions();
							return;
						}

						game.LogSystemMessage("Gaia Phase", true);
						// Turn transdim planets to Gaia planets
						game.BoardState.Map.Hexes
							.WithGaiaformer()
							.Gaiaformed(false)
							.ToList()
							.ForEach(h =>
							{
								h.WasGaiaformed = true;
								game.LogSystemMessage(
									$"Transdim planet in hex {h.Index} (sector {h.SectorNumber + 1}) becomes a Gaia planet");
							});

						// Return power from Gaia Area
						var nDecisions = 0;
						game.Players.OrderBy(p => p.TurnOrder).ToList().ForEach(p =>
						{
							if (p.RaceId == Race.BalTaks)
							{
								var gaiaformersInGaiaArea = p.State.Gaiaformers
									.Where(gf => gf.SpentInGaiaArea)
									.ToList();
								gaiaformersInGaiaArea.ForEach(gf =>
								{
									gf.SpentInGaiaArea = false;
									gf.Available = true;
								});
								if (gaiaformersInGaiaArea.Any())
								{
									game.LogPlayerMessage(p, $"returns {gaiaformersInGaiaArea.Count} gaiaformers from Gaia Area");
								}
							}

							var powerPools = p.State.Resources.Power;
							if (powerPools.GaiaArea == 0)
							{
								return;
							}

							var powerInGaiaArea = powerPools.GaiaArea;
							var hasBuiltPlanetaryInstitute = p.State.Buildings.PlanetaryInstitute;
							if (p.RaceId == Race.Terrans)
							{
								powerPools.Bowl2 += powerInGaiaArea;
								powerPools.GaiaArea = 0;
								game.LogPowerReturnsInGaiaPhase(p, powerInGaiaArea, false);

								if (hasBuiltPlanetaryInstitute)
								{
									var decision = new TerransDecideIncomeDecision(powerInGaiaArea).ForPlayer(p.Id);
									p.Actions.MustTakeDecision(decision);
									++nDecisions;
								}
								return;
							}

							if (p.RaceId == Race.Itars && hasBuiltPlanetaryInstitute && powerInGaiaArea >= 4)
							{
								var decision = new ItarsBurnPowerForTechnologyTileDecision().ForPlayer(p.Id);
								++nDecisions;
								p.Actions.MustTakeDecision(decision);
								return;
							}

							powerPools.Bowl1 += powerInGaiaArea;
							powerPools.GaiaArea = 0;
							var hasMovedBrainstone = false;
							if (p.RaceId == Race.Taklons && powerPools.Brainstone == PowerPools.BrainstoneLocation.GaiaArea)
							{
								powerPools.Brainstone = PowerPools.BrainstoneLocation.Bowl1;
								hasMovedBrainstone = true;
							}
							game.LogPowerReturnsInGaiaPhase(p, powerInGaiaArea, hasMovedBrainstone);
						});

						if (nDecisions == 0)
						{
							GotoActions();
						}
						else
						{
							roundsPhase.SubPhase = RoundSubPhase.IncomeDecisions;
						}
						return;
					}
				case RoundSubPhase.GaiaDecisions:
					{
						if (game.PendingDecisions.Any())
						{
							return;
						}
						GotoActions();
						return;
					}
				case RoundSubPhase.Actions:
					{
						UpdateFinalScorings(game);
						if (!game.Players.All(p => p.Actions.HasPassed))
						{
							return;
						}

						if (currentRound == 6)
						{
							PerformFinalScoring(game);
							return;
						}

						game.Players.ForEach(p =>
						{
							// Untap all technology tiles
							p.State.StandardTechnologyTiles.ForEach(st => st.Used = false);
							p.State.AdvancedTechnologyTiles.ForEach(at => at.Used = false);

							var newTurnOrder = currentRound == 0
								? p.InitialOrder
								: p.State.NextRoundTurnOrder ?? throw new Exception(
									$"Next round order for player {p.Username} should be determined during upkeep.");
							p.State.CurrentRoundTurnOrder = newTurnOrder;
							p.State.NextRoundTurnOrder = null;
						});

						// Untap all special actions
						foreach (var powerAction in game.BoardState.ResearchBoard.PowerActions)
						{
							powerAction.IsAvailable = true;
						}
						foreach (var qicAction in game.BoardState.ResearchBoard.QicActions)
						{
							qicAction.IsAvailable = true;
						}

						++roundsPhase.CurrentRound;
						roundsPhase.SubPhase = RoundSubPhase.Income;
						PerformUpkeep(game);
						return;
					}
			}
		}

		private void UpdateFinalScorings(GaiaProjectGame game)
		{
			int CountElements(FinalScoringTileType type, PlayerInGame player) => type switch
			{
				FinalScoringTileType.BuildingsInAFederation =>
					player.State.Federations.Select(fed => fed.NumBuildings).Sum(),
				FinalScoringTileType.BuildingsOnTheMap =>
					player.State.Buildings.Mines +
					player.State.Buildings.TradingStations +
					player.State.Buildings.ResearchLabs +
					(player.State.Buildings.PlanetaryInstitute ? 1 : 0) +
					(player.State.Buildings.AcademyLeft ? 1 : 0) +
					(player.State.Buildings.AcademyRight ? 1 : 0) +
					(player.State.Buildings.HasLostPlanet ? 1 : 0),
				FinalScoringTileType.GaiaPlanets => player.State.GaiaPlanets,
				FinalScoringTileType.KnownPlanetTypes => player.State.KnownPlanetTypes.Count,
				FinalScoringTileType.Satellites => player.State.Buildings.Satellites + player.State.Buildings.IvitsSpaceStations,
				FinalScoringTileType.Sectors => player.State.ColonizedSectors.Count,
				_ => throw new ArgumentOutOfRangeException("Final scoring type")
			};

			void ProcessFinalScoring(FinalScoringState finalScoring)
			{
				var scoringType = finalScoring.Id;
				var playersAndCounts = game.Players
					.Select(p => p.State != null
						? new { PlayerId = p.Id, Count = CountElements(scoringType, p) }
						: new { PlayerId = p.Id, Count = 0 }
					)
					.ToList();
				var sortedPlayers = playersAndCounts.OrderByDescending(o => o.Count).ToList();
				var points = new List<int> { 18, 12, 6, 0 };
				var players = new List<FinalScoringState.PlayerState>();
				sortedPlayers.ForEach(o =>
				{
					if (players.Any(p => p.PlayerId == o.PlayerId))
					{
						return;
					}
					var tiedPlayers = sortedPlayers.Where(p => p.Count == o.Count);
					var tiedPlayersCount = tiedPlayers.Count();
					var pointsToDistribute = points.GetRange(0, tiedPlayersCount).Sum();
					var pointsForEach = (int)Math.Floor((double)pointsToDistribute / tiedPlayersCount);
					players.AddRange(tiedPlayers.Select(tp => new FinalScoringState.PlayerState
					{
						PlayerId = tp.PlayerId,
						Count = tp.Count,
						Points = pointsForEach
					}));
					points = points.GetRange(tiedPlayersCount, points.Count - tiedPlayersCount);
				});
				finalScoring.Players = players;
			}

			ProcessFinalScoring(game.BoardState.ScoringBoard.FinalScoring1);
			ProcessFinalScoring(game.BoardState.ScoringBoard.FinalScoring2);
		}

		private void PerformFinalScoring(GaiaProjectGame game)
		{
			game.LogSystemMessage("Final Scoring", true);

			game.LogSystemMessage($"Objective 1: {game.BoardState.ScoringBoard.FinalScoring1.Id.ToDescription()}");
			var (firstScoringPlayers, secondScoringPlayers) = PointUtils.ProcessFinalScorings(game);
			firstScoringPlayers.OrderByDescending(ps => ps.Points).ToList().ForEach(ps =>
			{
				if (ps.PlayerId == PointUtils.GhostPlayer)
				{
					return;
				}
				var player = game.GetPlayer(ps.PlayerId);
				player.State.Points += ps.Points;
				game.LogPlayerMessage(player, $"gains {ps.Points}");
			});

			game.LogSystemMessage($"Objective 2: {game.BoardState.ScoringBoard.FinalScoring2.Id.ToDescription()}");
			secondScoringPlayers.OrderByDescending(ps => ps.Points).ToList().ForEach(ps =>
			{
				if (ps.PlayerId == PointUtils.GhostPlayer)
				{
					return;
				}
				var player = game.GetPlayer(ps.PlayerId);
				player.State.Points += ps.Points;
				game.LogPlayerMessage(player, $"gains {ps.Points}");
			});

			game.LogSystemMessage("Technology Tracks");
			const int threshold = 2;
			const int pointsPerStep = 4;
			const int creditsPerPoint = 3;
			game.Players.OrderBy(p => p.TurnOrder).ToList().ForEach(p =>
			{
				var nStepsAbove2 = p.State.ResearchAdvancements
					.Select(adv => Math.Max(0, adv.Steps - threshold))
					.Sum();
				var points = nStepsAbove2 * pointsPerStep;
				p.State.Points += points;
				game.LogPlayerMessage(p, $"gains {points} points for {nStepsAbove2} steps above level 2");
			});

			game.LogSystemMessage("Remaining resources");
			game.Players.OrderBy(p => p.TurnOrder).ToList().ForEach(p =>
			{
				var resources = p.State.Resources;
				var powerPools = resources.Power;
				while (powerPools.Bowl2 >= 2)
				{
					powerPools.Bowl2 -= 2;
					powerPools.Bowl3 += 1;
				}
				// If, after burning, brainstone is in Bowl2 with another power, move it to bowl 3
				if (p.RaceId == Race.Taklons && powerPools.Brainstone == PowerPools.BrainstoneLocation.Bowl2 && powerPools.Bowl2 == 1)
				{
					powerPools.Bowl3 += 3;
				}

				var leftOverCredits = resources.Credits + resources.Ores + resources.Knowledge + resources.Qic +
									  powerPools.Bowl3;
				var points = (int)Math.Floor((double)leftOverCredits / creditsPerPoint);
				p.State.Points += points;
				game.LogPlayerMessage(p, $"gains {points} points for leftover resources");
			});

			game.LogSystemMessage("Auction costs");
			if (game.Options.Auction)
			{
				game.Players.OrderBy(p => p.TurnOrder).ToList().ForEach(p =>
				{
					var auctionPoints = p.State.AuctionPoints.Value;
					if (auctionPoints == 0)
					{
						return;
					}
					p.State.Points -= auctionPoints;
					game.LogPlayerMessage(p, $"loses {auctionPoints} points paid in the auction");
				});
			}

			PointUtils.CalculatePlayersPlacement(game.Players);
			game.Ended = DateTime.Now;
		}

		private bool IsGameActive(GaiaProjectGame game)
		{
			return true;
		}

		public async Task<string> GetPlayerNotes(string playerId, string gameId)
		{
			return await _gameDataProvider.GetPlayerNotes(playerId, gameId);
		}

		public async Task SavePlayerNotes(string playerId, string gameId, string notes)
		{
			await _gameDataProvider.SavePlayerNotes(playerId, gameId, notes);
		}
	}
}
