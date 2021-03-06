using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(AcceptOrDeclineLastStepAction))]
	public class AcceptOrDeclineLastStepAction : PlayerAction
	{
		public override ActionType Type => ActionType.AcceptOrDeclineLastStep;
		public bool Accepted { get; set; }
		public ResearchTrackType Track { get; set; }

		public override string ToString()
		{
			return $"advances to the last step of technology {Track.ToDescription()}";
		}
	}
}