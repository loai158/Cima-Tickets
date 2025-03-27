using Microsoft.AspNetCore.Mvc;
using MovieTickets.UnitOfWorks;
using Stripe.Checkout;

namespace MovieTickets.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class CheckOutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Success(int orderId)
        {
            var order = _unitOfWork.Orders.GetOne(e => e.Id == orderId);

            if (order != null)
            {
                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.PaymentStripeId = session.PaymentIntentId;
                order.Status = true;
                order.PaymentStatus = true;

                _unitOfWork.Orders.Commit();
            }
            return View();

        }
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
