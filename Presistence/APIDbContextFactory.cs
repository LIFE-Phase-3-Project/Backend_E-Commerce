//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace Presistence;

//public class APIDbContextFactory : IDesignTimeDbContextFactory<APIDbContext>
//{
//    public APIDbContext CreateDbContext(string[] args)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<APIDbContext>();
        
//        var configurationPath = Path.Combine(Directory.GetCurrentDirectory(), "../Life-Ecommerce");


//        // Read configuration from appsettings.json
//        IConfigurationRoot configuration = new ConfigurationBuilder()
//            .SetBasePath(configurationPath)
//            .AddJsonFile("appsettings.json")
//            .Build();

//        var connectionString = configuration.GetConnectionString("DevConnection");

//        optionsBuilder.UseSqlServer(connectionString);

//        return new APIDbContext(optionsBuilder.Options);
//    }
//}