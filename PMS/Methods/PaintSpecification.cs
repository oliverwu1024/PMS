using System.Drawing;

namespace PMS.Methods;

public class PaintSpecification
{
    public PaintSpecification (string color, int liters)
    {   
        Color = color;
        SizeInLiters = liters;
        
    }

    public string Color {get;set;}

    public int SizeInLiters {get; set;}

    



}
