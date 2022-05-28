namespace Envelope.Logging;

public interface IErrorMessage<TIdentity> : ILogMessage<TIdentity>, Serializer.IDictionaryObject
	where TIdentity : struct
{
}
