using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum FederationTokenType
	{
		[Description("6VP, 2 Knowledge")]
		Knowledge,

		[Description("7VP, 6 Credits")]
		Credits,

		[Description("7VP, 2 Ores")]
		Ores,

		[Description("8VP, 2 Power Tokens")]
		PowerTokens,

		[Description("8VP, 1 QIC")]
		Qic,

		[Description("12VP")]
		Points,

		[Description("1 Ore, 1 Knowledge, 2 Credits")]
		Gleens = 42
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this FederationTokenType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}