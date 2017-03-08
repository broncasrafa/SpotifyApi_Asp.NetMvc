using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SpotifyWeb.Mvc.Service.Helpers;
using SpotifyWeb.Mvc.Model;
using SpotifyWeb.Mvc.Service;

namespace SpotifyWeb.Mvc.Controllers
{
    public class PlaylistController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        #region Lista as playlists
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPlaylists(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var spotifyUser = spotifyService.GetCurrentUserProfile(token);

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var playlists = spotifyService.GetUserPlaylists(token, spotifyUser.id, limit, offset);

            return Json(playlists.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Playlist pelo Id        
        public ActionResult Playlist(string id)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            Playlist playlist = spotifyService.GetPlaylist(token, spotifyUser.id, id);

            Session["SpotifyUser"] = spotifyUser;
            Session["Playlist"] = playlist;
            Session["spotifyUserId"] = spotifyUser.id;
            Session["playlistId"] = playlist.id;
            Session["totalTracksCount"] = playlist.tracks.total;

            ViewBag.PlaylistId = playlist.id;
            ViewBag.Image = playlist.images[0].url;
            ViewBag.PlaylistName = playlist.name;
            ViewBag.Followers = playlist.followers.total;
            ViewBag.Owner = playlist.owner.id;
            ViewBag.Tracks = playlist.tracks.total;
            ViewBag.Publico = playlist.publico == true ? "Sim" : "Não";

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, 50, 0);

            return View(tracksFromPlaylist.ToList());
        }
        public ActionResult GetTracks(int? page)
        {
            if (string.IsNullOrEmpty(Session["token"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            string userId = Session["spotifyUserId"].ToString();
            string playlistId = Session["playlistId"].ToString();
            int limit = 50;
            var pagina = page ?? 0;
            int offset = (pagina * limit);
            int totalRecordCount = (int)Session["totalTracksCount"];

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, userId, playlistId, limit, offset);

            //var PageCount = totalRecordCount > 0 ? (int)Math.Ceiling(totalRecordCount / (double)pagesize) : 0;

            return Json(tracksFromPlaylist.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Exportar
        public ActionResult ExportToExcel(string id)
        {
            if (string.IsNullOrEmpty(Session["token"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            string playlistId = id;

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);

            // primeira consulta ao metodo ------------------------------------------------------------------------------------
            Playlist playlist = spotifyService.GetPlaylist(token, spotifyUser.id, playlistId);

            // Demais consultas
            List<PlaylistTrackItem> listTracks = GetTracksFromPlaylist(token, playlist, spotifyUser);

            if (listTracks.Count > 0)
            {
                string playlistName = playlist.name;

                SpotifyHelpers.ExportToExcel(listTracks, playlistName);

                return new EmptyResult();
            }

            return RedirectToAction("Index", "Playlist");
        }
        #endregion

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