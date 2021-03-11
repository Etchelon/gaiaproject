using System;

namespace GaiaProject.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class GameManagerAttribute : Attribute
	{
		public string GameName { get; set; }

		public GameManagerAttribute(string gameName)
		{
			this.GameName = gameName;
		}
	}
}
