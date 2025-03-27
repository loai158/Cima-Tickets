using Microsoft.EntityFrameworkCore.Storage;
using MovieTickets.IRepositries;

namespace MovieTickets.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IActorRepositry Actors { get; }
        IMovieRepositry Movies { get; }
        ICategoryRepositry Categories { get; }
        ICinemaRepositry Cinemas { get; }
        ICartRepositry Cart { get; }
        IOrderRepositry Orders { get; }
        IOderItemRepositry Orderitems { get; }
        public IApplicationUserRepositry ApplicationUsers { get; }

        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
        Task<int> CompleteAsync();
    }
}
