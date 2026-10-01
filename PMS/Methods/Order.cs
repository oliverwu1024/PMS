namespace PMS.Methods;

public class Order
{
    public readonly DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

    public PaintProduct Product;
    public int Quantity;
    public decimal TotalPrice;

    public Order (PaintProduct paintProduct, int quantity)
    {
        Product = paintProduct;
        Quantity = quantity;
        TotalPrice = GetTotalPrice();
    }

    public void DisplayOrder()
    {
        Console.WriteLine($"Product is:\n\n{Product.DisplayInfo()}\n\nQuantity {Quantity}\nTotal {TotalPrice}");
    }

    public decimal GetTotalPrice ()
    {
        return Product.GetFinalPrice() * (decimal) Quantity; 
    }





}
