using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum IncomeType
	{
		Resources,
		Power,
		PowerToken
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this IncomeType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}