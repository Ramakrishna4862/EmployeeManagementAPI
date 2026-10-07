namespace EmployeeManagementAPI.DTOs
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public Decimal Salary { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
