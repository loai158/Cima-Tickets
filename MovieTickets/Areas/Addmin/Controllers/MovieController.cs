using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.Models;
using MovieTickets.UnitOfWorks;

namespace MovieTickets.Areas.Addmin.Controllers
{
    [Area("Addmin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class MovieController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public MovieController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var movies = unitOfWork.Movies.Get(includes: [mc => mc.MovieCategories, ma => ma.MovieActors, m => m.MovieCinemas]).ToList();
            return View(movies);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Cinemas"] = unitOfWork.Cinemas.Get().ToList();
            ViewData["Categories"] = unitOfWork.Categories.Get().ToList();
            ViewData["Actors"] = unitOfWork.Actors.Get().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie model, IFormFile? file, List<int> MovieCategories, List<int> MovieCinemas, List<int> MovieActors)
        {
            if (file != null && file.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Save img name in db
                model.ImgUrl = fileName;

                foreach (var id in MovieCategories)
                {
                    var category = unitOfWork.Categories.GetOne(c => c.Id == id);
                    model.MovieCategories.Add(category);
                }
                foreach (var id in MovieActors)
                {
                    var actor = unitOfWork.Actors.GetOne(a => a.Id == id);
                    model.MovieActors.Add(actor);
                }
                foreach (var id in MovieCinemas)
                {
                    var cinema = unitOfWork.Cinemas.GetOne(c => c.Id == id);
                    model.MovieCinemas.Add(cinema);
                }
                unitOfWork.Movies.Create(model);
                await unitOfWork.CompleteAsync();
                await unitOfWork.CompleteAsync();
                //productRepository.Create(product);
                //productRepository.Commit();

                return RedirectToAction("Index");
            }

            //var categories = categoryRepository.Get();
            ////ViewBag.Categories = categories;
            //ViewData["Categories"] = categories.ToList();
            ViewData["Cinemas"] = unitOfWork.Cinemas.Get().ToList();
            ViewData["Categories"] = unitOfWork.Categories.Get().ToList();
            ViewData["Actors"] = unitOfWork.Actors.Get().ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int movieId)
        {
            var movie = unitOfWork.Movies.GetOne(m => m.Id == movieId, includes: [ma => ma.MovieActors, mc => mc.MovieCinemas, c => c.MovieCategories]);
            if (movie != null)
            {
                ViewData["Cinemas"] = unitOfWork.Cinemas.Get().ToList();
                ViewData["Categories"] = unitOfWork.Categories.Get().ToList();
                ViewData["Actors"] = unitOfWork.Actors.Get().ToList();
                return View(movie);
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Movie model, IFormFile? file, List<int> MovieCategories, List<int> MovieCinemas, List<int> MovieActors)
        {
            var modelDb = unitOfWork.Movies.GetOne(m => m.Id == model.Id, tracked: false, includes: [ma => ma.MovieActors, mc => mc.MovieCinemas, c => c.MovieCategories]);
            if (modelDb != null && file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", modelDb.ImgUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save img name in db
                model.ImgUrl = fileName;
            }
            else
            {
                model.ImgUrl = modelDb.ImgUrl;
            }
            if (model != null)
            {
                foreach (var item in MovieCategories)
                {
                    var category = unitOfWork.Categories.GetOne(c => c.Id == item);

                    if (category != null && !modelDb.MovieCategories.Any(c => c.Id == category.Id))
                    {
                        model.MovieCategories.Add(category);
                    }
                }
                foreach (var id in MovieActors)
                {
                    var actor = unitOfWork.Actors.GetOne(a => a.Id == id);

                    if (actor != null && !modelDb.MovieActors.Any(a => a.Id == id))
                    {
                        model.MovieActors.Add(actor);
                    }
                }
                foreach (var id in MovieCinemas)
                {
                    var cinema = unitOfWork.Cinemas.GetOne(c => c.Id == id);
                    if (cinema != null && !modelDb.MovieCinemas.Any(a => a.Id == id))
                    {
                        model.MovieCinemas.Add(cinema);
                    }
                }
                unitOfWork.Movies.Edit(model);
                await unitOfWork.CommitTransactionAsync();
                await unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult Delete(int movieId)
        {
            var movie = unitOfWork.Movies.GetOne(e => e.Id == movieId);

            if (movie != null)
            {
                // Delete old img from wwwroot
                if (movie.ImgUrl != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", movie.ImgUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Delete img name in db
                unitOfWork.Movies.Delete(movie);
                unitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }
        public IActionResult DeleteImg(int movieId)
        {
            var movieDb = unitOfWork.Movies.GetOne(e => e.Id == movieId);

            if (movieDb != null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", movieDb.ImgUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete img name in db
                movieDb.ImgUrl = null;
                unitOfWork.Movies.Commit();

                return RedirectToAction("Edit", new { movieId });
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "customer" });
        }
    }
}
