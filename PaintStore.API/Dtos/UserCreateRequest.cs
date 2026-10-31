using System.ComponentModel.DataAnnotations;

namespace PaintStore.API.Dtos;

//This is a request dto for creating a new user
// request dto is used to validate the data before creating a new user

public class UserCreateRequest
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
