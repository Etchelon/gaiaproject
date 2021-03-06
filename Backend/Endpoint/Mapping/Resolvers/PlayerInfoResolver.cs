using System;
using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Model;
using GaiaProject.ViewModels.Players;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class PlayerInfoResolver : IMemberValueResolver<object, object, string, PlayerInfoViewModel>
	{
		public PlayerInfoViewModel Resolve(object source, object destination, string playerId, PlayerInfoViewModel player, ResolutionContext context)
		{
			if (playerId == null)
			{
				return null;
			}
			var game = context.Items["Game"] as GaiaProjectGame;
			if (game == null)
			{
				throw new ArgumentException($"Game must be passed to PlayerInGameResolver");
			}
			var playerInGame = game.Players.Single(p => p.Id == playerId);
			return context.Mapper.Map<PlayerInfoViewModel>(playerInGame);
		}
	}
}
