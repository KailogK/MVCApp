using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class CreateProgramViewModel
    {
        [Required]
        [StringLength(50)]
        public string ProgramName { get; set; }

        [Required]
        public string Benefits { get; set; }

        [Required]
        [Range(0, 99999.99)]
        public decimal Charge { get; set; }
    }

}
