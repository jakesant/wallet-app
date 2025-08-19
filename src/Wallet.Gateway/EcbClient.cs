using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Wallet.Gateway.Interfaces;
using Wallet.Gateway.Models;

namespace Wallet.Gateway
{
    public class EcbClient : IEcbClient
    {
        private readonly EcbClientOptions _options;
        private readonly HttpClient _http;

        public EcbClient(EcbClientOptions options, HttpClient http)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _http = http ?? throw new ArgumentNullException(nameof(http));

            _http.Timeout = TimeSpan.FromSeconds(Math.Max(1, _options.TimeoutSeconds));
            //_http.DefaultRequestHeaders.Accept.Clear();
            //_http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            _options = options ?? throw new ArgumentNullException(nameof(options));
            _http = http;
        }
        public async Task<CurrencyRate> GetLatestAsync(CancellationToken ct = default)
        {
            try
            {
                var requestUri = new Uri(new Uri(_options.BaseUrl, UriKind.Absolute), _options.Path);

                using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct)
                                            .ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();

                var xml = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return EcbParser.ParseLatestRates(xml);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
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
