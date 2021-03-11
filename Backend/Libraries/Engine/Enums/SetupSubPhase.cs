using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum SetupSubPhase
	{
		[Description("Randomize Research Board")]
		RandomizeResearchBoard,

		[Description("Randomize Rounds Board")]
		RandomizeRoundsBoard,

		[Description("Randomize Round Boosters")]
		RandomizeRoundBoosters,

		[Description("Randomize Map")]
		RandomizeMap,

		[Description("Select Player Order")]
		SelectPlayerOrder,

		[Description("Map Adjustment")]
		LastPlayerAssemblesMap,

		[Description("Race Selection")]
		SelectRaces,

		[Description("Initial Placement")]
		InitialPlacement,

		[Description("Round Booster Selection")]
		SelectRoundBoosters,

		[Description("End of Setup Phase")]
		EndSetup,

		[Description("Auction")]
		Auction
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this SetupSubPhase o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}