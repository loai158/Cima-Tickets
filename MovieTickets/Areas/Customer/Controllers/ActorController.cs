using Microsoft.AspNetCore.Mvc;
using MovieTickets.UnitOfWorks;

namespace MovieTickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ActorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Details(int Id)
        {
            var actor = unitOfWork.Actors.GetOne(a => a.Id == Id);
            return View(actor);
        }
    }
}
