using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Converters;

internal class EmptyStringToNullableIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return int.MinValue;

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
                return int.MinValue; // empty string → null

            if (int.TryParse(str, out var value))
                return value;

            throw new JsonException($"Invalid number: {str}");
        }

        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt32();

        throw new JsonException($"Unexpected token: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
