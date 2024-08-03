using Presistence.Repositories.OrderRepo;
using Application.Services.Category;
using Application.Services.Discount;
using Application.Services.Email;
using Application.Services.ImageStorage;
using Application;
using Application.Services.Order;
using Application.Services.Payment;
using Application.Services.Product;
using Application.Services.Review;
using Application.Services.Search;
using Application.Services.ShoppingCart;
using Application.Services.Subcategory;
using Application.Services.User;
using Application.Services.UserRepository;
using Application.Services.Wishlist;
using Presistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Application.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using System.Text;
using Application.Services.UserAddress;
using Presistence.Repositories.ProductAnalytics;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Application.Services.ProductAnalytics;
using Hangfire;
using Hangfire.SqlServer;
using BackgroundJobs;


namespace Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

               var mapperConfiguration = new MapperConfiguration(
                        mc => mc.AddProfile(new AutoMapperConfiguration()));

               IMapper mapper = mapperConfiguration.CreateMapper();

               services.AddSingleton(mapper);
               services.AddScoped<IEmailService, EmailService>();
               services.AddScoped<IOrderRepository, OrderRepository>();
               services.AddScoped<IUserService, UserService>();
               services.AddScoped<IUserContext, UserContext>();
               services.AddScoped<IUnitOfWork, UnitOfWork>();
               services.AddScoped<IProductService, ProductService>();
               services.AddScoped<IReviewService, ReviewService>();
               services.AddScoped<ISearchService, SearchService>();
               services.AddScoped<ICategoryService, CategoryService>();
               services.AddScoped<ISubCategoryService, SubCategoryService>();
               services.AddScoped<IShoppingCartService, ShoppingCartService>();
               services.AddScoped<IWishlistService, WishlistService>();
               services.AddScoped<IOrderService, OrderService>();
               services.AddScoped<IPaymentService, PaymentService>();
               services.AddScoped<IStorageService, StorageService>();
               services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IUserAddressService, UserAddressService>();

            services.AddScoped<IProductAnalyticsService, ProductAnalyticsService>();
            services.AddScoped<IProductAnalyticsRepo, ProductAnalyticsRepo>();
            services.AddScoped<ProductAnalyticsJobs>();


            return services;
        }

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add persistence services
            services.AddDbContext<APIDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DevConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Elastic Search configuration
            var uri = new Uri(configuration["ElasticSearch:Uri"]);
            var password = configuration["ElasticSearch:Password"];
            var settings = new ConnectionSettings(uri)
                .BasicAuthentication("elastic", password)
                .DefaultIndex("products_v2");

            var elasticClient = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(elasticClient);


            // Redis configuration
            var redisConnectionString = configuration["Redis:ConnectionString"];
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            return services;
        }

        public static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DevConnection");
           services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                   {
                       CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                       SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                       QueuePollInterval = TimeSpan.Zero,
                       UseRecommendedIsolationLevel = true,
                       DisableGlobalLocks = true
                   }));

            services.AddHangfireServer();
            return services;
        }
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(configuration);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
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
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddDataProtection();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            services.AddCors(options =>
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



            services.AddSession(options =>
            {
                // Set a reasonable timeout for session
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.Name = "LifeEcommerce.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;

        }


    }
}
