using System;
using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class TechnologyTrackBonusTakenEffect : Effect
	{
		public ResearchTrackType Track { get; }

		public TechnologyTrackBonusTakenEffect(ResearchTrackType track)
		{
			Track = track;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			var track = game.BoardState.ResearchBoard.Tracks.Single(t => t.Id == Track);

			string message;
			switch (Track)
			{
				default:
					throw new ArgumentOutOfRangeException(nameof(Track), $"Track {Track} does not have a reward at the top of the track");
				case ResearchTrackType.Terraformation:
					track.IsFederationTokenAvailable = false;
					message = $"takes the federation token on the Terraformation track";
					break;
				case ResearchTrackType.Navigation:
					track.IsLostPlanetAvailable = false;
					message = $"takes the Lost Planet";
					break;
			}
			game.LogEffect(this, message);
		}
	}
}