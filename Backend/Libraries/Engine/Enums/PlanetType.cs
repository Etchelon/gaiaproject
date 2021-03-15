using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum PlanetType
	{
		Terra,
		Swamp,
		Desert,
		Oxide,
		Titanium,
		Volcanic,
		Ice,
		Gaia,
		Transdim,
		LostPlanet
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this PlanetType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}