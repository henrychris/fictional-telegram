using System;

namespace api.Objects.RetailLevel
{
    ///<summary>
    /// Represents the details of a branch
    ///</summary>
    // used in all branch level reports
    public class BranchDetails
    {
        public double agoPercentage { get; set; }
        public double agoTotalVolume { get; set; }
        public string city { get; set; }
        public string companyId { get; set; }
        public string companyLogoUrl { get; set; }
        public string companyName { get; set; }
        public string country { get; set; }
        public DateTime date { get; set; }
        public object daySaleEndHour { get; set; }
        public string dealerId { get; set; }
        public string dealerName { get; set; }
        public object displayProduct { get; set; }
        public double dpkPercentage { get; set; }
        public double dpkTotalVolume { get; set; }
        public string email { get; set; }
        public string engagementLevel { get; set; }
        public string fullAddress { get; set; }
        public object groupId { get; set; }
        public object groupName { get; set; }
        public string id { get; set; }
        public bool isCompanyActive { get; set; }
        public double lpgPercentage { get; set; }
        public double lpgTotalVolume { get; set; }
        public string name { get; set; }
        public bool online { get; set; }
        public object pefStationId { get; set; }
        public string phone { get; set; }
        public double pmsPercentage { get; set; }
        public double pmsTotalVolume { get; set; }
        public double ratio { get; set; }
        public object repId { get; set; }
        public string sendReportMail { get; set; }
        public object serviceType { get; set; }
        public string state { get; set; }
        public string street { get; set; }
        public double totalVolume { get; set; }
        public object userId { get; set; }
        public object wmStationId { get; set; }
    }
}