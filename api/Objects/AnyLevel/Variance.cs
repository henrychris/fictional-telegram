using System;

namespace api.Objects.AnyLevel
{
    ///<summary>
    ///used in branch and Company Variance
    ///</summary>
    public class Variance
    {
        public double volumeSold { get; set; }
        public double amountSold { get; set; }
        public double mannualVolumeSold { get; set; }
        public double mannualAmountSold { get; set; }
        public string productName { get; set; }
        public DateTime date { get; set; }
        public double productVariance { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
    }
}