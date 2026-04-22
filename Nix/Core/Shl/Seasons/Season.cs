using System.Linq;
using Nix.Core.Shl.Seasons.Private;

namespace Nix.Core.Shl.Seasons;

public record Season(
    string Id,
    string Code,
    string Year
)
{
    internal static Season Create(SeasonDto dto) =>
        new(dto.Id, dto.Code, dto.Names.FirstOrDefault()?.SeasonYear ?? "N/A");
}