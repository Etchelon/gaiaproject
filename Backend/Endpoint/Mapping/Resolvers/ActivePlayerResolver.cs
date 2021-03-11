using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Logic.Board.Map;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Decisions;
using GaiaProject.Engine.Model.Players;
using GaiaProject.ViewModels.AvailableActions;
using GaiaProject.ViewModels.Decisions;
using GaiaProject.ViewModels.Players;
using Newtonsoft.Json;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class ActivePlayerResolver
	{
		private readonly List<PowerActionType> PowerActionsForColonization = new List<PowerActionType> { PowerActionType.Gain1TerraformingStep, PowerActionType.Gain2TerraformingSteps };

		// Id of the player that we're mapping the ViewModel for
		public ActivePlayerInfoViewModel Resolve(GaiaProjectGame game, string requestPlayerId)
		{
			if (game.Ended.HasValue)
			{
				return null;
			}

			// If there are players that must take a pending decision, the active player is the first of them in turn order.
			// The turn order has already been calculated when creating the decision
			var decision = game.PendingDecisions.FirstOrDefault();
			if (decision != null)
			{
				var player = game.Players.Single(p => p.Id == decision.PlayerId);
				return new ActivePlayerInfoViewModel
				{
					Id = decision.PlayerId,
					Username = player.Username,
					RaceId = player.RaceId,
					Reason = GetReason(player, decision),
					AvailableActions = new List<AvailableActionViewModel>(),
					PendingDecision = player.Id == requestPlayerId
						? GetPendingDecision(player, decision, game)
						: null
				};
			}

			// Otherwise the current player is the active one
			var currentPlayer = game.CurrentPlayer;
			if (currentPlayer == null)
			{
				return null;
			}

			var availableActions = GetAvailableActions(currentPlayer, game);
			return new ActivePlayerInfoViewModel
			{
				Id = currentPlayer.Id,
				Username = currentPlayer.Username,
				RaceId = currentPlayer.RaceId,
				Reason = GetReason(currentPlayer, availableActions),
				AvailableActions = currentPlayer.Id == requestPlayerId
					? availableActions
					: new List<AvailableActionViewModel>()
			};
		}

		private string GetReason(PlayerInGame player, PendingDecision decision)
		{
			var raceName = player.RaceId.HasValue
				? Enum.GetName(typeof(Race), player.RaceId.Value)
				: null;
			return raceName != null
				? $"{player.Username} ({raceName}) {decision.Description}"
				: $"{player.Username} {decision.Description}";
		}

		private string GetReason(PlayerInGame player, List<AvailableActionViewModel> availableActions)
		{
			var description = availableActions.Count > 1
				? "should take an action"
				: availableActions.Single().Description;
			var raceName = player.RaceId.HasValue
				? Enum.GetName(typeof(Race), player.RaceId.Value)
				: null;
			return raceName != null
				? $"{player.Username} ({raceName}) {description}"
				: $"{player.Username} {description}";
		}

		private List<AvailableActionViewModel> GetAvailableActions(PlayerInGame player, GaiaProjectGame game)
		{
			var mapService = new MapService(game.BoardState.Map);
			// Special case: player must take main action
			if (!player.Actions.PossibleActions.Any())
			{
				Debug.Assert(!player.Actions.HasPerformedMainAction, "Player has already performed the main action and, if active, should have an action to perform.");

				var availableActions = new List<AvailableActionViewModel>
				{
					new AvailableActionViewModel
					{
						Type = ActionType.Pass,
						Description = "Pass",
						InteractionState = new InteractionStateViewModel
						{
							ClickableRoundBoosters = game.BoardState.RoundBoosters.AvailableRoundBooster
								.Where(b => !b.IsTaken)
								.Select(b => b.Id)
								.ToList()
						}
					},
				};

				// Get all reachable hexes for build and upgrade actions
				var hasAvailableMines = player.State.Buildings.Mines < BuildingUtils.NumMines;
				var colonizableHexesInfo = mapService.GetHexesColonizableBy(player);
				var colonizableHexes = colonizableHexesInfo.Select(o => o.hex).ToList();
				var colonizableTransdimPlanets = colonizableHexes.OfType(PlanetType.Transdim).Empty().Gaiaformed(false).ToArray();
				var colonizableNonTransdimPlanets = colonizableHexes.Except(colonizableTransdimPlanets).ToArray();
				if (hasAvailableMines && colonizableNonTransdimPlanets.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.ColonizePlanet,
						Description = "Colonize",
						InteractionState = new InteractionStateViewModel
						{
							ClickableHexes = colonizableNonTransdimPlanets.Select(h =>
							{
								var id = h.Id;
								var requiredQics = colonizableHexesInfo.Single(o => o.hex.Id == id).requiredQics;
								return new InteractionStateViewModel.ColonizableHexViewModel { Id = id, RequiredQics = requiredQics };
							}).ToList()
						}
					});
				}

				var hasAvailableGaiaformers = player.State.Gaiaformers.Any(gf => gf.Unlocked && gf.Available);
				if (hasAvailableGaiaformers && colonizableTransdimPlanets.Any() && GaiaformingUtils.CanStartGaiaProject(player, game))
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.StartGaiaProject,
						Description = "Gaia project",
						InteractionState = new InteractionStateViewModel
						{
							ClickableHexes = colonizableTransdimPlanets.Select(h =>
							{
								var id = h.Id;
								var requiredQics = colonizableHexesInfo.Single(o => o.hex.Id == id).requiredQics;
								return new InteractionStateViewModel.ColonizableHexViewModel { Id = id, RequiredQics = requiredQics };
							}).ToList()
						}
					});
				}

				var playersHexes = mapService.GetPlayersHexes(player.Id, false, player.RaceId != Race.Lantids);
				var hexesWithUpgradableBuildings = playersHexes
					.Except(playersHexes.WithBuildingType(BuildingType.PlanetaryInstitute))
					.Except(playersHexes.WithBuildingType(BuildingType.AcademyLeft))
					.Except(playersHexes.WithBuildingType(BuildingType.AcademyRight))
					.Except(playersHexes.WithBuildingType(BuildingType.LostPlanet))
					.Except(playersHexes.WithBuildingType(BuildingType.IvitsSpaceStation))
					.Except(playersHexes.WithBuildingType(BuildingType.Satellite))
					.Select(h => h.Id).ToList();
				if (hexesWithUpgradableBuildings.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.UpgradeExistingStructure,
						Description = "Upgrade",
						InteractionState = new InteractionStateViewModel
						{
							ClickableHexes = hexesWithUpgradableBuildings.Select(id => new InteractionStateViewModel.ColonizableHexViewModel { Id = id }).ToList(),
						}
					});
				}

				var researchableTechnologies = player.State.ResearchAdvancements
					.Where(rad =>
					{
						var canPayResearchTechnologyCost = ResourceUtils.CanPayCost(
							new ResourcesCost(new Resources { Knowledge = 4 }),
							new ActionContext(new NullAction { PlayerId = player.Id }, game), out _);
						return canPayResearchTechnologyCost && ResearchUtils.CanPlayerAdvanceToLevel(rad.Steps + 1, rad.Track, player.Id, game).can;
					})
					.Select(rad => new InteractionStateViewModel.ResearcheableTechnologyViewModel { Track = rad.Track, NextStep = rad.Steps + 1 })
					.ToList();
				if (researchableTechnologies.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.ResearchTechnology,
						Description = "Research",
						InteractionState = new InteractionStateViewModel
						{
							ClickableResearchTracks = researchableTechnologies,
						}
					});
				}

				var availablePowerActions = game.BoardState.ResearchBoard.PowerActions.Where(pa => pa.IsAvailable);
				// If the player has no mines left or cannot colonize any planet, remove the two actions that give terraformation steps
				if (!hasAvailableMines || !colonizableNonTransdimPlanets.Any())
				{
					availablePowerActions = availablePowerActions.Where(o => !PowerActionsForColonization.Contains(o.Type));
				}

				var usablePowerActions = availablePowerActions
					.Select(pa =>
					{
						var powerCost = pa.Type.GetAttributeOfType<ActionCostAttribute>().Cost;
						var (canPay, toBurn) = ResourceUtils.CanPayPowerCost(powerCost, new ActionContext(new NullAction { PlayerId = player.Id }, game));
						return new { Type = pa.Type, CanPay = canPay, ToBurn = toBurn };
					})
					.Where(o => o.CanPay)
					.ToList();
				if (usablePowerActions.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.Power,
						Description = "Power action",
						InteractionState = new InteractionStateViewModel
						{
							ClickablePowerActions = usablePowerActions.Select(o => new InteractionStateViewModel.UsablePowerActionViewModel
							{
								Type = o.Type,
								PowerToBurn = o.ToBurn,
							})
							.ToList(),
						}
					});
				}

				var usableQicActions = game.BoardState.ResearchBoard.QicActions
					.Where(qa =>
					{
						if (!qa.IsAvailable)
						{
							return false;
						}

						var qicCost = qa.Type.GetAttributeOfType<ActionCostAttribute>().Cost;
						var canPay = ResourceUtils.CanPayCost(
							new ResourcesCost(new Resources { Qic = qicCost }),
							new ActionContext(new NullAction { PlayerId = player.Id }, game), out _
						);
						return canPay;
					})
					.Select(qa => qa.Type)
					.ToList();
				if (usableQicActions.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.Qic,
						Description = "Qic action",
						InteractionState = new InteractionStateViewModel
						{
							ClickableQicActions = usableQicActions,
						}
					});
				}

				var usableOwnStandardTiles = player.State.StandardTechnologyTiles
					.Where(st => st.HasAction && !st.CoveredByAdvancedTile && !st.Used)
					.Select(st => st.Id)
					.ToList();
				var usableOwnAdvancedTiles = player.State.AdvancedTechnologyTiles
					.Where(at => at.HasAction && !at.Used)
					.Select(at => at.Id)
					.ToList();
				if (usableOwnStandardTiles.Any() || usableOwnAdvancedTiles.Any())
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.UseTechnologyTile,
						Description = "Use Tech Tile",
						InteractionState = new InteractionStateViewModel
						{
							ClickableOwnStandardTiles = usableOwnStandardTiles,
							ClickableOwnAdvancedTiles = usableOwnAdvancedTiles
						}
					});
				}

				var canUseOwnRoundBooster = player.State.RoundBooster.HasAction && !player.State.RoundBooster.Used;
				if (player.State.RoundBooster.Id == RoundBoosterType.TerraformActionGainCredits)
				{
					canUseOwnRoundBooster = canUseOwnRoundBooster && hasAvailableMines;
				}
				if (player.State.RoundBooster.Id == RoundBoosterType.BoostRangeGainPower)
				{
					canUseOwnRoundBooster = canUseOwnRoundBooster && (hasAvailableMines || hasAvailableGaiaformers);
				}

				if (canUseOwnRoundBooster)
				{
					availableActions.Add(new AvailableActionViewModel
					{
						Type = ActionType.UseRoundBooster,
						Description = "Use Round Booster",
						InteractionState = new InteractionStateViewModel
						{
							CanUseOwnRoundBooster = true
						}
					});
				}

				var canUsePlanetaryInstitute = RaceUtils.GetPlanetaryInstituteActionInfo(player) != null
											   && player.State.Buildings.PlanetaryInstitute
											   && !player.Actions.HasUsedPlanetaryInstitute;
				var canUseRightAcademy = player.State.Buildings.AcademyRight && !player.Actions.HasUsedRightAcademy;
				var canUseRaceAction = RaceUtils.GetRaceActionInfo(player) != null && !player.Actions.HasUsedRaceAction;

				if (canUsePlanetaryInstitute)
				{
					var actionInfo = RaceUtils.GetPlanetaryInstituteActionInfo(player);
					var clickableHexes = player.RaceId switch
					{
						Race.Ambas => mapService.GetPlayersHexes(player.Id, false, false).WithBuildingType(BuildingType.Mine).Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList(),
						Race.Firaks => mapService.GetPlayersHexes(player.Id, false, false).WithBuildingType(BuildingType.ResearchLab).Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList(),
						Race.Ivits => mapService.GetHexesReachableBy(player)
							.Where(o => !o.hex.PlanetType.HasValue)
							.Select(o => new InteractionStateViewModel.ColonizableHexViewModel { Id = o.hex.Id, RequiredQics = o.requiredQics })
							.ToList(),
						_ => new List<InteractionStateViewModel.ColonizableHexViewModel>()
					};
					if (clickableHexes.Any())
					{
						availableActions.Add(new AvailableActionViewModel
						{
							Type = actionInfo.Id,
							Description = $"PI - {actionInfo.Name}",
							InteractionState = new InteractionStateViewModel
							{
								ClickableHexes = clickableHexes,
								CanUsePlanetaryInstitute = true
							}
						});
					}
				}
				if (canUseRightAcademy)
				{
					var actionInfo = RaceUtils.GetRightAcademyActionInfo(player);
					availableActions.Add(new AvailableActionViewModel
					{
						Type = actionInfo.Id,
						Description = $"AC - {actionInfo.Name}",
						InteractionState = new InteractionStateViewModel
						{
							CanUseRightAcademy = true
						}
					});
				}
				if (canUseRaceAction)
				{
					var actionInfo = RaceUtils.GetRaceActionInfo(player);
					availableActions.Add(new AvailableActionViewModel
					{
						Type = actionInfo.Id,
						Description = $"{player.RaceId.Value} - {actionInfo.Name}",
						InteractionState = new InteractionStateViewModel
						{
							CanUseRaceAction = true
						}
					});
				}

				var formFederationAction = GetFormFederationAction(player, mapService);
				if (formFederationAction != null)
				{
					availableActions.Add(formFederationAction);
				}

				availableActions.Add(new AvailableActionViewModel
				{
					Type = ActionType.Conversions,
					Description = "Conversions",
				});
				return availableActions;
			}

			return player.Actions.PossibleActions.Select(type =>
			{
				switch (type)
				{
					default:
						throw new ArgumentOutOfRangeException(nameof(type), $"Action type {type} not handled.");
					case ActionType.ColonizePlanet:
						{
							Debug.Assert(
								player.State.TempTerraformationSteps.HasValue || player.State.RangeBoost.HasValue,
								"A player is mandated to colonize a planet only after obtaining temp boost or terraforming steps from either a Round Booster or a Power Action"
							);

							var playersPlanetType = RaceUtils.GetRacePlanetType(player.RaceId.Value);
							var colonizableHexesInfo = mapService.GetHexesColonizableBy(player);
							IEnumerable<Hex> colonizableHexes = colonizableHexesInfo.Select(o => o.hex);
							colonizableHexes = colonizableHexes
								.Except(colonizableHexes.OfType(PlanetType.Transdim).Gaiaformed(false));
							if (player.State.TempTerraformationSteps.HasValue)
							{
								colonizableHexes = colonizableHexes.Except(colonizableHexes.Gaia());
							}

							return new AvailableActionViewModel
							{
								Type = ActionType.ColonizePlanet,
								Description = "Colonize",
								InteractionState = new InteractionStateViewModel
								{
									ClickableHexes = colonizableHexes.Select(h =>
									{
										var id = h.Id;
										var requiredQics = colonizableHexesInfo.Single(o => o.hex.Id == id).requiredQics;
										return new InteractionStateViewModel.ColonizableHexViewModel { Id = id, RequiredQics = requiredQics };
									}).ToList()
								}
							};
						}
					case ActionType.StartGaiaProject:
						{
							Debug.Assert(player.State.RangeBoost.HasValue,
								"A player is mandated to colonize a planet OR start a gaia project only after obtaining temp boost"
							);
							return GetStartGaiaProjectAction(player, game, mapService);
						}
					case ActionType.PlaceInitialStructure:
						{
							Debug.Assert(player.RaceId != null, "player.RaceId != null");
							var racePlanetType = RaceUtils.GetRacePlanetType(player.RaceId.Value);
							return new AvailableActionViewModel
							{
								Type = type,
								Description = player.RaceId == Race.Ivits ? "must place the Planetary Institute" : "must place a mine",
								InteractionState = new InteractionStateViewModel
								{
									ClickableHexes = mapService.GetColonizableHexesOfType(racePlanetType).Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList()
								}
							};
						}
					case ActionType.SelectRace:
						{
							var allRaces = new List<Race>();
							var existingRaces = Enum.GetValues(typeof(Race));
							var enumerator = existingRaces.GetEnumerator();
							while (enumerator.MoveNext())
							{
								allRaces.Add((Race)enumerator.Current);
							}

							allRaces.Remove(Race.None);
							var takenRaces = game.Options.Auction
								? game.Setup.AuctionState.AvailableRaces
								: game.Players
									.Select(p => p.RaceId ?? Race.None)
									.Where(r => r != Race.None)
									.ToList();
							var availableRaces = allRaces
								.Except(allRaces.Where(r =>
									takenRaces.Any(tr => RaceUtils.GetColor(tr) == RaceUtils.GetColor(r))))
								.ToList();
							return new AvailableActionViewModel
							{
								Type = type,
								Description = "must select the race",
								InteractionState = new InteractionStateViewModel
								{
									AvailableRaces = availableRaces
								}
							};
						}
					case ActionType.BidForRace:
						{
							return new AvailableActionViewModel
							{
								Type = type,
								Description = "must bid for a race",
								InteractionState = new InteractionStateViewModel()
							};
						}
					case ActionType.SelectStartingRoundBooster:
						{
							var availableRoundBoosters = game.BoardState.RoundBoosters
								.AvailableRoundBooster
								.Where(rb => !rb.IsTaken)
								.Select(rb => rb.Id)
								.ToList();
							return new AvailableActionViewModel
							{
								Type = type,
								Description = "must select the starting round booster",
								InteractionState = new InteractionStateViewModel
								{
									ClickableRoundBoosters = availableRoundBoosters
								}
							};
						}
				}
			})
			.Where(action => action != null)
			.ToList();
		}

		private PendingDecisionViewModel GetPendingDecision(PlayerInGame player, PendingDecision decision, GaiaProjectGame game)
		{
			switch (decision)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(decision.Type), $"Decision type {decision.Type} not handled.");
				case PerformConversionOrPassTurnDecision cop:
					return new PerformConversionOrPassTurnDecisionViewModel();
				case ChargePowerDecision cp:
					return new ChargePowerDecisionViewModel(cp.Amount);
				case AcceptOrDeclineLastStepDecision als:
					return new AcceptOrDeclineLastStepDecisionViewModel(als.Track);
				case PlaceLostPlanetDecision plp:
					{
						var mapService = new MapService(game.BoardState.Map);
						var reachableHexesInfo = mapService.GetHexesReachableBy(player);
						var emptySpaceHexes = reachableHexesInfo.Select(o => o.hex)
							.Space()
							.Empty();
						return new PlaceLostPlanetDecisionViewModel
						{
							InteractionState = new InteractionStateViewModel
							{
								ClickableHexes = emptySpaceHexes.Select(h =>
								{
									var id = h.Id;
									var requiredQics = reachableHexesInfo.Single(o => o.hex.Id == id).requiredQics;
									return new InteractionStateViewModel.ColonizableHexViewModel { Id = id, RequiredQics = requiredQics };
								}).ToList()
							}
						};
					}
				case TerransDecideIncomeDecision cpfg:
					return new TerransConvertPowerFromGaiaDecisionViewModel(cpfg.Power);
				case ItarsBurnPowerForTechnologyTileDecision bpftt:
					return new ItarsBurnPowerForTechnologyTileDecisionViewModel();
				case SortIncomesDecision si:
					return new SortIncomesDecisionViewModel(si.PowerIncomes, si.PowerTokenIncomes);
				case ChooseTechnologyTileDecision chooseTechTile:
					{
						var interaction = new InteractionStateViewModel();
						var selectableStandardTiles = new List<StandardTechnologyTileType>();
						foreach (var standardTileType in Enum.GetValues(typeof(StandardTechnologyTileType)))
						{
							var type = (StandardTechnologyTileType)standardTileType;
							if (player.State.StandardTechnologyTiles.Any(t => t.Id == type))
							{
								continue;
							}
							selectableStandardTiles.Add(type);
						}

						interaction.ClickableStandardTiles = selectableStandardTiles;
						var hasFederationTokensToSpend = player.State.FederationTokens.Any(f => !f.UsedForTechOrAdvancedTile);
						var hasStandardTilesToCover = player.State.StandardTechnologyTiles.Any(f => !f.CoveredByAdvancedTile);
						if (hasFederationTokensToSpend && hasStandardTilesToCover)
						{
							var technologiesAtLevel4Or5 = player.State.ResearchAdvancements.Where(adv => adv.Steps >= 4);
							var selectableAdvancedTiles = technologiesAtLevel4Or5
								.Select(tech => game.BoardState.ResearchBoard.Tracks.Single(t => t.Id == tech.Track))
								.Where(track => track.IsAdvancedTileAvailable)
								.Select(track => track.AdvancedTileType)
								.ToList();
							interaction.ClickableAdvancedTiles = selectableAdvancedTiles;
						}
						return new ChooseTechnologyTileDecisionViewModel
						{
							InteractionState = interaction
						};
					}
				case FreeTechnologyStepDecision freeTech:
					{
						IEnumerable<ResearchAdvancement> researchableTechnologies = player.State.ResearchAdvancements;
						if (freeTech.ResearchableTechnologies?.Any() ?? false)
						{
							researchableTechnologies = researchableTechnologies.Where(adv => freeTech.ResearchableTechnologies.Contains(adv.Track));
						}
						var clickableResearchTracks = researchableTechnologies
							.Where(adv =>
								ResearchUtils.CanPlayerAdvanceToLevel(adv.Steps + 1, adv.Track, player.Id, game).can)
							.Select(adv => new InteractionStateViewModel.ResearcheableTechnologyViewModel
							{
								Track = adv.Track,
								NextStep = adv.Steps + 1
							})
							.ToList();
						return new FreeTechnologyStepDecisionViewModel
						{
							InteractionState = new InteractionStateViewModel
							{
								ClickableResearchTracks = clickableResearchTracks
							}
						};
					}
				case SelectFederationTokenToScoreDecision reScoreDecision:
					{
						return new SelectFederationTokenToScoreDecisionViewModel
						{
							InteractionState = new InteractionStateViewModel
							{
								ClickableOwnFederations = reScoreDecision.AvailableTokens.ToList()
							}
						};
					}
				case TaklonsLeechDecision leechDecision:
					{
						return new TaklonsLeechDecisionViewModel(leechDecision.ChargeablePowerBeforeToken, leechDecision.ChargeablePowerAfterToken);
					}
			}
		}

		private AvailableActionViewModel GetStartGaiaProjectAction(PlayerInGame player, GaiaProjectGame game, MapService mapService)
		{
			var colonizableHexesInfo = mapService.GetHexesColonizableBy(player);
			var colonizableHexes = colonizableHexesInfo.Select(o => o.hex);
			var colonizableTransdimPlanets = colonizableHexes.OfType(PlanetType.Transdim).Empty().Gaiaformed(false).ToArray();
			var hasAvailableGaiaformers = player.State.Gaiaformers.Any(gf => gf.Unlocked && gf.Available);
			if (hasAvailableGaiaformers && colonizableTransdimPlanets.Any() && GaiaformingUtils.CanStartGaiaProject(player, game))
			{
				return new AvailableActionViewModel
				{
					Type = ActionType.StartGaiaProject,
					Description = "Gaia project",
					InteractionState = new InteractionStateViewModel
					{
						ClickableHexes = colonizableTransdimPlanets.Select(h =>
						{
							var id = h.Id;
							var requiredQics = colonizableHexesInfo.Single(o => o.hex.Id == id).requiredQics;
							return new InteractionStateViewModel.ColonizableHexViewModel { Id = id, RequiredQics = requiredQics };
						}).ToList()
					}
				};
			}

			return null;
		}

		private AvailableActionViewModel GetFormFederationAction(PlayerInGame player, MapService mapService)
		{
			var powerRequiredForFederation = player.RaceId == Race.Xenos && player.State.Buildings.PlanetaryInstitute
				? 6
				: 7;

			var playersHexes = mapService.GetPlayersHexes(player.Id);
			var playersHexesInFederation = player.State.Federations.SelectMany(fed => fed.HexIds).ToList();
			var hexesWithBuildingsNotInFederation = playersHexes
				.Except(playersHexes.WithSatellites())
				.Where(h => !playersHexesInFederation.Contains(h.Id))
				.ToList();
			var powerValueOfBuildingsNotInFederation = hexesWithBuildingsNotInFederation
				.SelectMany(h => h.Buildings)
				.Where(b => b.PlayerId == player.Id)
				.Sum(b => b.PowerValueInFederation);

			if (player.RaceId == Race.Ivits)
			{
				var ivitsFederation = player.State.Federations.SingleOrDefault(fed => fed.HexIds.Any());
				if (ivitsFederation == null)
				{
					return powerValueOfBuildingsNotInFederation >= powerRequiredForFederation
						? new AvailableActionViewModel
						{
							Type = ActionType.FormFederation,
							Description = "Form your federation",
							InteractionState = new InteractionStateViewModel
							{
								ClickableHexes = hexesWithBuildingsNotInFederation.Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList(),
							}
						}
						: null;
				}

				var (canTakeMoreTokens, excessPowerValueOfFederation) = FederationTokenUtils.CanIvitsTakeMoreTokens(player, ivitsFederation);
				if (canTakeMoreTokens)
				{
					return new AvailableActionViewModel
					{
						Type = ActionType.FormFederation,
						Description = "Expand your federation",
						InteractionState = new InteractionStateViewModel
						{
							ClickableHexes = new List<InteractionStateViewModel.ColonizableHexViewModel>()
						},
						AdditionalData = JsonConvert.SerializeObject(new IvitsExpandFederationInfoViewModel
						{
							RequiredPower = 0,
							CanTakeMoreTokens = true
						})
					};
				}

				var canExpand = excessPowerValueOfFederation + powerValueOfBuildingsNotInFederation >=
								powerRequiredForFederation;
				if (canExpand)
				{
					return new AvailableActionViewModel
					{
						Type = ActionType.FormFederation,
						Description = "Expand your federation",
						InteractionState = new InteractionStateViewModel
						{
							ClickableHexes = hexesWithBuildingsNotInFederation.Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList(),
						},
						AdditionalData = JsonConvert.SerializeObject(new IvitsExpandFederationInfoViewModel
						{
							RequiredPower = powerRequiredForFederation - excessPowerValueOfFederation,
							CanTakeMoreTokens = false
						})
					};
				}
				return null;
			}

			return powerValueOfBuildingsNotInFederation >= powerRequiredForFederation
				? new AvailableActionViewModel
				{
					Type = ActionType.FormFederation,
					Description = "Form a federation",
					InteractionState = new InteractionStateViewModel
					{
						ClickableHexes = hexesWithBuildingsNotInFederation.Select(h => new InteractionStateViewModel.ColonizableHexViewModel { Id = h.Id }).ToList(),
					}
				}
				: null;
		}
	}
}
