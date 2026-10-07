using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EmployeeManagementAPI.DTOs;
    


namespace EmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            var response = employees.Select(employee => new EmployeeResponseDto
            { 
              Id = employee.Id,
              Name = employee.Name,
              Email = employee.Email,
              Department = employee.Department,
              Salary = employee.Salary,
              CreatedDate = employee.CreatedDate
            }).ToList();
            return Ok(response);


        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(EmployeeCreateDto employeeDto)
        {
            try
            {
                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Email = employeeDto.Email,
                    Department = employeeDto.Department,
                    Salary = employeeDto.Salary, 
                    CreatedDate = DateTime.UtcNow
                };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if(employee == null)
            {
                return NotFound();
            }

            var response = new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary,
                CreatedDate = employee.CreatedDate
            };


            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto employeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            
            if(employee == null)
            {
                return NotFound();
            }

            employee.Name = employeeDto.Name;
            employee.Email = employeeDto.Email;
            employee.Department = employeeDto.Department;
            employee.Salary = employeeDto.Salary;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
