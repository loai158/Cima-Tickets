using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;

namespace MovieTickets.Repositries
{
    public class ActorRepositry : GenericRepositry<Actor>, IActorRepositry
    {
        public ActorRepositry(ApplicationDbContext dbContext) : base(dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}
