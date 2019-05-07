using Microsoft.AspNetCore.Mvc;

namespace ticketsbackend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~");
        }
    }
}