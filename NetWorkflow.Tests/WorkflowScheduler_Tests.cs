using Microsoft.Extensions.DependencyInjection;
using NetWorkflow.Extensions;
using NetWorkflow.Scheduler;
using NetWorkflow.Tests.Examples;

namespace NetWorkflow.Tests
{
    public class WorkflowScheduler_Tests
    {
        [Fact]
        public void No_Options_Success()
        {
            // Arrange
            bool hit = false;

            // Act
            try
            {
               _ = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), null);

                hit = true;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsType<ArgumentNullException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void No_Factory_Success()
        {
            // Arrange


            bool hit = false;

            // Act
            try
            {
                _ = new WorkflowScheduler<HelloWorldWorkflow, bool>(null, config => config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(200)));

                hit = true;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsType<ArgumentNullException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Frequency_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(50));
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(TimeSpan.FromMilliseconds(149));

            tokenSource.Cancel();

            Assert.Equal(2, count);
        }

        [Fact]
        public void Frequency_Max_Count_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(50)).Until(2);
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(TimeSpan.FromMilliseconds(200));

            tokenSource.Cancel();

            Assert.Equal(2, count);
        }

        [Fact]
        public void AtTime_Minute_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtMinute(DateTime.Now.Minute);
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(1000); // Must sleep for a second 

            tokenSource.Cancel();

            Assert.Equal(1, count); // Count should be two because both WorkflowStep's increment
        }

        [Fact]
        public void AtTime_Hour_Success()
        {
            // Arrange
            int count = 0;

            DateTime now = DateTime.Now;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtHour(now.Hour, now.Minute);
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(1000); // Must sleep for a second 

            tokenSource.Cancel();

            Assert.Equal(1, count); // Count should be two because both WorkflowStep's increment
        }

        [Fact]
        public void AtTime_Day_Success()
        {
            // Arrange
            int count = 0;

            DateTime now = DateTime.Now;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtDay(now.Day, now.Hour, now.Minute);
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(1000); // Must sleep for a second 

            tokenSource.Cancel();

            Assert.Equal(1, count); // Count should be two because both WorkflowStep's increment
        }

        [Fact]
        public void AtTime_No_Fire_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtHour(1, DateTime.Now.Minute - 1);
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(200);

            tokenSource.Cancel();

            Assert.Equal(0, count);
        }

        [Fact]
        public void AddWorkflowScheduler_Extensions_Success()
        {
            // Arrange
            // Act
            var workflowScheduler = new ServiceCollection()
                .AddWorkflowScheduler(() => new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
                {
                    config.ExecuteAt = WorkflowTime.AtHour(1);
                }))
                .BuildServiceProvider()
                .GetRequiredService<WorkflowScheduler<HelloWorldWorkflow, bool>>();

            // Assert
            Assert.NotNull(workflowScheduler);
        }

        [Fact]
        public void Dispose_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow(), config =>
            {
                config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(50));
            });

            scheduler.OnExecuted += (o, e) => ++count;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            scheduler.StartAsync(tokenSource.Token);

            // Act
            scheduler.Dispose();

            Thread.Sleep(100);

            // Assert
            Assert.Equal(0, count);
        }
    }
}
