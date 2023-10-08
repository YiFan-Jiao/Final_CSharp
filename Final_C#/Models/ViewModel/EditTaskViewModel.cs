using AdvancedTopic_FinalProject.Models;
using System.ComponentModel.DataAnnotations;

namespace AdvancedTopic_FinalProject.Models.ViewModel
{
    public class EditTaskViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required hours are required.")]
        [Range(1, 999, ErrorMessage = "Required hours must be between 1 and 999 (inclusive).")]
        public int RequiredHours { get; set; }

       
    }
}
