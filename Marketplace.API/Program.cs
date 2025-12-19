using Marketplace.API.Middlewares;
using Marketplace.Business.Interfaces;
using Marketplace.Business.Services;
using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin(); 
        });
});

builder.Services.AddControllers();

// Database
builder.Services.AddScoped<DatabaseConnection>();

// Products
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Users
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();


var app = builder.Build();

// Middlewares
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

// Endpoints
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");


app.Run();
