using PaintStore.Models.Methods;

namespace PaintStore.API.Dtos;

// converts entities into response dtos
// "this" makes each method an extension method, so we can call product.ToDto()
public static class DtoMappings
{
    public static PaintProductDto ToDto(this PaintProduct product)
    {
        return new PaintProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.BrandName.ToString(),
            Type = product.Type.ToString(),
            Color = product.Specification.Color,
            SizeInLiters = product.Specification.SizeInLiters,
            Price = product.Price
        };
    }

    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            TotalPrice = order.TotalPrice,
            Products = order.Products.Select(p => p.ToDto()).ToList(),
            Quantities = order.Quantities
        };
    }

    public static UserDTO ToDto(this User user)
    {
        return new UserDTO
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserEmail = user.UserEmail
        };
    }
}
