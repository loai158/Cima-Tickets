using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.Models;
using MovieTickets.UnitOfWorks;

namespace MovieTickets.Areas.Addmin.Controllers
{
    [Area("Addmin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ActorController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ActorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var actors = unitOfWork.Actors.Get(includes: [m => m.ActorsMovie]);
            return View(actors);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Movies"] = unitOfWork.Movies.Get().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor model, IFormFile? file, List<int> ActorsMovie)
        {
            if (file != null && file.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Save img name in db
                model.ProfilePicture = fileName;
                model.ActorsMovie = new List<Movie>();
                foreach (var id in ActorsMovie)
                {
                    var movie = unitOfWork.Movies.GetOne(m => m.Id == id);
                    model.ActorsMovie.Add(movie);
                }
                unitOfWork.Actors.Create(model);
                await unitOfWork.CompleteAsync();
                await unitOfWork.CompleteAsync();
                //productRepository.Create(product);
                //productRepository.Commit();

                return RedirectToAction("Index");
            }
            ViewData["Actors"] = unitOfWork.Actors.Get().ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int actorId)
        {
            var actor = unitOfWork.Actors.GetOne(m => m.Id == actorId, includes: [ma => ma.ActorsMovie]);
            if (actor != null)
            {
                ViewData["Movies"] = unitOfWork.Movies.Get().ToList();
                return View(actor);
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Actor model, IFormFile? file, List<int> ActorsMovie)
        {
            var modelDb = unitOfWork.Actors.GetOne(m => m.Id == model.Id, tracked: false, includes: [m => m.ActorsMovie]);
            if (modelDb != null && file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", modelDb.ProfilePicture);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save img name in db
                model.ProfilePicture = fileName;
            }
            else
            {
                model.ProfilePicture = modelDb.ProfilePicture;
            }
            if (model != null)
            {
                foreach (var item in ActorsMovie)
                {
                    var movie = unitOfWork.Movies.GetOne(m => m.Id == item);

                    if (movie != null && !modelDb.ActorsMovie.Any(c => c.Id == movie.Id))
                    {
                        model.ActorsMovie.Add(movie);
                    }
                }
                unitOfWork.Actors.Edit(model);
                await unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult Delete(int actorId)
        {
            var actor = unitOfWork.Actors.GetOne(e => e.Id == actorId);

            if (actor != null)
            {
                // Delete old img from wwwroot
                if (actor.ProfilePicture != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", actor.ProfilePicture);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Delete img name in db
                unitOfWork.Actors.Delete(actor);
                unitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });
        }

        public IActionResult DeleteImg(int actorId)
        {
            var actorDb = unitOfWork.Actors.GetOne(e => e.Id == actorId);

            if (actorDb != null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", actorDb.ProfilePicture);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete img name in db
                actorDb.ProfilePicture = null;
                unitOfWork.Movies.Commit();

                return RedirectToAction("Edit", new { actorId });
            }

            return RedirectToAction("NotFoundPage", "Home", new { area = "customer" });
        }
    }
}
