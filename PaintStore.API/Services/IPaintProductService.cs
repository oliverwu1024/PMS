using PaintStore.Models.Methods;
using PaintStore.API.Dtos;


namespace PaintStore.API.Services;

public interface IPaintProductService
{
    Task<PaintProduct> GetPaintProductById(int id);

    Task<List<PaintProduct>> GetAllAsync();

    Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

    Task<List<PaintProduct>> GetByUserIdAsync(int userId);

    Task<PaintProduct> CreateAsync(PaintProductRequest request);

    Task<PaintProduct> UpdateAsync(int id, PaintProductRequest request);

    Task DeleteAsync(int id);

    Task<List<PaintProduct>> SearchAsync(PaintProductSearchQuery query);


}
