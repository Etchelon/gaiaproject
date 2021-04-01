using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	/// <summary>
	/// Base class for all actions performed by users.
	/// At its minimum, contains an incremental id which counts the actions taken,
	/// and an action name which is hard coded into all the available actions.
	/// </summary>
	[BsonNoId]
	[BsonKnownTypes(
		// Setup CurrentPhase
		typeof(AdjustSectorsAction),
		typeof(BidForRaceAction),
		typeof(PlaceInitialStructureAction),
		typeof(SelectRaceAction),
		typeof(SelectStartingRoundBoosterAction),
		// Rounds CurrentPhase
		typeof(AmbasSwapPlanetaryInstituteAndMineAction),
		typeof(BescodsResearchProgressAction),
		typeof(ChargePowerAction),
		typeof(ChooseTechnologyTileAction),
		typeof(ColonizePlanetAction),
		typeof(ConversionsAction),
		typeof(FiraksDowngradeResearchLabAction),
		typeof(FormFederationAction),
		typeof(ItarsBurnPowerForTechnologyTileAction),
		typeof(IvitsPlaceSpaceStationAction),
		typeof(PassAction),
		typeof(PassTurnAction),
		typeof(PlaceLostPlanetAction),
		typeof(PowerAction),
		typeof(QicAction),
		typeof(RescoreFederationTokenAction),
		typeof(ResearchTechnologyAction),
		typeof(SortIncomesAction),
		typeof(StartGaiaProjectAction),
		typeof(TaklonsLeechAction),
		typeof(TerransDecideIncomeAction),
		typeof(UpgradeExistingStructureAction),
		typeof(UseRightAcademyAction),
		typeof(UseRoundBoosterAction),
		typeof(UseTechnologyTileAction),
		typeof(AcceptOrDeclineLastStepAction)
	)]
	public abstract class PlayerAction
	{
		/// <summary>
		/// Id of the action. It's an incremental number
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Type of action
		/// </summary>
		public abstract ActionType Type { get; }
		/// <summary>
		/// Id of the player performing the action
		/// </summary>
		public string PlayerId { get; set; }
		/// <summary>
		/// Username of the player performing the action
		/// </summary>
		public string PlayerUsername { get; set; }
		/// <summary>
		/// Link to another action that caused this one
		/// </summary>
		[BsonIgnoreIfNull]
		public int? SpawnedFromActionId { get; set; }

		public override string ToString()
		{
			return $"performs action {Type.ToDescription()}";
		}

		public virtual PlayerAction Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}