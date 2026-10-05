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

    public void AddProduct(PaintProduct newPaintProduct)
    {
        Products.Add(newPaintProduct);
    }

    public void AddProducts(List<PaintProduct> newPaintProducts)
    {
        Products.AddRange(newPaintProducts);        
    }

    public void RemoveProduct(PaintProduct removeProduct)
    {
        Products.Remove(removeProduct);
    }

    public void ContainsProduct (PaintProduct checkContains)
    {
        if (Products.Contains(checkContains))
        {
            Console.WriteLine($"The order contains the product {checkContains}");
        }

        else
        {
            Console.WriteLine($"The order does contain the product {checkContains}");    
        }
        
    }

    public void FindProductByName(PaintProduct findProduct)
    {
        if (Products.FindAll(p => p.Name==findProduct.Name).Count == 0)
        {
            Console.WriteLine("Cannot find matching product names");
        }
        else if (Products.FindAll(p => p.Name==findProduct.Name).Count == 1)
        {
            Console.WriteLine($"Found a matching product name {findProduct.Name}");
        }
        else
        {
            Console.WriteLine($"Found {Products.FindAll(p => p.Name==findProduct.Name).Count} matching product names {findProduct.Name}");
        }
    }







}
