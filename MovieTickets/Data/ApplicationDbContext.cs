using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Models;

namespace MovieTickets.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                        .HasMany(m => m.MovieCinemas)
                        .WithMany(c => c.CinemaMovies)
                        .UsingEntity(j => j.ToTable("MovieCinemas"));

            modelBuilder.Entity<Movie>()
            .HasMany(m => m.MovieActors)
            .WithMany(c => c.ActorsMovie)
            .UsingEntity(j => j.ToTable("MovieActors"));

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.MovieCategories)
                .WithMany(c => c.CategoryMovies)
                .UsingEntity(j => j.ToTable("MovieCategories"));

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderItem)
                .WithOne(oi => oi.Order)
                .HasForeignKey<OrderItem>(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasKey(x => new { x.Id, x.OrderId });
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.MovieUsers)
                .WithMany(u => u.UsersMovies)
                .UsingEntity(j => j.ToTable("MovieUsers"));
        }
        public DbSet<MovieTickets.ViewModels.RegiserVM> RegiserVM { get; set; } = default!;
        public DbSet<MovieTickets.ViewModels.LoginVM> LoginVM { get; set; } = default!;
        public DbSet<MovieTickets.ViewModels.UserProfileVM> UserProfileVM { get; set; } = default!;
        public DbSet<MovieTickets.ViewModels.AppUserVM> AppUserVM { get; set; } = default!;

    }
}
