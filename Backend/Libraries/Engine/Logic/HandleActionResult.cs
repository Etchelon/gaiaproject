using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic
{
	public class HandleActionResult
	{
		public bool Handled { get; private set; }
		public string ErrorMessage { get; private set; }
		public GaiaProjectGame NewState { get; set; }

		private HandleActionResult() { }

		public static HandleActionResult Ok(GaiaProjectGame gameState)
		{
			return new HandleActionResult
			{
				Handled = true,
				NewState = gameState
			};
		}

		public static HandleActionResult Failure(string message)
		{
			return new HandleActionResult
			{
				Handled = false,
				ErrorMessage = message
			};
		}
	}
}
