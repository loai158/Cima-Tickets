using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.Models;
using MovieTickets.UnitOfWorks;

namespace MovieTickets.Areas.Addmin.Controllers
{
    [Area("Addmin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CinemaController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CinemaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var cinemas = unitOfWork.Cinemas.Get(includes: [m => m.CinemaMovies]);
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Movies"] = unitOfWork.Movies.Get().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema model, IFormFile? file, List<int> CinemaMovies)
        {
            if (file != null && file.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\logos", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Save img name in db
                model.CinemaLogo = fileName;
                foreach (var id in CinemaMovies)
                {
                    var movie = unitOfWork.Movies.GetOne(m => m.Id == id);
                    model.CinemaMovies.Add(movie);
                }
                unitOfWork.Cinemas.Create(model);
                await unitOfWork.CompleteAsync();
                //productRepository.Create(product);
                //productRepository.Commit();

                return RedirectToAction("Index");
            }
            ViewData["Movies"] = unitOfWork.Movies.Get().ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int cinemaId)
        {
            var cinema = unitOfWork.Cinemas.GetOne(c => c.Id == cinemaId, includes: [ma => ma.CinemaMovies]);
            if (cinema != null)
            {
                ViewData["Movies"] = unitOfWork.Movies.Get().ToList();
                return View(cinema);
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cinema model, IFormFile? file, List<int> CinemaMovies)
        {
            var modelDb = unitOfWork.Cinemas.GetOne(m => m.Id == model.Id, tracked: false, includes: [m => m.CinemaMovies]);
            if (modelDb != null && file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\logos", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\logos", modelDb.CinemaLogo);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save img name in db
                model.CinemaLogo = fileName;
            }
            else
            {
                model.CinemaLogo = modelDb.CinemaLogo;
            }
            if (model != null)
            {
                foreach (var item in CinemaMovies)
                {
                    var movie = unitOfWork.Movies.GetOne(m => m.Id == item);

                    if (movie != null && !modelDb.CinemaMovies.Any(c => c.Id == movie.Id))
                    {
                        model.CinemaMovies.Add(movie);
                    }
                }
                unitOfWork.Cinemas.Edit(model);
                await unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult Delete(int cinemaId)
        {
            var cinema = unitOfWork.Cinemas.GetOne(e => e.Id == cinemaId);

            if (cinema != null)
            {
                // Delete old img from wwwroot
                if (cinema.CinemaLogo != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\logos", cinema.CinemaLogo);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Delete img name in db
                unitOfWork.Cinemas.Delete(cinema);
                unitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }

        public IActionResult DeleteImg(int cinemaId)
        {
            var cinemaDb = unitOfWork.Cinemas.GetOne(e => e.Id == cinemaId);

            if (cinemaDb != null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\logos", cinemaDb.CinemaLogo);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete img name in db
                cinemaDb.CinemaLogo = null;
                unitOfWork.Movies.Commit();

                return RedirectToAction("Edit", new { cinemaId });
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "customer" });
        }
    }
}
