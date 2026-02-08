using Nix.Core.Shl.Overview.Private;
using System;

namespace Nix.Core.Shl.Overview;

public record Match(
    string Id,
    State State,
    OverviewTeam HomeTeam,
    OverviewTeam AwayTeam,
    string Venue,
    DateTime StartDateTime)
{
    public bool IsPlaying() =>
        !HasEnded() && HasStarted();

    public bool HasStarted() =>
        DateTime.UtcNow >= StartDateTime;

    public bool HasEnded() =>
        State == State.Played;

    internal static Match Create(MatchDto dto) =>
        new(
            dto.Id,
            dto.State.ToState(),
            OverviewTeam.Create(dto.HomeTeam),
            OverviewTeam.Create(dto.AwayTeam),
            dto.Venue.Name,
            dto.StartDateTime);
}
