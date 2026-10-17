using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(options => {options.JsonSerializerOptions.ReferenceHandler
                                = ReferenceHandler.IgnoreCycles;});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);



var app = builder.Build();

// Automatically apply migrations on startup
/*
old one, replaced with the following DbInitializer.Initialize(app.Services);
why is DbInitializer.Initialize(app.Services); better?
1. It's more explicit and easier to understand what's happening.
2. It doesn't rely on the DbContext being available in the scope, which can lead to issues if the DbContext is not properly initialized.
More easier explanation:
The old code uses the `AddDbContext` method to register the `AppDbContext` with the dependency injection container. 
This means that the `AppDbContext` is available in the scope of the application, which is necessary for the `DbInitializer.Initialize` method to work correctly.


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
*/
DbInitializer.Initialize(app.Services);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
