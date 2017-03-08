using System.Web.Mvc;
using SpotifyWeb.Mvc.Service;

namespace SpotifyWeb.Mvc.Controllers
{
    public class HomeController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        public ActionResult Index()
        {
            ViewBag.AuthorizationUri = spotifyService.GetAuthorizationUri();
            return View();
        }

        public ActionResult AuthorizationResponse(string access_token, string token_type, string expires_in, string state)
        {
            if (string.IsNullOrEmpty(access_token))
            {
                return View();
            }
            else
            {
                Session["token"] = access_token;

                var spotifyUser = spotifyService.GetCurrentUserProfile(access_token);

                Session["UserImage"] = spotifyUser.images[0].url;
                Session["UserId"] = spotifyUser.display_name;

                return RedirectToAction("Index", "Playlist");
            }

            return View();
        }







        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}