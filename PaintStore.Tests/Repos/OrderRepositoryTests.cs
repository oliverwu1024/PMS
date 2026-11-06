using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using PaintStore.API.Repos;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;

namespace PaintStore.Tests.Repos;

// No Moq here: the repository IS the thing being tested, so it gets a real AppDbContext
// pointed at an in-memory database instead of SQL Server.
public class OrderRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly OrderRepository _orderRepository;

    // Runs before EACH test: a brand-new, empty database with a random name,
    // so tests never see each other's data.
    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _orderRepository = new OrderRepository(_context);
    }

    // Runs after EACH test: we created the context with "new", so we clean it up.
    public void Dispose()
    {
        _context.Dispose();
    }

    // ---------- helpers ----------

    private static Order MakeOrder(int orderId, int userId = 1, List<PaintProduct>? products = null)
    {
        products ??= new List<PaintProduct>();
        var quantities = products.Select(_ => 1).ToList();
        return new Order(orderId, userId, products, quantities, new OrderHistory());
    }

    private static PaintProduct MakeProduct(int id, string name)
    {
        return new PaintProduct(id, Brand.Dulux, name, PaintType.Glossy,
            new PaintSpecification(name, 4), 20m);
    }

    // Put orders into the in-memory database (this is the Arrange step for most tests)
    private async Task SeedAsync(params Order[] orders)
    {
        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();
    }

    // ---------- GetByIdAsync ----------

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        await SeedAsync(MakeOrder(1));

        var result = await _orderRepository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.OrderId.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderHasProducts_ShouldIncludeProducts()
    {
        var order = MakeOrder(1, products: new List<PaintProduct>
        {
            MakeProduct(1, "Red Paint"),
            MakeProduct(2, "Blue Paint")
        });
        await SeedAsync(order);

        var result = await _orderRepository.GetByIdAsync(1);

        result!.Products.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderMissing_ShouldReturnNull()
    {
        await SeedAsync(MakeOrder(1));

        var result = await _orderRepository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GetByIdAsync_WhenOrderIdIsZeroOrNegative_ShouldReturnNull(int orderId)
    {
        var result = await _orderRepository.GetByIdAsync(orderId);

        result.Should().BeNull();
    }

    // ---------- GetPagedAsync ----------

    [Fact]
    public async Task GetPagedAsync_WhenOrdersExist_ShouldReturnFirstPageOrderedById()
    {
        // added out of order on purpose, to prove the repository sorts them
        await SeedAsync(MakeOrder(3), MakeOrder(1), MakeOrder(2), MakeOrder(5), MakeOrder(4));

        var result = await _orderRepository.GetPagedAsync(1, 3);

        result.Select(o => o.OrderId).Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task GetPagedAsync_WhenSecondPage_ShouldSkipFirstPage()
    {
        await SeedAsync(MakeOrder(3), MakeOrder(1), MakeOrder(2), MakeOrder(5), MakeOrder(4));

        var result = await _orderRepository.GetPagedAsync(2, 3);

        result.Select(o => o.OrderId).Should().Equal(4, 5);   // only 2 left on page 2
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageBeyondData_ShouldReturnEmpty()
    {
        await SeedAsync(MakeOrder(1), MakeOrder(2));

        var result = await _orderRepository.GetPagedAsync(5, 10);

        result.Should().BeEmpty();
    }

    // ---------- GetByPriceRangeAsync ----------

    [Fact]
    public async Task GetByPriceRangeAsync_ShouldIncludeBothEnds()
    {
        var cheap = MakeOrder(1);  cheap.TotalPrice = 50m;
        var low = MakeOrder(2);    low.TotalPrice = 100m;   // exactly min
        var high = MakeOrder(3);   high.TotalPrice = 200m;  // exactly max
        var pricey = MakeOrder(4); pricey.TotalPrice = 300m;
        await SeedAsync(cheap, low, high, pricey);

        var result = await _orderRepository.GetByPriceRangeAsync(100m, 200m);

        result.Select(o => o.OrderId).Should().BeEquivalentTo(new[] { 2, 3 });
    }

    // ---------- GetByUserIdAsync / GetByPaintIdAsync ----------

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnOnlyThatUsersOrders()
    {
        await SeedAsync(MakeOrder(1, userId: 1), MakeOrder(2, userId: 2), MakeOrder(3, userId: 1));

        var result = await _orderRepository.GetByUserIdAsync(1);

        result.Select(o => o.OrderId).Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Fact]
    public async Task GetByPaintIdAsync_ShouldReturnOnlyOrdersContainingThatPaint()
    {
        var red = MakeProduct(10, "Red Paint");
        var blue = MakeProduct(11, "Blue Paint");
        await SeedAsync(
            MakeOrder(1, products: new List<PaintProduct> { red }),
            MakeOrder(2, products: new List<PaintProduct> { blue }),
            MakeOrder(3, products: new List<PaintProduct> { red, blue }));

        var result = await _orderRepository.GetByPaintIdAsync(10);

        result.Select(o => o.OrderId).Should().BeEquivalentTo(new[] { 1, 3 });
    }

    // ---------- date queries ----------

    [Fact]
    public async Task GetByDateAsync_ShouldReturnOnlyOrdersFromThatDay()
    {
        var day = new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.Zero);
        var sameDay = MakeOrder(1);  sameDay.CreatedAt = day;
        var nextDay = MakeOrder(2);  nextDay.CreatedAt = day.AddDays(1);
        await SeedAsync(sameDay, nextDay);

        var result = await _orderRepository.GetByDateAsync(new DateTime(2026, 3, 15));

        result.Select(o => o.OrderId).Should().Equal(1);
    }

    [Fact]
    public async Task GetCreatedSinceAsync_ShouldReturnOrdersOnOrAfterDate()
    {
        var since = new DateTimeOffset(2026, 3, 15, 0, 0, 0, TimeSpan.Zero);
        var before = MakeOrder(1);  before.CreatedAt = since.AddDays(-1);
        var exactly = MakeOrder(2); exactly.CreatedAt = since;            // boundary: >= so included
        var after = MakeOrder(3);   after.CreatedAt = since.AddDays(1);
        await SeedAsync(before, exactly, after);

        var result = await _orderRepository.GetCreatedSinceAsync(since);

        result.Select(o => o.OrderId).Should().BeEquivalentTo(new[] { 2, 3 });
    }

    // ---------- Add / Update / Delete ----------
    // For writes, check the DATABASE afterwards, not just what the method returned.

    [Fact]
    public async Task AddAsync_ShouldSaveOrderToDatabase()
    {
        var order = MakeOrder(1, userId: 7);

        await _orderRepository.AddAsync(order);

        var saved = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == 1);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(7);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSaveChangesToDatabase()
    {
        var order = MakeOrder(1);
        await SeedAsync(order);

        order.TotalPrice = 123.45m;
        await _orderRepository.UpdateAsync(order);

        var saved = await _context.Orders.FirstAsync(o => o.OrderId == 1);
        saved.TotalPrice.Should().Be(123.45m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveOrderFromDatabase()
    {
        var order = MakeOrder(1);
        await SeedAsync(order, MakeOrder(2));

        await _orderRepository.DeleteAsync(order);

        _context.Orders.Select(o => o.OrderId).Should().Equal(2);
    }
}
