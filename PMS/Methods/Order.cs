using System.Security.Principal;
using System.Text;

namespace PMS.Methods;

public class Order
{
    public readonly DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

    public List <PaintProduct> Products;
    public List<int> Quantities;
    public decimal TotalPrice;

    public Order (List <PaintProduct> paintProducts, List<int> quantities)
    {
        Products = new List <PaintProduct> (paintProducts);
        Quantities = new List<int> (quantities);
        TotalPrice = GetTotalOrderPrice();
    }

    public void DisplayOrder()
    {
        for (int i = 0; i<Products.Count;i++)
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
        for (int i = 0; i<Products.Count;i++)
        {
            total += Products[i].GetFinalPrice() * (decimal) Quantities[i];
        }
        return total;


    }





}
