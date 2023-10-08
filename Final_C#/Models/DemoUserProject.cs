using AdvancedTopic_FinalProject.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AdvancedTopic_FinalProject.Models
{ 
    public class DemoUserProject 
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public TaskUser DemoUser { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
