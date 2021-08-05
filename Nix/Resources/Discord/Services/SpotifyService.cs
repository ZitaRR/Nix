using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public sealed class SpotifyService
    {
        private readonly Regex UrlRegex = new Regex(@"(?<=(playlist|track)\/).*?(?=\?)");
        private readonly Regex UriRegex = new Regex("(?<=(playlist|track):).*");
        private SpotifyClient client;

        public SpotifyService()
        {
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(
                    Config.Data.SpotifyId,
                    Config.Data.SpotifySecret));
            client = new SpotifyClient(config);
        }

        public async Task<(List<FullTrack> tracks, string playlistName)?> GetPlaylist(string url)
        {
            string id = GetId(url);
            if (id == "")
            {
                return null;
            }

            FullPlaylist playlist = await client.Playlists.Get(id);
            List<FullTrack> tracks = new();

            foreach (PlaylistTrack<IPlayableItem> item in playlist.Tracks.Items)
            {
                tracks.Add(item.Track as FullTrack);
            }
            return (tracks, playlist.Name);
        }

        public async Task<FullTrack> GetTrack(string url)
        {
            string id = GetId(url);
            if (id == "")
            {
                return null;
            }

            FullTrack track = await client.Tracks.Get(id);
            return track;
        }

        public bool IsSpotifyUri(string url)
        {
            if (url.Contains("spotify"))
            {
                return true;
            }
            return false;
        }

        public bool IsPlaylist(string url)
        {
            if (url.Contains("playlist"))
            {
                return true;
            }
            return false;
        }

        public bool IsTrack(string url)
        {
            if (url.Contains("track"))
            {
                return true;
            }
            return false;
        }

        private string GetId(string url)
        {
            string id = UrlRegex.Match(url).Value;
            if (id == "")
            {
                id = UriRegex.Match(url).Value;
            }

            return id;
        }
    }
}
