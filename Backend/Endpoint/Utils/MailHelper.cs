using System.Linq;
using GaiaProject.Core.Model;
using GaiaProject.Endpoint.Shared;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.ViewModels;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GaiaProject.Endpoint.Utils
{
	public class MailHelper
	{
		private readonly string _reactFrontendUrl;

		public MailHelper(string reactFrontendUrl)
		{
			_reactFrontendUrl = reactFrontendUrl;
		}

		public SendGridMessage GetEmail(User recipient, GameStateViewModel game, NotificationReason reason)
		{
			var mail = new SendGridMessage();
			mail.SetFrom(new EmailAddress("gaiaproject@donotreply.com", "The GaiaProject online team"));
			mail.AddTo(new EmailAddress(recipient.Email, $"{recipient.FirstName} {recipient.LastName}"));
			var (subject, content) = reason switch
			{
				NotificationReason.GameStarted => GetYourTurnMessage(recipient, game),
				NotificationReason.YourTurn => GetYourTurnMessage(recipient, game),
				NotificationReason.GameEnded => GetGameEndedMessage(recipient, game),
				NotificationReason.GameDeleted => GetGameDeletedMessage(recipient, game),
			};
			mail.SetSubject(subject);
			mail.AddContent(MimeType.Html, content);
			return mail;
		}

		private (string subject, string content) GetYourTurnMessage(User recipient, GameStateViewModel game)
		{
			var subject = $"Your turn to move in Gaia Project game {game.Name}";

			var activePlayer = game.ActivePlayer;
			var otherPlayersNames = game.Players.Where(p => p.Id != recipient.Id).Select(p => p.Username);
			var content = @$"
<h1>You {activePlayer.Reason}</h1>
<p>It's your turn to play in game {game.Name} against {string.Join(", ", otherPlayersNames)}.</p>
<br>
<a href='{_reactFrontendUrl}game/{game.Id}'>Click here to go to the app and make your move.</a>
";
			return (subject, content);
		}

		private (string subject, string content) GetGameEndedMessage(User recipient, GameStateViewModel game)
		{
			var subject = $"The Gaia Project game {game.Name} has ended";

			var targetPlayer = game.Players.Single(p => p.Id == recipient.Id);
			var content = @$"
<h1>You ended up in {targetPlayer.Placement}° position</h1>
<p>Here is the final situation.</p>
<br>
{string.Join("<br>", game.Players.Select(p => $"{p.Placement}°: {p.Username} ({RaceUtils.GetName(p.RaceId)}) - {p.State.Points} pts").ToArray())}
<br>
<a href='{_reactFrontendUrl}game/{game.Id}'>Click here to go check out the game.</a>
";
			return (subject, content);
		}

		private (string subject, string content) GetGameDeletedMessage(User recipient, GameStateViewModel game)
		{
			var subject = $"The Gaia Project game {game.Name} has been deleted";

			var gameCreator = game.CreatedBy.Username;
			var content = @$"
<h1>{gameCreator} has decided to delete the game.</h1>
<p>The game won't be visible any longer in your list, nor in the history of your played games.</p>
";
			return (subject, content);
		}
	}
}