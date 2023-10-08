using AdvancedTopic_FinalProject.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AdvancedTopic_FinalProject.Models
{
    public class DemoUserTask
    {
        [Key]
        public int Id { get; set; }

        public string RoleId { get; set; }

        public TaskUser DemoUser { get; set; }

        public int TaaskId { get; set; }

        public Taask Taask { get; set; }
    }
}
