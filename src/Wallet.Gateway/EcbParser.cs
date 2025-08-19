using System.Globalization;
using System.Xml.Linq;
using Wallet.Gateway.Models;

namespace Wallet.Gateway
{
    public class EcbParser
    {
        /// <summary>
        /// Parses the ECB daily XML into a strongly-typed snapshot. Assumes base EUR.
        /// Robust to namespaces (matches LocalName).
        /// </summary>
        public static CurrencyRate ParseLatestRates(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML content is empty.", nameof(xml));

            var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

            // Find the first Cube element that has a 'time' attribute (the daily snapshot)
            var timeCube = doc
                .Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "Cube" && e.Attributes("time").Any())
                ?? throw new FormatException("ECB XML did not contain a time Cube.");

            var timeAttr = timeCube.Attribute("time")?.Value
                ?? throw new FormatException("ECB XML time Cube missing 'time' attribute.");

            if (!DateTime.TryParseExact(timeAttr, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt) &&
                !DateTime.TryParse(timeAttr, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
            {
                throw new FormatException($"Unrecognised ECB time format: '{timeAttr}'.");
            }

            var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var rateCube in timeCube.Elements().Where(e => e.Name.LocalName == "Cube"))
            {
                var curr = rateCube.Attribute("currency")?.Value;
                var rateStr = rateCube.Attribute("rate")?.Value;

                if (string.IsNullOrWhiteSpace(curr) || string.IsNullOrWhiteSpace(rateStr))
                    continue;

                if (!decimal.TryParse(rateStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var rate))
                    throw new FormatException($"Invalid rate '{rateStr}' for currency '{curr}'.");

                rates[curr] = rate;
            }

            if (rates.Count == 0)
                throw new FormatException("ECB XML contained no currency rates.");

            return new CurrencyRate("EUR", DateTime.SpecifyKind(dt, DateTimeKind.Utc), rates);
        }
    }
}
