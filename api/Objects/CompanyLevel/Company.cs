namespace api.Objects.CompanyLevel
{
    ///<summary>
    /// Contains info on companies.
    /// Used in all reports.
    ///</summary>
    public class Company
    {
        public string name { get; set; }
        public string country { get; set; }
        public object street { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string state { get; set; }
        public string url { get; set; }
        public int numberOfDealers { get; set; }
        public string userId { get; set; }
        public int numberOfBranches { get; set; }
        public object customUrl { get; set; }
        public object shortName { get; set; }
        public object isActive { get; set; }
        public object companyMailRecipients { get; set; }
        public string phone { get; set; }
        public string id { get; set; }
    }
}