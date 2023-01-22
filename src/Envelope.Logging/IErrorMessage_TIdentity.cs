namespace Envelope.Logging;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IErrorMessage<TIdentity> : ILogMessage<TIdentity>, Serializer.IDictionaryObject
	where TIdentity : struct
{
}
