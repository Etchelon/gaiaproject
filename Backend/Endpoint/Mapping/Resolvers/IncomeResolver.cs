using System.Diagnostics;
using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class IncomeResolver : IValueResolver<PlayerState, PlayerStateViewModel, IncomeViewModel>
	{
		public IncomeViewModel Resolve(PlayerState source, PlayerStateViewModel destination, IncomeViewModel destMember, ResolutionContext context)
		{
			var player = context.Items["Player"] as PlayerInGame;
			Debug.Assert(player != null, "Player must be provided in the resolution context.");
			var incomes = source.Incomes;
			// Only show incomes from the round booster after the player has passed
			// During the round it would trick the player to think he has income from the round booster
			// as he would return it at the end of the round
			if (!player.HasPassed)
			{
				incomes = incomes.Where(inc => inc.SourceType != IncomeSource.RoundBooster).ToList();
			}
			var resourceIncomes = incomes.OfType<ResourceIncome>().ToArray();
			return new IncomeViewModel
			{
				Credits = resourceIncomes.Aggregate(0, (acc, income) => acc + income.Credits),
				Ores = resourceIncomes.Aggregate(0, (acc, income) => acc + income.Ores),
				Knowledge = resourceIncomes.Aggregate(0, (acc, income) => acc + income.Knowledge),
				Qic = resourceIncomes.Aggregate(0, (acc, income) => acc + income.Qic),
				Power = incomes.OfType<PowerIncome>().Aggregate(0, (acc, income) => acc + income.Power),
				PowerTokens = incomes.OfType<PowerTokenIncome>().Aggregate(0, (acc, income) => acc + income.PowerTokens)
			};
		}
	}
}
