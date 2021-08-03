using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entities
{
    public class Company
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string EpumpDataId { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        [ForeignKey("EpumpDataId")]
        public virtual EpumpData EpumpData { get; set; }
    }
}