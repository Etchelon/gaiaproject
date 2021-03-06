using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class FederationToken
	{
		public FederationTokenType Type { get; set; }
		public bool UsedForTechOrAdvancedTile { get; set; }

		[BsonIgnoreIfNull]
		public string FederationId { get; set; }

		public FederationToken Clone()
		{
			return new FederationToken
			{
				Type = Type,
				FederationId = FederationId,
				UsedForTechOrAdvancedTile = UsedForTechOrAdvancedTile,
			};
		}

		public static FederationToken FromFederation(string federationId, FederationTokenType type)
		{
			return new FederationToken
			{
				Type = type,
				FederationId = federationId,
				UsedForTechOrAdvancedTile = type == FederationTokenType.Points
			};
		}

		public static FederationToken FromBonus(FederationTokenType type)
		{
			return new FederationToken
			{
				Type = type,
				UsedForTechOrAdvancedTile = type == FederationTokenType.Points
			};
		}
	}
}