using System.ComponentModel.DataAnnotations;
namespace PaintStore.API.Dtos;

public class SignupRequest
{

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Phone]
    public string Phone { get; set; }

    public int Age { get; set; }


}
