using System.ComponentModel.DataAnnotations;

namespace MovieTickets.ViewModels
{
    public class UserProfileVM
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        public string? ImageUrl { get; set; }

    }
}
