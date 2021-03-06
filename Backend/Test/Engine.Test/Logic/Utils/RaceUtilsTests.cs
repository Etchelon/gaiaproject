using System;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using Xunit;

namespace Engine.Test.Logic.Utils
{
	public class RaceUtilsTests
	{
		[Fact]
		public void InitialResearchStep()
		{
			var races = Enum.GetValues(typeof(Race));
			foreach (var raceObj in races)
			{
				var race = (Race)raceObj;
				if (race == Race.None)
				{
					continue;
				}
				var initialStep = RaceUtils.GetInitialResearchStep(race);
				switch (race)
				{
					default:
						throw new Exception($"Race {race} not handled.");
					case Race.Geodens:
						Assert.Equal(ResearchTrackType.Terraformation, initialStep);
						break;
					case Race.Ambas:
					case Race.Gleens:
						Assert.Equal(ResearchTrackType.Navigation, initialStep);
						break;
					case Race.Xenos:
						Assert.Equal(ResearchTrackType.ArtificialIntelligence, initialStep);
						break;
					case Race.BalTaks:
					case Race.Terrans:
						Assert.Equal(ResearchTrackType.Gaiaformation, initialStep);
						break;
					case Race.HadschHallas:
						Assert.Equal(ResearchTrackType.Economy, initialStep);
						break;
					case Race.Nevlas:
						Assert.Equal(ResearchTrackType.Science, initialStep);
						break;
					case Race.Bescods:
					case Race.Firaks:
					case Race.Itars:
					case Race.Ivits:
					case Race.Lantids:
					case Race.Taklons:
						Assert.Null(initialStep);
						break;
				}
			}
		}
	}
}
