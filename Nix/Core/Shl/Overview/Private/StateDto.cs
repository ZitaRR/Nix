using Nix.Infrastructure.Converters;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Overview.Private;

[JsonConverter(typeof(MatchStateConverter))]
internal enum StateDto
{
    Played,
    NotPlayed
}
