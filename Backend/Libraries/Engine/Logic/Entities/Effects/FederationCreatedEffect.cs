using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class IvitsFederationExpandedEffect : Effect
	{
		public List<Hex> FederatedHexes { get; }

		public IvitsFederationExpandedEffect(List<Hex> federatedHexes)
		{
			FederatedHexes = federatedHexes;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var federation = player.State.Federations.Single();
			federation.HexIds.AddRange(FederatedHexes.Select(h => h.Id));
			federation.NumBuildings += FederatedHexes.Count(h => h.Buildings.Any());
			federation.TotalPowerValue += FederatedHexes
				.Select(h => h.Buildings.SingleOrDefault(b => b.PlayerId == PlayerId)?.PowerValueInFederation ?? 0)
				.Sum();
		}
	}
}