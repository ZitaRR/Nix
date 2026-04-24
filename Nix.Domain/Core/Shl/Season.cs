using System.Collections.Immutable;

namespace Nix.Domain.Core.Shl;

public class Season(
    ImmutableArray<Match> matches,
    ImmutableArray<Match> playoffs,
    ImmutableArray<Match> relegations,
    ImmutableArray<Team> teams)
{
    public ImmutableArray<Match> RegularMatches { get; } = matches;
    public ImmutableArray<Match> PlayoffMatches { get; } = playoffs;
    public ImmutableArray<Match> RelegationMatches { get; } = relegations;
    public ImmutableArray<Team> Teams { get; } = teams;
}
