using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using System.Text.Json.Serialization;
using PaintStore.API.Services;
using PaintStore.API.Repos;
using Microsoft.AspNetCore.Mvc;
using PaintStore.API.Middleware;
using Serilog;
using Serilog.Events;




var builder = WebApplication.CreateBuilder(args);
/*
builder.Logging.ClearProviders(); // remove the default providers
builder.Logging.AddConsole();     // write to the terminal
builder.Logging.AddDebug();       // write to the debugger's output window
*/
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();


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

builder.Services.AddScoped<IPaintProductService, /*PaintProductMockService*/PaintProductService>();
builder.Services.AddScoped<PaintProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddSingleton<ExceptionHandlingService>();
/*
var controller = new Controller(service);

dbcontext
repo
service
*/

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlingService = context.RequestServices.GetRequiredService<ExceptionHandlingService>();
        await exceptionHandlingService.HandleExceptionAsync(context);

    });
});

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
