using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.UnitOfWorks;
using MovieTickets.ViewModels;

namespace MovieTickets.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class MovieController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public MovieController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var movies = unitOfWork.Movies.Get(includes: [m => m.MovieCinemas, mc => mc.MovieCategories]);

            return View(movies.ToList());
        }
        [HttpGet]
        public IActionResult Details(int Id)
        {
            var movie = unitOfWork.Movies.GetOne(filter: e => e.Id == Id, includes: [m => m.MovieCinemas, ma => ma.MovieActors, mc => mc.MovieCategories]);//, secondInclude: [mc => mc.MovieActors]
            return View(movie);
        }
        [HttpGet]
        public IActionResult moviesByCategoryId(int Id)
        {

            var movie = unitOfWork.Movies.Get(includes: [m => m.MovieCategories, c => c.MovieCinemas]).Where(c => c.MovieCategories.Any(mc => mc.Id == Id));
            return View("Index", movie.ToList());
        }
        [HttpGet]
        public IActionResult moviesByCinemaId(int Id)
        {

            var movie = unitOfWork.Movies.Get(includes: [m => m.MovieCategories, c => c.MovieCinemas]).Where(c => c.MovieCinemas.Any(mc => mc.Id == Id));
            return View("Index", movie.ToList());
        }
        [HttpGet]
        public IActionResult search(SearchVM model)
        {

            if (ModelState.IsValid)
            {
                var result = unitOfWork.Movies.Get(m => m.Name.Contains(model.value) || m.MovieActors.Any(mc => mc.FirstName.Contains(model.value)), includes: [c => c.MovieCategories, a => a.MovieActors, m => m.MovieCinemas]);
                return View("Index", result.ToList());
            }
            else
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
        }
        [Authorize]
        public IActionResult Booking(int movieId)
        {
            var movie = unitOfWork.Movies.GetOne(m => m.Id == movieId, includes: [m => m.MovieCinemas, ma => ma.MovieActors, mc => mc.MovieCategories]);
            return View(movie);
        }
    }
}
