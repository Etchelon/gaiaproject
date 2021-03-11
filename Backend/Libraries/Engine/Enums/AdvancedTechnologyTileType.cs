using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum AdvancedTechnologyTileType
	{
		[Description("ACT: 1 Qic 5 Credits")]
		ActionGain1Qic5Credits,

		[Description("ACT: 3 Ores")]
		ActionGain3Ores,

		[Description("ACT: 3 Knw")]
		ActionGain3Knowledge,

		[Description("2VP x mine")]
		Immediate2PointsPerMine,

		[Description("4VP x Trading station")]
		Immediate4PointsPerTradingStation,

		[Description("5VP x Federation")]
		Immediate5PointsPerFederation,

		[Description("2VP x Sector")]
		Immediate2PointsPerSector,

		[Description("2VP x Gaia")]
		Immediate2PointsPerGaiaPlanet,

		[Description("1 Ore x Sector")]
		Immediate1OrePerSector,

		[Description("Pass: 3VP x Federation")]
		Pass3PointsPerFederation,

		[Description("Pass: 3VP x Research Lab")]
		Pass3PointsPerResearchLab,

		[Description("Pass: 1VP x Planet type")]
		Pass1PointsPerPlanetType,

		[Description("Research -> 2VP")]
		Passive2PointsPerResearchStep,

		[Description("Mine -> 3VP")]
		Passive3PointsPerMine,

		[Description("Trading Station -> 3VP")]
		Passive3PointsPerTradingStation,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this AdvancedTechnologyTileType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}