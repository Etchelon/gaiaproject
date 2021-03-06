using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum RoundSubPhase
	{
		Income,
		IncomeDecisions,
		Gaia,
		GaiaDecisions,
		Actions,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this RoundSubPhase o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}