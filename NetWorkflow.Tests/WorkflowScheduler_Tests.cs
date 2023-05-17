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
            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow());

            bool hit = false;

            // Act
            try
            {
                scheduler.StartAsync();

                hit = true;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsType<InvalidOperationException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void No_Factory_Success()
        {
            // Arrange
            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(null)
                .Configure(config =>
                {
                    config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(200));
                });

            bool hit = false;

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            try
            {
                scheduler.StartAsync(tokenSource.Token);

                hit = true;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsType<InvalidOperationException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Frequency_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow((stepName) =>
            {
                count++;
            }))
            .Configure(config =>
            {
                config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(200));
            });

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);
            DateTime now = DateTime.Now;
            Thread.Sleep(TimeSpan.FromMilliseconds(450));
            tokenSource.Cancel();

            // Assert
            Assert.Equal(6, count);
        }

        [Fact]
        public void AtTime_Success()
        {
            // Arrange
            int count = 0;
            
            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow((stepName) =>
            {
                count++;
            }))
            .Configure(config =>
            {
                config.ExecuteAt = WorkflowTime.AtMinute(DateTime.Now.Minute).Repeat();
            });

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Act
            scheduler.StartAsync(tokenSource.Token);

            // Assert
            Thread.Sleep(1000); // Must sleep for a second 

            tokenSource.Cancel();

            Assert.Equal(2, count); // Count should be two because both WorkflowStep's increment
        }

        [Fact]
        public void AtTime_No_Fire_Success()
        {
            // Arrange
            int count = 0;
            
            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow((stepName) =>
            {
                count++;
            }))
            .Configure(config =>
            {
                config.ExecuteAt = WorkflowTime.AtHour(1, DateTime.Now.Minute - 1).Repeat();
            });

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
                .AddWorkflowScheduler(() => new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow()))
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

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow((stepName) =>
            {
                count++;
            }))
            .Configure(config =>
            {
                config.ExecuteAt = WorkflowTime.AtFrequency(TimeSpan.FromMilliseconds(50));
            });

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
