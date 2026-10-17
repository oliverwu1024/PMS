namespace PaintStore.Models.Methods;

public class PaintSeries
{   
    public int Id {get; set;}
    public int BrandId {get; set;}

    public string Name {get;set;}

    public PaintBrand Brand {get; set;}

    public List<PaintProduct> Products {get; set;} = new();
}
