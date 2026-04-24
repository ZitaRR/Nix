using Nix.Infrastructure.Converters.Internal;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

[JsonConverter(typeof(MatchStateConverter))]
internal enum StateDto
{
    Played,
    NotPlayed
}
