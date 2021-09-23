using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entities
{
    public class AppUser
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChatId { get; set; }
        [Required] public string FirstName { get; set; }
        public string Username { get; set; }
        public DateTime AuthDate { get; set; }
        public string Hash { get; set; }
        public string State { get; set; }
        public string CurrentBranch { get; set; }
        public string Email { get; set; }

        public string EpumpDataId { get; set; } = null;
        public virtual EpumpData EpumpData { get; set; } = null;
    }
}