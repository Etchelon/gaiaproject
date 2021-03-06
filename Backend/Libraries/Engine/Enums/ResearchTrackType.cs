using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum ResearchTrackType
	{
		Terraformation,
		Navigation,

		[Description("Artificial Intelligence")]
		ArtificialIntelligence,
		Gaiaformation,
		Economy,
		Science
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this ResearchTrackType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}