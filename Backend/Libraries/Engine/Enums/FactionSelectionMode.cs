using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum RaceSelectionMode
	{
		Random,
		TurnOrder,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this RaceSelectionMode o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}
