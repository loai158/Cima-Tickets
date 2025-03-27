using System.ComponentModel.DataAnnotations;

namespace MovieTickets.ViewModels
{
    public class AppUserVM
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? Address { get; set; }

        public string? ImageUrl { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }



    }
}
