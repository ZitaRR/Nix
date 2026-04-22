using Nix.Core.Shl.Overview;
using Nix.Core.Shl.Overview.Private;
using Nix.Core.Shl.Seasons;
using Nix.Core.Shl.Seasons.Private;
using Nix.Core.Shl.Teams;
using System;

namespace Nix.Core.Shl;

public static class ShlExtensions
{
    internal static Position ToPlayerPosition(this string position) =>
        position switch
        {
            "GK" => Position.GoalKeeper,
            "D" => Position.Defensemen,
            "F" => Position.Forwards,
            _ => throw new ArgumentOutOfRangeException(nameof(position), $"Unable to convert: {position}")
        };

    internal static State ToState(this StateDto state) =>
        state switch
        {
            StateDto.NotPlayed => State.NotPlayed,
            StateDto.Played => State.Played,
            _ => throw new ArgumentOutOfRangeException(nameof(state), $"Unable to convert: {state}")
        };

    internal static GameState ToGameState(this GameTypeDto dto) =>
        dto.Code.ToLowerInvariant() switch
        {
            "regular" => GameState.Regular,
            "playoff" => GameState.Playoffs,
            "qualdown" => GameState.Relegation,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), $"Unable to convert: {dto.Code}")
        };
}
