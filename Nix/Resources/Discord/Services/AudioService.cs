using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Victoria;

namespace Nix.Resources.Discord
{
    public sealed class AudioService
    {
        private readonly LavaNode lavaNode;
        private readonly ILogger logger;
        private readonly SpotifyService spotify;
        private readonly IDiscord discord;
        private ConcurrentDictionary<ulong, NixPlayer> players;
        private ConcurrentDictionary<ulong, LavalinkData> data;

        public AudioService(
            LavaNode lavaNode,
            ILogger logger,
            SpotifyService spotify,
            IDiscord discord)
        {
            this.lavaNode = lavaNode;
            this.logger = logger;
            this.spotify = spotify;
            this.discord = discord;

            this.discord.Client.UserVoiceStateUpdated += OnVoiceStateUpdate;
            this.discord.Client.Disconnected += OnDisconnection;
            this.discord.Client.Ready += OnReady;

            players = new ConcurrentDictionary<ulong, NixPlayer>();
            data = new ConcurrentDictionary<ulong, LavalinkData>();
        }

        public async Task<NixPlayer> CreatePlayerForGuildAsync(IGuild guild, IVoiceChannel voice, ITextChannel text = null)
        {
            if (voice is null)
                return null;
            if (TryGetPlayer(guild, out NixPlayer player))
                return null;

            player = new NixPlayer(lavaNode, spotify, this);
            await player.JoinAsync(voice, text);
            players.TryAdd(guild.Id, player);
            logger.AppendLog("AUDIO", $"Created player for {guild.Name}");
            return player;
        }

        public Task<bool> RemovePlayerFromGuildAsync(IGuild guild)
        {
            if (!players.TryRemove(guild.Id, out _))
                return Task.FromResult(false);

            logger.AppendLog("AUDIO", $"Player removed from {guild.Name}");
            return Task.FromResult(true);
        }

        public bool TryGetPlayer(IGuild guild, out NixPlayer player)
        {
            if (!players.TryGetValue(guild.Id, out player))
                return false;
            return true;
        }

        private async Task OnVoiceStateUpdate(SocketUser user, SocketVoiceState origin, SocketVoiceState destination)
        {
            if (user.IsBot)
                return;
            if (!TryGetPlayer((user as SocketGuildUser).Guild, out NixPlayer player))
                return;
            else if (player.VoiceChannel.Id != origin.VoiceChannel?.Id &&
                player.VoiceChannel.Id != destination.VoiceChannel?.Id)
                return;

            if ((player.VoiceChannel as SocketVoiceChannel).Users.Count <= 1)
            {
                _ = player.InitiateDisconnectAsync();
            }
            else
            {
                await player.CancelDisconnectAsync();
            }
        }

        private Task OnDisconnection(Exception _)
        {
            foreach (var nix in players.Values)
            {
                data.TryAdd(nix.VoiceChannel.GuildId, new LavalinkData(nix));
            }

            players.Clear();
            return Task.CompletedTask;
        }

        private async Task OnReady()
        {
            if (!lavaNode.IsConnected)
            {
                await lavaNode.ConnectAsync();
            }

            if (data.Count <= 0)
                return;

            foreach (var data in data)
            {
                await lavaNode.LeaveAsync(data.Value.VoiceChannel);
                var player = await CreatePlayerForGuildAsync(
                    data.Value.VoiceChannel.Guild,
                    data.Value.VoiceChannel,
                    data.Value.TextChannel);
                players.TryAdd(data.Key, player);
                await player.PlayAsync(data.Value);
            }

            data.Clear();
        }
    }
}
