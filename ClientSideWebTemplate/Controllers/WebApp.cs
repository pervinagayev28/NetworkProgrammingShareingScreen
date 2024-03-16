using Microsoft.AspNetCore.Mvc;

namespace ClientSideWebTemplate.Controllers
{
    public class WebApp : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
