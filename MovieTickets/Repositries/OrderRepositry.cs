using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class OrderRepositry : GenericRepositry<Order>, IOrderRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public OrderRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
