using Marketplace.API.Middlewares;
using Marketplace.Business.Interfaces;
using Marketplace.Business.Services;
using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

//
// 🔹 Services
//

// Controllers
builder.Services.AddControllers();

// CORS (para Vercel y pruebas)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//
// 🔹 Middlewares
//

app.UseCors("AllowFrontend");

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

// Swagger (habilitado también en producción para portafolio)
app.UseSwagger();
app.UseSwaggerUI();

//
// 🔹 Endpoints
//

app.MapControllers();

// 🔥 Render usa el puerto por variable de entorno
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();
