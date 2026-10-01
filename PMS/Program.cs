using PMS.Methods;
using PMS.Enums;
using PMS.Interfaces;
using System.Net.Http.Headers;

//first product
PaintType type1 = PaintType.BaseCoat;
PaintSpecification spec1 = new PaintSpecification("Red", 5);
PaintProduct product1 = new PaintProduct("Robo", type1, spec1, 100);
//second product 
PaintType type2 = PaintType.Glossy;
PaintSpecification spec2 = new PaintSpecification("blue", 15);
PaintProduct product2 = new PaintProduct("Solo", type2, spec2, 50);


//third product
PaintType type3 = PaintType.Matte;
PaintSpecification spec3 = new PaintSpecification("black", 8);
PaintProduct product3 = new PaintProduct("Polar", type3, spec3, 75);

//display all products
PaintProduct[] allProducts = [product1, product2, product3]; 
int count = 1;
foreach (PaintProduct product in allProducts)
{
    Console.WriteLine($"Product #{count}");
    Console.WriteLine(product.DisplayInfo());
    count+=1;
}


//display order detail
Order order1 = new Order(product1, 5);
Console.WriteLine(order1.DisplayOrder());

