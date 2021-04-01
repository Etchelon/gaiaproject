using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(AdjustSectorsAction))]
	public class AdjustSectorsAction : PlayerAction
	{
		public override ActionType Type => ActionType.AdjustSectors;
		public List<SectorAdjustment> Adjustments { get; set; }

		public class SectorAdjustment
		{
			public string SectorId { get; set; }
			public int Rotation { get; set; }
		}
	}
}