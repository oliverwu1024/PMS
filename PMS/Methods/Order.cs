using System.Security.Principal;
using System.Text;
using PMS.Enums;

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

    public bool CheckIfHasNullName()
    {
        return Products.Any((PaintProduct paintProduct) =>
        {
            if(paintProduct.Name == null) {return true;}
            else {return false;}
        }
        );
    }

    public List<PaintProduct> GetExpensivePaintProducts()
    {   
        /*
        IEnumerable<PaintProduct> products = Products.Where(
            (PaintProduct paintProduct) =>
            {
                if (paintProduct.Price > 10) {return true;}
                return false;
            }
            

        );
        return products.ToList();
        */
        return Products.Where(p => p.Price >10).ToList();


    }
    public List<PaintProduct> GetMostExpensivePaintProducts()
    {   
        /*
        IEnumerable<PaintProduct> products = Products.Where(
            (PaintProduct paintProduct) =>
            {
                if (paintProduct.Price > 10) {return true;}
                return false;
            }
            

        );
        return products.ToList();
        */
        decimal maxPrice = Products.Max(p => p.Price);

        return Products.Where(p => p.Price == maxPrice).ToList();


    }

    // I am not sure about this as in the previous homework there is no product ID
    // I added an ID in product
    public void RemoveProduct (int productId)
    {
        Products.RemoveAll(p => p.Id == productId);
    }

    // used original price not discounted final price
    public List <PaintProduct> FindProductBetweenPrice (decimal x, decimal y)
    {
        return Products.Where(p => p.Price < y && p.Price >x).ToList();
    }

    public void TypeTotalPrice ()
    {
        List <PaintType> types = new List<PaintType>();
        List <decimal> prices= new List<decimal>();

        foreach (PaintProduct product in Products)
        {
            if (types.Contains(product.Type))
            {
                int idx = types.IndexOf(product.Type);
                prices[idx] += product.Price;
            }
            else
            {
                types.Add(product.Type);
                prices.Add(product.Price);
            }
        }
        
        for (int i = 0; i < types.Count; i++)
        {
            Console.WriteLine($"The total price of paint type {types[i]} is {prices[i]}");
        }
        
    }




}
