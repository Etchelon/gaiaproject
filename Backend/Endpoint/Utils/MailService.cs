using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GaiaProject.Endpoint.Utils
{
	public class MailService
	{
		private readonly ISendGridClient _sendGrid;

		public MailService(ISendGridClient sendGrid)
		{
			_sendGrid = sendGrid;
		}

		public async Task Send(SendGridMessage message)
		{
			await _sendGrid.SendEmailAsync(message);
		}
	}
}