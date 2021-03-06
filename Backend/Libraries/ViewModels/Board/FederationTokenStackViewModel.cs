using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class FederationTokenStackViewModel
	{
		private FederationTokenType _type;
		public FederationTokenType Type
		{
			get => _type;
			set
			{
				if (_type == value) return;
				_type = value;
			}
		}

		private int _initialQuantity;
		public int InitialQuantity
		{
			get => _initialQuantity;
			set
			{
				if (_initialQuantity == value) return;
				_initialQuantity = value;
			}
		}

		private int _remaining;
		public int Remaining
		{
			get => _remaining;
			set
			{
				if (_remaining == value) return;
				_remaining = value;
			}
		}
	}
}