using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum RoundBoosterType
	{
		[Description("+1 Ore +1 Knw")]
		GainOreGainKnowledge,

		[Description("+2 Tokens +1 Ore")]
		GainPowerTokensGainOre,

		[Description("+2 Credits +1 Qic")]
		GainCreditsGainQic,

		[Description("ACT: terraform; +2 Credits")]
		TerraformActionGainCredits,

		[Description("ACT: +3 Nav; +2 Power")]
		BoostRangeGainPower,

		[Description("Pass: 1VP x M; +1 Ore")]
		PassPointsPerMineGainOre,

		[Description("Pass: 2VP x TS; +1 Ore")]
		PassPointsPerTradingStationsGainOre,

		[Description("Pass: 3VP x RL; +1 Knw")]
		PassPointsPerResearchLabsGainKnowledge,

		[Description("Pass: 4VP x PI-AC; +4 Power")]
		PassPointsPerBigBuildingsGainPower,

		[Description("Pass: 1VP x Gaia; +4 Credits")]
		PassPointsPerGaiaPlanetsGainCredits
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this RoundBoosterType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}