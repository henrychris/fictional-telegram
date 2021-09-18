namespace api.Objects.CompanyLevel
{
    ///<summary>
    /// used in Outstanding Payments Report
    ///</summary>
    public class OutstandingPayments
    {
        public string branchId { get; set; }
        public string branchName { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bank { get; set; }
        public double amount { get; set; }
    }
}