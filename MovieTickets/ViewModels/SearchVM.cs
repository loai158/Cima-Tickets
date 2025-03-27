using System.ComponentModel.DataAnnotations;

namespace MovieTickets.ViewModels
{
    public class SearchVM
    {
        [Required(ErrorMessage = "a value is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Search value must be between 2 and 100 characters.")]
        public string value { get; set; }
    }
}
