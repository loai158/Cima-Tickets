using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public Decimal Price { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ImgUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? TrailerUrl { get; set; }
        [NotMapped]
        public bool? IsFavorite { get; set; }
        public List<ApplicationUser>? MovieUsers { get; set; }

        public List<Actor> MovieActors { get; set; }
        public List<Cinema> MovieCinemas { get; set; }
        public List<Category> MovieCategories { get; set; }


    }
}
