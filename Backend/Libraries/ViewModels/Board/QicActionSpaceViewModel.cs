using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class QicActionSpaceViewModel : IActionSpaceViewModel<QicActionType>
	{
		public string Kind => "qic";

		private QicActionType _type;
		public QicActionType Type
		{
			get => _type;
			set
			{
				if (_type == value) return;
				_type = value;
			}
		}

		private bool _isAvailable;
		public bool IsAvailable
		{
			get => _isAvailable;
			set
			{
				if (_isAvailable == value) return;
				_isAvailable = value;
			}
		}
	}
}