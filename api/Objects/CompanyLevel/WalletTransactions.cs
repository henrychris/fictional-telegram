using System;

namespace api.Objects.CompanyLevel
{
    public class WalletTransactions
    {
        // used in company wallet report
        public DateTime date { get; set; }
        public string amount { get; set; }
        public string walletBalance { get; set; }
        public string source { get; set; }
        public string status { get; set; }
        public string walletId { get; set; }
        public string customerId { get; set; }
        public string retainer { get; set; }
        public string id { get; set; }
    }
}