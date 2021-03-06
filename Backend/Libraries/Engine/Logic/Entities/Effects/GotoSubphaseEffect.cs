using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class GotoSubphaseEffect : Effect
	{
		public SetupSubPhase SubPhase { get; }

		public GotoSubphaseEffect(SetupSubPhase subPhase)
		{
			SubPhase = subPhase;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			game.Setup.SubPhase = SubPhase;
			game.LogSystemMessage($"Phase {SubPhase.ToDescription()} begins");
		}
	}
}
