namespace SpotifyWeb.Mvc.Model
{
    public class AudioFeaturesTrack
    {
        public double danceability { get; set; }
        public double energy { get; set; }
        public int key { get; set; }
        public double loudness { get; set; }
        public int mode { get; set; }
        public double speechiness { get; set; }
        public double acousticness { get; set; }
        public int instrumentalness { get; set; }
        public double liveness { get; set; }
        public double valence { get; set; }
        public double tempo { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string uri { get; set; }
        public string track_href { get; set; }
        public string analysis_url { get; set; }
        public int duration_ms { get; set; }
        public int time_signature { get; set; }
    }
}
