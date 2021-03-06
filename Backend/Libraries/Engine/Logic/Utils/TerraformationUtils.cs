using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Utils
{
	public static class TerraformationUtils
	{
		private static readonly Dictionary<Race, Dictionary<PlanetType, TerraformationCost>> TerraformingCostsByRace = new Dictionary<Race, Dictionary<PlanetType, TerraformationCost>>
		{
			{
				Race.Terrans,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Terra, TerraformationCost.ZeroSteps },
					{ PlanetType.Ice, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.TwoSteps },
					{ PlanetType.Volcanic, TerraformationCost.TwoSteps },
					{ PlanetType.Desert, TerraformationCost.ThreeSteps },
					{ PlanetType.Swamp, TerraformationCost.ThreeSteps},
				}
			},
			{
				Race.Lantids,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Terra, TerraformationCost.ZeroSteps },
					{ PlanetType.Ice, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.TwoSteps },
					{ PlanetType.Volcanic, TerraformationCost.TwoSteps },
					{ PlanetType.Desert, TerraformationCost.ThreeSteps },
					{ PlanetType.Swamp, TerraformationCost.ThreeSteps},
				}
			},
			{
				Race.Ambas,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Swamp, TerraformationCost.ZeroSteps },
					{ PlanetType.Desert, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.OneStep },
					{ PlanetType.Ice, TerraformationCost.TwoSteps },
					{ PlanetType.Volcanic, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.ThreeSteps },
					{ PlanetType.Oxide, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Taklons,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Swamp, TerraformationCost.ZeroSteps },
					{ PlanetType.Desert, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.OneStep },
					{ PlanetType.Ice, TerraformationCost.TwoSteps },
					{ PlanetType.Volcanic, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.ThreeSteps },
					{ PlanetType.Oxide, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Bescods,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Titanium, TerraformationCost.ZeroSteps },
					{ PlanetType.Swamp, TerraformationCost.OneStep },
					{ PlanetType.Ice, TerraformationCost.OneStep },
					{ PlanetType.Desert, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.TwoSteps },
					{ PlanetType.Oxide, TerraformationCost.ThreeSteps },
					{ PlanetType.Volcanic, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Firaks,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Titanium, TerraformationCost.ZeroSteps },
					{ PlanetType.Swamp, TerraformationCost.OneStep },
					{ PlanetType.Ice, TerraformationCost.OneStep },
					{ PlanetType.Desert, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.TwoSteps },
					{ PlanetType.Oxide, TerraformationCost.ThreeSteps },
					{ PlanetType.Volcanic, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Itars,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Ice, TerraformationCost.ZeroSteps },
					{ PlanetType.Terra, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.TwoSteps },
					{ PlanetType.Swamp, TerraformationCost.TwoSteps },
					{ PlanetType.Desert, TerraformationCost.ThreeSteps },
					{ PlanetType.Volcanic, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Nevlas,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Ice, TerraformationCost.ZeroSteps },
					{ PlanetType.Terra, TerraformationCost.OneStep },
					{ PlanetType.Titanium, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.TwoSteps },
					{ PlanetType.Swamp, TerraformationCost.TwoSteps },
					{ PlanetType.Desert, TerraformationCost.ThreeSteps },
					{ PlanetType.Volcanic, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.HadschHallas,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Oxide, TerraformationCost.ZeroSteps },
					{ PlanetType.Terra, TerraformationCost.OneStep },
					{ PlanetType.Volcanic, TerraformationCost.OneStep },
					{ PlanetType.Desert, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.TwoSteps },
					{ PlanetType.Titanium, TerraformationCost.ThreeSteps },
					{ PlanetType.Swamp, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Ivits,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Oxide, TerraformationCost.ZeroSteps },
					{ PlanetType.Terra, TerraformationCost.OneStep },
					{ PlanetType.Volcanic, TerraformationCost.OneStep },
					{ PlanetType.Desert, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.TwoSteps },
					{ PlanetType.Titanium, TerraformationCost.ThreeSteps },
					{ PlanetType.Swamp, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.BalTaks,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Volcanic, TerraformationCost.ZeroSteps },
					{ PlanetType.Desert, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.OneStep },
					{ PlanetType.Swamp, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.ThreeSteps },
					{ PlanetType.Titanium, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Geodens,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Volcanic, TerraformationCost.ZeroSteps },
					{ PlanetType.Desert, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.OneStep },
					{ PlanetType.Swamp, TerraformationCost.TwoSteps },
					{ PlanetType.Terra, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.ThreeSteps },
					{ PlanetType.Titanium, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Gleens,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Desert, TerraformationCost.ZeroSteps },
					{ PlanetType.Swamp, TerraformationCost.OneStep },
					{ PlanetType.Volcanic, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.TwoSteps },
					{ PlanetType.Titanium, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.ThreeSteps },
					{ PlanetType.Terra, TerraformationCost.ThreeSteps },
				}
			},
			{
				Race.Xenos,
				new Dictionary<PlanetType, TerraformationCost>
				{
					{ PlanetType.Desert, TerraformationCost.ZeroSteps },
					{ PlanetType.Swamp, TerraformationCost.OneStep },
					{ PlanetType.Volcanic, TerraformationCost.OneStep },
					{ PlanetType.Oxide, TerraformationCost.TwoSteps },
					{ PlanetType.Titanium, TerraformationCost.TwoSteps },
					{ PlanetType.Ice, TerraformationCost.ThreeSteps },
					{ PlanetType.Terra, TerraformationCost.ThreeSteps },
				}
			},
		};

		public static PlanetType GetRaceNativePlanetType(Race race)
		{
			var raceData = TerraformingCostsByRace[race];
			return raceData.First(kvp => kvp.Value == TerraformationCost.ZeroSteps).Key;
		}

		public static TerraformationCost GetBaseTerraformingCost(Race race, PlanetType targetPlanetType)
		{
			return targetPlanetType switch
			{
				PlanetType.Gaia => TerraformationCost.OneQic,
				_ => TerraformingCostsByRace[race][targetPlanetType]
			};
		}

		public static ResourcesCost GetActualTerraformationCost(PlayerInGame player, PlanetType targetPlanetType, out int nSteps)
		{
			var race = player.RaceId.Value;
			var baseCost = GetBaseTerraformingCost(race, targetPlanetType);
			switch (baseCost)
			{
				case TerraformationCost.OneQic:
					var isGleenWithoutQicAcademy = race == Race.Gleens && !player.State.Buildings.AcademyRight;
					nSteps = 0;
					return isGleenWithoutQicAcademy ? ResourcesCost.TerraformationCost(1) : ResourcesCost.TerraformationOfGaiaPlanetCost();
				case TerraformationCost.ZeroSteps:
					nSteps = 0;
					return ResourcesCost.TerraformationCost(0);
				case TerraformationCost.OneStep:
				case TerraformationCost.TwoSteps:
				case TerraformationCost.ThreeSteps:
					var oresPerStep = player.State.TerraformingCost;
					var tempSteps = player.State.TempTerraformationSteps ?? 0;
					var actualSteps = Math.Max((int)baseCost - tempSteps, 0);
					var requiredOres = oresPerStep * actualSteps;
					nSteps = (int)baseCost;
					return ResourcesCost.TerraformationCost(requiredOres);
				default:
					throw new Exception($"Base terraformation cost {baseCost} not handled.");
			}
		}
	}
}