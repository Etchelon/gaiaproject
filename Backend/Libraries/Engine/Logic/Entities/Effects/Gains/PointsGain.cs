namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class PointsGain : Gain
	{
		public override GainType Type => GainType.Points;
		public int Points { get; }
		public string Source { get; }

		public PointsGain(int points, string source)
		{
			Points = points;
			Source = source;
		}

		public override string ToString()
		{
			return $"{Points}VP ({Source})";
		}
	}
}
