using Application;
using Presistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Mapping;
using Application.Services.Category;
using Application.Services.Product;
using Application.Services.Review;
using Application.Services.Subcategory;
using Application.Services.UserRepository;
using Application.Services.ShoppingCart;
using Application.Services.Wishlist;
using Application.Services.Order;
using Application.Repositories.OrderRepo;
using Application.Services.Payment;
using Application.Services.User;
using Application.Services.Email;
using Nest;
using Application.Services.Search;
using Application.Services.ImageStorage;
using Application.Services.Discount;
using Application.Services.TokenService;
using Configurations;

using Elasticsearch.Net;
using System;
using Application.Services.ImageStorage;
using Life_Ecommerce.Hubs;
using Application.Repositories.ChatRepo;
using Application.Services.Chat;

var builder = WebApplication.CreateBuilder(args);



// Add Serilog as the logging provider
builder.Services.AddLogging(builder.Configuration);


// Add services to the container.
builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSignalR();
// elastic Search
var uri = new Uri(builder.Configuration["ElasticSearch:Uri"]);
var password = builder.Configuration["ElasticSearch:Password"];
var settings = new ConnectionSettings(uri)
    .BasicAuthentication("elastic", password)
    .DefaultIndex("products_v2");

var elasticClient = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(elasticClient);
builder.Services.AddSession(options =>
{
    // Set a reasonable timeout for session
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.Name = "LifeEcommerce.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});

var mapperConfiguration = new MapperConfiguration(
                        mc => mc.AddProfile(new AutoMapperConfiguration()));

IMapper mapper = mapperConfiguration.CreateMapper();

builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<APIDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, Application.Services.Product.ProductService>();
builder.Services.AddScoped<IReviewService, Application.Services.Review.ReviewService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IDiscountService, Application.Services.Discount.DiscountService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("OUR_SECRET_KEY_FROM_LIFE_FROM_GJIRAFA")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Authentication Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JsonWebToken",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
           .WithOrigins("http://localhost:3000");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthMiddleware>();

app.UseSession();
app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
