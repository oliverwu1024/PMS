using FluentAssertions;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;

namespace PaintStore.Tests.Models;

// The simplest kind of test: Order is a plain class with no dependencies,
// so there is nothing to fake. Just create it and check the result.
public class OrderModelTests
{
    // Prices used below (PaintProduct.GetFinalPrice = price * 0.95 product discount * 1.10 tax, rounded):
    //   price 20  -> 20.90 each
    //   price 100 -> 104.50 each
    private static PaintProduct MakeProduct(int id, decimal price, string name = "Paint")
    {
        return new PaintProduct(id, Brand.Dulux, name, PaintType.Glossy,
            new PaintSpecification("White", 4), price);
    }

    private static Order MakeOrder(List<PaintProduct> products, List<int> quantities)
    {
        return new Order(1, 1, products, quantities, new OrderHistory());
    }

    // ---------- constructor / TotalPrice ----------

    [Fact]
    public void Constructor_WhenNoProducts_ShouldHaveZeroTotal()
    {
        var order = MakeOrder(new List<PaintProduct>(), new List<int>());

        order.TotalPrice.Should().Be(0m);
    }

    [Fact]
    public void Constructor_WhenSingleProduct_ShouldSetTotalFromFinalPriceTimesQuantity()
    {
        var order = MakeOrder(new List<PaintProduct> { MakeProduct(1, 20m) }, new List<int> { 2 });

        order.TotalPrice.Should().Be(41.80m);   // 20.90 * 2
    }

