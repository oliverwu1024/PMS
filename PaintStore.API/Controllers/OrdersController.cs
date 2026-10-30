using Microsoft.AspNetCore.Mvc;
using PaintStore.API.Dtos;
using PaintStore.API.Services;
using PaintStore.Models.Methods;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order.ToDto());
    }

    // 创建新的CreateOrder接口, 传入参数, 返回201
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new {id = order.OrderId}, order.ToDto());

    }

    // 创建新的Endpoint FilterOrders, 传入MaxPrice和MinPrice, 筛选总价在两者之间的订单
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {

        var result = await _orderService.GetByPriceRangeAsync(minPrice, maxPrice);
        return Ok(result.Select(o => o.ToDto()));
    }

    // 创建新的endpoint GetPaginatedOrders, 传入pageNumber和pageSize, 用Skip和Take分页
    [HttpGet]  
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {

        var orders = await _orderService.GetPagedAsync(pageNumber, pageSize);
        return Ok(orders.Select(o=>o.ToDto()));
    }
    [HttpGet("paint/{paintId:int}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByPaintId (int paintId)
    {
        var orders = await _orderService.GetByPaintIdAsync(paintId);
        return Ok(orders.Select(o=>o.ToDto()));
    }

    [HttpGet("user/{userId:int}")]     
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId (int userId)
    {
        var orders = await _orderService.GetByUserIdAsync(userId);
        return Ok(orders.Select(o => o.ToDto()));
    }   

    [HttpGet("last-month")]  
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetLastMonthOrders()
    { 


        var orders = await _orderService.GetLastMonthAsync();

        return Ok(orders.Select(o => o.ToDto()));
    }
    [HttpGet("date/{date}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByDate(DateTime date)
    {
        var orders = await _orderService.GetByDateAsync(date);

        return Ok(orders.Select(o => o.ToDto()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
    {
        var order = await _orderService.UpdateOrderAsync(id, request);
        return Ok(order.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }


}
