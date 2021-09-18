using System;

namespace api.Objects.RetailLevel
{
    // used in BranchSalesTransactions.cs and ProductSummary.cs
    // as long as double type values aren't null in the JSON file,
    // it'll all be fine
    
    public class Sales
    {
        public string id { get; set; }
        public double openingReading { get; set; }
        public double lastReading { get; set; }
        public double volumeSold { get; set; }
        public double amountSold { get; set; }
        public double price { get; set; }
        public string productName { get; set; }
        public DateTime date { get; set; }
        public string pumpName { get; set; }
        public string pumpId { get; set; }
        public object closingDate { get; set; }
    }
}