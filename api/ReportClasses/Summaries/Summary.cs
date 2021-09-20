namespace api.ReportClasses.Summaries
{
    public class Summary
    {
        public byte[] PdfReport { get; set; }
        public string PmsAmount { get; set; }
        public string AgoAmount { get; set; }
        public string DpkAmount { get; set; }
        public string TotalAmount { get; set; }

        // pos transaction
        public string CashbackAmount { get; set; }
        public string SuccesfulAmount { get; set; }
        public string FailedAmount { get; set; }

        // sales summary
        public string PmsPumpSale { get; set; }
        public string PmsTankSale { get; set; }
        public string AgoPumpSale { get; set; }
        public string AgoTankSale { get; set; }
        public string DpkTankSale { get; set; }
        public string DpkPumpSale { get; set; }

        // volume
        public string PmsVolume { get; set; }
        public string AgoVolume { get; set; }
        public string DpkVolume { get; set; }
        public string LpgVolume { get; set; }
        public string TotalVolume { get; set; }

        // tanks filled
        public string EpumpDischarge { get; set; }
        public string ManualDischarge { get; set; }

        // variance
        public string TotalEpumpVolumeSold { get; set; }
        public string TotalManualVolumeSold { get; set; }
        public string TotalVariance { get; set; }

        // wallet fund
        public string AmountPaid { get; set; }

        // asset groups
        public string numberOfAssets;
        public string numberOfAssetsRequiringAttention;

        // company walllet
        public string WalletBalance { get; set; }
        public string WalletBookBalance { get; set; }

        // outstanding payments
        public string OutstandingAmount { get; set; }

        // product summary
        public string PmsAmountSold { get; set; }
        public string AgoAmountSold { get; set; }
        public string DpkAmountSold { get; set; }
        public string PmsVolumeSold { get; set; }
        public string AgoVolumeSold { get; set; }
        public string DpkVolumeSold { get; set; }
        public string TotalAmountSold { get; set; }
        public string TotalVolumeSold { get; set; }
    }
}