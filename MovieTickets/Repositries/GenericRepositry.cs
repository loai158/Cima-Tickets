using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.IRepositries;
using System.Linq.Expressions;

namespace MovieTickets.Repositries
{
    public class GenericRepositry<T> : IGenericRepositry<T> where T : class
    {
        //  ApplicationDbContext dbContext = new ApplicationDbContext();

        public DbSet<T> dbSet;
        private readonly ApplicationDbContext dbContext;
        public GenericRepositry(ApplicationDbContext dbContext)
        {
            dbSet = dbContext.Set<T>();
            this.dbContext = dbContext;
        }

        // CRUD
        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Create(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
            dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(List<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        public T? GetOne(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
            return Get(filter, includes, tracked).FirstOrDefault();
        }


    }

}
