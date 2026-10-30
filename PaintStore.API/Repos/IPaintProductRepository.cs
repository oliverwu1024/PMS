using PaintStore.Models.Methods;
using PaintStore.API.Dtos;


namespace PaintStore.API.Repos;

public interface IPaintProductRepository
{
    Task<PaintProduct?> GetPaintProductById(int id);

    Task<List<PaintProduct>> GetAllAsync();

    Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

    Task<List<PaintProduct>> GetByUserIdAsync(int userId);

    Task<PaintProduct> AddAsync(PaintProduct product);

    Task UpdateAsync(PaintProduct product);

    Task DeleteAsync(PaintProduct product);

    Task<bool> SeriesExistsAsync(int seriesId);

    Task<List<PaintProduct>> SearchAsync(PaintProductSearchQuery query);



}
