using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class ApplicationUserRepositry : GenericRepositry<ApplicationUser>, IApplicationUserRepositry
    {
        private readonly ApplicationDbContext dbContext;

        public ApplicationUserRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
