using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic.Entities.Effects.Costs
{
	public class ResourcesCost : Cost
	{
		public override CostType Type => CostType.Resources;
		public Resources Resources { get; set; }
		public int Credits => Resources?.Credits ?? 0;
		public int Ores => Resources?.Ores ?? 0;
		public int Knowledge => Resources?.Knowledge ?? 0;
		public int Qic => Resources?.Qic ?? 0;
		public int Power => Resources?.Power ?? 0;

		public static ResourcesCost operator +(ResourcesCost lhs, ResourcesCost rhs)
		{
			var credits = lhs.Resources.Credits + rhs.Resources.Credits;
			var ores = lhs.Resources.Ores + rhs.Resources.Ores;
			var knowledge = lhs.Resources.Knowledge + rhs.Resources.Knowledge;
			var qic = lhs.Resources.Qic + rhs.Resources.Qic;
			var power = lhs.Resources.Power + rhs.Resources.Power;

			var ret = new ResourcesCost(new Resources
			{
				Credits = credits,
				Ores = ores,
				Knowledge = knowledge,
				Qic = qic,
				Power = power
			});
			return ret;
		}

		public ResourcesCost(Resources resources)
		{
			Resources = resources;
		}

		/// <summary>
		/// Calculates the cost of building a structure.
		/// When building a trading station, you must also specify the distance from the closest building.
		/// Player state must be passed when calculating the cost of placing a Gaiaformer.
		/// </summary>
		/// <param name="buildingType">The type of structure that is being built</param>
		/// <param name="distanceFromClosestBuilding"></param>
		/// <returns></returns>
		public static ResourcesCost BuildingCost(BuildingType buildingType, bool? isCloseToEnemies = null)
		{
			switch (buildingType)
			{
				case BuildingType.Mine:
					return new ResourcesCost(new Resources { Ores = 1, Credits = 2 });
				case BuildingType.TradingStation:
					System.Diagnostics.Debug.Assert(
						isCloseToEnemies.HasValue,
						$"When calculating the cost of a trading station, {nameof(isCloseToEnemies)} must be provided"
					);
					return new ResourcesCost(new Resources { Ores = 2, Credits = isCloseToEnemies.Value ? 3 : 6 });
				case BuildingType.ResearchLab:
					return new ResourcesCost(new Resources { Ores = 3, Credits = 5 });
				case BuildingType.PlanetaryInstitute:
					return new ResourcesCost(new Resources { Ores = 4, Credits = 6 });
				case BuildingType.AcademyLeft:
				case BuildingType.AcademyRight:
					return new ResourcesCost(new Resources { Ores = 6, Credits = 6 });
				default:
					throw new Exception($"buildingType {buildingType} cannot be built with resources.");
			}
		}

		public static ResourcesCost TerraformationCost(int ores)
		{
			return new ResourcesCost(new Resources { Ores = ores });
		}

		public static ResourcesCost TerraformationOfGaiaPlanetCost()
		{
			return new ResourcesCost(new Resources { Qic = 1 });
		}

		public override string ToString()
		{
			var ret = "";
			if (Resources.Credits > 0)
			{
				ret += $"{Resources.Credits} credits";
			}
			if (Resources.Ores > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Resources.Ores} ores";
			}
			if (Resources.Knowledge > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Resources.Knowledge} knowledge";
			}
			if (Resources.Qic > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Resources.Qic} qic";
			}
			if (Resources.Power > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Resources.Power} power";
			}
			return string.IsNullOrEmpty(ret) ? null : ret;
		}

		/// <summary>
		/// Returns the cost for researching a technology
		/// </summary>
		/// <param name="playerRaceId">Unused, in case an expansion will modify the fixed cost of 4k</param>
		public static ResourcesCost ResearchAdvancementCost(Race playerRaceId)
		{
			return new ResourcesCost(new Resources { Knowledge = 4 });
		}
	}
}
