using System.ComponentModel.DataAnnotations;

namespace api.Controllers
{
    ///<summary>
    /// Used in POS Transactions Report endpoint
    ///</summary>
    public class POSRequestParameters
    {
        [Required]
        public string companyId { get; set; }
        public string branchId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string status { get; set; }
    }
}