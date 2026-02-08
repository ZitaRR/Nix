using Nix.Infrastructure.Converters;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Teams.Private;

public record PlayerDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string Surname,
    [property: JsonPropertyName("jerseyNumber"), JsonConverter(typeof(EmptyStringToNullableIntConverter))] int Number,
    [property: JsonPropertyName("renderedLatestPortrait")] PortraitDto Portrait);
