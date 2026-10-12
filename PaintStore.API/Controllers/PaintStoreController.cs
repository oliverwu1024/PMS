using System.Collections.Concurrent;
using System.Data.Common;
using System.Drawing;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaintProductController: ControllerBase 
{ 
    public static readonly List<PaintProduct> ProductsMock1 = new()
    {
        new PaintProduct(1, Brand.Dulux, "Interior Matte", PaintType.Matte, new PaintSpecification("White", 4), 29.99m),
        new PaintProduct(2, Brand.Taubmans, "Exterior Gloss", PaintType.Gloss, new PaintSpecification("Blue", 10), 39.50m),
        new PaintProduct(3, Brand.Haymes, "Primer", PaintType.BaseCoat, new PaintSpecification("Gray", 15), 19.00m),
    
    };

    public static List<PaintProduct> ProductsMock2 = new()
    {
        new PaintProduct(1, Brand.Dulux, "Interior Matte", PaintType.Matte, new PaintSpecification("White", 4), 29.99m),
        new PaintProduct(2, Brand.Taubmans, "Exterior Gloss", PaintType.Gloss, new PaintSpecification("Blue", 10), 39.50m),
        new PaintProduct(3, Brand.Haymes, "Primer", PaintType.BaseCoat, new PaintSpecification("Gray", 15), 19.00m),
    
    };

    public static OrderHistory history1 = new OrderHistory(); 
    public static OrderHistory history2 = new OrderHistory(); 


    private static readonly List<Order> OrdersMock = new()
    {
        new Order(1,1234, ProductsMock1, new List<int> {5, 10, 15}, history1){ CreatedAt = DateTimeOffset.UtcNow.AddDays(-3) },
        new Order(2,5678, ProductsMock2, new List<int> {7, 19, 14}, history2){ CreatedAt = DateTimeOffset.UtcNow.AddDays(-45) },
    };
    private static int _nextOrderId = 3;

     private static readonly List<User> UsersMock = new()
    {
        new User("Steven", "steven@example.com", "0400000000", 20, 1234),
        new User("Rachel", "rachel@example.com", "0400000123" ,25, 5678),
    };   




    //创建一个API接口Endpoint, 获取所有PaintProduct (无数据， 从MOCK数据拿)
    [HttpGet("all")]
    public ActionResult<IEnumerable<PaintProduct>> GetAll()
    {
        return ProductsMock1.OrderBy((product=>product.Id)).ToList();
    }

    //创建一个API接口Endpoint, 获取所有Orders (无数据，从内存拿)
    /* retired without pagination
    [HttpGet("orders")]  
    public ActionResult<IEnumerable<Order>> GetAllOrders()
    {
        return OrdersMock.ToList();
    }   
    */

    [HttpGet("orders/order/{id:int}")]
    public ActionResult<Order> GetOrderById(int id)
    {
        var order = OrdersMock.FirstOrDefault(o=>o.OrderId ==id);
        return order is null ? NotFound(): Ok(order);
    }

    // 创建新的CreateOrder接口, 传入参数, 返回201
    [HttpPost("orders")]
    public ActionResult<Order> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.ProductIds.Count == 0 || request.ProductIds.Count != request.Quantities.Count)
        {
            return BadRequest("ProductIds and Quantities must be non-empty and the same length.");
        }

        var products = new List<PaintProduct>();
        foreach (var productId in request.ProductIds)
        {
            var product = ProductsMock1.FirstOrDefault(p=>p.Id == productId);
            if (product is null) {
                return NotFound($"Product {productId} not found");
            }
            products.Add(product);
        }

        var order = new Order(_nextOrderId++, request.UserId, products, request.Quantities, history1);
        OrdersMock.Add(order);

        // 201 Created + Location header pointing to get orders/{id}
        return CreatedAtAction(nameof(GetOrderById), new {id =order.OrderId}, order);
    }

    // 创建新的Endpoint FilterOrders, 传入MaxPrice和MinPrice, 筛选总价在两者之间的订单
    [HttpGet("orders/filter")]
    public ActionResult<IEnumerable<Order>> GetOrdersByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice > maxPrice)
        {
            return BadRequest("minPrice cannot be greater than max price");
        }

        var result = OrdersMock.Where(o=>o.TotalPrice >= minPrice && o.TotalPrice <= maxPrice).ToList();
        return Ok(result);
    }

    // 创建新的endpoint GetPaginatedOrders, 传入pageNumber和pageSize, 用Skip和Take分页
    [HttpGet("orders")]  
    public ActionResult<IEnumerable<Order>> GetAllOrders([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var orders = OrdersMock.OrderBy(o=>o.OrderId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(orders);
    }
    [HttpGet("orders/paint/{paintId}")]
    public ActionResult<IEnumerable<Order>> GetOrdersByPaintId (int paintId)
    {
        var orders = OrdersMock.Where(o => o.Products.Any(p=>p.Id == paintId)).ToList();
        return orders.Count == 0 ? NotFound(): Ok(orders);
    }

    [HttpGet("orders/user/{userId}")]     
    public ActionResult<IEnumerable<Order>> GetOrdersByUserId (int userId)
    {
        var orders = OrdersMock.Where(o=>o.UserId == userId).ToList();
        return orders.Count == 0 ? NotFound(): Ok(orders);
    }   

    [HttpGet("orders/last-month")]  
    public ActionResult<IEnumerable<Order>> GetLastMonthOrders()
    { 
        var oneMonthAgo = DateTimeOffset.UtcNow.AddMonths(-1);

        var orders = OrdersMock
            .Where(o => o.CreatedAt >= oneMonthAgo)
            .ToList();

        return Ok(orders);
    }
    [HttpGet("orders/date/{date}")]
    public ActionResult<IEnumerable<Order>> GetOrdersByDate(DateTime date)
    {
        var orders = OrdersMock
            .Where(o => o.CreatedAt.Date == date.Date)
            .ToList();

        return Ok(orders);
    }

    [HttpGet("paintproducts")]  
    public ActionResult<IEnumerable<PaintProduct>> GetAllPaintProducts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var products = ProductsMock1.OrderBy(p=>p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(products);
    }
    [HttpGet("paintproducts/filter")]
    public ActionResult<IEnumerable<PaintProduct>> GetPaintProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice > maxPrice)
        {
            return BadRequest("minPrice cannot be greater than max price");
        }

        var result = ProductsMock1.Where(p=>p.Price >= minPrice && p.Price <= maxPrice).ToList();
        return Ok(result);
    }

    [HttpGet("paintproducts/paint/{paintId:int}")]
    public ActionResult<PaintProduct> GetPaintProductsByPaintId (int paintId)
    {
        var products = ProductsMock1.FirstOrDefault(p =>p.Id == paintId);
        return products is null ? NotFound(): Ok(products);
    }




    [HttpGet("paintproducts/user/{userId}")]     
    public ActionResult<IEnumerable<PaintProduct>> GetPaintProductsByUserId (int userId)
    {
        var products = OrdersMock.Where(o => o.UserId == userId).SelectMany(o => o.Products).DistinctBy(p => p.Id).ToList();

        return products.Count == 0 ? NotFound(): Ok(products);
    }  





    [HttpGet("users")]  
    public ActionResult<IEnumerable<User>> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var users = UsersMock.OrderBy(u=>u.UserId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(users);
    }
    [HttpGet("users/user/{userId:int}")]
    public ActionResult<User> GetUserById (int userId)
    {
        var user =  UsersMock.FirstOrDefault(u =>u.UserId ==userId);
        return user is null ? NotFound(): Ok(user);
    }
    [HttpGet("users/username/{userName}")]
    public ActionResult<IEnumerable<User>> GetUserByName (string userName)
    {
        var users =  UsersMock.Where(u =>u.UserName ==userName).ToList();
        return users.Count == 0 ? NotFound(): Ok(users);
    }

    [HttpGet("users/usermail/{userMail}")]
    public ActionResult<IEnumerable<User>> GetUserByEmail (string userMail)
    {
        var users =  UsersMock.Where(u =>u.UserEmail ==userMail).ToList();
        return users.Count == 0 ? NotFound(): Ok(users);
    }

}
