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
using PaintStore.API.Data;
using Microsoft.EntityFrameworkCore;
using PaintStore.API.Services;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaintProductController: ControllerBase 
{ 
    private readonly AppDbContext _db;

    private readonly IPaintProductService _paintProductService;
    private readonly ILogger<PaintProductController> _logger; 

    private readonly IOrderService _orderService;

    public PaintProductController(IPaintProductService paintProductService, ILogger<PaintProductController> logger, IOrderService orderService) {
        //_db=db;
        _paintProductService = paintProductService;
        _logger = logger;
        _orderService = orderService;
       // _paintProductService = new PaintProductService();
        //_paintProductService = paintProductService;
    }


    //创建一个API接口Endpoint, 获取所有PaintProduct (无数据， 从MOCK数据拿)
    [HttpGet("all")]
    public ActionResult<IEnumerable<PaintProduct>> GetAll()
    {
        return _db.PaintProducts.OrderBy((product=>product.Id)).ToList();
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
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order);
    }

    // 创建新的CreateOrder接口, 传入参数, 返回201
    [HttpPost("orders")]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new {id = order.OrderId}, order);

    }

    // 创建新的Endpoint FilterOrders, 传入MaxPrice和MinPrice, 筛选总价在两者之间的订单
    [HttpGet("orders/filter")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {

        var result = await _orderService.GetByPriceRangeAsync(minPrice, maxPrice);
        return Ok(result);
    }

    // 创建新的endpoint GetPaginatedOrders, 传入pageNumber和pageSize, 用Skip和Take分页
    [HttpGet("orders")]  
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {

        var orders = await _orderService.GetPagedAsync(pageNumber, pageSize);
        return Ok(orders);
    }
    [HttpGet("orders/paint/{paintId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByPaintId (int paintId)
    {
        var orders = await _orderService.GetByPaintIdAsync(paintId);
        return Ok(orders);
    }

    [HttpGet("orders/user/{userId}")]     
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId (int userId)
    {
        var orders = await _orderService.GetByUserIdAsync(userId);
        return Ok(orders);
    }   

    [HttpGet("orders/last-month")]  
    public async Task<ActionResult<IEnumerable<Order>>> GetLastMonthOrders()
    { 


        var orders = await _orderService.GetLastMonthAsync();

        return Ok(orders);
    }
    [HttpGet("orders/date/{date}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByDate(DateTime date)
    {
        var orders = await _orderService.GetByDateAsync(date);

        return Ok(orders);
    }

    [HttpGet("paintproducts")]  
    public ActionResult<IEnumerable<PaintProduct>> GetAllPaintProducts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var products = _db.PaintProducts.OrderBy(p=>p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(products);
    }
    [HttpGet("paintproducts/filter")]
    public ActionResult<IEnumerable<PaintProduct>> GetPaintProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice > maxPrice)
        {
            return BadRequest("minPrice cannot be greater than max price");
        }

        var result = _db.PaintProducts.Where(p=>p.Price >= minPrice && p.Price <= maxPrice).ToList();
        return Ok(result);
    }

    [HttpGet("paintproducts/paint/{paintId:int}")]
    public async Task<ActionResult<PaintProduct>> GetPaintProductsByPaintId(int paintId)
    {
        _logger.LogInformation("Received request for paint product with id {Id}", paintId);

        if (paintId <= 0)
        {
            _logger.LogWarning("Invalid paint product id: {Id}", paintId);
            return BadRequest("Invalid paint product id");
        }

        var product = await _paintProductService.GetPaintProductById(paintId);
        _logger.LogInformation("Returning paint product with id {Id}", paintId);
        return Ok(product);

    }





    [HttpGet("paintproducts/user/{userId}")]     
    public ActionResult<IEnumerable<PaintProduct>> GetPaintProductsByUserId (int userId)
    {
        var products = 	_db.Orders.Include(o => o.Products).Where(o => o.UserId == userId).SelectMany(o => o.Products).DistinctBy(p => p.Id).ToList();

        return products.Count == 0 ? NotFound(): Ok(products);
    }  





    [HttpGet("users")]  
    public ActionResult<IEnumerable<User>> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var users = _db.Users.OrderBy(u=>u.UserId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(users);
    }
    [HttpGet("users/user/{userId:int}")]
    public ActionResult<User> GetUserById (int userId)
    {
        var user =  _db.Users.FirstOrDefault(u =>u.UserId ==userId);
        return user is null ? NotFound(): Ok(user);
    }
    [HttpGet("users/username/{userName}")]
    public ActionResult<IEnumerable<User>> GetUserByName (string userName)
    {
        var users =  _db.Users.Where(u =>u.UserName ==userName).ToList();
        return users.Count == 0 ? NotFound(): Ok(users);
    }

    [HttpGet("users/usermail/{userMail}")]
    public ActionResult<IEnumerable<User>> GetUserByEmail (string userMail)
    {
        var users =  _db.Users.Where(u =>u.UserEmail ==userMail).ToList();
        return users.Count == 0 ? NotFound(): Ok(users);
    }

}
