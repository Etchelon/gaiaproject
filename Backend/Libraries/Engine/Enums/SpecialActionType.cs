using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum SpecialActionType
	{
		[Description("Activate Standard Tile")]
		StandardTechnologyTile,

		[Description("Activate Advanced Tile")]
		AdvancedTechnologyTile,

		[Description("Activate Round Booster")]
		RoundBooster,

		[Description("Use Race Action")]
		RaceAction,

		[Description("Use Planetary Institute")]
		PlanetaryInstitute,

		[Description("Use Academy")]
		RightAcademy
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this SpecialActionType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}