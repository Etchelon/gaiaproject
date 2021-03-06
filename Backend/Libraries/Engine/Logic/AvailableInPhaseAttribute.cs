using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AvailableInPhaseAttribute : Attribute
	{
		public GamePhase Phase { get; }

		public AvailableInPhaseAttribute(GamePhase phase)
		{
			Phase = phase;
		}
	}
}
