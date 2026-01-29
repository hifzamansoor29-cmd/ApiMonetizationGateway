using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Services;
using ApiMonetizationGateway.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<MonetizationDbContext>(options =>
    options.UseInMemoryDatabase("GatewayDb"));
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddSingleton<IUsageTracker, UsageTracker>();
builder.Services.AddHostedService<UsageProcessingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonetizationDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseMiddleware<RateLimitMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();