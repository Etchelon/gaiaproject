using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Entities.Effects.Costs;
using GaiaProject.Engine.Logic.Entities.Effects.Gains;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Board;
using GaiaProject.Engine.Model.Rounds;
using GaiaProject.Engine.Model.Setup;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using ScoreSheets.Common.Database;

namespace GaiaProject.Engine.Model
{
	[CollectionName("GaiaProject.Games")]
	public class GaiaProjectGame : MongoEntity
	{
		public const int LastRound = 6;

		public string Name { get; set; }
		// Id of the User that created this game
		public string CreatedBy { get; set; }
		public DateTime Created { get; set; }
		public DateTime? Ended { get; set; }
		public List<PlayerInGame> Players { get; set; }
		public GamePhase CurrentPhaseId { get; set; }
		public GameOptions Options { get; set; }

		[BsonIgnoreIfNull]
		public BoardState BoardState { get; set; }

		[BsonIgnoreIfNull]
		public SetupPhase Setup { get; set; }

		[BsonIgnoreIfNull]
		public RoundsPhase Rounds { get; set; }

		public int CurrentTurn { get; set; }

		[BsonIgnoreIfNull]
		public List<PlayerAction> Actions = new List<PlayerAction>();

		[BsonIgnoreIfNull]
		public List<GameLog> GameLogs { get; set; } = new List<GameLog>();

		[BsonIgnore]
		public string CurrentPlayerId => CurrentPlayer?.Id;

		[BsonIgnore]
		public PlayerInGame CurrentPlayer => Players.SingleOrDefault(p => p.Actions.IsCurrentPlayer);

		[BsonIgnore]
		public string ActivePlayerId => PendingDecisions.FirstOrDefault()?.PlayerId
			?? CurrentPlayerId;

		[BsonIgnore]
		public PlayerInGame ActivePlayer => Players.SingleOrDefault(p => p.Id == ActivePlayerId);

		[BsonIgnore]
		public IEnumerable<PendingDecision> PendingDecisions => Players
			.Where(p => p.Actions.ActivationState == ActivationState.WaitingForDecision)
			.OrderBy(p => TurnOrderUtils.GetTurnOrderRelativeTo(p.Id, CurrentPlayerId, this))
			.Select(p => p.Actions.PendingDecision)
			.Where(pd => pd != null);

		#region Logs (new)

		private void AddGameLog(GameLog log)
		{
			var parentLog = GameLogs.SingleOrDefault(gl => gl.ActionId > 0 && gl.ActionId == log.ActionId);
			if (parentLog != null)
			{
				(parentLog.SubLogs ?? (parentLog.SubLogs = new List<GameLog>())).Add(log);
				return;
			}
			GameLogs.Add(log);
		}

		private GameLog LogForPlayer(string playerId, string message, int? actionId = null)
		{
			var player = GetPlayer(playerId);
			return new GameLog
			{
				Turn = CurrentTurn,
				Timestamp = DateTime.Now,
				Message = message,
				ActionId = actionId,
				PlayerId = playerId,
				Race = player.RaceId,
			};
		}

		public void LogSystemMessage(string message, bool important = false)
		{
			var log = new GameLog
			{
				Timestamp = DateTime.Now,
				Message = message,
				Important = important
			};
			AddGameLog(log);
		}

		public void LogAction(PlayerAction action)
		{
			var message = action.ToString();
			var log = LogForPlayer(action.PlayerId, message, action.Id);

			if (action.SpawnedFromActionId.HasValue)
			{
				var parentLog = GameLogs.Single(gl => gl.ActionId == action.SpawnedFromActionId.Value);
				(parentLog.SubLogs ?? (parentLog.SubLogs = new List<GameLog>())).Add(log);
				return;
			}

			GameLogs.Add(log);
		}

		public void LogEffect(Effect effect, string message)
		{
			if (!effect.Loggable)
			{
				return;
			}
			var log = LogForPlayer(effect.PlayerId, message, effect.ActionId);
			AddGameLog(log);
		}

		public void LogGain(Gain gain)
		{
			LogEffect(gain, $"gains {gain}");
		}

		public void LogCost(Cost cost)
		{
			LogEffect(cost, $"spends {cost}");
		}

		public void LogPowerReturnsInGaiaPhase(PlayerInGame player, int power, bool hasMovedBrainstone)
		{
			var message = $"moves {power} power {(hasMovedBrainstone ? " and brainstone" : "")} from Gaia Area to Bowl {(player.RaceId == Race.Terrans ? 2 : 1)}";
			var log = LogForPlayer(player.Id, message);
			AddGameLog(log);
		}

		public void LogPlayerMessage(PlayerInGame player, string message)
		{
			var log = LogForPlayer(player.Id, message);
			AddGameLog(log);
		}

		#endregion

		public GaiaProjectGame Clone()
		{
			return new GaiaProjectGame
			{
				Id = Id,
				Name = Name,
				CreatedBy = CreatedBy,
				Created = Created,
				Ended = Ended,
				Players = Players.Select(p => p.Clone()).ToList(),
				CurrentPhaseId = CurrentPhaseId,
				Options = Options?.Clone(),
				BoardState = BoardState?.Clone(),
				Setup = Setup?.Clone(),
				Rounds = Rounds?.Clone(),
				Actions = Actions.ToList(),
				GameLogs = GameLogs.Select(l => l.Clone()).ToList(),
				CurrentTurn = CurrentTurn,
			};
		}

		public PlayerInGame GetPlayer(string id)
		{
			return Players.Single(p => p.Id == id);
		}
	}
}