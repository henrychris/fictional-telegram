using System;

namespace api.Objects.CompanyLevel
{
    // used in ZonesReport
    public class Zones
    {
        public string name { get; set; }
        public string companyId { get; set; }
        public string companyName { get; set; }
        public int branches { get; set; }
        public DateTime date { get; set; }
        public string id { get; set; }
    }
}