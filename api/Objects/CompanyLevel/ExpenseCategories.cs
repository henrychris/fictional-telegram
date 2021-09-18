using System;

namespace api.Objects.CompanyLevel
{
    ///<summary>
    /// used in ExpenseCatergories report
    ///</summary>
    public class ExpenseCategories
    {
        public string category { get; set; }
        public string companyId { get; set; }
        public DateTime date { get; set; }
        public string id { get; set; }
    }
}