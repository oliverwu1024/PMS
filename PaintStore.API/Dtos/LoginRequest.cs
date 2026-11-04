using System.ComponentModel.DataAnnotations;
namespace PaintStore.API.Dtos;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

}
