using Microsoft.Extensions.DependencyInjection;

namespace NetWorkflow.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a transient Workflow of type TWorkflow to the IOC container
        /// </summary>
        /// <typeparam name="TWorkflow">The type of workflow to register</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to</param>
        public static IServiceCollection AddWorkflow<TWorkflow>(this IServiceCollection services, Func<TWorkflow> func)
            where TWorkflow : class, IWorkflow
        {
            return services.AddTransient<TWorkflow>(x => func.Invoke());
        }

        /// <summary>
        /// Adds a transient Workflow of type TWorkflow with an implementation of TImplementation to the IOC container
        /// </summary>
        /// <typeparam name="TWorkflow">The type of Workflow to register</typeparam>
        /// <typeparam name="TImplementation">The implementation Workflow type to resolve to</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to</param>
        public static IServiceCollection AddWorkflow<TWorkflow, TImplementation>(this IServiceCollection services, Func<TImplementation> func)
            where TWorkflow : class, IWorkflow
            where TImplementation : class, TWorkflow
        {
            return services.AddTransient<TWorkflow, TImplementation>(x => func.Invoke());
        }
    }
}