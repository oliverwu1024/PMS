namespace PMS.Methods;
using PMS.Enums;
using PMS.Interfaces;


public class PaintProduct: IBuyable
{
    public readonly int TaxRate;

    const int DefaultDiscount = 5;

    public string Name {get;set;}

    public PaintType Type {get;set;}

    public PaintSpecification Specification {get;set;}

    public decimal Price {get;set;}

    public PaintProduct (string name, PaintType type, PaintSpecification specification, decimal price, int taxRate = 10)
    {
        Name = name;
        Type = type;
        Specification = specification;
        Price = price;
        TaxRate = taxRate;
    }

    // I am not sure if this part is correct but the requirement said:
    // 计算折扣后含税价格, i can only think of this method to pass in discount
    public decimal GetFinalPrice()
    {
        int discount = GetMaxDiscount();
        return Math.Round(Price * (1m - (decimal) discount / 100m) * (1m + (decimal)TaxRate/100m), 2);
    }

    public string DisplayInfo ()
    {
        return $"Product Name: {Name}\nProduct Type: {Type}\nPaint Specification: {Specification.DisplaySpecification()}\nPrice: {Price}\nTax rate: {TaxRate}";
    }

    public int GetMaxDiscount (int rate = DefaultDiscount, bool isOverridable = false) 
    {
        if (isOverridable) {return rate;}
        else {return DefaultDiscount;}
        
    }



}
