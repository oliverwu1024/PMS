using System.Collections.Concurrent;
using System.Data.Common;
using System.Drawing;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/controller")]
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
        new Order(1,1234, ProductsMock1, new List<int> {5, 10, 15}, history1),
        new Order(2,5678, ProductsMock2, new List<int> {7, 19, 14}, history2),
    };
    private static int _nextOrderId = 3;




    //创建一个API接口Endpoint, 获取所有PaintProduct (无数据， 从MOCK数据拿)
    [HttpGet("all")]
    public ActionResult<IEnumerable<PaintProduct>> GetAll()
    {
        return ProductsMock1.OrderBy((product=>product.Id)).ToList();
    }

    //创建一个API接口Endpoint, 获取所有Orders (无数据，从内存拿)
    [HttpGet("orders")]  
    public ActionResult<IEnumerable<Order>> GetAllOrders()
    {
        return OrdersMock.ToList();
    }   

    [HttpGet("orders/{id:int}")]
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
    public ActionResult<IEnumerable<Order>> FilterOrders([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice > maxPrice)
        {
            return BadRequest("minPrice cannot be greater than max price");
        }

        var result = OrdersMock.Where(o=>o.TotalPrice > minPrice && o.TotalPrice < maxPrice).ToList();
        return Ok(result);
    }

    // 创建新的endpoint GetPaginatedOrders, 传入pageNumber和pageSize, 用Skip和Take分页
    public ActionResult<IEnumerable<Order>> GetPaginatedOrders([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("pageNumber and pageSize must be at least 1.");
        }

        var orders = OrdersMock.OrderBy(o=>o.OrderId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Ok(orders);



    }








}
