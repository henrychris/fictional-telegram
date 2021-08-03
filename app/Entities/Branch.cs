using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entities
{
    public class Branch
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }

        [ForeignKey("CompanyId")]
        public string CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}