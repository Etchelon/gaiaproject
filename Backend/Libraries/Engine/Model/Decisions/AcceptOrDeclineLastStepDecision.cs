using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Actions;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(AcceptOrDeclineLastStepDecision))]
	public class AcceptOrDeclineLastStepDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.AcceptOrDeclineLastStep;
		public ResearchTrackType Track { get; set; }

		public override string Description => $"must decide whether to advance in track {Track.ToDescription()}";

		public static AcceptOrDeclineLastStepDecision Construct(PlayerAction action, ResearchTrackType track)
		{
			return new AcceptOrDeclineLastStepDecision
			{
				SpawnedFromActionId = action.Id,
				PlayerId = action.PlayerId,
				Track = track
			};
		}
	}
}
