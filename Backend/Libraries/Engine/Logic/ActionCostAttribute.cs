using System;

namespace GaiaProject.Engine.Logic
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ActionCostAttribute : Attribute
	{
		public int Cost { get; }

		public ActionCostAttribute(int cost)
		{
			Cost = cost;
		}
	}
}