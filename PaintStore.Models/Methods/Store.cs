namespace PaintStore.Models.Methods;

public class Store
{
    public List<PaintProduct> Products;
    public Store (List<PaintProduct> products)
    {
        Products = new List<PaintProduct>(products);
    }

    public List<PaintProduct> AllProducts()
    {
        return Products;
    }
}
