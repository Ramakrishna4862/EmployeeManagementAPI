using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EmployeeManagementAPI.DTOs;
using Microsoft.Data.SqlClient;



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
        public async Task<ActionResult<PaginatedResponseDto<EmployeeResponseDto>>> GetEmployees(int pageNumber=1,int pageSize=10,string? empName = null, string? department = null, string? sortBy="id",string? sortOrder = "asc")
        {
            if (pageNumber < 1 || pageSize< 1 || pageSize>100)
            {
                return BadRequest(new ErrorResponse
                { 
                  StatusCode = StatusCodes.Status400BadRequest,
                  Message = "Page number must be at least 1 and page size must be between 1 and 100.",
                  TraceId = HttpContext.TraceIdentifier
                });

            }

            var query = _context.Employees.AsNoTracking();

            if(!string.IsNullOrWhiteSpace(empName))
            {
                empName = empName.Trim();
                query = query.Where(e => e.Name.Contains(empName));
            }
            if(!string.IsNullOrWhiteSpace(department))
            {
                department = department.Trim();
                query = query.Where(e => e.Department == department);
            }

            int totalEmployees = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(
                totalEmployees / (double)pageSize);

            if (pageNumber > totalPages && totalPages > 0)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Page number cannot exceed {totalPages}.",
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            if (sortBy != "id" &&
    sortBy != "name" &&
    sortBy != "salary")
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid sortBy value. Allowed values are id, name, salary.",
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            if (sortOrder != "asc" && sortOrder != "desc")
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid sortOrder value. Allowed values are asc, desc.",
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            sortBy = string.IsNullOrWhiteSpace(sortBy)
    ? "id"
    : sortBy.Trim().ToLowerInvariant();

            sortOrder = string.IsNullOrWhiteSpace(sortOrder)
                ? "asc"
                : sortOrder.Trim().ToLowerInvariant();

            query = (sortBy, sortOrder) switch
            {
                ("id", "asc") => query.OrderBy(e => e.Id),
                ("id", "desc") => query.OrderByDescending(e => e.Id),

                ("name", "asc") =>
                    query.OrderBy(e => e.Name).ThenBy(e => e.Id),
                ("name", "desc") =>
                    query.OrderByDescending(e => e.Name).ThenBy(e => e.Id),

                ("salary", "asc") =>
                    query.OrderBy(e => e.Salary).ThenBy(e => e.Id),
                ("salary", "desc") =>
                    query.OrderByDescending(e => e.Salary).ThenBy(e => e.Id),

                _ => query.OrderBy(e => e.Id)
            };

            var employees = await query
                .Skip((int)(((long)pageNumber - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();



            var response = employees.Select(employee => new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary,
                CreatedDate = employee.CreatedDate
            }).ToList();

            return Ok(new PaginatedResponseDto<EmployeeResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalEmployees = totalEmployees,
                TotalPages = totalPages,
                Data = response
            });

        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(EmployeeCreateDto employeeDto)
        {

            var emailExists = await _context.Employees.AnyAsync(e => e.Email == employeeDto.Email);

            if (emailExists)
            {
                return Conflict(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "An employee with this email already exists.",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            
                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Email = employeeDto.Email,
                    Department = employeeDto.Department,
                    Salary = employeeDto.Salary, 
                    CreatedDate = DateTime.UtcNow
                };
                _context.Employees.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "An employee with this email already exists.",
                    TraceId = HttpContext.TraceIdentifier
                });
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

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                response);
            
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
        public async Task<ActionResult<EmployeeResponseDto>> UpdateEmployee(int id, EmployeeUpdateDto employeeDto)
        {
            // Step 1: Check validation errors
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Step 2: Find the employee
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }


            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email == employeeDto.Email
                            && e.Id != id);


            if (emailExists)
            {
                return Conflict(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "An employee with this email already exists.",
                    TraceId = HttpContext.TraceIdentifier
                });
            }


            // Step 3: Update employee details
            employee.Name = employeeDto.Name;
            employee.Email = employeeDto.Email;
            employee.Department = employeeDto.Department;
            employee.Salary = employeeDto.Salary;

            // Step 4: Save changes
            await _context.SaveChangesAsync();

            // Step 5: Return HTTP 204
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
