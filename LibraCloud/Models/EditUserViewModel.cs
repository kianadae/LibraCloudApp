using System.ComponentModel.DataAnnotations;

namespace LibraCloud.Models
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
