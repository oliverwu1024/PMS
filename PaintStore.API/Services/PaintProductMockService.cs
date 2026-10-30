using PaintStore.Models.Enums;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;


namespace PaintStore.API.Services;

public class PaintProductMockService : IPaintProductService
{
    public Task<PaintProduct> GetPaintProductById(int id)
    {
        var fakeProduct = new PaintProduct(
            id,
            Brand.Dulux,
            "MOCK - Fake White Paint",
            PaintType.BaseCoat,
            new PaintSpecification("White", 10),
            99.99m
        );

        return Task.FromResult(fakeProduct);
    }
    public Task<List<PaintProduct>> GetAllAsync()
    {
        throw new NotImplementedException();

    }

    public Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();

    }
    public Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        throw new NotImplementedException();

    }

    public Task<List<PaintProduct>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();

    }

    public Task<PaintProduct> CreateAsync(PaintProductRequest request) => throw new NotImplementedException();

    public Task<PaintProduct> UpdateAsync(int id, PaintProductRequest request) => throw new NotImplementedException();

    public Task DeleteAsync(int id) => throw new NotImplementedException();

    public Task<List<PaintProduct>> SearchAsync(PaintProductSearchQuery query) => throw new NotImplementedException();

}



