using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Settings.Internal.Private;

internal record SettingsDto([property: JsonPropertyName("config")] ConfigDto Config);
