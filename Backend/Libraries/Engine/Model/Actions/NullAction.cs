using System;
using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Model.Actions
{
	public class NullAction : PlayerAction
	{
		public override ActionType Type => throw new NotImplementedException();
	}
}