using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
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

			// If the federation has no satellites, mark one of the buildings with the Federation Marker
			var buildings = FederatedHexes
				.SelectMany(h => h.Buildings)
				.Where(b => b.PlayerId == PlayerId)
				.ToList();
			if (buildings.Any(b => b.Type == BuildingType.Satellite || b.Type == BuildingType.IvitsSpaceStation))
			{
				return;
			}
			var buildingToMark = buildings.OrderByDescending(b => b.PowerValueInFederation).First();
			var hex = game.BoardState.Map.Hexes.Single(h => h.Id == buildingToMark.HexId);
			hex.Buildings.Single(b => b.Id == buildingToMark.Id).ShowFederationMarker = true;
		}
	}
}