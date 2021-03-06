namespace ScoreSheets.Common.Bus
{
	public class OperationStartedEvent : IEvent
	{
		public string OperationId { get; set; }

		public OperationStartedEvent(string operationId)
		{
			this.OperationId = operationId;
		}
	}
}
