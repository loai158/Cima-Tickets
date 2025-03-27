using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class CategoryRepositry : GenericRepositry<Category>, ICategoryRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
