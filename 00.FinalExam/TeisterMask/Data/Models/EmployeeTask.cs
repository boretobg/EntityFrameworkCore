using System.ComponentModel.DataAnnotations.Schema;

namespace TeisterMask.Data.Models
{
    public class EmployeeTask
    {
        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("TaskId")]
        public int TaskId  { get; set; }
        public Task Task { get; set; }
    }
}
