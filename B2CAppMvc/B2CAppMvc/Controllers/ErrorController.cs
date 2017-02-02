using System.Web.Mvc;

namespace B2CAppMvc.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string error, [Bind(Prefix = "error_description")] string errorDescription)
        {
            ViewBag.Error = error;
            ViewBag.ErrorDescription = errorDescription;
            return View();
        }
    }
}
