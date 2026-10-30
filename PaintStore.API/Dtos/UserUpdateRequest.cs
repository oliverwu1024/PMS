using System.ComponentModel.DataAnnotations;

namespace PaintStore.API.Dtos;

public class UserUpdateRequest
{
    public string UserName {get;set;}

    [EmailAddress]
    public string UserEmail {get;set;}

    [Phone]
    public string UserPhone {get;set;}

    public int Age {get;set;}
}
