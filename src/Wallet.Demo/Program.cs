using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Repository;

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

            services.Configure<EcbClientOptions>(config.GetSection("Client"));
            services.AddHttpClient<IEcbClient, EcbClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.ecb.europa.eu");
                client.Timeout = TimeSpan.FromSeconds(5);
            });

            services.AddScoped<ExchangeRateRepository>();
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
