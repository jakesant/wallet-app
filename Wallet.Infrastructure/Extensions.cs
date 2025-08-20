using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Gateway;
using Wallet.Gateway.Interfaces;

namespace Wallet.Infrastructure
{
    public static class Extensions
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
