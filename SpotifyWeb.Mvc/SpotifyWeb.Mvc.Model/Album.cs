using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyWeb.Mvc.Model
{
    public class Album
    {
        public string album_type { get; set; }
        public List<Artist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        [JsonProperty("type")]
        public string tipo { get; set; }
        public string uri { get; set; }
        public List<Copyright> copyrights { get; set; }
        public ExternalIds external_ids { get; set; }
        public List<object> genres { get; set; }
        public string label { get; set; }
        public int popularity { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public Tracks tracks { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        [JsonProperty("explicit")]
        public bool explicito { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string YearReleaseDate
        {
            get
            {
                if(this.release_date != null)
                {
                    return this.release_date.Substring(0, 4);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
