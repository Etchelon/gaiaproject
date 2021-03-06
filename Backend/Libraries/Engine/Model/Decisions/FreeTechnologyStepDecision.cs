using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Decisions
{
	[BsonDiscriminator(nameof(FreeTechnologyStepDecision))]
	public class FreeTechnologyStepDecision : PendingDecision
	{
		public override PendingDecisionType Type => PendingDecisionType.FreeTechnologyStep;
		public override string Description => "must decide which technology to research";

		[BsonIgnoreIfNull]
		public List<ResearchTrackType> ResearchableTechnologies { get; set; }

		public FreeTechnologyStepDecision() { }

		public FreeTechnologyStepDecision(List<ResearchTrackType> researchableTechnologies = null)
		{
			ResearchableTechnologies = researchableTechnologies;
		}
	}
}