namespace PaintStore.API.Dtos;

public class UpdateOrderRequest
{
    public List<int> ProductIds {get; set;} = new();
    public List<int> Quantities {get;set;} = new();
}
