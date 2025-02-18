using System.Text.Json;
using System.Text.Json.Serialization;

namespace FB98.Shared.Utils.Extensions
{
	public class GuidConverterExtensions : JsonConverter<Guid>
	{
		public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return Guid.TryParse(reader.GetString(), out var guid) ? guid : Guid.Empty;
		}

		public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
