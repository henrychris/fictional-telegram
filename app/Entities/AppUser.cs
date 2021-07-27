using System;
using System.ComponentModel.DataAnnotations;

namespace app.Entities
{
    public class AppUser
    {
        [Key]
        [Required]
        public long ChatId { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string Username { get; set; }
        public DateTime AuthDate { get; set; }
        public string Hash { get; set; }
        public string EpumpAuthKey { get; set; }
    }
}