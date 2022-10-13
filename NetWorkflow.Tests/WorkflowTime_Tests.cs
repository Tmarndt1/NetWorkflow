using NetWorkflow.Exceptions;
using NetWorkflow.Scheduler;

namespace NetWorkflow.Tests
{
    public class WorkflowTime_Tests
    {
        [Fact]
        public void Day_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtDay(0);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Day_Range_Max_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtDay(32);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Hour_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtHour(-1);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Hour_Range_Max_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtDay(60);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Minute_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtMinute(-1);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Minute_Range_Max_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowTime.AtMinute(60);

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkflowInvalidValueException>(ex);
            }

            // Assert
            Assert.False(hit);
        }
    }
}
