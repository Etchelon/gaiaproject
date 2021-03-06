using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class EnlargeFederationEffect : Effect
	{
		public string FederationId { get; }
		public Hex ConnectingHex { get; }
		public List<List<Hex>> AdditionalClusters { get; }

		public EnlargeFederationEffect(string federationId, Hex connectingHex, List<List<Hex>> additionalClusters)
		{
			FederationId = federationId;
			ConnectingHex = connectingHex;
			AdditionalClusters = additionalClusters;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var federation = player.State.Federations.Single(fed => fed.Id == FederationId);
			federation.Add(PlayerId, ConnectingHex);
			AdditionalClusters?.ForEach(cluster => federation.Enlarge(PlayerId, cluster));
		}
	}
}