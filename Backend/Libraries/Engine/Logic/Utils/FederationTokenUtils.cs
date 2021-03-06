using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class FederationTokenUtils
	{
		internal static FederationTokenGain GetFederationTokenGain(FederationTokenType token, GaiaProjectGame game, bool fromQicAction = false)
		{
			var ret = new FederationTokenGain(token);
			ret.Add(GetFederationBonuses(token).ToArray());
			if (fromQicAction)
			{
				return ret;
			}

			var roundNo = game.Rounds.CurrentRound;
			var currentRoundTile = game.BoardState.ScoringBoard.ScoringTiles.First(st => st.RoundNumber == roundNo);
			var roundGivesBonusPoints = currentRoundTile.Id == RoundScoringTileType.PointsPerFederation5;
			if (roundGivesBonusPoints)
			{
				ret.Add(new PointsGain(5, "Scoring Tile"));
			}
			return ret;
		}

		private static List<Gain> GetFederationBonuses(FederationTokenType federationId)
		{
			var gains = new List<Gain>();
			switch (federationId)
			{
				default:
					throw new Exception($"Federation type {federationId} not handled.");
				case FederationTokenType.Credits:
					gains.Add(new ResourcesGain(new Entities.Resources { Credits = 6 }));
					gains.Add(new PointsGain(7, $"federation {federationId}"));
					break;
				case FederationTokenType.Ores:
					gains.Add(new ResourcesGain(new Entities.Resources { Ores = 2 }));
					gains.Add(new PointsGain(7, $"federation {federationId}"));
					break;
				case FederationTokenType.Knowledge:
					gains.Add(new ResourcesGain(new Entities.Resources { Knowledge = 2 }));
					gains.Add(new PointsGain(6, $"federation {federationId}"));
					break;
				case FederationTokenType.Qic:
					gains.Add(new ResourcesGain(new Entities.Resources { Qic = 1 }));
					gains.Add(new PointsGain(8, $"federation {federationId}"));
					break;
				case FederationTokenType.PowerTokens:
					gains.Add(new ResourcesGain(new Entities.Resources { PowerTokens = 2 }));
					gains.Add(new PointsGain(8, $"federation {federationId}"));
					break;
				case FederationTokenType.Points:
					gains.Add(new PointsGain(12, $"federation {federationId}"));
					break;
				case FederationTokenType.Gleens:
					gains.Add(new ResourcesGain(new Entities.Resources { Credits = 2, Ores = 1, Knowledge = 1 }));
					break;
			}
			return gains;
		}

		internal static PlayerState SpendFederationToken(PlayerInGame player)
		{
			var federationToTurn = player.State.FederationTokens.FirstOrDefault(fed => !fed.UsedForTechOrAdvancedTile);
			if (federationToTurn == null)
			{
				throw new Exception("User has no federations tokens to spend!");
			}
			federationToTurn.UsedForTechOrAdvancedTile = true;
			return player.State;
		}

		public static (bool can, int excessPower) CanIvitsTakeMoreTokens(PlayerInGame player, Federation ivitsFederation)
		{
			const int powerRequiredForFederation = 7;
			var excessPowerValueOfFederation = ivitsFederation.TotalPowerValue % powerRequiredForFederation;
			var federationSize = (int)Math.Floor(ivitsFederation.TotalPowerValue / (double)powerRequiredForFederation);
#warning TODO: tokens aren't currently linked to Federation IDs so I must figure out if one comes from Terraformation Track
			//var currentTokensClaimed = player.State.FederationTokens.Count(tok => tok.FederationId == ivitsFederation.Id);
			var currentTokensClaimed = player.State.FederationTokens.Count;
			if (player.State.ResearchAdvancements.Single(adv => adv.Track == ResearchTrackType.Terraformation)
				.Steps == ResearchUtils.MaxSteps)
			{
				currentTokensClaimed -= 1;
			}
			return (currentTokensClaimed < federationSize, excessPowerValueOfFederation);
		}
	}
}