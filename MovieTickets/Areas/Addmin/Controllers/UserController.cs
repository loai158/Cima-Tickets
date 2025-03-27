using Microsoft.AspNetCore.Mvc;
using MovieTickets.UnitOfWorks;

namespace MovieTickets.Areas.Addmin.Controllers
{
    [Area("Addmin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? query, int page = 1)
        {
            var users = unitOfWork.ApplicationUsers.Get();
            if (query != null)
            {
                users = users.Where(u => u.UserName.Contains(query)
                || u.Email.Contains(query));
            }
            var totalPages = Math.Ceiling((decimal)(users.ToList().Count / 2));
            if (totalPages < page - 1)
                return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });

            users = users.Skip((page - 1) * 5).Take(5).ToList();
            ViewBag.totalPages = totalPages;
            return View(users);
        }
    }
}
