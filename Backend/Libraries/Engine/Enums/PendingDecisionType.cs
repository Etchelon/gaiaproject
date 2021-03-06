using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum PendingDecisionType
	{
		ChargePower,
		PlaceLostPlanet,
		TerransDecideIncome,
		ItarsBurnPowerForTechnologyTile,
		PerformConversionOrPassTurn,
		SortIncomes,
		FreeTechnologyStep,
		ChooseTechnologyTile,
		SelectFederationTokenToScore,
		TaklonsLeech,
		AcceptOrDeclineLastStep
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this PendingDecisionType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}