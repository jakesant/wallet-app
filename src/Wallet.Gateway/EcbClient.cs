using System.Net.Http.Headers;
using Wallet.Gateway.Interfaces;
using Wallet.Gateway.Models;
using Microsoft.Extensions.Options;

namespace Wallet.Gateway
{
    public class EcbClient : IEcbClient
    {
        private readonly EcbClientOptions _options;
        private readonly HttpClient _http;

        public EcbClient(IOptions<EcbClientOptions> options, HttpClient http)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (http == null) throw new ArgumentNullException(nameof(http));

            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _http = http;

            _http.Timeout = TimeSpan.FromSeconds(Math.Max(1, _options.TimeoutSeconds));
            //_http.DefaultRequestHeaders.Accept.Clear();
            //_http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }
        public async Task<CurrencyRate> GetLatestAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = new Uri(new Uri(_options.BaseUrl, UriKind.Absolute), _options.Path);

                using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                                            .ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();

                var xml = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return EcbParser.ParseLatestRates(xml);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EcbGatewayException("Failed to fetch or parse ECB rates.", ex);
            }
        }
    }
}
