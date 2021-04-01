using System.ComponentModel;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Logic;

namespace GaiaProject.Engine.Enums
{
	public enum ActionType
	{
		// Setup
		[AvailableInPhase(GamePhase.Setup)]
		[Description("adjust sectors rotation")]
		AdjustSectors,

		[AvailableInPhase(GamePhase.Setup)]
		[Description("select race to play with")]
		SelectRace,

		[AvailableInPhase(GamePhase.Setup)]
		[Description("bid for race")]
		BidForRace,

		[AvailableInPhase(GamePhase.Setup)]
		[Description("place initial structure")]
		PlaceInitialStructure,

		[AvailableInPhase(GamePhase.Setup)]
		[Description("select starting round booster")]
		SelectStartingRoundBooster,

		// Rounds
		[AvailableInPhase(GamePhase.Rounds)]
		[FreeAction]
		[Description("conversions")]
		Conversions,

		[AvailableInPhase(GamePhase.Rounds)]
		[MaybeDecision]
		[Description("colonize a planet")]
		ColonizePlanet,

		[AvailableInPhase(GamePhase.Rounds)]
		[PlanetaryInstituteAction(Race.Ivits)]
		[Description("place Ivits space station")]
		IvitsPlaceSpaceStation,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("start a Gaia project")]
		StartGaiaProject,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("upgrade a structure")]
		UpgradeExistingStructure,

		[AvailableInPhase(GamePhase.Rounds)]
		[PlanetaryInstituteAction(Race.Firaks)]
		[Description("downgrade research lab")]
		FiraksDowngradeResearchLab,

		[AvailableInPhase(GamePhase.Rounds)]
		[PlanetaryInstituteAction(Race.Ambas)]
		[Description("swap Planetary Institute and mine")]
		AmbasSwapPlanetaryInstituteAndMine,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("form a federation")]
		FormFederation,

		[AvailableInPhase(GamePhase.Rounds)]
		[MaybeDecision]
		[Description("research a technology")]
		ResearchTechnology,

		[AvailableInPhase(GamePhase.Rounds)]
		[RaceAction(Race.Bescods)]
		[Description("research a technology")]
		BescodsResearchProgress,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("pass the rest of the round")]
		Pass,

		[AvailableInPhase(GamePhase.Rounds)]
		[FreeAction]
		[Decision]
		[Description("end the turn")]
		PassTurn,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("power action")]
		Power,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("Qic action")]
		Qic,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("take a technology tile")]
		ChooseTechnologyTile,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("charge power")]
		ChargePower,

		[AvailableInPhase(GamePhase.Rounds)]
		[RaceAction(Race.Terrans)]
		[Decision]
		[Description("convert power from Gaia area")]
		TerransDecideIncome,

		[AvailableInPhase(GamePhase.Rounds)]
		[RaceAction(Race.Itars)]
		[Decision]
		[Description("burnt power from Gaia area for technology tile")]
		ItarsBurnPowerForTechnologyTile,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("activate academy")]
		UseRightAcademy,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("sort power and power token incomes")]
		SortIncomes,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("use a technology tile")]
		UseTechnologyTile,

		[AvailableInPhase(GamePhase.Rounds)]
		[Description("use a round booster")]
		UseRoundBooster,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("place the lost planet")]
		PlaceLostPlanet,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("rescore federation token")]
		RescoreFederationToken,

		[AvailableInPhase(GamePhase.Rounds)]
		[RaceAction(Race.Taklons)]
		[Decision]
		[Description("Taklon's power leech")]
		TaklonsLeech,

		[AvailableInPhase(GamePhase.Rounds)]
		[Decision]
		[Description("accept or decline last step")]
		AcceptOrDeclineLastStep
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this ActionType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}