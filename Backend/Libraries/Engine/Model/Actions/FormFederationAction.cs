using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(FormFederationAction))]
	public class FormFederationAction : PlayerAction
	{
		public override ActionType Type => ActionType.FormFederation;
		public List<string> SelectedBuildings { get; set; }
		public List<string> SelectedSatellites { get; set; }
		public FederationTokenType SelectedFederationToken { get; set; }

		public override string ToString()
		{
			return $"forms a federation";
		}
	}
}