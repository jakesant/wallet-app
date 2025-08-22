using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Threading.RateLimiting;
using Wallet.Demo.Jobs;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Repository;
using Wallet.Infrastructure.Strategy;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EcbClientOptions>(config.GetSection("Client"));
builder.Services.AddHttpClient<IEcbClient, EcbClient>(client =>
{
    client.BaseAddress = new Uri("https://www.ecb.europa.eu");
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<ExchangeRateRepository>();
builder.Services.AddScoped<IWalletAccountRepository, WalletAccountRepository>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<IBalanceStrategyResolver, BalanceStrategyResolver>();
builder.Services.AddScoped<AddFundsStrategy>();
builder.Services.AddScoped<SubtractFundsStrategy>();
builder.Services.AddScoped<ForceSubtractFundsStrategy>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("RateUpdateJob");
    q.AddJob<RateUpdateJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts.ForJob(jobKey)
        .WithIdentity("RateUpdateJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(30),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.RejectionStatusCode = 429;
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CacheService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();

public partial class Program { }