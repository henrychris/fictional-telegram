namespace api.Objects.CompanyLevel
{
    public class WalletDetails
    {
        // used in company wallet report
        public string companyId { get; set; }
        public string bank { get; set; }
        public string bankCode { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string minBalance { get; set; }
        public string payTime { get; set; }
        public string accountType { get; set; }
        public string balance { get; set; }
        public string bookbalance { get; set; }
        public bool branchCanUpdate { get; set; }
        public string id { get; set; }
    }
}