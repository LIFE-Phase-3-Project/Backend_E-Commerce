using Application;
using Presistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Services.Category;
using Application.Services.Product;
using Application.Services.Review;
using Application.Services.Subcategory;
using Application.Services.UserRepository;
using Life_Ecommerce.TokenService;
using Application.Services.ShoppingCart;
using Application.Services.Wishlist;
using Application.Services.Order;
using Application.Repositories.OrderRepo;
using Application.Services.Payment;
using Application.Services.User;
using Application.Services.Email;
using Nest;
using Domain.Helpers;
using Application.Services.Search;
using System.Security.Claims;
using Application.Services.ImageStorage;
using Domain.DTOs.User;
using Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();

// Elastic Search
var uri = new Uri(builder.Configuration["ElasticSearch:Uri"]);
var password = builder.Configuration["ElasticSearch:Password"];
var settings = new ConnectionSettings(uri)
    .BasicAuthentication("elastic", password)
    .DefaultIndex("products_v2");

var elasticClient = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(elasticClient);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.Name = "LifeEcommerce.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var mapperConfiguration = new MapperConfiguration(
                        mc => mc.AddProfile(new AutoMapperConfiguration()));

IMapper mapper = mapperConfiguration.CreateMapper();

builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
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
builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

// GjirafaScheme authentication setup
builder.Services.AddAuthentication("GjirafaScheme")
    .AddJwtBearer("GjirafaScheme", options =>
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

// Auth0Scheme authentication setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        options.RequireHttpsMetadata = false;
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}/",
            ValidAudience = builder.Configuration["Auth0:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth0:ClientSecret"])),
            // RoleClaimType = $"{builder.Configuration["Auth0:Namespace"]}roles"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                
                context.HttpContext.User = context.Principal ?? new ClaimsPrincipal();
                var auth0UserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = context.Principal?.FindFirst("https://ecommerce-life-2.com/roles")?.Value;
                
                
                var _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

                var existingUser = _unitOfWork.Repository<User>().GetByCondition(x => x.Auth0UserId == auth0UserId).FirstOrDefault();

                if (existingUser == null)
                {
                    var user = new User()
                    {
                        FirstName = context.Principal?.FindFirst("https://ecommerce-life-2.com/given_name")?.Value,
                        LastName = context.Principal?.FindFirst("https://ecommerce-life-2.com/family_name")?.Value,
                        Address = "context.HttpContext.User.FindFirst(ClaimTypes.StreetAddress)?.Value",
                        Auth0UserId = auth0UserId,
                        Email = context.Principal?.FindFirst("https://ecommerce-life-2.com/email")?.Value,
                        Password = "test",
                        PhoneNumber = context.Principal?.FindFirst("https://ecommerce-life-2.com/userId")?.Value,
                        RoleId = 3,
                    };
                        
                    _unitOfWork.Repository<User>().Create(user);
                    _unitOfWork.Complete();
                    
                    //// Generate your own JWT token
                    //var token = TokenService.GenerateToken(user.Id, user.UserRole.RoleName, user.Email);
                
                    //// Return the token to the user
                    //context.HttpContext.Response.ContentType = "application/json";
                    //await context.HttpContext.Response.WriteAsync($"{{\"token\":\"{token}\"}}");
                }
                else
                {
                    //// Generate your own JWT token
                    //var token = TokenService.GenerateToken(existingUser.Id, existingUser.UserRole.RoleName, existingUser.Email);
                
                    //// Return the token to the user
                    //context.HttpContext.Response.ContentType = "application/json";
                    //await context.HttpContext.Response.WriteAsync($"{{\"token\":\"{token}\"}}");
                }

            }
        };
    });

// Custom authorization policy that allows both schemes
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("DualSchemePolicy", policy =>
//     {
//         policy.AddAuthenticationSchemes("GjirafaScheme", JwtBearerDefaults.AuthenticationScheme);
//         // policy.RequireAuthenticatedUser();
//         policy.RequireRole("1");
//     });
//
//     // Existing policy
//     // options.AddPolicy("Organizer", policy =>
//     // {
//     //     policy.RequireRole("Organizer");
//     // });
// });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LifeEcommerce", Version = "v1" });

    // OAuth2 Security Definition
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            Implicit  = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"https://{builder.Configuration["Auth0:Domain"]}/oauth/token"),
                AuthorizationUrl = new Uri($"https://{builder.Configuration["Auth0:Domain"]}/authorize?audience={builder.Configuration["Auth0:Audience"]}"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenId" },
                }
            }
        }
    });

    // Bearer Token Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Authentication Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JsonWebToken",
        Scheme = "Bearer"
    });

    // Security Requirements (OAuth2)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "openid" }
        }
    });

    // Security Requirements (Bearer)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapControllers();

app.Run();
