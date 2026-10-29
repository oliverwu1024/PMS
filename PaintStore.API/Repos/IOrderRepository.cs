using PaintStore.Models.Methods;

namespace PaintStore.API.Repos;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);

    Task<List<Order>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<Order>> GetByPriceRangeAsync (decimal minPrice, decimal maxPrice);

    Task<List<Order>> GetByPaintIdAsync (int paintId);


    Task<List<Order>> GetByUserIdAsync (int userId);


    Task<List<Order>> GetByDateAsync (DateTime date);


    Task<List<Order>> GetCreatedSinceAsync (DateTimeOffset date);

    Task<Order> AddAsync (Order order);


}
