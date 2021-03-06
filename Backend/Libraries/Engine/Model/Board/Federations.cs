using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class Federations
	{
		[BsonNoId]
		public class FederationTokenPile
		{
			public FederationTokenType Type { get; set; }
			public int InitialQuantity { get; set; }
			public int Remaining { get; set; }
		}

		public List<FederationTokenPile> Tokens { get; set; }
	}
}
