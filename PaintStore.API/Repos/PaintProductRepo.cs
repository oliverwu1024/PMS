using PaintStore.Models.Methods;
using PaintStore.API.Data;

using Microsoft.EntityFrameworkCore;

namespace PaintStore.API.Repos;

public class PaintProductRepository: IPaintProductRepository
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

    public async Task<List<PaintProduct>> GetAllAsync()
    {
        return await _context.PaintProducts.OrderBy((product=>product.Id)).ToListAsync();
    }

    public async Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var products = await _context.PaintProducts.OrderBy(p=>p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return products;
    }

    public async Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var result = await _context.PaintProducts.Where(p=>p.Price >= minPrice && p.Price <= maxPrice).ToListAsync();
        return result;
    }

    public async Task<List<PaintProduct>> GetByUserIdAsync(int userId)
    {
        var products = 	await _context.Orders.Where(o => o.UserId == userId).SelectMany(o => o.Products).Distinct().ToListAsync();

        return products;
    }

    
}
