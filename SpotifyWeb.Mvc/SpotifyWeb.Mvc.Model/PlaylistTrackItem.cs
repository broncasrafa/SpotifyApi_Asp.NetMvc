namespace SpotifyWeb.Mvc.Model
{
    public class PlaylistTrackItem
    {
        public string added_at { get; set; }
        public AddedBy added_by { get; set; }
        public bool is_local { get; set; }
        public Track track { get; set; }
    }
}
