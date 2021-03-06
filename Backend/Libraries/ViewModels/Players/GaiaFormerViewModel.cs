
namespace GaiaProject.ViewModels.Players
{
	public class GaiaformerViewModel
	{
		private int _id;
		public int Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private bool _available;
		public bool Available
		{
			get => _available;
			set
			{
				if (_available == value) return;
				_available = value;
			}
		}

		private bool _spentInGaiaArea;
		public bool SpentInGaiaArea
		{
			get => _spentInGaiaArea;
			set
			{
				if (_spentInGaiaArea == value) return;
				_spentInGaiaArea = value;
			}
		}

		private string _onHexId;
		public string OnHexId
		{
			get => _onHexId;
			set
			{
				if (_onHexId == value) return;
				_onHexId = value;
			}
		}
	}
}