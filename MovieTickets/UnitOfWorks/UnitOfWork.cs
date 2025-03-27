using Microsoft.EntityFrameworkCore.Storage;
using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Repositries;

namespace MovieTickets.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IActorRepositry Actors { get; }

        public IMovieRepositry Movies { get; }
        public ICartRepositry Cart { get; }

        public ICategoryRepositry Categories { get; }
        //  public IApplicationUserRepositry Users { get; }
        public ICinemaRepositry Cinemas { get; }

        public IApplicationUserRepositry ApplicationUsers { get; }

        public IOrderRepositry Orders { get; }

        public IOderItemRepositry Orderitems { get; }

        // private readonly Dictionary<Type, object> - 
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            Movies = new MovieRepositry(dbContext);
            Cinemas = new CinemaRepositry(dbContext);
            Categories = new CategoryRepositry(dbContext);
            Actors = new ActorRepositry(dbContext);
            ApplicationUsers = new ApplicationUserRepositry(dbContext);
            Cart = new CartRepositry(dbContext);
            Orders = new OrderRepositry(dbContext);
            Orderitems = new OrderItemRepositry(dbContext);
            this._context = dbContext;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
