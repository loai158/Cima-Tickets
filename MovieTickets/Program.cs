using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.IRepositries;
using MovieTickets.Models;
using MovieTickets.Repositries;
using MovieTickets.UnitOfWorks;
using MovieTickets.Utilities;
using Stripe;

namespace MovieTickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                }).AddEntityFrameworkStores<ApplicationDbContext>()
                                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IMovieRepositry, MovieRepositry>();
            builder.Services.AddScoped<ICinemaRepositry, CinemaRepositry>();
            builder.Services.AddScoped<ICategoryRepositry, CategoryRepositry>();
            builder.Services.AddScoped<IActorRepositry, ActorRepositry>();
            builder.Services.AddScoped<IApplicationUserRepositry, ApplicationUserRepositry>();
            builder.Services.AddScoped<ICartRepositry, CartRepositry>();
            builder.Services.AddScoped<IOrderRepositry, OrderRepositry>();
            builder.Services.AddScoped<IOderItemRepositry, OrderItemRepositry>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            //stripe Payment
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddAutoMapper(typeof(Program));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
