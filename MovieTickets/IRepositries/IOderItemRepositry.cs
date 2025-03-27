using MovieTickets.Models;

namespace MovieTickets.IRepositries
{
    public interface IOderItemRepositry : IGenericRepositry<OrderItem>
    {
        public void CreateRange(IEnumerable<OrderItem> orderItems);
    }
}
