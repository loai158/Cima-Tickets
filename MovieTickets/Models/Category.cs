namespace MovieTickets.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Movie> CategoryMovies { get; set; }

    }
}
