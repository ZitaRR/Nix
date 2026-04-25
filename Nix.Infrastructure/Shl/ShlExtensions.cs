using Nix.Domain.Core.Shl;
using Nix.Infrastructure.Shl.Overview.Internal.Private;
using Nix.Infrastructure.Shl.Seasons.Internal.Private;
using Nix.Infrastructure.Shl.Standings.Internal.Private;
using Nix.Infrastructure.Shl.Teams.Internal.Private;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Nix.Infrastructure.Shl;

public static class ShlExtensions
{
    internal static Position ToPlayerPosition(this string position) =>
        position switch
        {
            "GK" => Position.Goalie,
            "D" => Position.Defense,
            "F" => Position.Forward,
            _ => throw new ArgumentOutOfRangeException(nameof(position), $"Unable to convert: {position}")
        };

    /*internal static State ToState(this StateDto state) =>
        state switch
        {
            StateDto.NotPlayed => State.NotPlayed,
            StateDto.Played => State.Played,
            _ => throw new ArgumentOutOfRangeException(nameof(state), $"Unable to convert: {state}")
        };*/

    internal static GameState ToGameState(this GameTypeDto dto) =>
        dto.Code.ToLowerInvariant() switch
        {
            "regular" => GameState.Regular,
            "playoff" => GameState.Playoffs,
            "qualdown" => GameState.Relegation,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), $"Unable to convert: {dto.Code}")
        };

    internal static Match ToMatch(this MatchDto dto, ImmutableArray<Team> teams) => 
        new(
            dto.StartDateTime,
            teams.First(t => dto.HomeTeam.Id.Equals(t.Id, StringComparison.OrdinalIgnoreCase)),
            teams.First(t => dto.AwayTeam.Id.Equals(t.Id, StringComparison.OrdinalIgnoreCase)));

    internal static Team ToTeam(this TeamDto dto, byte[] iconBytes, ImmutableArray<(PlayerDto, Position)> players, ImmutableArray<StandingsTeamDto> standings) =>
        new(
            dto.Id,
            dto.Names.Long, 
            dto.Names.Code, 
            iconBytes,
            standings.First(t => dto.Id.Equals(t.Info.TeamId, StringComparison.OrdinalIgnoreCase)).ToStanding(), 
            [.. players.Select(ToPlayer)]);

    internal static Player ToPlayer(this (PlayerDto Player, Position Position) dto)
    {
        _ = Uri.TryCreate(dto.Player.Portrait?.Url, UriKind.Absolute, out var uri);
        return new(dto.Player.FirstName, dto.Player.Surname, dto.Player.Number, dto.Position, uri);
    }

    internal static Standing ToStanding(this StandingsTeamDto dto) =>
        new(dto.Rank, dto.Points);
}
