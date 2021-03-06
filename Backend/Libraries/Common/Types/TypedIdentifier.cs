namespace ScoreSheets.Common.Types
{
	public abstract class TypedIdentifier<T>
	{
		protected T Id { get; set; }
		protected TypedIdentifier() { }
	}
}
