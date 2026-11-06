using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PaintStore.API.Options;
using PaintStore.API.Repos;
using PaintStore.API.Services;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using PaintStore.Models.Enums;



namespace PaintStore.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IPaintProductRepository> _productRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var discount = Options.Create(new DiscountRuleOptions
        {
            DiscountThreshold = 100m,
            DiscountPercentage = 10m
        });

        _service = new OrderService(
            _orderRepo.Object,
            NullLogger<OrderService>.Instance,
            _productRepo.Object,
            _userRepo.Object,
            discount);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsKeyNotFound_WhenOrderMissing()
    {
        // Arrange
        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Order?)null);

        // Act
        var act = () => _service.GetByIdAsync(42);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsOrder_WhenFound()
    {
        var order = new Order(42, 1, new List<PaintProduct>(), new List<int>(), new OrderHistory());

        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(order);

        var result = await _service.GetByIdAsync(42);

        result.Should().BeSameAs(order);
    }
    [Fact]
    public async Task GetPagedAsync_Throws_WhenPageNumberIsZero()
    {
        var act = () => _service.GetPagedAsync(0, 10);

        await act.Should().ThrowAsync<ArgumentException>();
    }
    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public async Task GetPagedAsync_Throws_WhenPagingInvalid(int pageNumber, int pageSize)
    {
        var act = () => _service.GetPagedAsync(pageNumber, pageSize);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsOrders_WhenPagingValid()
    {
        var orders = new List<Order>
        {
            new Order(1, 1, new List<PaintProduct>(), new List<int>(), new OrderHistory())
        };

        _orderRepo.Setup(r => r.GetPagedAsync(1, 1)).ReturnsAsync(orders);

        var result = await _service.GetPagedAsync(1, 1);

        result.Should().BeSameAs(orders);
    }

    [Fact]
    public async Task GetByPriceRangeAsync_Throws_WhenMinGreaterThanMax()
    {
        var act = () => _service.GetByPriceRangeAsync(200m, 100m);

        await act.Should().ThrowAsync<ArgumentException>();
    }
    [Fact]
    public async Task CreateOrderAsync_Throws_WhenUserNotFound()
    {
        // Script: "if asked for user 99, answer null (doesn't exist)"
        _userRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // build the request: user 99 wants 1 of product 1
        var request = new CreateOrderRequest
        {
            UserId = 99,
            ProductIds = new List<int> { 1 },
            Quantities = new List<int> { 1 }
        };

        var act = () => _service.CreateOrderAsync(request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);

    }

    // ---------- helpers ----------
    // Most CreateOrder tests need "a user that exists" and "a product with some stock",
    // so these save typing the same long lines in every test.

    private void SetupExistingUser(int userId)
    {
        _userRepo.Setup(r => r.GetByIdAsync(userId))
                 .ReturnsAsync(new User("bob", "bob@test.com", "0400000000", 30, userId));
    }

    private PaintProduct SetupProduct(int id, decimal price, int stock)
    {
        var product = new PaintProduct(id, Brand.Dulux, "White", PaintType.Glossy,
            new PaintSpecification("White", 4), price);
        product.Stock = stock;
        _productRepo.Setup(r => r.GetPaintProductById(id)).ReturnsAsync(product);
        return product;
    }

    // AddAsync returns the saved order. "(Order o) => o" means: give back the same order you were given.
    // Without this, the fake would return null and the service would crash on order.OrderId.
    private void SetupAddReturnsSameOrder()
    {
        _orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
    }

    // ---------- CreateOrderAsync ----------

    [Fact]
    public async Task CreateOrderAsync_Throws_WhenNotEnoughStock()
    {
        SetupExistingUser(1);
        var product = SetupProduct(10, 20m, stock: 2);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 5 }
        };

        var act = () => _service.CreateOrderAsync(request);

        await act.Should().ThrowAsync<ArgumentException>();
        product.Stock.Should().Be(2);   // stock not touched
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_SubtractsStockAndSaves_WhenValid()
    {
        SetupExistingUser(1);
        var product = SetupProduct(10, 20m, stock: 5);
        SetupAddReturnsSameOrder();

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 2 }
        };

        var result = await _service.CreateOrderAsync(request);

        product.Stock.Should().Be(3);   // 5 - 2
        result.UserId.Should().Be(1);
        // one paint = 20 * 0.95 (5% product discount) * 1.10 (10% tax) = 20.90, times 2 = 41.80
        // 41.80 is under the 100 threshold, so no order discount
        result.TotalPrice.Should().Be(41.80m);
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    // boundary: ordering exactly all the stock is allowed (the code uses <, not <=)
    [Fact]
    public async Task CreateOrderAsync_Succeeds_WhenQuantityEqualsStock()
    {
        SetupExistingUser(1);
        var product = SetupProduct(10, 20m, stock: 2);
        SetupAddReturnsSameOrder();

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 2 }
        };

        await _service.CreateOrderAsync(request);

        product.Stock.Should().Be(0);
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_Throws_WhenProductNotFound()
    {
        SetupExistingUser(1);
        _productRepo.Setup(r => r.GetPaintProductById(10)).ReturnsAsync((PaintProduct?)null);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 1 }
        };

        var act = () => _service.CreateOrderAsync(request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    // Each row is a bad request that LoadProductsAsync should reject.
    // InlineData can't hold a List, so the rows use arrays and the test turns them into lists.
    [Theory]
    [InlineData(new int[] { }, new int[] { })]           // empty
    [InlineData(new[] { 10, 11 }, new[] { 1 })]          // lengths don't match
    [InlineData(new[] { 10 }, new[] { 0 })]              // quantity 0
    [InlineData(new[] { 10 }, new[] { -1 })]             // negative quantity
    [InlineData(new[] { 10, 10 }, new[] { 1, 1 })]       // same product twice
    public async Task CreateOrderAsync_Throws_WhenRequestInvalid(int[] productIds, int[] quantities)
    {
        SetupExistingUser(1);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = productIds.ToList(),
            Quantities = quantities.ToList()
        };

        var act = () => _service.CreateOrderAsync(request);

        await act.Should().ThrowAsync<ArgumentException>();
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    // ---------- discount (threshold 100, 10% off, set in the constructor) ----------

    [Fact]
    public async Task CreateOrderAsync_AppliesDiscount_WhenTotalAboveThreshold()
    {
        SetupExistingUser(1);
        SetupProduct(10, 100m, stock: 5);
        SetupAddReturnsSameOrder();

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 1 }
        };

        var result = await _service.CreateOrderAsync(request);

        // one paint = 100 * 0.95 * 1.10 = 104.50 -> above 100, so 10% off -> 94.05
        result.TotalPrice.Should().Be(94.05m);
    }

    // boundary: exactly 100 is NOT discounted, because the code uses > not >=
    [Fact]
    public async Task CreateOrderAsync_NoDiscount_WhenTotalExactlyAtThreshold()
    {
        SetupExistingUser(1);
        SetupProduct(10, 23.92m, stock: 5);
        SetupAddReturnsSameOrder();

        var request = new CreateOrderRequest
        {
            UserId = 1,
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 4 }
        };

        var result = await _service.CreateOrderAsync(request);

        // one paint = 23.92 * 0.95 * 1.10 = 24.9964 -> rounded to 25.00, times 4 = exactly 100.00
        result.TotalPrice.Should().Be(100.00m);
    }

    // ---------- other read methods ----------

    [Fact]
    public async Task GetByPriceRangeAsync_Allows_MinEqualToMax()
    {
        var orders = new List<Order>();
        _orderRepo.Setup(r => r.GetByPriceRangeAsync(50m, 50m)).ReturnsAsync(orders);

        var result = await _service.GetByPriceRangeAsync(50m, 50m);

        result.Should().BeSameAs(orders);
    }

    [Fact]
    public async Task GetByUserIdAsync_Throws_WhenNoOrders()
    {
        _orderRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<Order>());

        var act = () => _service.GetByUserIdAsync(1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByPaintIdAsync_Throws_WhenNoOrders()
    {
        _orderRepo.Setup(r => r.GetByPaintIdAsync(10)).ReturnsAsync(new List<Order>());

        var act = () => _service.GetByPaintIdAsync(10);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ---------- UpdateOrderAsync ----------

    [Fact]
    public async Task UpdateOrderAsync_Throws_WhenOrderMissing()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Order?)null);

        var request = new UpdateOrderRequest
        {
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 1 }
        };

        var act = () => _service.UpdateOrderAsync(42, request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _orderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOrderAsync_ReplacesProductsAndSaves_WhenValid()
    {
        var order = new Order(42, 1, new List<PaintProduct>(), new List<int>(), new OrderHistory());
        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(order);
        var product = SetupProduct(10, 20m, stock: 5);

        var request = new UpdateOrderRequest
        {
            ProductIds = new List<int> { 10 },
            Quantities = new List<int> { 2 }
        };

        var result = await _service.UpdateOrderAsync(42, request);

        result.Products.Should().ContainSingle().Which.Should().BeSameAs(product);
        result.Quantities.Should().Equal(2);
        result.TotalPrice.Should().Be(41.80m);
        _orderRepo.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    // ---------- DeleteOrderAsync ----------

    [Fact]
    public async Task DeleteOrderAsync_Throws_WhenOrderMissing()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Order?)null);

        var act = () => _service.DeleteOrderAsync(42);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _orderRepo.Verify(r => r.DeleteAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task DeleteOrderAsync_Deletes_WhenOrderExists()
    {
        var order = new Order(42, 1, new List<PaintProduct>(), new List<int>(), new OrderHistory());
        _orderRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(order);

        await _service.DeleteOrderAsync(42);

        // checks it deleted THIS order, not just any order
        _orderRepo.Verify(r => r.DeleteAsync(order), Times.Once);
    }
}
