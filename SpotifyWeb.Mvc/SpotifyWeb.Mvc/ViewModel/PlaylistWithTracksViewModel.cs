using System.Collections.Generic;
using SpotifyWeb.Mvc.Model;

namespace SpotifyWeb.Mvc.ViewModel
{
    public class PlaylistWithTracksViewModel
    {
        public List<Image> images { get; set; }        
        public Followers followers { get; set; }
        public Owner owner { get; set; }
        public List<PlaylistTrackItem> tracks { get; set; }
        public string name { get; set; }
        public bool publico { get; set; }
        public int tracks_total { get; set; }
    }
}