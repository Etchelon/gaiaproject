using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Decisions;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Model
{
	[BsonNoId]
	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		// Rounds CurrentPhase
		typeof(ChargePowerDecision),
		typeof(ChooseTechnologyTileDecision),
		typeof(FreeTechnologyStepDecision),
		typeof(ItarsBurnPowerForTechnologyTileDecision),
		typeof(PlaceLostPlanetDecision),
		typeof(PerformConversionOrPassTurnDecision),
		typeof(SelectFederationTokenToScoreDecision),
		typeof(SortIncomesDecision),
		typeof(TaklonsLeechDecision),
		typeof(TerransDecideIncomeDecision),
		typeof(AcceptOrDeclineLastStepDecision)
	)]
	public abstract class PendingDecision
	{
		/// <summary>
		/// Incremental id of the decision
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Type of decision
		/// </summary>
		public abstract PendingDecisionType Type { get; }
		/// <summary>
		/// Id of the player that must take the decision
		/// </summary>
		public string PlayerId { get; protected set; }
		/// <summary>
		/// The player action that spawned this decision
		/// </summary>
		public int? SpawnedFromActionId { get; set; }
		/// <summary>
		/// Description of what the specific decision is about
		/// </summary>
		public abstract string Description { get; }

		public PendingDecision ForPlayer(string playerId)
		{
			PlayerId = playerId;
			return this;
		}

		public virtual PendingDecision Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}