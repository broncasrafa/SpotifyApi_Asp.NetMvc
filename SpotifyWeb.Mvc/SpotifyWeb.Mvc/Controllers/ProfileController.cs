using System.Web.Mvc;
using SpotifyWeb.Mvc.Service;
using SpotifyWeb.Mvc.Service.Helpers;

namespace SpotifyWeb.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        // GET: Profile
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var spotifyUser = spotifyService.GetCurrentUserProfile(token);

            ViewBag.Aniversario = SpotifyHelpers.FormattedBirthdate(spotifyUser.birthdate);
            ViewBag.Pais = SpotifyHelpers.GetCountryName(spotifyUser.country);

            return View(spotifyUser);
        }
    }
}