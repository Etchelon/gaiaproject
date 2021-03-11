namespace GaiaProject.Common.Bus
{
	public class OperationCompleteEvent : IEvent
	{
		public string OperationId { get; set; }

		public OperationCompleteEvent(string operationId)
		{
			this.OperationId = operationId;
		}
	}
}
