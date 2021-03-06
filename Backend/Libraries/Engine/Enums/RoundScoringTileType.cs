using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum RoundScoringTileType
	{
		PointsPerTerraformingStep2,
		PointsPerResearchStep2,
		PointsPerMine2,
		PointsPerTradingStation3,
		PointsPerTradingStation4,
		PointsPerGaiaPlanet3,
		PointsPerGaiaPlanet4,
		PointsPerBigBuilding5,
		PointsPerBigBuilding5Bis,
		PointsPerFederation5
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this RoundScoringTileType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}
