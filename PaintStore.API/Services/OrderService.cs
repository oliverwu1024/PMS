using PaintStore.API.Repos;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
namespace PaintStore.API.Services;

public class OrderService: IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private readonly ILogger<OrderService> _logger;

    private readonly PaintProductRepository _paintProductRepository;


    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger, PaintProductRepository paintProductRepository)
    {
        _orderRepository = orderRepository;   
        _logger = logger;  // Injected logger for logging
        _paintProductRepository = paintProductRepository;
    } 

    public async Task<Order> GetByIdAsync(int id) {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {   
            _logger.LogWarning("Order with ID {Id} not found", id); // Log warning if order not found   
            throw new KeyNotFoundException($"Order with ID {id} not found.");
        }
        return order;
    }

    public async Task<List<Order>> GetPagedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            _logger.LogWarning("Invalid paging: page {PageNumber}, size {PageSize}", pageNumber, pageSize);
            throw new ArgumentException("Page number and page size must be greater than 0.");

        }  
        var orders = await _orderRepository.GetPagedAsync(pageNumber, pageSize);
        return orders;
    }

    public async Task<List<Order>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice > maxPrice)
        {
            _logger.LogWarning("Invalid price range: min {MinPrice}, max {MaxPrice}", minPrice, maxPrice);
            throw new ArgumentException("Minimum price cannot be greater than maximum price.");
        }

        var orders = await _orderRepository.GetByPriceRangeAsync(minPrice, maxPrice);

        return orders;
    }

    public async Task<List<Order>> GetByPaintIdAsync (int paintId)
    {
        var orders = await _orderRepository.GetByPaintIdAsync(paintId);

        if(orders.Count == 0)
        {
            _logger.LogWarning("No orders found for paint {PaintId}", paintId);
            throw new KeyNotFoundException("No orders with the paint id is found");
        }

        return orders;


        
    }

    public async Task<List<Order>> GetByUserIdAsync (int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);

        if(orders.Count == 0)
        {
            _logger.LogWarning("No orders found for user {UserId}", userId);
            throw new KeyNotFoundException("No orders related to the user id is found");
        }

        return orders;
        
    }

    public async Task<List<Order>> GetByDateAsync (DateTime date)
    {
        return await _orderRepository.GetByDateAsync(date);
    }

    public async Task<List<Order>> GetLastMonthAsync () 
    {
        var oneMonthAgo = DateTimeOffset.UtcNow.AddMonths(-1);
        var orders = await _orderRepository.GetCreatedSinceAsync(oneMonthAgo);
        return orders;
    }

    public async Task<Order> CreateOrderAsync (CreateOrderRequest request)
    {
        if (request.ProductIds.Count == 0 || request.ProductIds.Count != request.Quantities.Count)
        {
            _logger.LogWarning("Invalid order request for user {UserId}", request.UserId);
            throw new ArgumentException("ProductIds and Quantities must be non-empty and the same length.");
        }

        var products = new List<PaintProduct>();
        foreach (var productId in request.ProductIds)
        {
            var product = await _paintProductRepository.GetPaintProductById(productId);
            if (product is null) {
                _logger.LogWarning("Product {ProductId} not found", productId);         
                throw new KeyNotFoundException($"Product {productId} not found"); 
            }
            products.Add(product);

        }
        var order = new Order(0, request.UserId, products, request.Quantities, new OrderHistory());
        order = await _orderRepository.AddAsync(order);
        _logger.LogInformation("Order {OrderId} created for user {UserId}", order.OrderId, request.UserId);
        return order;

    }

}
