using System.ComponentModel;
using GaiaProject.Common.Reflection;

namespace GaiaProject.Engine.Enums
{
	public enum TerraformationCost
	{
		ZeroSteps,
		OneStep,
		TwoSteps,
		ThreeSteps,
		OneQic,
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this TerraformationCost o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}