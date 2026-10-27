namespace PaintStore.API.Services;

using PaintStore.API.Dtos;
using PaintStore.Models.Methods;

public interface IOrderService
{
    Task<Order> GetByIdAsync(int id);

    Task<List<Order>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<Order>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice); 

    Task<List<Order>> GetByPaintIdAsync (int paintId);
    Task<List<Order>> GetByUserIdAsync (int userId);

    Task<List<Order>> GetByDateAsync (DateTime date);

    Task<List<Order>> GetLastMonthAsync ();

    Task<Order> CreateOrderAsync (CreateOrderRequest request);
}
