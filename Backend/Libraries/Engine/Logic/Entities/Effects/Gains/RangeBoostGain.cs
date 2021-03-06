namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class RangeBoostGain : Gain
	{
		public override GainType Type => GainType.RangeBoost;
		public int Boost { get; set; }

		public RangeBoostGain(int boost)
		{
			Boost = boost;
		}

		public override string ToString()
		{
			return $"range boost of {Boost} to use on a colonization action";
		}
	}
}