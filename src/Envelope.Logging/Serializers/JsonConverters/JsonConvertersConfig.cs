#if NET6_0_OR_GREATER
using Envelope.Extensions;
using System.Text.Json.Serialization;

namespace Envelope.Logging.Serializers.JsonConverters;

public static class JsonConvertersConfig
{
	public static void AddLoggingReadConverters(IList<JsonConverter> converters)
	{
		if (converters == null)
			throw new ArgumentNullException(nameof(converters));

		Envelope.Serializer.JsonConverters.JsonConvertersConfig.AddCoreReadConverters(converters);
		converters.AddUniqueItem(new LogMessageJsonConverter());
		converters.AddUniqueItem(new ErrorMessageJsonConverter());
	}
}
#endif
