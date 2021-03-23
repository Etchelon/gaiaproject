using System.Threading.Tasks;
using GaiaProject.Endpoint.WorkerServices;
using GaiaProject.Engine.Commands;
using GaiaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GaiaProject.Endpoint.Controllers
{
    public class GaiaProjectController : ApiBaseController
    {
        private readonly ILogger<GaiaProjectController> _logger;
        private readonly GamesWorkerService _workerService;

        public GaiaProjectController(ILogger<GaiaProjectController> logger, GamesWorkerService workerService)
        {
            _logger = logger;
            _workerService = workerService;
        }

        [HttpGet]
        public async Task<ActionResult<GameInfoViewModel[]>> GetUserGames(string userId, string kind)
        {
            userId ??= User.Id;
            bool onlyWaitingForAction = kind == "waiting";
            bool finished = kind == "finished";
            var games = finished
                ? await _workerService.GetUserFinishedGames(userId)
                : await _workerService.GetUserGames(userId, onlyWaitingForAction);
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameStateViewModel>> GetGame(string id)
        {
            var game = await _workerService.GetGame(id, UserId);
            return Ok(game);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> RollbackGameAtAction(string id, int actionId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Game Id must be specified");
            }
            if (actionId <= 0)
            {
                return BadRequest("Action Id must be greater than 0");
            }

            var game = await _workerService.GetGame(id, UserId);
            if (UserId != game.CreatedBy.Id)
            {
                return Unauthorized("Only the player who created the game can rollback the state to a certain action.");
            }

            await _workerService.RollbackGameAtAction(id, actionId, true);
            return NoContent();
        }

        [HttpPost]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> CreateGame([FromBody] CreateGameCommand command)
        {
            var gameId = await _workerService.CreateGame(command, UserId);
            return Ok(gameId);
        }

        [HttpPost("{gameId}")]
        public async Task<ActionResult> Action(string gameId, [FromBody] JToken action)
        {
            var result = await _workerService.HandleAction(gameId, UserId, action);
            return Ok(new { result.Handled, result.ErrorMessage });
        }

        #region Notes

        [HttpGet("{gameId}")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> GetNotes(string gameId)
        {
            var notes = await _workerService.GetPlayerNotes(UserId, gameId);
            return Ok(notes);
        }

        public class SaveNotesData
        {
            public string Notes { get; set; }
        }

        [HttpPut("{gameId}")]
        public async Task<IActionResult> SaveNotes(string gameId, [FromBody] SaveNotesData data)
        {
            await _workerService.SavePlayerNotes(UserId, gameId, data.Notes);
            return NoContent();
        }

        #endregion
    }
}
