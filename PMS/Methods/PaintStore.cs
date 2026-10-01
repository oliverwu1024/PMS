namespace PMS.Methods;

public class PaintStore
{
    public PaintProduct[] Products;
    public PaintStore (PaintProduct[] products)
    {
        Products = products;
    }

    public PaintProduct[] AllProducts()
    {
        return Products;
    }
}
