using System.Security.Claims;
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
using Hangfire;
using Hangfire.SqlServer;
using BackgroundJobs;
using Life_Ecommerce.Hubs;
using Presistence.Repositories.ChatRepo;
using Application.Services.Chat;
using Domain.Entities;
using Application.BackgroundJobs.ProductAnalytics;
using Application.BackgroundJobs.ShoppingCartCleanUp;
using Application.Services.TokenService;


namespace Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

               var mapperConfiguration = new MapperConfiguration(
                        mc => mc.AddProfile(new AutoMapperConfiguration()));

               IMapper mapper = mapperConfiguration.CreateMapper();

                services.AddSignalR();

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
               services.AddScoped<TokenHelper>();



               services.AddScoped<IProductAnalyticsService, ProductAnalyticsService>();
               services.AddScoped<IProductAnalyticsRepo, ProductAnalyticsRepo>();

               services.AddScoped<IChatRepository, ChatRepository>();
               services.AddScoped<IChatService, ChatService>();
               services.AddScoped<IGuestShoppingCartService, GuestShoppingCartService>();
               services.AddScoped<ProductAnalyticsJobs>();
               services.AddScoped<CleanUpGuestShoppingCarts>();
               services.AddScoped<IGuestShoppingCartService, GuestShoppingCartService>();
               services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();
                services.AddScoped<TokenHelper>();



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

            #region SwaggerGen

            services.AddSwaggerGen(c =>
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
                            TokenUrl = new Uri($"https://{configuration["Auth0:Domain"]}/oauth/token"),
                            AuthorizationUrl = new Uri($"https://{configuration["Auth0:Domain"]}/authorize?audience={configuration["Auth0:Audience"]}"),
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


            #endregion
            
            /*services.AddSwaggerGen(options =>
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
            });*/

            services.AddDataProtection();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            
            #region auth0andJWTauthentication

                        // GjirafaScheme authentication setup
services.AddAuthentication("GjirafaScheme")
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
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"https://{configuration["Auth0:Domain"]}";
        options.RequireHttpsMetadata = false;
        options.Audience = configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = $"https://{configuration["Auth0:Domain"]}/",
            ValidAudience = configuration["Auth0:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth0:ClientSecret"])),
            // RoleClaimType = $"{builder.Configuration["Auth0:Namespace"]}roles"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                
                context.HttpContext.User = context.Principal ?? new ClaimsPrincipal();
                var auth0UserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.Principal?.FindFirst("https://ecommerce-life-2.com/role")?.Value;
                
                
                var _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var _emailService = context.HttpContext.RequestServices.GetRequiredService<IEmailService>();
                
                var existingUser = _unitOfWork.Repository<User>().GetByCondition(x => x.Id == auth0UserId).FirstOrDefault();
                
                if (existingUser == null)
                {
                    var user = new User()
                    {
                        Id = auth0UserId,
                        FirstName = context.Principal?.FindFirst("https://ecommerce-life-2.com/given_name")?.Value,
                        LastName = context.Principal?.FindFirst("https://ecommerce-life-2.com/family_name")?.Value,
                        Email = context.Principal?.FindFirst("https://ecommerce-life-2.com/email")?.Value,
                        Password = "test",
                        Role = role ?? "Customer"
                    };
                        
                    _unitOfWork.Repository<User>().Create(user);
                    _unitOfWork.Complete();
                    
                    var subject = "Welcome to Life Ecommerce!";
                    var message = $@"
                    Hi {user.FirstName},

                    Welcome to the Life Ecommerce family!

                    We are thrilled to have you with us. At Life Ecommerce, we strive to provide you with the best shopping experience possible. If you have any questions, need assistance, or just want to say hello, don't hesitate to reach out to our support team.

                    Happy shopping!

                    Best regards,
                    The Life Ecommerce Team

                    -- 
                    Life Ecommerce
                    Your go-to place for all your needs";

                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }
                else
                {
                    existingUser.Role = role ?? "Customer";
                    _unitOfWork.Repository<User>().Update(existingUser);
                    _unitOfWork.Complete();
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

            #endregion


            #region AuthorizationPolicy

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOny", policy =>
                {
                    policy.RequireClaim("https://ecommerce-life-2.com/role", "Admin");
                });
                

                
            });

            #endregion

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
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;

        }


    }
}
