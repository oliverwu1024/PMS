namespace PaintStore.Models.Methods;

public class PaintBrand
{
    public int Id {get; set;}
    public string Name {get; set;}

    public List<PaintSeries> PaintSeries {get; set;} = new List<PaintSeries>();
    


}
