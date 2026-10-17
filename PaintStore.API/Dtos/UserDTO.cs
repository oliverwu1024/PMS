using System.ComponentModel.DataAnnotations;


namespace PaintStore.API.Dtos;

//This is a response dto for a user
// response dto is used to send data back to the client
public class UserDTO
{
    public int UserId {get;set;}
    public string UserName {get; set;}

    [EmailAddress]
    public string UserEmail {get;set;}
}
