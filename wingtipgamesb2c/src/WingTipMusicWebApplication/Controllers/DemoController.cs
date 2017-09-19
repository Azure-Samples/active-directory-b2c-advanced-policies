using Microsoft.AspNetCore.Mvc;

namespace WingTipGamesWebApplication.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
