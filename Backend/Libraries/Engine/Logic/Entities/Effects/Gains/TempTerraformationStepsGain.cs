namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class TempTerraformationStepsGain : Gain
	{
		public override GainType Type => GainType.TempTerraformationSteps;
		public int Steps { get; set; }

		public TempTerraformationStepsGain(int steps)
		{
			Steps = steps;
		}

		public override string ToString()
		{
			return $"{Steps} terraformation steps to spend on a colonization action";
		}
	}
}