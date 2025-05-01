namespace WebApplication3.Models
{
    public class EditProgramViewModel
    {
        public string SelectedProgramName { get; set; } // Holds the selected program name

        public List<string> AvailablePrograms { get; set; } // List of all available programs

        public string ProgramName { get; set; } // Editable field
        public string Benefits { get; set; } // Editable field
        public decimal Charge { get; set; } // Editable field
    }

}
