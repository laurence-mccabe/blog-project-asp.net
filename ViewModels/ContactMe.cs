using System.ComponentModel.DataAnnotations;

namespace BlogProj_12_10_22.ViewModels
{
    public class ContactMe
    {
        public string? Name { get; set; }
        // [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }


    }
}
