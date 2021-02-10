using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public sealed class SpotifyService
    {
        private const int TRACK = 31;
        private const int PLAYLIST = 34;

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
            string id;
            FullPlaylist playlist;

            try
            {
                id = GetIdFromUrl(url, PLAYLIST);
                playlist = await client.Playlists.Get(id);
            }
            catch
            {
                return null;
            }

            var tracks = new List<FullTrack>();
            foreach (var item in playlist.Tracks.Items)
            {
                tracks.Add(item.Track as FullTrack);
            }
            return (tracks, playlist.Name);
        }

        public async Task<FullTrack> GetTrack(string url)
        {
            string id;
            FullTrack track;

            try
            {
                id = GetIdFromUrl(url, TRACK);
                track = await client.Tracks.Get(id);
            }
            catch
            {
                throw;
            }

            return track;
        }

        private string GetIdFromUrl(string url, int offset)
        {
            string id;

            try
            {
                id = url.Substring(offset);
                id = id.Substring(0, id.IndexOf("?"));
            }
            catch
            {
                throw;
            }

            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Invalid URL");

            return id;
        }
    }
}
