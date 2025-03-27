using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.Models;
using MovieTickets.UnitOfWorks;
using Stripe.Checkout;

namespace MovieTickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var cart = _unitOfWork.Cart.Get(e => e.ApplicationUserId == _userManager.GetUserId(User), includes: [e => e.Movie, e => e.ApplicationUser]);

            var total = cart.Sum(e => e.Movie.Price * e.Count);
            ViewBag.Total = total;
            ViewBag.OrdersCount = cart.Sum(e => e.Count);
            return View(cart);
        }
        public async Task<IActionResult> Add(int movieId, int count)
        {
            var order = new Cart()
            {
                MovieId = movieId,
                Count = count,
                ApplicationUserId = _userManager.GetUserId(User)
            };
            var orderDb = _unitOfWork.Cart
                .GetOne(o => o.MovieId == movieId &&
                o.ApplicationUserId == _userManager.GetUserId(User), tracked: false);
            if (orderDb == null)
            {
                _unitOfWork.Cart.Create(order);
                await _unitOfWork.CompleteAsync();
                TempData["notifaction"] = "Add Movie to cart successfuly";
                return RedirectToAction("index", "Movie", new { area = "Customer" });
            }
            else
            {
                TempData["notifaction"] = " Movie Already Added To The Cart";
                return RedirectToAction("index", "Movie", new { area = "Customer" });
            }
        }
        public IActionResult GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            var count = _unitOfWork.Cart.Get(e => e.ApplicationUserId == userId).Sum(e => e.Count);
            return Json(count);
        }
        public IActionResult Increment(int movieId)
        {


            var cart = _unitOfWork.Cart.GetOne(o => o.ApplicationUserId == _userManager.GetUserId(User) && o.MovieId == movieId);

            if (cart != null)
            {
                cart.Count++;
                _unitOfWork.Cart.Commit();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Decrement(int movieId)
        {

            var cart = _unitOfWork.Cart.GetOne(o => o.ApplicationUserId == _userManager.GetUserId(User) && o.MovieId == movieId);
            if (cart != null)
            {
                if (cart.Count > 0)
                {
                    cart.Count--;
                    _unitOfWork.Cart.Commit();
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = _unitOfWork.Cart.GetOne(o => o.ApplicationUserId == userId && o.MovieId == movieId, tracked: true);

            if (cartItem != null)
            {
                _unitOfWork.Cart.Delete(cartItem);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Pay()
        {
            var userId = _userManager.GetUserId(User);
            var cart = _unitOfWork.Cart.Get(e => e.ApplicationUserId == userId, includes: [e => e.Movie, e => e.ApplicationUser]);

            var order = new Order();
            order.ApplicationUserId = userId;
            order.OrderDate = DateTime.Now;
            order.OrderTotal = (double)cart.Sum(e => e.Movie.Price * e.Count);

            _unitOfWork.Orders.Create(order);
            _unitOfWork.Orders.Commit();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Movie.Name,
                                Description = item.Movie.Description,
                            },
                            UnitAmount = (long)item.Movie.Price * 100,
                        },
                        Quantity = item.Count,
                    }
                );
            }

            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            _unitOfWork.Orders.Commit();

            List<OrderItem> orderItems = [];
            foreach (var item in cart)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    Count = item.Count,
                    Price = (double)item.Movie.Price,
                    MovieId = item.MovieId,
                };

                orderItems.Add(orderItem);
            }
            _unitOfWork.Orderitems.CreateRange(orderItems);
            _unitOfWork.Orderitems.Commit();

            return Redirect(session.Url);

        }
    }
}

