using System;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class RawStateChangeEffect : Effect
	{
		public Func<ActionContext, (GaiaProjectGame newState, string description)> ChangeFn { get; }

		public RawStateChangeEffect(Func<ActionContext, (GaiaProjectGame newState, string description)> changeFn)
		{
			ChangeFn = changeFn;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			throw new NotImplementedException();
		}
	}
}
