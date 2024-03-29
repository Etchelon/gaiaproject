﻿using System.Collections.Generic;
using GaiaProject.Common.Database;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Setup;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Engine.Model
{
	[CollectionName("GaiaProject.InitialGameStates")]
	public class InitialGaiaProjectGameState : MongoEntity
	{
		public string GameId { get; set; }
		public List<PlayerInGame> Players { get; set; }
		public BoardState BoardState { get; set; }
		public SetupPhase Setup { get; set; }
	}
}