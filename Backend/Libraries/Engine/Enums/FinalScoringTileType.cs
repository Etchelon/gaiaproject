using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum FinalScoringTileType
	{
		[Description("Number of buildings that are part of a federations")]
		BuildingsInAFederation,

		[Description("Number of buildings on the map")]
		BuildingsOnTheMap,

		[Description("Number of different planet types")]
		KnownPlanetTypes,

		[Description("Number of colonized Gaia planets")]
		GaiaPlanets,

		[Description("Number of sectors with at least 1 colonized planet")]
		Sectors,

		[Description("Number of satellites used to form federations")]
		Satellites
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this FinalScoringTileType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}