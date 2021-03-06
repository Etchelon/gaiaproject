using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RaceActionAttribute : Attribute
	{
		public Race Race { get; }

		public RaceActionAttribute(Race race)
		{
			Race = race;
		}
	}
}