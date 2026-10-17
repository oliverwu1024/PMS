using System.ComponentModel.DataAnnotations;

namespace PaintStore.API.Dtos;

//This is a request dto for creating a new user
// request dto is used to validate the data before creating a new user

public class UserCreateRequest
{
    public string UserName {get;set;}

    [EmailAddress]
    public string UserEmail {get;set;}

    [Phone]
    public string UserPhone {get;set;}

    public int Age {get;set;}

    public int UserId {get;set;}
}
