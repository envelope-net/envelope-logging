#if NET6_0_OR_GREATER
using Envelope.Logging.Serializers.JsonConverters;
using System.Text.Json;

namespace Envelope.Extensions;

public static class JsonSerializerOptionsExtensions
{
	public static JsonSerializerOptions AddLoggingReadConverters(this JsonSerializerOptions options)
	{
		if (options == null)
			throw new ArgumentNullException(nameof(options));

		JsonConvertersConfig.AddLoggingReadConverters(options.Converters);
		return options;
	}
}
#endif
