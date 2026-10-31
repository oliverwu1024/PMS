using System.ComponentModel.DataAnnotations;

namespace PaintStore.API.Dtos;

public class UserUpdateRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string UserName {get;set;}

    [Required]
    [EmailAddress]
    public string UserEmail {get;set;}

    [Required]
    [Phone]
    public string UserPhone {get;set;}

    [Range(1, 120)]
    public int Age {get;set;}
}


