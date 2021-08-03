using System.Collections.Generic;

namespace app.Entities
{
    public class Company
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
    }
}