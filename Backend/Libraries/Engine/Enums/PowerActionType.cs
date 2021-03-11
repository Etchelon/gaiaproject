using System.ComponentModel;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Logic;

namespace GaiaProject.Engine.Enums
{
	public enum PowerActionType
	{
		[ActionCost(7)]
		[Description("3 Knowledge")]
		Gain3Knowledge,

		[ActionCost(5)]
		[Description("ACT: terraform (+2 steps)")]
		Gain2TerraformingSteps,

		[ActionCost(4)]
		[Description("2 Ores")]
		Gain2Ores,

		[ActionCost(4)]
		[Description("7 Credits")]
		Gain7Credits,

		[ActionCost(4)]
		[Description("2 Knowledge")]
		Gain2Knowledge,

		[ActionCost(3)]
		[Description("ACT: terraform (+1 step)")]
		Gain1TerraformingStep,

		[ActionCost(3)]
		[Description("2 Tokens")]
		Gain2PowerTokens
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this PowerActionType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}