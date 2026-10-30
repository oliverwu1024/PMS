using PaintStore.Models.Methods;
using PaintStore.API.Data; 
using Microsoft.EntityFrameworkCore;

namespace PaintStore.API.Repos;

public class OrderRepository: IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository (AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var order = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(o=>o.OrderId ==id);
        return order;
    }

    public async Task<List<Order>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var orders = await _context.Orders.Include(o => o.Products).OrderBy(o=>o.OrderId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return orders;
    }

    public async Task<List<Order>> GetByPriceRangeAsync (decimal minPrice, decimal maxPrice)
    {
        var orders = await _context.Orders.Include(o => o.Products).Where(o=>o.TotalPrice >= minPrice && o.TotalPrice <= maxPrice).ToListAsync();
        return orders;
    }

    public async Task<List<Order>> GetByPaintIdAsync (int paintId)
    {
        var orders = await _context.Orders.Include(o => o.Products).Where(o => o.Products.Any(p => p.Id  == paintId)).ToListAsync();
        return orders;
    }

    public async Task<List<Order>> GetByUserIdAsync (int userId)
    {
        var orders = await _context.Orders.Include(o => o.Products).Where(o => o.UserId == userId).ToListAsync();
        return orders;
    }

    public async Task<List<Order>> GetByDateAsync (DateTime date)
    {
        var orders = await _context.Orders.Include(o => o.Products).Where(o=> o.CreatedAt.Date == date.Date).ToListAsync();
        return orders;
    }

    public async Task<List<Order>> GetCreatedSinceAsync (DateTimeOffset date)
    {
        var orders = await _context.Orders.Include(o => o.Products).Where(o=> o.CreatedAt >= date).ToListAsync();
        return orders;
    }


    public async Task<Order> AddAsync (Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(); //save changes to database
        return order;
    }


    public async Task UpdateAsync (Order order)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync (Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

}
