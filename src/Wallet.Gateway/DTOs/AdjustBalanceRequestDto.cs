using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Gateway.DTOs
{
    public class AdjustBalanceRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
    }
}
