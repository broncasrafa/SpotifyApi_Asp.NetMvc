using System.Collections.Generic;
using SpotifyWeb.Mvc.Model;

namespace SpotifyWeb.Mvc.ViewModel
{
    public class AlbumDetailsViewModel
    {
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public string ImageUrl { get; set; }
        public List<Copyright> Copyrights { get; set; }
        public int TracksCount { get; set; }
        public string ExtendedDuration { get; set; }
        public string ReleaseDate { get; set; }
        public List<Track> Tracks { get; set; }
    }
}