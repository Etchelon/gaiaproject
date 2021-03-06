using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum GamePhase
	{
		Setup,
		Rounds,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this GamePhase o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}