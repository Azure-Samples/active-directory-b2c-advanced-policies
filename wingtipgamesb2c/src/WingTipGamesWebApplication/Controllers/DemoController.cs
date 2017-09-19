using Microsoft.AspNetCore.Mvc;

namespace WingTipMusicWebApplication.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
