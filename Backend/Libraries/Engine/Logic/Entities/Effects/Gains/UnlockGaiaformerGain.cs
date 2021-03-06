namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class UnlockGaiaformerGain : Gain
	{
		public override GainType Type => GainType.UnlockGaiaformer;

		public override string ToString()
		{
			return "a new gaiaformer to use";
		}
	}
}
