using System;

namespace GaiaProject.Engine.Logic
{
	public class InvalidActionException : Exception
	{
		public InvalidActionException(string reason)
			: base(reason)
		{
		}
	}
}
