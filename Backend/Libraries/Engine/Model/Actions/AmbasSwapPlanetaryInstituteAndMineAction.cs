using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(AmbasSwapPlanetaryInstituteAndMineAction))]
	public class AmbasSwapPlanetaryInstituteAndMineAction : PlayerAction
	{
		public override ActionType Type => ActionType.AmbasSwapPlanetaryInstituteAndMine;
		public string HexId { get; set; }

		public override string ToString()
		{
			return "swaps the planetary institute with a mine";
		}
	}
}