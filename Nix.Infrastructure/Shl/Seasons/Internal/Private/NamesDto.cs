using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal record NamesDto([property: JsonPropertyName("translation")] string SeasonYear);