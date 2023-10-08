using System.ComponentModel.DataAnnotations;

namespace AdvancedTopic_FinalProject.Models
{
    public class Taask
    {
        

        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Task title must be between 5 and 200 characters.")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Required hours are required.")]
        [Range(1, 999, ErrorMessage = "Required hours must be between 1 and 999 (inclusive).")]
        public int RequiredHours { get; set; }

        public Priority Priority { get; set; }
        public bool CompletedTask { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        //public ICollection<DemoUserTask> DemoUserTasks { get; set; } = new List<DemoUserTask>();
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
