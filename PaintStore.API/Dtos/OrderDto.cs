
namespace PaintStore.API.Dtos;

public class OrderDto
{
    public int OrderId {get; set;}

    public int UserId {get; set;}

    public DateTimeOffset CreatedAt {get; set;}

    public decimal TotalPrice {get; set;}

    public List<PaintProductDto> Products {get; set;} =new();

    public List<int> Quantities {get; set;} = new();
}
