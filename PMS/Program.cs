using PMS.Methods;
using PMS.Enums;
using PMS.Interfaces;

PaintType type1 = PaintType.BaseCoat;
Console.WriteLine(type1);

PaintSpecification spec1 = new PaintSpecification("Red", 5);
Console.WriteLine(spec1.DisplaySpecification());

PaintProduct product1 = new PaintProduct("Robo", type1, spec1, 100);

Console.WriteLine(product1.GetFinalPrice());


