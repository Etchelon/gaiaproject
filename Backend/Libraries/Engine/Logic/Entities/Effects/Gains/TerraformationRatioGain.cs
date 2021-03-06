namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class TerraformationRatioGain : Gain
	{
		public override GainType Type => GainType.TerraformationRatio;
		public int Ratio { get; set; }

		public TerraformationRatioGain(int ratio)
		{
			Ratio = ratio;
		}

		public override string ToString()
		{
			return $"decreased terraformation cost to {Ratio} ore{(Ratio > 1 ? "s" : "")} per step";
		}
	}
}
