using System;

namespace api.Objects.RetailLevel
{
    // used in TankReport.cs
    public class TankReportObj
    {
        public double volumeSold { get; set; }
        public double volumeFilled { get; set; }
        public double openingDip { get; set; }
        public double closingDip { get; set; }
        public string productName { get; set; }
        public string tankName { get; set; }
        public string branchName { get; set; }
        public DateTime date { get; set; }
        public string branchId { get; set; }
    }
}