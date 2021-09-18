using System;

namespace api.Objects.CompanyLevel
{
    public class BankTransactions
    {
        // used in WalletFundRequest
         public string bankName { get; set; }
        public double amountPaid { get; set; }
        public string depositorName { get; set; }
        public string depositSlipNo { get; set; }
        public DateTime datePaid { get; set; }
        public DateTime dateReq { get; set; }
        public bool isAdded { get; set; }
        public string walletId { get; set; }
        public string userName { get; set; }
        public string referralCode { get; set; }
        public string corpId { get; set; }
        public string id { get; set; }
    }
}