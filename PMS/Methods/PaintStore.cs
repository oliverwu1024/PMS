namespace PMS.Methods;

public class PaintStore
{
    public List<PaintProduct> Products;
    public PaintStore (List<PaintProduct> products)
    {
        Products = new List<PaintProduct>(products);
    }

    public List<PaintProduct> AllProducts()
    {
        return Products;
    }
}
