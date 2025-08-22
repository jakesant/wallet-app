using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Wallet.Demo.Jobs;
using Wallet.Gateway;
using Wallet.Gateway.Extensions;
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
builder.Services.AddScoped<WalletAccountRepository>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<BalanceStrategyResolver>();

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
app.MapControllers();

app.Run();
