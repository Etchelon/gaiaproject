using System.Collections.Generic;
using System.Linq;

namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public abstract class CompositeGain : Gain
	{
		public List<Gain> Gains { get; } = new List<Gain>();

		public void Add(params Gain[] gains)
		{
			foreach (var gain in gains)
			{
				gain.LinkToGain(this);
				Gains.Add(gain);
			}
		}

		public override string ToString()
		{
			return string.Join("\n", Gains.Select(g => g.ToString()).ToArray());
		}
	}
}
