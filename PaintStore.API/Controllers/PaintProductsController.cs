
using Microsoft.AspNetCore.Mvc;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using PaintStore.API.Services;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/paintproducts")]
public sealed class PaintProductsController: ControllerBase 
{ 


    private readonly IPaintProductService _paintProductService;
    private readonly ILogger<PaintProductsController> _logger; 


    public PaintProductsController(IPaintProductService paintProductService, ILogger<PaintProductsController> logger) {

        _paintProductService = paintProductService;
        _logger = logger;
    }


    //创建一个API接口Endpoint, 获取所有PaintProduct (无数据， 从MOCK数据拿)
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<PaintProductDto>>> GetAll()
    {
        var products = await _paintProductService.GetAllAsync();
        return Ok(products.Select(p => p.ToDto())); 
    }

    [HttpGet("user/{userId:int}")]     
    public async Task<ActionResult<IEnumerable<PaintProductDto>>> GetPaintProductsByUserId (int userId)
    {
        var products = await _paintProductService.GetByUserIdAsync(userId);
        return Ok(products.Select(p => p.ToDto()));  
    }  



    [HttpGet]  
    public async Task<ActionResult<IEnumerable<PaintProductDto>>> GetAllPaintProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var products =  await _paintProductService.GetPagedAsync(pageNumber, pageSize);
        return Ok(products.Select(p => p.ToDto()));
    }
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<PaintProductDto>>> GetPaintProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var products = await _paintProductService.GetByPriceRangeAsync(minPrice, maxPrice);
        return Ok(products.Select(p => p.ToDto()));
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaintProductDto>> GetPaintProductsByPaintId(int id)
    {
        var products =await _paintProductService.GetPaintProductById(id);
        return Ok(products.ToDto());

    }


    [HttpPost]
    public async Task<ActionResult<PaintProductDto>> CreatePaintProduct([FromBody] PaintProductRequest request)
    {
        var product = await _paintProductService.CreateAsync(request);
        return CreatedAtAction(nameof(GetPaintProductsByPaintId), new { id = product.Id }, product.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PaintProductDto>> UpdatePaintProduct(int id, [FromBody] PaintProductRequest request)
    {
        var product = await _paintProductService.UpdateAsync(id, request);
        return Ok(product.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePaintProduct(int id)
    {
        await _paintProductService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PaintProductDto>>> SearchPaintProducts([FromQuery] PaintProductSearchQuery query)
    {
        var products = await _paintProductService.SearchAsync(query);
        return Ok(products.Select(p => p.ToDto()));
    }





}
