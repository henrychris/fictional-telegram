using System;

namespace api.Objects.CompanyLevel
{
    // used in CompanySalesSummary.cs
    public class saleSummary
    {
        public DateTime date { get; set; }
        public string branchName { get; set; }
        public double pmsPumpSale { get; set; }
        public double pmsTankSale { get; set; }
        public double pmsSurplus { get; set; }
        public double agoPumpSale { get; set; }
        public double agoTankSale { get; set; }
        public double agoSurplus { get; set; }
        public double dpkPumpSale { get; set; }
        public double dpkTankSale { get; set; }
        public double dpkSurplus { get; set; }
    }
}