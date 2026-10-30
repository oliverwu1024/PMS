using PaintStore.Models.Methods;
using PaintStore.API.Data;
using PaintStore.API.Repos;
using PaintStore.API.Dtos;



namespace PaintStore.API.Services;

public class PaintProductService: IPaintProductService
{

    private readonly IPaintProductRepository _paintProductRepository;
    private readonly ILogger<PaintProductService> _logger;
    public PaintProductService(IPaintProductRepository paintProductRepository, ILogger<PaintProductService> logger)
    {   
        //var options
        _paintProductRepository = paintProductRepository;
        _logger = logger;
        //_paintProductRepository = new PaintProductRepository(new AppDbContext(options));
    }
    public async Task<PaintProduct> GetPaintProductById(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid paint product id {Id}", id);       
            throw new ArgumentException( "Paint product id must be greater than 0.");
        }

        var paintProduct = await _paintProductRepository.GetPaintProductById(id);

        if (paintProduct ==null)
        {
            _logger.LogWarning("Paint product with ID {Id} not found", id);
            throw new KeyNotFoundException($"Paint product with ID {id} not found");
        }
        /*
        else
        {
            if (paintProduct.Stock<=0)
            {
                Console.WriteLine("Product is out of stock");
            }
        }
        */
        return paintProduct;


    }

    public async Task<List<PaintProduct>> GetAllAsync()
    {
        return await _paintProductRepository.GetAllAsync();
    }

    public async Task<List<PaintProduct>> GetPagedAsync(int pageNumber, int pageSize)
    {

        if (pageNumber < 1 || pageSize < 1)
        {
            _logger.LogWarning("Invalid paging: page {PageNumber}, size {PageSize}", pageNumber, pageSize);
            throw new ArgumentException("Page number and page size must be greater than 0.");

        }  
        return await _paintProductRepository.GetPagedAsync(pageNumber, pageSize);
    }

    public async Task<List<PaintProduct>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {

        if (minPrice > maxPrice)
        {
            _logger.LogWarning("Invalid price range: min {MinPrice}, max {MaxPrice}", minPrice, maxPrice);
            throw new ArgumentException("Minimum price cannot be greater than maximum price.");
        }
        return await _paintProductRepository.GetByPriceRangeAsync(minPrice, maxPrice);
    }

    public async Task<List<PaintProduct>> GetByUserIdAsync(int userId)
    {
        var products = await _paintProductRepository.GetByUserIdAsync(userId);

        if(products.Count == 0)
        {
            _logger.LogWarning("No products found for user {UserId}", userId);
            throw new KeyNotFoundException("No products related to the user id is found");
        }

        return products;
    }

    private async Task EnsureSeriesExistsAsync(int seriesId)
    {
        if (!await _paintProductRepository.SeriesExistsAsync(seriesId))
        {
            _logger.LogWarning("Series {SeriesId} not found", seriesId);
            throw new ArgumentException($"Series {seriesId} does not exist.");
        }
    }

    public async Task<PaintProduct> CreateAsync(PaintProductRequest request)
    {
        await EnsureSeriesExistsAsync(request.SeriesId);

        var spec = new PaintSpecification(request.Color, request.SizeInLiters);
        var product = new PaintProduct(0, request.BrandName, request.Name, request.Type, spec, request.Price)
        {
            SeriesId = request.SeriesId
        };

        product = await _paintProductRepository.AddAsync(product);
        _logger.LogInformation("Paint product {Id} created", product.Id);
        return product;
    }

    public async Task<PaintProduct> UpdateAsync(int id, PaintProductRequest request)
    {
        var product = await GetPaintProductById(id);   // 404 if missing
        await EnsureSeriesExistsAsync(request.SeriesId);

        product.Name = request.Name;
        product.BrandName = request.BrandName;
        product.Type = request.Type;
        product.Price = request.Price;
        product.SeriesId = request.SeriesId;
        product.Specification.Color = request.Color;
        product.Specification.SizeInLiters = request.SizeInLiters;

        await _paintProductRepository.UpdateAsync(product);
        _logger.LogInformation("Paint product {Id} updated", id);
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetPaintProductById(id);

        await _paintProductRepository.DeleteAsync(product);
        _logger.LogInformation("Paint product {Id} deleted", id);
    }


    public async Task<List<PaintProduct>> SearchAsync(PaintProductSearchQuery query)
    {
        if (query.MinPrice > query.MaxPrice)
        {
            _logger.LogWarning("Invalid price range: min {MinPrice}, max {MaxPrice}", query.MinPrice, query.MaxPrice);
            throw new ArgumentException("Minimum price cannot be greater than maximum price.");
        }

        return await _paintProductRepository.SearchAsync(query);
    }



}
