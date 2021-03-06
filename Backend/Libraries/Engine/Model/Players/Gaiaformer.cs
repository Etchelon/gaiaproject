using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class Gaiaformer
	{
		public int Id { get; set; }
		public bool Unlocked { get; set; }
		public bool Available { get; set; }

		[BsonIgnoreIfDefault]
		public bool SpentInGaiaArea { get; set; }

		[BsonIgnoreIfNull]
		public string OnHexId { get; set; }

		internal static List<Gaiaformer> AllToUnlock()
		{
			return new List<Gaiaformer>
			{
				new Gaiaformer { Id = 1 },
				new Gaiaformer { Id = 2 },
				new Gaiaformer { Id = 3 },
			};
		}

		internal Gaiaformer Clone()
		{
			return new Gaiaformer
			{
				Id = Id,
				Unlocked = Unlocked,
				Available = Available,
				SpentInGaiaArea = SpentInGaiaArea,
				OnHexId = OnHexId
			};
		}
	}
}