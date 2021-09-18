using System;

namespace api.Objects.CompanyLevel
{
    /*
    used for CompanyBranchSales.cs or Company Level Branch Sales
    */
    public class SalesPerBranch
    {
        public double volumeSold { get; set; }
        public double amountSold { get; set; }
        public double price { get; set; }
        public string productName { get; set; }
        public DateTime date { get; set; }
        public string branchName { get; set; }
        public string branchId { get; set; }
        public string dealerName { get; set; }
    }
}