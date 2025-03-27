using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class CartRepositry : GenericRepositry<Cart>, ICartRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public CartRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
