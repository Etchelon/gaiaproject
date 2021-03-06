using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class QicActionSpace
	{
		public QicActionType Type { get; set; }
		public bool IsAvailable { get; set; }
	}
}
