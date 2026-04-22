using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Seasons.Private;

internal record NamesDto([property: JsonPropertyName("translation")] string SeasonYear);