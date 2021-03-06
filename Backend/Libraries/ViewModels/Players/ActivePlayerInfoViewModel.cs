using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.AvailableActions;
using GaiaProject.ViewModels.Decisions;

namespace GaiaProject.ViewModels.Players
{
	public class ActivePlayerInfoViewModel
	{
		private string _id;
		public string Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private string _username;
		public string Username
		{
			get => _username;
			set
			{
				if (_username == value) return;
				_username = value;
			}
		}

		private Race? _raceId;
		public Race? RaceId
		{
			get => _raceId;
			set
			{
				if (_raceId == value) return;
				_raceId = value;
			}
		}

		private string _reason;
		public string Reason
		{
			get => _reason;
			set
			{
				if (_reason == value) return;
				_reason = value;
			}
		}

		private List<AvailableActionViewModel> _availableActions;
		public List<AvailableActionViewModel> AvailableActions
		{
			get => _availableActions;
			set
			{
				if (_availableActions == value) return;
				_availableActions = value;
			}
		}

		private PendingDecisionViewModel _pendingDecision;

		public PendingDecisionViewModel PendingDecision
		{
			get => _pendingDecision;
			set
			{
				if (_pendingDecision == value) return;
				_pendingDecision = value;
			}
		}
	}
}