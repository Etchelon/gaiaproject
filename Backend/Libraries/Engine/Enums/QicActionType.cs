using System.ComponentModel;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Logic;

namespace GaiaProject.Engine.Enums
{
	public enum QicActionType
	{
		[ActionCost(4)]
		[Description("4 QIC -> Gain Technology Tile")]
		GainTechnologyTile,

		[ActionCost(3)]
		[Description("3 QIC -> Rescore Federation Token")]
		RescoreFederationBonus,

		[ActionCost(2)]
		[Description("2 QIC -> Points per Planet Types")]
		GainPointsPerPlanetTypes
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this QicActionType o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}