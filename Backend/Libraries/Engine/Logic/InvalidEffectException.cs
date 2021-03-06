using System;

namespace GaiaProject.Engine.Logic
{
	public class InvalidEffectException : Exception
	{
		public InvalidEffectException(string typeName, string reason)
			: base($"Effect {typeName} cannot be applied because of the following reason: {reason}.")
		{
		}
	}
}
