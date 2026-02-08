using Nix.Core.Shl.Teams.Private;
using System;
using System.Collections.Generic;

namespace Nix.Core.Shl.Teams;

 public record Team(
     string Id,
     string Code,
     string LongName,
     string FullName,
     Uri IconUri,
     IEnumerable<Player> Players)
{
    public string[] HexTeamColors() =>
        Code switch
        {
            "FHC" => HexFrölunda(),
            "BIF" => HexBrynäs(),
            "DIF" => HexDjurgården(),
            "FBK" => HexFärjestad(),
            "HV71" => HexHv71(),
            "LIF" => HexLeksands(),
            "LHC" => HexLinköping(),
            "LHF" => HexLuleå(),
            "MIF" => HexMalmö(),
            "RBK" => HexRögle(),
            "SKE" => HexSkellefteå(),
            "TIK" => HexTimrå(),
            "VLH" => HexVäxjö(),
            "ÖRE" => HexÖrebro(),
            _ => throw new InvalidOperationException($"No hex colors available for {Code}")
        };

    private string[] HexFrölunda() =>
        ["#00573F", "#C8102E"]; // Green & red

    private string[] HexBrynäs() =>
        ["#0F0C00", "#FFD700"]; // Black & yellow

    private string[] HexDjurgården() =>
        ["#0568AF", "#E21F26"]; // Blue & red

    private string[] HexFärjestad() =>
        ["#009B5E", "#FFD121"]; // Green & yellow

    private string[] HexHv71() =>
        ["#0C2950", "#FCD213"]; // Dark blue & yellow

    private string[] HexLeksands() =>
        ["#FFFFFF", "#26358C"]; // White & blue

    private string[] HexLinköping() =>
        ["#003690", "#FF151F"]; // Blue & red

    private string[] HexLuleå() =>
        ["#1D1D1B", "#E30613"]; // Black & red

    private string[] HexMalmö() =>
        ["#ED1C24", "#000000"]; // Red & black

    private string[] HexRögle() =>
        ["#118235", "#FFFFFF"]; // Green & white

    private string[] HexSkellefteå() =>
        ["#FFC323", "#000000"]; // Yellow & black

    private string[] HexTimrå() =>
        ["#EB2B2E", "#FFFFFF"]; // Red & white

    private string[] HexVäxjö() =>
        ["#013A80", "#F37835"]; // Blue & orange

    private string[] HexÖrebro() =>
        ["#ED1737", "#FFFFFF"]; // Red & white

    internal static Team Create(TeamDto dto, IEnumerable<Player> players) =>
        new(
            dto.Id, 
            dto.Names.Code,
            dto.Names.Long,
            dto.Names.Full,
            new Uri(dto.IconUrl),
            players);
}
