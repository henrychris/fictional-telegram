namespace app.Components.Summary
{
    public class SummaryData
    {
        // TODO Name Variables better
        public byte[] pdfReport { get; set; }
        public string pmsAmount { get; set; }
        public string agoAmount { get; set; }
        public string dpkAmount { get; set; }
        public string totalAmount { get; set; }

        // pos transaction
        public string cashbackAmount { get; set; }
        public string succesfulAmount { get; set; }
        public string failedAmount { get; set; }

        // sales summary
        public string pmsPumpSale { get; set; }
        public string pmsTankSale { get; set; }
        public string agoPumpSale { get; set; }
        public string agoTankSale { get; set; }
        public string dpkTankSale { get; set; }
        public string dpkPumpSale { get; set; }

        // volume
        public string pmsVolume { get; set; }
        public string agoVolume { get; set; }
        public string dpkVolume { get; set; }
        public string lpgVolume { get; set; }
        public string totalVolume { get; set; }

        // tanks filled
        public string epumpDischarge { get; set; }
        public string manualDischarge { get; set; }

        // variance
        public string totalEpumpVolumeSold { get; set; }
        public string totalManualVolumeSold { get; set; }
        public string totalVariance { get; set; }

        // wallet fund
        public string amountPaid { get; set; }

        // asset groups
        public string numberOfAssets;
        public string numberOfAssetsRequiringAttention;

        // company walllet
        public string walletBalance { get; set; }
        public string walletBookBalance { get; set; }

        // outstanding payments
        public string outstandingAmount { get; set; }

        // product summary
        public string pmsAmountSold { get; set; }
        public string agoAmountSold { get; set; }
        public string dpkAmountSold { get; set; }
        public string pmsVolumeSold { get; set; }
        public string agoVolumeSold { get; set; }
        public string dpkVolumeSold { get; set; }
        public string totalAmountSold { get; set; }
        public string totalVolumeSold { get; set; }
    }
}