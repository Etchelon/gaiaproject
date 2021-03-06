using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum IncomeSource
	{
		Base,
		Buildings,
		EconomyTrack,
		ScienceTrack,
		RoundBooster,
		StandardTechnologyTile,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this IncomeSource o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}