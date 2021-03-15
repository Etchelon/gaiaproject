using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum BuildingType
	{
		[Description("Mine")]
		Mine,

		[Description("Trading Station")]
		TradingStation,

		[Description("Planetary Institute")]
		PlanetaryInstitute,

		[Description("Research Lab")]
		ResearchLab,

		[Description("Academy (Knw)")]
		AcademyLeft,

		[Description("Academy (Qic)")]
		AcademyRight,

		[Description("Gaiaformer")]
		Gaiaformer,

		[Description("Satellite")]
		Satellite,

		[Description("Lost Planet")]
		LostPlanet,

		[Description("Ivits Station")]
		IvitsSpaceStation
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this BuildingType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}