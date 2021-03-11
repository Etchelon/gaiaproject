using GaiaProject.Common.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class BoardState
	{
		public Map Map { get; set; }
		public ScoringBoard ScoringBoard { get; set; }
		public ResearchBoard ResearchBoard { get; set; }
		public RoundBoosters RoundBoosters { get; set; }
		public Federations Federations { get; set; }

		public BoardState Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}
