namespace PaintStore.API.Dtos;

public class CreateOrderRequest
{
    public int UserId {get;set;}
    public List<int> ProductIds {get;set;} = new();
    public List<int> Quantities {get;set;} = new();

}
