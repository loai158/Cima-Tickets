using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieTickets.Areas.Addmin.Controllers
{
    [Area("Addmin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
