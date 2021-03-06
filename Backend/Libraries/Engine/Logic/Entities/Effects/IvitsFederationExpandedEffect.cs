using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class FederationCreatedEffect : Effect
	{
		public List<Hex> FederatedHexes { get; }

		public FederationCreatedEffect(List<Hex> federatedHexes)
		{
			FederatedHexes = federatedHexes;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var counter = player.State.Federations.Count() + 1;
			var newFederation = Federation.FromBuildings(PlayerId, counter, FederatedHexes);
			player.State.Federations.Add(newFederation);
		}
	}
}