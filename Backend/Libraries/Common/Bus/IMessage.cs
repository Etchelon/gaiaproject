namespace GaiaProject.Common.Bus
{
	public interface IMessage
	{
	}

	public interface ICommand : IMessage
	{
	}

	public interface IEvent : IMessage
	{
	}

	public interface IInternalEvent : IMessage
	{
	}

	public interface IExternalEvent : IMessage
	{
	}
}
