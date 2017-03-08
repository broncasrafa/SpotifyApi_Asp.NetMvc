using System.Collections.Generic;

namespace SpotifyWeb.Mvc.Model
{
    public class Artists
    {
        public List<ItemArtist> items { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public string href { get; set; }
        public Cursors cursors { get; set; }
    }

    public class SeveralArtists
    {
        public List<ItemArtist> artists { get; set; }
    }

    public class ArtistTopTracks
    {
        public List<Track> tracks { get; set; }
    }

    public class RelatedArtists
    {
        public List<ItemArtist> artists { get; set; }
    }
}
