using System.ComponentModel.DataAnnotations;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportHallDto
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; }

        public bool Is4Dx { get; set; }

        public bool Is3D { get; set; }

        [Range(typeof(int), "1", "2147483647")]
        public int Seats { get; set; }
    }
}
