using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.ImportDtos
{
    public class ImportUserDto
    {
        public ImportUserDto()
        {
            Cards = new HashSet<ImportCardDto>();
        }

        [Required]
        [RegularExpression("^[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+$")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3, 103)]
        public int Age { get; set; }

        public ICollection<ImportCardDto> Cards { get; set; }
    }

    public class ImportCardDto
    {
        [Required]
        [RegularExpression("^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression("^[0-9]{3}$")]
        public string Cvc { get; set; }

        public string Type { get; set; }
    }
}
