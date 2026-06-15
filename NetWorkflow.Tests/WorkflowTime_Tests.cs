using NetWorkflow.Exceptions;
using NetWorkflow.Scheduler;

namespace NetWorkflow.Tests
{
    public class WorkflowSchedule_Tests
    {
        [Fact]
        public void Day_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowSchedule.AtDay(0);

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
                var time = WorkflowSchedule.AtDay(32);

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
                var time = WorkflowSchedule.AtHour(-1);

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
                var time = WorkflowSchedule.AtDay(60);

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
                var time = WorkflowSchedule.AtMinute(-1);

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
                var time = WorkflowSchedule.AtMinute(60);

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
        public void Frequency_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowSchedule.AtFrequency(TimeSpan.Zero);

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
        public void Until_Range_Min_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
                var time = WorkflowSchedule.AtFrequency(TimeSpan.FromSeconds(1)).Until(0);

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
