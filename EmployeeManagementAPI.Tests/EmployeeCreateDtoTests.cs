using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using EmployeeManagementAPI.DTOs;
using Xunit;

namespace EmployeeManagementAPI.Tests
{
    public class EmployeeCreateDtoTests
    {

        [Fact]
        public void SalaryBelowMinimum_ShouldFailValidation()
        {
            // Arrange
            var employee = new EmployeeCreateDto
            {
                Name = "Rama Krishna",
                Email = "rama@example.com",
                Department = "IT",
                Salary = 500
            };

            var context = new ValidationContext(employee);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(
                employee,
                context,
                results,
                validateAllProperties: true);

            // Debug
            var salaryProperty = typeof(EmployeeCreateDto)
                .GetProperty(nameof(EmployeeCreateDto.Salary));

            var rangeAttribute = salaryProperty?
                .GetCustomAttributes(typeof(RangeAttribute), true)
                .FirstOrDefault() as RangeAttribute;

            // Assert
            Assert.NotNull(salaryProperty);
            Assert.NotNull(rangeAttribute);

            Assert.False(
                isValid,
                $"Validation returned {isValid}. " +
                $"Salary = {employee.Salary}, " +
                $"Range = {rangeAttribute.Minimum} to {rangeAttribute.Maximum}, " +
                $"Errors = {string.Join("; ", results.Select(r => r.ErrorMessage))}");
        }

    }
}
