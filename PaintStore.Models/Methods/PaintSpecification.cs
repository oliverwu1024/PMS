using System.Drawing;

namespace PaintStore.Models.Methods;

public class PaintSpecification
{
    public string Color {get;set;}
    public int SizeInLiters {get;set;}

    //private PaintSpecification(){}
    public PaintSpecification (string color, int sizeInLiters)
    {
        Color = color;
        SizeInLiters = sizeInLiters;
    }

    public string DisplaySpecification ()
    {
        return $"The paint color is {Color}. The size is {SizeInLiters}";
    }

}
