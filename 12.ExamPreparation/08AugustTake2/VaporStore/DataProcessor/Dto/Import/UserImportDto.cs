using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UserImportDto
    {
        [Required]
        [RegularExpression("[A-Z]{1}[A-z]+ [A-Z]{1}[A-z]+")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Range(3, 103)]
        public int Age { get; set; }

        public CardsImportDto[] Cards { get; set; }
    }
}
