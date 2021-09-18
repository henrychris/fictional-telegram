using System;

namespace api.Objects.CompanyLevel
{
    ///<summary>
    /// Used in asset groups report
    /// Regarding group type, there should be a switch case for it in the report
    ///</summary>
    public class AssetGroups
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int groupType { get; set; }
        public DateTime dateCreated { get; set; }
        public string assetsCount { get; set; }
        public string assetsRequiringAttentionCount { get; set; }
    }
}