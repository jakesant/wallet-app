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

try
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddEcbGateway(options =>
                config.GetSection("Client").Bind(options));

            services.AddScoped<ExchangeRateRepository>();
            services.AddScoped<WalletAccountRepository>();
            services.AddScoped<WalletService>();
            services.AddScoped<BalanceStrategyResolver>();

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("RateUpdateJob");
                q.AddJob<RateUpdateJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts.ForJob(jobKey)
                    .WithIdentity("RateUpdateJob-trigger")
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        })
        .Build();

    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
