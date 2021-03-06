using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class FederationTokenTakenEffect : Effect
	{
		public FederationTokenType Token { get; }

		public FederationTokenTakenEffect(FederationTokenType token)
		{
			Token = token;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var pile = game.BoardState.Federations.Tokens.Single(p => p.Type == Token);
			pile.Remaining -= 1;
		}
	}
}