using Microsoft.Extensions.DependencyInjection;

namespace NetWorkflow.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Registers a Workflow type to the inversion of control services container
        /// </summary>
        /// <typeparam name="TWorkflow">The type of workflow to register</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to</param>
        public static IServiceCollection AddWorkflow<TWorkflow>(this IServiceCollection services)
            where TWorkflow : class, IWorkflow
        {
            return services.AddTransient<TWorkflow>();
        }
    }
}