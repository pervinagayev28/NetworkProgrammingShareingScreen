using ClientSideWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClientSideWeb.Controllers
{
    public class WebApp : Controller
    {

       
        public IActionResult Start()
        {
            return View();
        }

      
    }
}
