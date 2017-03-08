using System.Collections.Generic;

namespace SpotifyWeb.Mvc.Model
{
    public class Albums
    {
        public string href { get; set; }
        public List<Album> items { get; set; }
        public int limit { get; set; }
        public object next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class SeveralAlbums
    {
        public List<Album> albums { get; set; }
    }
}
