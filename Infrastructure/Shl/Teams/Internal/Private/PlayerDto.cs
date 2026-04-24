using Nix.Infrastructure.Converters.Internal;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal record PlayerDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string Surname,
    [property: JsonPropertyName("jerseyNumber"), JsonConverter(typeof(EmptyStringToNullableIntConverter))] int Number,
    [property: JsonPropertyName("renderedLatestPortrait")] PortraitDto Portrait);
