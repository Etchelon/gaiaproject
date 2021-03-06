using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class ResearchStepGain : Gain
	{
		public override GainType Type => GainType.ResearchStep;
		public ResearchTrackType Track { get; set; }

		public ResearchStepGain(ResearchTrackType track)
		{
			Loggable = false;
			Track = track;
		}

		public override string ToString()
		{
			return $"a research step in technology track {Track}";
		}
	}
}
