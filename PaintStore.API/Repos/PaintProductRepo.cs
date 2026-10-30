using PaintStore.Models.Methods;
using PaintStore.API.Data;
using PaintStore.API.Dtos;

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


    public async Task<PaintProduct> AddAsync(PaintProduct product)
    {
        _context.PaintProducts.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(PaintProduct product)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PaintProduct product)
    {
        _context.PaintProducts.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SeriesExistsAsync(int seriesId)
    {
        return await _context.PaintSeries.AnyAsync(s => s.Id == seriesId);
    }


    public async Task<List<PaintProduct>> SearchAsync(PaintProductSearchQuery query)
    {
        IQueryable<PaintProduct> products = _context.PaintProducts;

        if (!string.IsNullOrWhiteSpace(query.Name))
            products = products.Where(p => p.Name.Contains(query.Name));

        if (query.Brand.HasValue)
            products = products.Where(p => p.BrandName == query.Brand.Value);

        if (query.Type.HasValue)
            products = products.Where(p => p.Type == query.Type.Value);

        if (!string.IsNullOrWhiteSpace(query.Color))
            products = products.Where(p => p.Specification.Color == query.Color);

        if (query.MinPrice.HasValue)
            products = products.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            products = products.Where(p => p.Price <= query.MaxPrice.Value);

        return await products.OrderBy(p => p.Id).ToListAsync();
    }



    
}
