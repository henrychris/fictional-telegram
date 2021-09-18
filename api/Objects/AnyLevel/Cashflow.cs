using System;

namespace api.Objects.AnyLevel
{
    ///<summary>
    /// Used in branch and Company Cashflow
    ///</summary>
    public class Cashflow
    {
        public DateTime date { get; set; }
        public string companyId { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public double previousBalance { get; set; }
        public double totalSales { get; set; }
        public double expense { get; set; }
        public double epumpSales { get; set; }
        public double retainerSales { get; set; }
        public double posSales { get; set; }
        public double bankedCash { get; set; }
        public double outstandingCash { get; set; }
        public double cashInsuranceRatio { get; set; }
        public double compliance { get; set; }
        public string id { get; set; }
    }
}