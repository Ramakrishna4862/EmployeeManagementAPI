using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EmployeeManagementAPI.Tests.MoqPractice;

namespace EmployeeManagementAPI.Tests.MoqPractice
{
    public class EmployeeServiceMoqTests
    {
        [Fact]
        public void GetEmployeeName_ExistingId_ReturnsEmployeeName()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();

            mockService
                .Setup(service => service.GetEmployeeName(1))
                .Returns("Rama");

            // Act
            var result = mockService.Object.GetEmployeeName(1);

            // Assert 

            // Assert
            Assert.Equal("Rama", result);

            mockService.Verify(
                service => service.GetEmployeeName(1),
                Times.Once);

        }


        [Fact]
        public void GetEmployeeName_DifferentId_ReturnsNull()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();

            mockService
                .Setup(service => service.GetEmployeeName(1))
                .Returns("Rama");

            // Act
            var result = mockService.Object.GetEmployeeName(2);

            // Assert
            Assert.Null(result);

            mockService.Verify(
                service => service.GetEmployeeName(2),
                Times.Once);
        }


        [Fact]
        public void GetEmployeeName_WhenCalledWithId1_DoesNotCallId2()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();

            mockService
                .Setup(service => service.GetEmployeeName(1))
                .Returns("Rama");

            // Act
            var result = mockService.Object.GetEmployeeName(1);

            // Assert
            Assert.Equal("Rama", result);

            mockService.Verify(
                service => service.GetEmployeeName(2),
                Times.Never);
        }

        [Fact]
        public void GetEmployeeName_WhenServiceThrowsException_ThrowsException()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();

            mockService
                .Setup(service => service.GetEmployeeName(1))
                .Throws(new Exception("Service failed"));

            // Act and Assert
            var exception = Assert.Throws<Exception>(
                () => mockService.Object.GetEmployeeName(1));

            Assert.Equal("Service failed", exception.Message);

            mockService.Verify(
                service => service.GetEmployeeName(1),
                Times.Once);
        }


        [Fact]
        public void GetEmployeeReport_ExistingEmployee_ReturnsReport()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();

            mockService
                .Setup(service => service.GetEmployeeName(1))
                .Returns("Rama");

            var reportService = new EmployeeReportService(
                mockService.Object);

            // Act
            var result = reportService.GetEmployeeReport(1);

            // Assert
            Assert.Equal("Employee Report: Rama", result);

            mockService.Verify(
                service => service.GetEmployeeName(1),
                Times.Once);
        }


    }
}
