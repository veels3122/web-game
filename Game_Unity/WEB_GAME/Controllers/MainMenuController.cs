using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WEB_GAME.Models;

namespace WEB_GAME.Controllers
{
    public class MainMenuController : Controller
    {
        public IActionResult Index()
        {
            bool isAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin"));
            var model = new MainMenuViewModel
            {
                IsAdmin = isAdmin
            };
            return View(model);
        }
    }
}
