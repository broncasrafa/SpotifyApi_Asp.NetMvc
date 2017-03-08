using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyWeb.Mvc.Model
{
    public class Playlist
    {
        public bool collaborative { get; set; }
        public object description { get; set; }
        public ExternalUrls external_urls { get; set; }
        public Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public Owner owner { get; set; }
        [JsonProperty("public")]
        public bool publico { get; set; }
        public string snapshot_id { get; set; }
        public PlaylistTracks tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}
