using AdvancedTopic_FinalProject.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedTopic_FinalProject.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Project title is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Project title must be between 5 and 200 characters.")]
        public string title { get; set; }

      
        public string DemoUserID { get; set; }
        public TaskUser DemoUser { get; set; }

        public HashSet<Taask> Tasks { get; set; } = new HashSet<Taask>();




    }
}
