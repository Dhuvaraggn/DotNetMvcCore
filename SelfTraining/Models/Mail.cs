using System.ComponentModel.DataAnnotations;

namespace SelfTraining.Models
{
    public class Mail
    {
        public int ID { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public string MailBody { get; set; }
        public string? Status { get; set; }
    }
}
