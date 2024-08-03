using Application.Services.TokenService;
using BackgroundJobs;
using Configurations;
using Hangfire;
using Hangfire.SqlServer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddWebServices(builder.Configuration);

builder.Services.AddHangfireConfig(builder.Configuration);



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

app.UseMiddleware<AuthMiddleware>();
RecurringJob.AddOrUpdate<ProductAnalyticsJobs>(job => job.RecalculateTopRatedProductsAsync(), Cron.Daily);
RecurringJob.AddOrUpdate<ProductAnalyticsJobs>(job => job.RecalculateTopSoldProductsAsync(), Cron.Daily);

app.UseSession();
app.MapControllers();

app.Run();
