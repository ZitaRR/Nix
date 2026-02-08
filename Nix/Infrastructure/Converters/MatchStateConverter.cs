using Nix.Core.Shl.Overview.Private;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Converters;

internal class MatchStateConverter : JsonConverter<StateDto>
{
    public override StateDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "post-game" => StateDto.Played,
            "pre-game" => StateDto.NotPlayed,
            _ => throw new JsonException($"Unknown State: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, StateDto value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            StateDto.Played => "post-game",
            StateDto.NotPlayed => "pre-game",
            _ => throw new JsonException()
        };
        writer.WriteStringValue(str);
    }
}
