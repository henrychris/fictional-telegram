using System;

namespace api.Objects.AnyLevel
{
    ///<summary>
    /// Used in branch and Company POSTransactions
    ///</summary>
    public class PosTransactions
    {
        public string transactionReference { get; set; }
        public string reference { get; set; }
        public string amount { get; set; }
        public string type { get; set; }
        public string retrievalReferenceNumber { get; set; }
        public string maskedPan { get; set; }
        public string cardScheme { get; set; }
        public string customerName { get; set; }
        public string statusCode { get; set; }
        public string statusDescription { get; set; }
        public string currency { get; set; }
        public string merchantId { get; set; }
        public string stan { get; set; }
        public string cardExpiry { get; set; }
        public string cardHash { get; set; }
        public string additionalInformation { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public object pumpId { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public object branchAddress { get; set; }
        public string vendor { get; set; }
        public string terminalId { get; set; }
        public DateTime paymentDate { get; set; }
        public string cashBack { get; set; }
        public string charge { get; set; }
        public string pumpName { get; set; }
    }
}