using Microsoft.AspNetCore.Mvc;
using Nix.MVC;
using Nix.Resources;
using Nix.Resources.Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nix.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NixController : ControllerBase
    {
        private readonly IDiscord discord;
        private readonly INixProvider nixProvider;
        private readonly AudioService audio;

        public NixController(IDiscord discord, INixProvider nixProvider, AudioService audio)
        {
            this.discord = discord;
            this.nixProvider = nixProvider;
            this.audio = audio;
        }

        [HttpGet]
        public string Get()
        {
            return "Nix";
        }

        [HttpGet("users")]
        public async Task<IEnumerable<NixUser>> GetAllUsers()
        {
            var users = await nixProvider.Users.GetAll();
            return users;
        }

        [HttpGet("join/{guildId}/{voiceId}")]
        public async Task<string> JoinVoiceChannel(ulong guildId, ulong voiceId)
        {
            var guild = discord.Client.GetGuild(guildId);
            if (guild is null)
                return "Guild does not exist";

            if (audio.TryGetPlayer(guild, out _))
                return "Player is already active";

            var voice = guild.GetVoiceChannel(voiceId);
            if (voice is null)
                return "Voice-channel does not exist";

            var player = await audio.CreatePlayerForGuildAsync(guild, voice);
            return $"Joined {player.VoiceChannel} and bound to {player.TextChannel}";
        }
    }
}
