using System.ComponentModel.DataAnnotations;

namespace crud_api.Dtos;
public class LoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string? Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[A-Z]).{5,}$", ErrorMessage = "Password must be at least 5 characters long and contain at least 1 uppercase letter.")]
    public string? Password { get; set; }
}