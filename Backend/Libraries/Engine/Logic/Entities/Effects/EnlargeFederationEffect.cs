using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class EnlargeFederationEffect : Effect
	{
		public string FederationId { get; }
		public string ConnectingHexId { get; }
		public List<List<Hex>> AdditionalClusters { get; }

		public EnlargeFederationEffect(string federationId, string connectingHexId, List<List<Hex>> additionalClusters)
		{
			FederationId = federationId;
			ConnectingHexId = connectingHexId;
			AdditionalClusters = additionalClusters;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var connectingHex = game.BoardState.Map.Hexes.Single(h => h.Id == ConnectingHexId);
			var federation = player.State.Federations.Single(fed => fed.Id == FederationId);
			federation.Add(PlayerId, connectingHex);
			AdditionalClusters?.ForEach(cluster => federation.Enlarge(PlayerId, cluster));
		}
	}
}