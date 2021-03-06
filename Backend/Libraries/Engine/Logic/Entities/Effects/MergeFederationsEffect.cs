using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class MergeFederationsEffect : Effect
	{
		public Hex ConnectingHex { get; }
		public List<string> FederationIds { get; }
		public List<List<Hex>> AdditionalClusters { get; }

		public MergeFederationsEffect(Hex connectingHex, List<string> federationIds, List<List<Hex>> additionalCluster = null)
		{
			ConnectingHex = connectingHex;
			FederationIds = federationIds;
			Debug.Assert(FederationIds.Count >= 2);
			AdditionalClusters = additionalCluster;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var federationToEnlarge = player.State.Federations.Single(fed => fed.Id == FederationIds.First());
			var otherFederationIds = FederationIds.Skip(1);
			var otherFederations = player.State.Federations.Where(fed => otherFederationIds.Contains(fed.Id)).ToList();
			otherFederations.ForEach(fed =>
			{
				federationToEnlarge.HexIds.AddRange(fed.HexIds);
				federationToEnlarge.TotalPowerValue += fed.TotalPowerValue;
				federationToEnlarge.NumBuildings += fed.NumBuildings;
			});

			// Add the connecting hex
			federationToEnlarge.Add(PlayerId, ConnectingHex);
			// Add additional clusters
			AdditionalClusters?.ForEach(cluster => federationToEnlarge.Enlarge(PlayerId, cluster));
			// Remove the merged federations
			player.State.FederationTokens
				.Where(token => token.FederationId != null && otherFederationIds.Contains(token.FederationId))
				.ToList()
				.ForEach(token => token.FederationId = federationToEnlarge.Id);
			player.State.Federations.RemoveAll(fed => otherFederationIds.Contains(fed.Id));
		}
	}
}