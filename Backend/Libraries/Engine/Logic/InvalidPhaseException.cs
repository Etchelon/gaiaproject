using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic
{
	public class InvalidPhaseException : Exception
	{
		public InvalidPhaseException(GamePhase expectedPhase, GamePhase actionPhase, string actionName)
			: base($"Action {actionName} cannot be performed during game phase {expectedPhase} but only during game phase {actionPhase}.")
		{
		}
	}
}
