using PaintStore.Models.Methods;

namespace PaintStore.API.Repos;

public interface IPaintProductRepository
{
    Task<PaintProduct?> GetPaintProductById(int id);

    Task<List<PaintProduct>> GetAllAsync();

    Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

    Task<List<PaintProduct>> GetByUserIdAsync(int userId);
}
