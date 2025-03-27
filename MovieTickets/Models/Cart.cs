using Microsoft.EntityFrameworkCore;

namespace MovieTickets.Models
{
    [PrimaryKey(nameof(MovieId), nameof(ApplicationUserId))]
    public class Cart
    {
        public int MovieId { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public Movie Movie { get; set; }
        public int Count { get; set; }
    }
}
