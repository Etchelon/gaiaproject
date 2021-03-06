using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class ResearchUtils
	{
		public const int MaxSteps = 5;

		public static List<Effect> ApplyStep(ResearchTrackType trackId, string playerId, GaiaProjectGame game, bool chosenByUser)
		{
			var effects = new List<Effect>
			{
				new ResearchStepGain(trackId)
			};

			var player = game.GetPlayer(playerId);
			var playerState = player.State;
			var track = game.BoardState.ResearchBoard.Tracks.Single(t => t.Id == trackId);

			var playerAdvancements = playerState.ResearchAdvancements.Single(padv => padv.Track == trackId);
			var steps = playerAdvancements.Steps + 1;

			if (steps == 3)
			{
				effects.Add(new PowerGain(3));
			}
			if (steps == MaxSteps)
			{
				var (canAdvance, reason) = CanPlayerAdvanceToLevel5(trackId, playerId, game);
				if (!canAdvance && chosenByUser)
				{
					throw new Exception($"Cannot advance to level 5 of research track {trackId}, because: {reason}");
				}
				effects.Add(new UseFederationTokenCost());
			}

			switch (trackId)
			{
				default:
					throw new Exception($"Track type {trackId} not handled.");
				case ResearchTrackType.Terraformation:
					{
						if (steps == 1 || steps == 4)
						{
							effects.Add(new ResourcesGain(new Resources { Ores = 2 }));
						}
						if (steps == 2)
						{
							effects.Add(new TerraformationRatioGain(2));
						}
						if (steps == 3)
						{
							effects.Add(new TerraformationRatioGain(1));
						}
						if (steps == MaxSteps)
						{
							var federationId = track.Federation;
							var federationTokenGain = FederationTokenUtils.GetFederationTokenGain(federationId, game);
							effects.Add(federationTokenGain);
							effects.Add(new TechnologyTrackBonusTakenEffect(ResearchTrackType.Terraformation));
						}
						break;
					}
				case ResearchTrackType.Navigation:
					{
						if (steps == 1 || steps == 3)
						{
							effects.Add(new ResourcesGain(new Resources { Qic = 1 }));
						}
						if (steps == 2)
						{
							effects.Add(new NavigationRangeGain(2));
						}
						if (steps == 4)
						{
							effects.Add(new NavigationRangeGain(3));
						}
						if (steps == MaxSteps)
						{
							effects.Add(new NavigationRangeGain(4));
							effects.Add(new TechnologyTrackBonusTakenEffect(ResearchTrackType.Navigation));
							effects.Add(new PendingDecisionEffect(new PlaceLostPlanetDecision()));
						}
						break;
					}
				case ResearchTrackType.ArtificialIntelligence:
					{
						if (steps == 1 || steps == 2)
						{
							effects.Add(new ResourcesGain(new Resources { Qic = 1 }));
						}
						if (steps == 3 || steps == 4)
						{
							effects.Add(new ResourcesGain(new Resources { Qic = 2 }));
						}
						if (steps == MaxSteps)
						{
							effects.Add(new ResourcesGain(new Resources { Qic = 4 }));
						}
						break;
					}
				case ResearchTrackType.Gaiaformation:
					{
						if (steps == 1 || steps == 3 || steps == 4)
						{
							effects.Add(new UnlockGaiaformerGain());
						}
						if (steps == 2)
						{
							effects.Add(new ResourcesGain(new Resources { PowerTokens = 3 }));
						}
						if (steps == MaxSteps)
						{
							var points = PointUtils.GetPointsForLastGaiaformationStep(playerId, game);
							effects.Add(points);
						}
						break;
					}
				case ResearchTrackType.Economy:
					{
						effects.Add(new IncomeVariationEffect(IncomeSource.EconomyTrack));
						if (steps == MaxSteps)
						{
							effects.Add(new ResourcesGain(new Resources { Ores = 3, Credits = 6 }));
							effects.Add(new PowerGain(6));
						}
						break;
					}
				case ResearchTrackType.Science:
					{
						effects.Add(new IncomeVariationEffect(IncomeSource.ScienceTrack));
						if (steps == MaxSteps)
						{
							var gain = new ResourcesGain(new Resources { Knowledge = 9 });
							effects.Add(gain);
						}
						break;
					}
			}

			// Check if points should be given
			var pointGains = PointUtils.GetPointsForResearchStep(playerId, game);
			effects.AddRange(pointGains);

			return effects;
		}

		public static (bool can, string reason) CanPlayerAdvanceToLevel(int level, ResearchTrackType trackId, string playerId, GaiaProjectGame game)
		{
			var player = game.GetPlayer(playerId);
			if (player.RaceId == Race.BalTaks && trackId == ResearchTrackType.Navigation && !player.State.Buildings.PlanetaryInstitute)
			{
				return (false, "You cannot advance on the navigation track until you build the Planetary Institute");
			}

			if (level >= MaxSteps)
			{
				return CanPlayerAdvanceToLevel5(trackId, playerId, game);
			}

			var playerAdvancements =
				game.GetPlayer(playerId).State.ResearchAdvancements.Single(adv => adv.Track == trackId);
			if (playerAdvancements.Steps >= level)
			{
				return (false,
					$"You cannot advance to level {level} as you already are at level {playerAdvancements.Steps}");
			}
			return (true, null);
		}

		private static (bool can, string reason) CanPlayerAdvanceToLevel5(ResearchTrackType trackId, string playerId, GaiaProjectGame game)
		{
			var playerAdvancements = game.Players.SelectMany(p => p.State.ResearchAdvancements.Where(padv => padv.Track == trackId));
			var spotIsTaken = playerAdvancements.Any(padv => padv.Steps == MaxSteps);
			if (spotIsTaken)
			{
				return (false, "The top spot is already taken.");
			}
			var playerState = game.GetPlayer(playerId).State;
			var playerHasNoFederationTokensToSpend = playerState.FederationTokens.All(fed => fed.UsedForTechOrAdvancedTile);
			if (playerHasNoFederationTokensToSpend)
			{
				return (false, "Player has no federation tokens to spend.");
			}
			return (true, null);
		}
	}
}