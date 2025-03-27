using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class OrderItemRepositry : GenericRepositry<OrderItem>, IOderItemRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public OrderItemRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreateRange(IEnumerable<OrderItem> orderItems)
        {
            dbContext.AddRange(orderItems);
        }
    }
}
