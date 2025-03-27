using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class CinemaRepositry : GenericRepositry<Cinema>, ICinemaRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public CinemaRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
