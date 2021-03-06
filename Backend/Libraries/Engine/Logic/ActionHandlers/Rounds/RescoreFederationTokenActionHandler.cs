using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Decisions;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class RescoreFederationTokenActionHandler : ActionHandlerBase<RescoreFederationTokenAction>
	{
		protected override List<Effect> HandleImpl(GaiaProjectGame game, RescoreFederationTokenAction action)
		{
			var effects = new List<Effect>
			{
				new PendingDecisionEffect(new PerformConversionOrPassTurnDecision())
			};

			var tokenGains = FederationTokenUtils.GetFederationTokenGain(action.Token, Game, true);
			effects.AddRange(tokenGains.Gains);
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, RescoreFederationTokenAction action)
		{
			if (!HasToken(action.Token))
			{
				return (false, "You don't have the selected token and therefore you cannot rescore it");
			}
			return (true, null);
		}

		private bool HasToken(FederationTokenType token)
		{
			return Player.State.FederationTokens.Any(t => t.Type == token);
		}
	}
}