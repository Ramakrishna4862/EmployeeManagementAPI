using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        [Range(1000, 1000000)]
        public decimal Salary { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
