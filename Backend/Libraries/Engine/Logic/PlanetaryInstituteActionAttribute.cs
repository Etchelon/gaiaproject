using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic
{
	[AttributeUsage(AttributeTargets.Field)]
	public class PlanetaryInstituteActionAttribute : Attribute
	{
		public Race Race { get; }

		public PlanetaryInstituteActionAttribute(Race race)
		{
			Race = race;
		}
	}
}