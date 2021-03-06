using System.ComponentModel;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum Race
	{
		None = 0,
		Terrans,
		Lantids,
		Taklons,
		Ambas,
		Gleens,
		Xenos,
		Ivits,
		HadschHallas,
		Bescods,
		Firaks,
		Geodens,
		BalTaks,
		Nevlas,
		Itars
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this Race o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}