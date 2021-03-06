using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(SelectFederationTokenToScoreDecision))]
	public class SelectFederationTokenToScoreDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.SelectFederationTokenToScore;
		public override string Description => "must select which Federation token to score";
		public List<FederationTokenType> AvailableTokens { get; set; }

		public SelectFederationTokenToScoreDecision() { }

		public SelectFederationTokenToScoreDecision(List<FederationTokenType> availableTokens)
		{
			AvailableTokens = availableTokens;
		}
	}
}