using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyWeb.Mvc.Model
{
    public class Track
    {
        public Album album { get; set; }
        public List<Artist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        [JsonProperty("explicit")]
        public bool explicito { get; set; }
        public ExternalIds external_ids { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        [JsonProperty("type")]
        public string tipo { get; set; }
        public string uri { get; set; }
        public string track_duration_formatted { get; set; }
        public string TrackDurationFormatted
        {
            get
            {
                return TrackDurationFormattedMethod(this.duration_ms.ToString());
            }
        }


        private static string TrackDurationFormattedMethod(string duration)
        {
            int durationLength = Convert.ToInt32(duration);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, durationLength);

            if (ts.Seconds < 10)
                return string.Format("{0}:0{1}", ts.Minutes, ts.Seconds);
            else
                return string.Format("{0}:{1}", ts.Minutes, ts.Seconds);
        }
    }
}
