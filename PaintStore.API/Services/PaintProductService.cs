using PaintStore.Models.Methods;
using PaintStore.API.Data;
using PaintStore.API.Repos;


namespace PaintStore.API.Services;

public class PaintProductService: IPaintProductService
{

    public readonly PaintProductRepository _paintProductRepository;
    public PaintProductService(PaintProductRepository paintProductRepository)
    {   
        //var options
        _paintProductRepository = paintProductRepository;
        //_paintProductRepository = new PaintProductRepository(new AppDbContext(options));
    }
    public async Task<PaintProduct> GetPaintProductById(int id)
    {

        var paintProduct = await _paintProductRepository.GetPaintProductById(id);

        if (paintProduct ==null)
        {
            throw new KeyNotFoundException($"Paint product with ID {id} not found");
        }
        /*
        else
        {
            if (paintProduct.Stock<=0)
            {
                Console.WriteLine("Product is out of stock");
            }
        }
        */
        return paintProduct;


    }
}
