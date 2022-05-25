namespace Envelope.Logging;

public interface IErrorMessage<TIdentity> : ILogMessage<TIdentity>, Serializer.IDictionaryObject
	where TIdentity : struct
{
}

public interface IErrorMessage : IErrorMessage<Guid>, ILogMessage<Guid>, Serializer.IDictionaryObject
{
}
