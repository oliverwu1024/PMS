using PaintStore.Models.Enums;
using PaintStore.Models.Methods;

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
}