    [Fact]
    public void GetTotalOrderPrice_WhenMultipleProducts_ShouldSumEachLine()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 20m), MakeProduct(2, 100m) },
            new List<int> { 2, 1 });

        order.GetTotalOrderPrice().Should().Be(146.30m);   // 20.90*2 + 104.50*1
    }

    [Fact]
    public void Constructor_ShouldAddOrderToHistory()
    {
        var history = new OrderHistory();

        var order = new Order(1, 1, new List<PaintProduct>(), new List<int>(), history);

        history.Orders.Should().ContainSingle().Which.Should().BeSameAs(order);
    }

    [Fact]
    public void Constructor_ShouldCopyLists_NotKeepCallersLists()
    {
        var products = new List<PaintProduct> { MakeProduct(1, 20m) };
        var quantities = new List<int> { 1 };
        var order = MakeOrder(products, quantities);

        // changing the caller's lists afterwards must not change the order
        products.Add(MakeProduct(2, 100m));
        quantities.Add(5);

        order.Products.Should().HaveCount(1);
        order.Quantities.Should().Equal(1);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtToNow()
    {
        var before = DateTimeOffset.UtcNow;

        var order = MakeOrder(new List<PaintProduct>(), new List<int>());

        var after = DateTimeOffset.UtcNow;
        order.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    // ---------- adding / removing products ----------

    [Fact]
    public void AddProduct_ShouldAddToProducts()
    {
        var order = MakeOrder(new List<PaintProduct>(), new List<int>());
        var product = MakeProduct(1, 20m);

        order.AddProduct(product);

        order.Products.Should().ContainSingle().Which.Should().BeSameAs(product);
        order.Quantities.Should().Equal(1);   // default quantity
    }

    [Fact]
    public void AddProduct_WithQuantity_ShouldKeepTotalWorking()
    {
        // this used to crash: Products had 2 items but Quantities only 1
        var order = MakeOrder(new List<PaintProduct> { MakeProduct(1, 20m) }, new List<int> { 1 });

        order.AddProduct(MakeProduct(2, 100m), 3);

        order.Quantities.Should().Equal(1, 3);
        order.GetTotalOrderPrice().Should().Be(334.40m);   // 20.90*1 + 104.50*3
    }

    [Fact]
    public void AddProducts_ShouldAddProductsAndQuantities()
    {
        var order = MakeOrder(new List<PaintProduct>(), new List<int>());

        order.AddProducts(new List<PaintProduct> { MakeProduct(1, 20m), MakeProduct(2, 100m) }, new List<int> { 2, 1 });

        order.Products.Select(p => p.Id).Should().Equal(1, 2);
        order.Quantities.Should().Equal(2, 1);
    }

    [Fact]
    public void AddProducts_WhenCountsDontMatch_ShouldThrowAndAddNothing()
    {
        var order = MakeOrder(new List<PaintProduct>(), new List<int>());

        var act = () => order.AddProducts(new List<PaintProduct> { MakeProduct(1, 20m), MakeProduct(2, 20m) }, new List<int> { 1 });

        act.Should().Throw<ArgumentException>();
        order.Products.Should().BeEmpty();
        order.Quantities.Should().BeEmpty();
    }

    [Fact]
    public void RemoveProductById_ShouldRemoveOnlyThatProduct()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 20m), MakeProduct(2, 20m) },
            new List<int> { 1, 1 });

        order.RemoveProduct(1);

        order.Products.Select(p => p.Id).Should().Equal(2);
    }

    [Fact]
    public void RemoveProductById_ShouldRemoveMatchingQuantity()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 20m), MakeProduct(2, 20m), MakeProduct(1, 20m) },
            new List<int> { 5, 7, 9 });

        order.RemoveProduct(1);

        order.Quantities.Should().Equal(7);   // product 2's quantity is the one left
    }

    [Fact]
    public void RemoveProduct_ByObject_ShouldRemoveMatchingQuantity()
    {
        var keep = MakeProduct(1, 20m);
        var remove = MakeProduct(2, 100m);
        var order = MakeOrder(new List<PaintProduct> { keep, remove }, new List<int> { 2, 4 });

        order.RemoveProduct(remove);

        order.Products.Should().ContainSingle().Which.Should().BeSameAs(keep);
        order.Quantities.Should().Equal(2);
    }

    [Fact]
    public void RemoveProduct_ByObject_WhenNotInOrder_ShouldChangeNothing()
    {
        var order = MakeOrder(new List<PaintProduct> { MakeProduct(1, 20m) }, new List<int> { 2 });

        order.RemoveProduct(MakeProduct(99, 20m));

        order.Products.Should().HaveCount(1);
        order.Quantities.Should().Equal(2);
    }

    // ---------- price filters (these use the base Price, not the final price) ----------

    [Fact]
    public void GetExpensivePaintProducts_ShouldReturnOnlyPriceAboveTen()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 9.99m), MakeProduct(2, 10m), MakeProduct(3, 10.01m) },
            new List<int> { 1, 1, 1 });

        var result = order.GetExpensivePaintProducts();

        result.Select(p => p.Id).Should().Equal(3);   // boundary: exactly 10 is NOT expensive (> 10)
    }

    [Fact]
    public void GetMostExpensivePaintProducts_ShouldReturnAllTiedForHighest()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 50m), MakeProduct(2, 80m), MakeProduct(3, 80m) },
            new List<int> { 1, 1, 1 });

        var result = order.GetMostExpensivePaintProducts();

        result.Select(p => p.Id).Should().BeEquivalentTo(new[] { 2, 3 });
    }

    [Fact]
    public void FindProductBetweenPrice_ShouldExcludeBothEnds()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 10m), MakeProduct(2, 15m), MakeProduct(3, 20m) },
            new List<int> { 1, 1, 1 });

        var result = order.FindProductBetweenPrice(10m, 20m);

        result.Select(p => p.Id).Should().Equal(2);
    }

    // ---------- CheckIfHasNullName ----------

    [Fact]
    public void CheckIfHasNullName_WhenAllNamed_ShouldReturnFalse()
    {
        var order = MakeOrder(new List<PaintProduct> { MakeProduct(1, 20m, "Red") }, new List<int> { 1 });

        order.CheckIfHasNullName().Should().BeFalse();
    }

    [Fact]
    public void CheckIfHasNullName_WhenOneNameIsNull_ShouldReturnTrue()
    {
        var order = MakeOrder(
            new List<PaintProduct> { MakeProduct(1, 20m, "Red"), MakeProduct(2, 20m, null!) },
            new List<int> { 1, 1 });

        order.CheckIfHasNullName().Should().BeTrue();
    }
}
