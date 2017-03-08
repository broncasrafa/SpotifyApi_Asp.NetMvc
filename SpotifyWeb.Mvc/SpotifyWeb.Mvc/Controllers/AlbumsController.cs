using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SpotifyWeb.Mvc.Service.Helpers;
using SpotifyWeb.Mvc.Model;
using SpotifyWeb.Mvc.Service;
using SpotifyWeb.Mvc.ViewModel;

namespace Spotify.Web.Mvc.Controllers
{
    public class AlbumsController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();


        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            // carrega os albums e preenche em uma sessão
            var albums = GetAllAlbums(token);
            albums = albums.DistinctBy(x => x.id).ToList();

            HttpContext.Session.Remove("albums");
            Session["albums"] = albums;

            return View();
        }

        public ActionResult GetData(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var albums = (List<Album>)Session["albums"];

            albums = albums.DistinctBy(x => x.id).ToList();

            var retorno = (from c in albums select c).Skip(page.Value * pageSize.Value).Take(pageSize.Value);

            return Json(retorno.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlbum(string albumId)
        {
            return RedirectToAction("AlbumDetails", new { albumId = albumId });
        }

        public ActionResult AlbumDetails(string albumId)
        {
            if (string.IsNullOrEmpty(Session["token"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var album = GetAlbumWithTracks(albumId, token);

            List<Track> tracks = new List<Track>();
            List<string> timesDurations = new List<string>();

            foreach (var trk in album.tracks.items)
            {
                Track track = new Track
                {
                    name = trk.name,
                    track_number = trk.track_number,
                    track_duration_formatted = SpotifyHelpers.TrackDurationFormatted(trk.duration_ms.ToString()),
                    explicito = trk.explicito
                };

                tracks.Add(track);
                timesDurations.Add(trk.duration_ms.ToString());
            }

            AlbumDetailsViewModel vm = new AlbumDetailsViewModel
            {
                AlbumName = album.name,
                ArtistName = album.artists[0].name,
                ImageUrl = album.images[0].url,
                Copyrights = album.copyrights,
                TracksCount = album.tracks.items.Count,
                ReleaseDate = string.IsNullOrEmpty(album.release_date) ? "" : album.release_date.Substring(0, 4),
                ExtendedDuration = SpotifyHelpers.TrackDurationFormatted_2(timesDurations),
                Tracks = tracks
            };

            return View(vm);
        }


        #region Album Information
        public ActionResult Album(string albumId)
        {
            return RedirectToAction("GetSavedAlbumDetails", new { albumId = albumId });
        }
        #endregion

        #region Saved Albums        
        public ActionResult SavedAlbums()
        {
            return View();
        }
        public ActionResult GetSavedAlbums(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var savedAlbumsList = spotifyService.GetSavedAlbums(token, limit, offset);

            return Json(savedAlbumsList.items.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSavedAlbumDetails(string albumId)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var album = spotifyService.GetAlbum(token, albumId);

            return View("SavedAlbumDetails", album);
        }
        #endregion

        private List<Album> GetAllAlbums(string token)
        {
            if (token == null)
                return null;

            List<Album> albums = new List<Album>();

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            List<Playlist> userPlaylists = spotifyService.GetUserPlaylists(token, spotifyUser.id, 50, 0);
            List<PlaylistTrackItem> tracks = new List<PlaylistTrackItem>();

            foreach (var playlist in userPlaylists)
            {
                var tracksFromPlaylist = GetTracksFromPlaylist(token, playlist, spotifyUser);

                foreach (var track in tracksFromPlaylist)
                {
                    var album = track.track.album;
                    albums.Add(album);
                }


                foreach (var t in tracksFromPlaylist)
                {
                    tracks.Add(t);
                }
            }

            Session["Tracks"] = tracks;

            return albums;
        }

        private Album GetAlbumWithTracks(string albumId, string token)
        {
            if (token == null)
                return null;

            // Recupera as musicas das playlists -------------------------------------------------------------------------
            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            List<Playlist> userPlaylists = spotifyService.GetUserPlaylists(token, spotifyUser.id, 50, 0);

            List<PlaylistTrackItem> tracksList = new List<PlaylistTrackItem>();
            tracksList = (List<PlaylistTrackItem>)Session["Tracks"];
            //------------------------------------------------------------------------------------------------------------

            var sessionAlbums = (List<Album>)Session["albums"];

            var album = spotifyService.GetAlbum(token, albumId);

            Tracks t = new Tracks();
            List<Track> it = new List<Track>();

            foreach (var trackAlbum in album.tracks.items)
            {
                foreach (var trackList in tracksList)
                {
                    if (trackAlbum.id == trackList.track.id)
                    {
                        it.Add(trackAlbum);
                    }
                }
            }

            //Limpa as musicas do album
            album.tracks.items.Clear();

            // adiciona as musicas que são do usuário e pertencem ao album
            t.items = it;
            album.tracks = t;

            return album;
        }

        private List<PlaylistTrackItem> GetTracksFromPlaylist(string token, Playlist playlist, UserProfiles spotifyUser)
        {
            // Adicionando a primeira consulta na lista -----------------------------------------------------------------------
            List<PlaylistTrackItem> listTracks = new List<PlaylistTrackItem>();

            int totalTracks = playlist.tracks.total;
            int limit = 50;
            int offset = 0;

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, limit, offset);

            foreach (var track in tracksFromPlaylist)
            {
                listTracks.Add(track);
            }

            int index = offset + 1;
            offset = limit * index;
            //-----------------------------------------------------------------------------------------------------------------

            if (tracksFromPlaylist.Count > 0)
            {
                // demais consultas ao metodo ------------------------------------------------------------------------------------
                for (int i = 0; i < totalTracks; i++)
                {
                    if (!(offset > totalTracks))
                    {
                        var tracksFromPlaylistOnLoop = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, limit, offset);

                        foreach (var track in tracksFromPlaylistOnLoop)
                        {
                            listTracks.Add(track);
                        }

                        index = index + 1;
                        offset = limit * index;
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------                                                
            }

            return listTracks;
        }

    }
}