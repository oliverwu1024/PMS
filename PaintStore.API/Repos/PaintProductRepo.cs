using PaintStore.Models.Methods;
using PaintStore.API.Data;

namespace PaintStore.API.Repos;

public class PaintProductRepository
{
    private readonly AppDbContext _context;
    public PaintProductRepository(AppDbContext context)
    {
        
        //var options
        _context = context;
        //_context = new AppDbContext(options);
    }

    public async Task<PaintProduct?> GetPaintProductById(int id)
    {
        var paintProduct = await _context.PaintProducts.FindAsync(id);
        return paintProduct;
    }



    
}
