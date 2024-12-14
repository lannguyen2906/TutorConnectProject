using Microsoft.AspNetCore.Mvc;

namespace TutoRum.FE.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
