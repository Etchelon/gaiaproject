using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Board
{
	[BsonNoId]
	public class PowerActionSpace
	{
		public PowerActionType Type { get; set; }
		public bool IsAvailable { get; set; }
	}
}
