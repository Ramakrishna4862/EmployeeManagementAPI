using Xunit;

namespace EmployeeManagementAPI.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Add_TwoNumbers_ReturnsCorrectResult()
        {
            //Arrange
            int number1 = 10;
            int number2 = 20;

            //Act
            int result = number1 + number2;

            //Assert
            Assert.Equal(30, result);
        }
    }
}