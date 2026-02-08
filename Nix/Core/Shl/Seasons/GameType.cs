using Nix.Core.Shl.Seasons.Private;

namespace Nix.Core.Shl.Seasons;

public record GameType(string Id, GameState State)
{
    internal static GameType Create(GameTypeDto dto) =>
        new(dto.Id, dto.ToGameState());
}