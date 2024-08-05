using Application.Services.TokenService;
using BackgroundJobs;
using Configurations;
using Hangfire;
using Life_Ecommerce.Hubs;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddWebServices(builder.Configuration);

builder.Services.AddHangfireConfig(builder.Configuration);

// Added health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<AuthMiddleware>();
RecurringJob.AddOrUpdate<ProductAnalyticsJobs>(job => job.RecalculateTopRatedProductsAsync(), Cron.Daily);
RecurringJob.AddOrUpdate<ProductAnalyticsJobs>(job => job.RecalculateTopSoldProductsAsync(), Cron.Daily);
RecurringJob.AddOrUpdate<CleanUpGuestShoppingCarts>(job => job.CleanUpAsync(), Cron.Monthly);

app.UseSession();
app.MapControllers();
app.MapHub<ChatHub>("/chat");

// Map health check endpoints
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // Show the readiness status
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true // Show the liveness status
});

app.Run();
