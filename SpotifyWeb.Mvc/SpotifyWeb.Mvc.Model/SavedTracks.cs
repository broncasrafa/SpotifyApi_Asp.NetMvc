using System.Collections.Generic;

namespace SpotifyWeb.Mvc.Model
{
    public class SavedTracks
    {
        public string href { get; set; }
        public List<ItemSavedTrack> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }
}
