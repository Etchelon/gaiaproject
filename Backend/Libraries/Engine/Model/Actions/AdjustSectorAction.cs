using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(AdjustSectorAction))]
	public class AdjustSectorAction : PlayerAction
	{
		public override ActionType Type => ActionType.AdjustSector;
		public int SectorId { get; set; }
		public int Rotation { get; set; }
	}
}