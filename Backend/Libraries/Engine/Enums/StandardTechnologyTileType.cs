using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum StandardTechnologyTileType
	{
		[Description("ACT: +4 Power")]
		ActionGain4Power,

		[Description("1 Ore 1 Qic")]
		Immediate1Ore1Qic,

		[Description("1 Knw x planet type")]
		Immediate1KnowledgePerPlanetType,

		[Description("7VP")]
		Immediate7Points,

		[Description("+1 Ore +1 Power")]
		Income1Ore1Power,

		[Description("+1 Credit +1 Knw")]
		Income1Knowledge1Coin,

		[Description("+4 Credits")]
		Income4Coins,

		[Description("PI-AC: 4 Power")]
		PassiveBigBuildingsWorth4Power,

		[Description("Gaia -> 3VP")]
		Passive3PointsPerGaiaPlanet,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this StandardTechnologyTileType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}