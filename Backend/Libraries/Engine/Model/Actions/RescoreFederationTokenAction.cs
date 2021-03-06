using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(RescoreFederationTokenAction))]
	public class RescoreFederationTokenAction : PlayerAction
	{
		public override ActionType Type => ActionType.RescoreFederationToken;
		public FederationTokenType Token { get; set; }

		public override string ToString()
		{
			return $"rescores federation token {Token.ToDescription()}";
		}
	}
}