using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EmployeeManagementAPI.DTOs
{
    public class EmployeeUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        [Range(1000,1000000)]
        public decimal Salary { get; set; } 
    }
}
