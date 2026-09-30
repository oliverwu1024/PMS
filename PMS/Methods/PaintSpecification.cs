using System.Drawing;

namespace PMS.Methods;

public class PaintSpecification
{
    public string Color {get;set;}
    public int SizeInLiters {get;set;}
    public PaintSpecification (string color, int liters)
    {
        Color = color;
        SizeInLiters = liters;
    }

    public void DisplaySpecification ()
    {
        Console.WriteLine($"The paint color is {Color}. The size is {SizeInLiters}");
    }

}
