namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class PowerGain : Gain
	{
		public override GainType Type => GainType.Power;

		public int Power { get; }
		public int? SpentPoints { get; }

		public PowerGain(int power, int? spendPoints = null)
		{
			Power = power;
			SpentPoints = spendPoints;
		}

		public override string ToString()
		{
			return SpentPoints.HasValue
				? $"{Power} power by spending {SpentPoints} VP"
				: $"{Power} power";
		}
	}
}
