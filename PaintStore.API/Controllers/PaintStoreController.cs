
using Microsoft.AspNetCore.Mvc;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using PaintStore.API.Services;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaintProductController: ControllerBase 
{ 


    private readonly IPaintProductService _paintProductService;
    private readonly ILogger<PaintProductController> _logger; 

    private readonly IOrderService _orderService;

    private readonly IUserService _userService;

    public PaintProductController(IPaintProductService paintProductService, ILogger<PaintProductController> logger, IOrderService orderService, IUserService userService) {

        _paintProductService = paintProductService;
        _logger = logger;
        _orderService = orderService;
        _userService = userService;
    }


    //创建一个API接口Endpoint, 获取所有PaintProduct (无数据， 从MOCK数据拿)
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<PaintProduct>>> GetAll()
    {
        return await _paintProductService.GetAllAsync();
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
    public async Task<ActionResult<IEnumerable<PaintProduct>>> GetAllPaintProducts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var products =  await _paintProductService.GetPagedAsync(pageNumber, pageSize);
        return Ok(products);
    }
    [HttpGet("paintproducts/filter")]
    public async Task<ActionResult<IEnumerable<PaintProduct>>> GetPaintProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var products = await _paintProductService.GetByPriceRangeAsync(minPrice, maxPrice);
        return products;
    }
    [HttpGet("paintproducts/paint/{paintId:int}")]
    public async Task<ActionResult<PaintProduct>> GetPaintProductsByPaintId(int paintId)
    {
        var products =await _paintProductService.GetPaintProductById(paintId);
        return products;

    }





    [HttpGet("paintproducts/user/{userId}")]     
    public async Task<ActionResult<IEnumerable<PaintProduct>>> GetPaintProductsByUserId (int userId)
    {
        var products = await _paintProductService.GetByUserIdAsync(userId);
        return products;
    }  





    [HttpGet("users")]  
    public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {


        var users = await _userService.GetPagedAsync(pageNumber, pageSize);
        return Ok(users);
    }


    [HttpGet("users/user/{userId:int}")]
    public async Task<ActionResult<User>> GetUserById (int userId)
    {
        var user =  await _userService.GetByIdAsync(userId);
        return Ok(user);
    }


    [HttpGet("users/username/{userName}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUserByName (string userName)
    {
        var users =  await _userService.GetByNameAsync(userName);
        return Ok(users);
    }

    [HttpGet("users/usermail/{userMail}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUserByEmail (string userMail)
    {
        var users = await _userService.GetByEmailAsync(userMail);
        return Ok(users);
    }

}
