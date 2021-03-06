namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class NavigationRangeGain : Gain
	{
		public override GainType Type => GainType.NavigationRange;
		public int Range { get; set; }

		public NavigationRangeGain(int range)
		{
			Range = range;
		}

		public override string ToString()
		{
			return $"increased navigation range to distance {Range}";
		}
	}
}
