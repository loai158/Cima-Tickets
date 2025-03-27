using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Movie> UsersMovies { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public List<Movie>? FavroitMovies { get; set; }
        public string? Address { get; set; }
    }
}
