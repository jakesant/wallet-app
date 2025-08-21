using Microsoft.Extensions.DependencyInjection;
using Wallet.Gateway.Interfaces;

namespace Wallet.Gateway.Extensions
{
    public static class EcbExtensions
    {
        public static IServiceCollection AddEcbGateway(
            this IServiceCollection services,
            Action<EcbClientOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddHttpClient<IEcbClient, EcbClient>();
            return services;
        }
    }
}