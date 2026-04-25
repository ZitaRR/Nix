using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Nix.Domain.Core.Shl;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Bot;

public class NixCommandContext(DiscordSocketClient client, SocketUserMessage msg) : SocketCommandContext(client, msg)
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public int Latency() => Client.Latency + (int)Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);
}
