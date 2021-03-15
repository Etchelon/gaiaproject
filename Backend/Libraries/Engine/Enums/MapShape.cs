using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum MapShape
	{
		Custom,
		Standard1P,
		Standard2P,
		Standard3P,
		Standard4P,
		IntroductoryGame2P,
		IntroductoryGame34P,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this MapShape o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}