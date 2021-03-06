using System.Linq;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class ChangePowerValueOfFederationEffect : Effect
	{
		public string FederationId { get; }
		public int Variation { get; }

		public ChangePowerValueOfFederationEffect(string federationId, int variation)
		{
			FederationId = federationId;
			Variation = variation;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var federation = player.State.Federations.Single(fed => fed.Id == FederationId);
			federation.TotalPowerValue += Variation;
		}
	}
}