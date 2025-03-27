using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class MovieRepositry : GenericRepositry<Movie>, IMovieRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public MovieRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
