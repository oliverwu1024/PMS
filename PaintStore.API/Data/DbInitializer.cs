using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;


/*
This file is used to initialize the database
it is called when the application starts
it does 
    1. create the database if it doesn't exist 
    2. insert some initial data into the database
    3. create some initial tables in the database
    4. create some initial records in the database
    5. create some initial relationships between the tables in the database
    6. create some initial records in the database


*/

namespace PaintStore.API.Data;

public class DbInitializer
{
    // this is a method that will be called when the application starts
    //it does 1. create the database if it doesn't exist 
    // 2. insert some initial data into the database
    // 3. create some initial tables in the database
    // 4. create some initial records in the database
    // 5. create some initial relationships between the tables in the database
    public static async Task InitializeAsync(IServiceProvider serviceProvider) {
        //Iservice Provider is a service that can be used to get instances of other services
        //it is used to get instances of the DbContext class
        
        // scope is a context that can be used to get instances of other services
        using var scope = serviceProvider.CreateScope();
        // dbContext is an instance of the DbContext class that is used to interact with the database
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();
        await SeedDataAsync(dbContext);
    }
    
    // this is a method that will be called when the Initialize method is called
    // it will insert some initial data into the database
    private static async Task SeedDataAsync(AppDbContext context) {

        if (! await context.PaintBrands.AnyAsync())
        {
            var dulux = new PaintBrand {Name = "Dulux"};
            var haymes = new PaintBrand {Name = "Haymes"};

            var wash = new PaintSeries {Name = "Wash & Wear", Brand = dulux};
            var exterior = new PaintSeries {Name = "Exterior", Brand = haymes};
            
            context.PaintBrands.AddRange(dulux, haymes);
            context.PaintSeries.AddRange(wash, exterior);
            await context.SaveChangesAsync(); 
        }

        if (! await context.PaintProducts.AnyAsync())
        {
            var wash = await context.PaintSeries.FirstAsync(s => s.Name == "Wash & Wear");
            var exterior = await context.PaintSeries.FirstAsync(s => s.Name == "Exterior");

            context.PaintProducts.AddRange(
                new PaintProduct(0, Brand.Dulux, "白色乳胶漆", PaintType.BaseCoat, new PaintSpecification("White", 15), 25.5m) { Series = wash, Stock = 50 },
                new PaintProduct(0, Brand.Haymes, "蓝色油漆", PaintType.Glossy, new PaintSpecification("Blue", 8), 30.0m) { Series = exterior, Stock = 50 }                
            );

            await context.SaveChangesAsync();  //save changes to database
        }

        if (! await context.Users.AnyAsync())
        {
            context.Users.AddRange(
                new User("Steven", "steven@example.com", "0400000000", 20, 0),
                new User("Rachel", "rachel@example.com", "0400000123", 25, 0)
            );
            await context.SaveChangesAsync();
        }
        

    }



}
