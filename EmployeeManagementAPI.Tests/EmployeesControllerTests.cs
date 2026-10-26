using EmployeeManagementAPI.Controllers;
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Tests
{
    public class EmployeesControllerTests
    {

        [Fact]
        public async Task CreateEmployee_DuplicateEmail_Returns409Conflict()
        {
            // Arrange: Create a temporary database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            // Add an employee who already exists
            context.Employees.Add(new Employee
            {
                Name = "Rama Krishna",
                Email = "rama@gmail.com",
                Department = "IT",
                Salary = 50000,
                CreatedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var newEmployee = new EmployeeCreateDto
            {
                Name = "Another Employee",
                Email = "rama@gmail.com",
                Department = "HR",
                Salary = 40000
            };

            // Act
            var result = await controller.CreateEmployee(newEmployee);

            // Assert: The API should return 409 Conflict
            var conflictResult = Assert.IsType<ConflictObjectResult>(
                result.Result);

            Assert.Equal(409, conflictResult.StatusCode);

            // Verify the duplicate employee was not added
            Assert.Equal(1, await context.Employees.CountAsync());
        }


        [Fact]
        public async Task CreateEmployee_ValidEmployee_Returns201Created()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var controller = new EmployeesController(context)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var employeeDto = new EmployeeCreateDto
            {
                Name = "Ravi Kumar",
                Email = "ravi@gmail.com",
                Department = "IT",
                Salary = 50000
            };

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(
                result.Result);

            Assert.Equal(201, createdResult.StatusCode);

            var savedEmployee = await context.Employees
                .SingleAsync(e => e.Email == "ravi@gmail.com");

            Assert.Equal("Ravi Kumar", savedEmployee.Name);
            Assert.Equal("IT", savedEmployee.Department);
            Assert.Equal(50000m, savedEmployee.Salary);
        }

        [Fact]
        public async Task GetEmployee_EmployeeDoesNotExist_Returns404NotFound()
        {
            // Arrange: Create a temperary database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            //Add an employee to the database


            var controller = new EmployeesController(context);

            //Act

            var result = await controller.GetEmployee(999);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task GetEmployee_ExistingId_Returns200OK()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var employee = new Employee
            {
                Id = 1,
                Name = "Rama Krishna",
                Email = "rama@gmail.com",
                Department = "IT",
                Salary = 50000,
                CreatedDate = DateTime.UtcNow
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.GetEmployee(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var response = Assert.IsType<EmployeeResponseDto>(okResult.Value);

            Assert.Equal(1, response.Id);
            Assert.Equal("Rama Krishna", response.Name);
            Assert.Equal("rama@gmail.com", response.Email);
            Assert.Equal("IT", response.Department);
            Assert.Equal(50000m, response.Salary);
        }


        [Fact]
        public async Task UpdateEmployee_SameEmail_Returns204NoContent()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var employee = new Employee
            {
                Name = "Test Employee",
                Email = "same-email@example.com",
                Department = "IT",
                Salary = 25000m
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new EmployeesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var updateDto = new EmployeeUpdateDto
            {
                Name = "Updated Employee",
                Email = "same-email@example.com",
                Department = "HR",
                Salary = 30000m
            };

            // Act
            var result = await controller.UpdateEmployee(
                employee.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);

            var updatedEmployee = await context.Employees
                .FindAsync(employee.Id);

            Assert.NotNull(updatedEmployee);
            Assert.Equal("Updated Employee", updatedEmployee.Name);
            Assert.Equal("same-email@example.com", updatedEmployee.Email);
            Assert.Equal("HR", updatedEmployee.Department);
            Assert.Equal(30000m, updatedEmployee.Salary);
        }


        [Fact]
        public async Task UpdateEmployee_ExistingId_Returns204NoContent()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Employees.Add(new Employee
            {
                Id = 1,
                Name = "Rama Krishna",
                Email = "rama@gmail.com",
                Department = "IT",
                Salary = 50000,
                CreatedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            var updateDto = new EmployeeUpdateDto
            {
                Name = "Rama Krishna Updated",
                Email = "rama.updated@gmail.com",
                Department = "Development",
                Salary = 60000
            };

            // Act
            var result = await controller.UpdateEmployee(1, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);

            var updatedEmployee = await context.Employees.FindAsync(1);

            Assert.NotNull(updatedEmployee);
            Assert.Equal("Rama Krishna Updated", updatedEmployee.Name);
            Assert.Equal("rama.updated@gmail.com", updatedEmployee.Email);
            Assert.Equal("Development", updatedEmployee.Department);
            Assert.Equal(60000m, updatedEmployee.Salary);
        }


        public async Task UpdateEmployee_EmployeeDoesNotExist_Returns404NotFound()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var controller = new EmployeesController(context);

            var updateDto = new EmployeeUpdateDto
            {
                Name = "Ravi Kumar",
                Email = "ravi@gmail.com",
                Department = "IT",
                Salary = 50000m
            };

            //Act
            var result = await controller.UpdateEmployee(999, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(0, await context.Employees.CountAsync());
        }

        [Fact]
        public async Task UpdateEmployee_NonExistingId_ReturnsNotFound()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var controller = new EmployeesController(context);
            var updateDto = new EmployeeUpdateDto
            { 
                Name = "Updated Employee",
                Email = "updated@example.com",
                Department = "IT",
                Salary = 30000m
            };

            //Act
            var result = await controller.UpdateEmployee(999, updateDto);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }


        [Fact]
        public async Task UpdateEmployee_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var employee = new Employee
            {
                Name = "Test Employee",
                Email = "update-test@example.com",
                Department = "IT",
                Salary = 25000
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            var updateDto = new EmployeeUpdateDto
            {
                Name = "",
                Email = "invalid-email",
                Department = "IT",
                Salary = 25000
            };

            // Act
            controller.ModelState.AddModelError(
                "Name", "Name is required.");

            var result = await controller.UpdateEmployee(
                employee.Id, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task DeleteEmployee_ExistingId_Returns204NoContent()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Employees.Add(new Employee
            {
                Id = 1,
                Name = "Rama Krishna",
                Email = "rama@gmail.com",
                Department = "IT",
                Salary = 50000,
                CreatedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsType<NoContentResult>(result);


            var deletedEmployee = await context.Employees.FindAsync(1);

            Assert.Null(deletedEmployee);
        }




        [Fact]
        public async Task DeleteEmployee_ExistingId_RemovesEmployeeFromDatabase()
        {
            // Arrange: Create a temporary InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var employee = new Employee
            {
                Id = 1,
                Name = "Test Employee",
                Email = "delete-test@example.com",
                Department = "IT",
                Salary = 25000m,
                CreatedDate = DateTime.UtcNow
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert: Verify HTTP 204 No Content
            Assert.IsType<NoContentResult>(result);

            // Verify that the employee was actually deleted
            var deletedEmployee = await context.Employees.FindAsync(1);

            Assert.Null(deletedEmployee);
        }



        [Fact]
        public async Task DeleteEmployee_NonExistingId_Returns404NotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.DeleteEmployee(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);

            // Confirm that the database remains empty
            Assert.Equal(0, await context.Employees.CountAsync());
        }


        [Fact]
        public async Task GetEmployees_SecondPage_ReturnsCorrectPagination()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Employees.AddRange(
                new Employee
                {
                    Id = 1,
                    Name = "Arun",
                    Email = "arun@example.com",
                    Department = "IT",
                    Salary = 30000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 2,
                    Name = "Bala",
                    Email = "bala@example.com",
                    Department = "HR",
                    Salary = 40000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 3,
                    Name = "Charan",
                    Email = "charan@example.com",
                    Department = "IT",
                    Salary = 50000m,
                    CreatedDate = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.GetEmployees(
                pageNumber: 2,
                pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var response =
                Assert.IsType<PaginatedResponseDto<EmployeeResponseDto>>(
                    okResult.Value);

            Assert.Equal(2, response.PageNumber);
            Assert.Equal(2, response.PageSize);
            Assert.Equal(3, response.TotalEmployees);
            Assert.Equal(2, response.TotalPages);
            Assert.Single(response.Data);
            Assert.Equal("Charan", response.Data.First().Name);
        }


        [Fact]
        public async Task GetEmployees_FilterByDepartment_ReturnsOnlyMatchingEmployees()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Employees.AddRange(
                new Employee
                {
                    Id = 1,
                    Name = "Arun",
                    Email = "arun@example.com",
                    Department = "IT",
                    Salary = 30000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 2,
                    Name = "Bala",
                    Email = "bala@example.com",
                    Department = "HR",
                    Salary = 40000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 3,
                    Name = "Charan",
                    Email = "charan@example.com",
                    Department = "IT",
                    Salary = 50000m,
                    CreatedDate = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.GetEmployees(
                pageNumber: 1,
                pageSize: 10,
                department: "IT");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var response =
                Assert.IsType<PaginatedResponseDto<EmployeeResponseDto>>(
                    okResult.Value);

            Assert.Equal(2, response.TotalEmployees);
            Assert.Equal(1, response.TotalPages);
            Assert.Equal(2, response.Data.Count());

            Assert.All(response.Data, employee =>
                Assert.Equal("IT", employee.Department));
        }


        [Fact]

        public async Task GetEmployees_SortBySalaryDescending_ReturnsHighestSalaryFirst()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Employees.AddRange(
                new Employee
                {
                    Id = 1,
                    Name = "Arun",
                    Email = "arun@example.com",
                    Department = "IT",
                    Salary = 30000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 2,
                    Name = "Bala",
                    Email = "bala@example.com",
                    Department = "HR",
                    Salary = 50000m,
                    CreatedDate = DateTime.UtcNow
                },
                new Employee
                {
                    Id = 3,
                    Name = "Charan",
                    Email = "charan@example.com",
                    Department = "IT",
                    Salary = 40000m,
                    CreatedDate = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();

            var controller = new EmployeesController(context);

            // Act
            var result = await controller.GetEmployees(
                pageNumber: 1,
                pageSize: 10,
                sortBy: "salary",
                sortOrder: "desc");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var response =
                Assert.IsType<PaginatedResponseDto<EmployeeResponseDto>>(
                    okResult.Value);

            var employees = response.Data.ToList();

            Assert.Equal(3, employees.Count);
            Assert.Equal(50000m, employees[0].Salary);
            Assert.Equal(40000m, employees[1].Salary);
            Assert.Equal(30000m, employees[2].Salary);
        }




        [Fact]
        public async Task GetEmployees_InvalidPageSize_Returns400BadRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var controller = new EmployeesController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.GetEmployees(
                pageNumber: 1,
                pageSize: 101);

            // Assert
            var badRequestResult =
                Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task UpdateEmployee_DuplicateEmail_ReturnsConflict()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var employee1 = new Employee
            { 
                Name = "Employee One",
                Email = "employee1@example.com",
                Department = "IT",
                Salary = 25000m
            };

            var employee2 = new Employee 
            { 
                Name = "Employee Two",
                Email = "employee2@example.com",
                Department = "HR",
                Salary = 30000m
            };
            context.Employees.AddRange(employee1, employee2);
            await context.SaveChangesAsync();

            var controller = new EmployeesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var updateDto = new EmployeeUpdateDto
            {
                Name = "Updated Employee One",
                Email = "employee2@example.com",
                Department = "IT",
                Salary = 27000m
            };

            var result = await controller.UpdateEmployee(employee1.Id, updateDto);

            //Assert 
            var conflictResult = Assert.IsType<ConflictObjectResult>(
                result.Result);

            Assert.Equal(409, conflictResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponse>(
                    conflictResult.Value);

            Assert.Equal(409, errorResponse.StatusCode);
            Assert.Equal(
                "An employee with this email already exists.",
                errorResponse.Message);

            Assert.False(string.IsNullOrEmpty(errorResponse.TraceId));

            var unchangedEmployee = await context.Employees.FindAsync(employee1.Id);

            Assert.NotNull(unchangedEmployee);
            Assert.Equal("Employee One", unchangedEmployee.Name);
            Assert.Equal("employee1@example.com", unchangedEmployee.Email);
        }

    }





    }

