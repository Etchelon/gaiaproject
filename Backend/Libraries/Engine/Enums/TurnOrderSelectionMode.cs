using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum TurnOrderSelectionMode
	{
		Random,
		Assigned,
		Auction
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this TurnOrderSelectionMode o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}
