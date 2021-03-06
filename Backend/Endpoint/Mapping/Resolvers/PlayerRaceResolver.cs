using System;
using System.Linq;
using AutoMapper;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class PlayerRaceResolver : IMemberValueResolver<object, object, string, Race>
	{
		public Race Resolve(object source, object destination, string playerId, Race raceId, ResolutionContext context)
		{
			var game = context.Items["Game"] as GaiaProjectGame;
			if (game == null)
			{
				throw new ArgumentException($"Game must be passed to PlayerRaceResolver");
			}
			var player = game.Players.Single(p => p.Id == playerId);
			return player.RaceId ?? Race.None;
		}
	}
}
