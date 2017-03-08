using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SpotifyWeb.Mvc.Service.Helpers;
using SpotifyWeb.Mvc.Model;
using SpotifyWeb.Mvc.Service;

namespace Spotify.Web.Mvc.Controllers
{
    public class ArtistsController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        // GET: Artists
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var artists = GetAllArtists(token);

            HttpContext.Session.Remove("artists");
            Session["artists"] = artists;

            return View();
        }


        public ActionResult GetData(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var artists = (List<ItemArtist>)Session["artists"];

            var retorno = (from c in artists select c)
                          .Skip(page.Value * pageSize.Value)
                          .Take(pageSize.Value);

            return Json(retorno.ToList(), JsonRequestBehavior.AllowGet);
        }

        // Retorna todos os artistas das playlists
        private List<ItemArtist> GetAllArtists(string token)
        {
            if (token == null)
                return null;

            List<ItemArtist> artists = new List<ItemArtist>();
            List<ItemArtist> artistsAux = new List<ItemArtist>();
            //Artists artistsAux = new Artists();

            List<string> ids_artists = new List<string>();

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            List<Playlist> userPlaylists = spotifyService.GetUserPlaylists(token, spotifyUser.id, 50, 0);
            List<PlaylistTrackItem> tracks = new List<PlaylistTrackItem>();

            // Para cada playlist do usuário, obtem as musicas
            foreach (var playlist in userPlaylists)
            {
                var tracksFromPlaylist = GetTracksFromPlaylist(token, playlist, spotifyUser);

                // Para cada musica das playlists, armazena o id do artista na lista
                foreach (var track in tracksFromPlaylist)
                {
                    var id = track.track.artists[0].id;
                    ids_artists.Add(id);
                }
            }

            //Obtem os artistas seguidos
            var followedArtists = GetFollowedArtists(token);

            foreach (var artistFollowed in followedArtists.items)
            {
                var id = artistFollowed.id;
                ids_artists.Add(id);
            }

            // Apenas ids DISTINTOS
            ids_artists = ids_artists.Distinct().ToList();

            int qtdeIds = ids_artists.Count;
            int maximumIds = 50;
            int qtdePartes = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(qtdeIds) / Convert.ToDouble(maximumIds)));

            var idsSeparadosEmPartes = SpotifyHelpers.SplitList<string>(ids_artists, qtdePartes);

            IEnumerable<string>[] partesDosIds = new IEnumerable<string>[qtdePartes];

            //Obtem os artistas das playlists
            for (int i = 0; i < idsSeparadosEmPartes.Count(); i++)
            {
                partesDosIds[i] = idsSeparadosEmPartes.ElementAt(i);
                artistsAux = spotifyService.GetSeveralArtists(token, partesDosIds[i].ToList());
                artistsAux.ForEach(c => artists.Add(c));
            }

            return artists;
        }

        // Retorna os artistas seguidos pelo usuário
        private Artists GetFollowedArtists(string token)
        {
            var followedArtists = spotifyService.GetFollowedArtists(token);

            return followedArtists.artists;
        }

        // Retorna as musicas de todas as playlists do usuário
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



        #region Detalhes do Artista        
        public ActionResult ArtistDetails(string artistId)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            Session["artistId"] = artistId;

            //var artist = GetArtist(token, artistId);
            //var albums = GetArtistAlbums(token, artistId);
            //var popular = GetPopularTracks(token, artistId);
            //var related = GetRelatedArtists(token, artistId);
            //var singles = GetArtistSingles(token, artistId);
            //var appearsOn = GetArtistAppearsOn(token, artistId);
            //var compilations = GetArtistCompilations(token, artistId);

            //ArtistDetailsViewModel vm = new ArtistDetailsViewModel
            //{
            //    Artist = artist,
            //    ArtistAlbums = albums,
            //    ArtistSingles = singles,
            //    ArtistAppearsOn = appearsOn,
            //    ArtistCompilation = compilations,
            //    ArtistTopTracks = popular,
            //    RelatedArtists = related
            //};

            //return View(vm);
            return View();
        }

        public ActionResult GetArtist()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var artist = spotifyService.GetArtist(token, artistId);

            return Json(artist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPopularTracks()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var popularTracks = spotifyService.GetArtistTopTracks(token, artistId);

            return Json(popularTracks, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRelatedArtists()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var relatedArtists = spotifyService.GetRelatedArtists(token, artistId);

            return Json(relatedArtists, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistAlbums(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistAlbums(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // Apenas ids DISTINTOS
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistSingles(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistSingles(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // Apenas ids DISTINTOS
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistAppearsOn(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistAppearsOn(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // Apenas ids DISTINTOS
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistCompilations(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistCompilations(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // Apenas ids DISTINTOS
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);


            //if (Session["token"] == null)
            //    return null;

            //var token = Session["token"].ToString();
            //var artistId = Session["artistId"].ToString();

            //List<Album> listCompilations = new List<Album>();

            //int limit = 50;
            //int offset = 0;

            //// 1ª busca na API
            //var compilations = spotifyService.GetArtistCompilations(token, artistId, limit, offset);
            //compilations.items.ForEach(c => listCompilations.Add(c));

            //int index = offset + 1;
            //offset = limit * index;

            //if (compilations.items.Count > 0)
            //{
            //    // demais buscas na API
            //    for (int i = 0; i < compilations.total; i++)
            //    {
            //        if (!(offset > compilations.total))
            //        {
            //            var compilationsOnLoop = spotifyService.GetArtistCompilations(token, artistId, limit, offset);
            //            compilationsOnLoop.items.ForEach(c => listCompilations.Add(c));

            //            index = index + 1;
            //            offset = limit * index;
            //        }
            //    }
            //}

            //// Após buscar todos os albums do artista, busca as informações completas assim com as musicas dos albums
            //List<Album> albumsCompletos = new List<Album>();

            //foreach (var album in listCompilations)
            //{
            //    var apiData = spotifyService.GetAlbum(token, album.id);
            //    albumsCompletos.Add(apiData);
            //}

            //return Json(albumsCompletos, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}