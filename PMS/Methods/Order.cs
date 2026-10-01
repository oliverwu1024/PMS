using System.Security.Principal;
using System.Text;

namespace PMS.Methods;

public class Order
{
    public readonly DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

    public PaintProduct[] Products;
    public int[] Quantities;
    public decimal TotalPrice;

    public Order (PaintProduct[] paintProducts, int[] quantities)
    {
        Products = paintProducts;
        Quantities = quantities;
        TotalPrice = GetTotalOrderPrice();
    }

    public void DisplayOrder()
    {
        for (int i = 0; i<Products.Length;i++)
        {
            Console.WriteLine($"Product is:\n\n{Products[i].DisplayInfo()}\n\nQuantity {Quantities[i]}\nProduct total price {Products[i].GetFinalPrice() * (decimal) Quantities[i]}");

        }
        Console.WriteLine($"Total Order Price is {TotalPrice}");
    }

    /*
    public decimal GetTotalPrice ()
    {
        return Product.GetFinalPrice() * (decimal) Quantity; 
    }
    */
    

    public decimal GetTotalOrderPrice()
    {
        decimal total = 0;
        for (int i = 0; i<Products.Length;i++)
        {
            total += Products[i].GetFinalPrice() * (decimal) Quantities[i];
        }
        return total;


    }





}
