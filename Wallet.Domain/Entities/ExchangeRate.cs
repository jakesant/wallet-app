using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Entities
{
    public class ExchangeRate
    {
        public DateOnly Date { get; set; }
        public string BaseCurrency { get; set; } = null!;
        public string CounterCurrency { get; set; } = null!;

        public decimal Rate { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}