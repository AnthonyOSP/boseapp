#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace boseapp.Integration.CurrencyExchange.dto
{
    public class CurrencySymbols
    {
        public bool success { get; set; }
        public Dictionary<string, string> Symbols { get; set; }
    }
}