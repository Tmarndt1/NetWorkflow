using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetWorkflow.Scheduler;

namespace NetWorkflow.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a Workflow of type TWorkflow to the IOC container using constructor injection.
        /// </summary>
        /// <typeparam name="TWorkflow">The type of Workflow to register.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to.</param>
        /// <param name="lifetime">The lifetime to use when registering the Workflow.</param>
        public static IServiceCollection AddWorkflow<TWorkflow, TResult>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TWorkflow : class, IWorkflow<TResult>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Add(new ServiceDescriptor(typeof(TWorkflow), typeof(TWorkflow), lifetime));
            services.AddTransient<IWorkflowRunner<TWorkflow, TResult>, WorkflowRunner<TWorkflow, TResult>>();

            return services;
        }

        /// <summary>
        /// Adds a Workflow service type with an implementation type to the IOC container using constructor injection.
        /// </summary>
        /// <typeparam name="TWorkflow">The Workflow service type to register.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <typeparam name="TImplementation">The implementation Workflow type to resolve to.</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to.</param>
        /// <param name="lifetime">The lifetime to use when registering the Workflow.</param>
        public static IServiceCollection AddWorkflow<TWorkflow, TResult, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TWorkflow : class, IWorkflow<TResult>
            where TImplementation : class, TWorkflow
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Add(new ServiceDescriptor(typeof(TWorkflow), typeof(TImplementation), lifetime));

            return services;
        }

        /// <summary>
        /// Adds a WorkflowStep to the IOC container.
        /// </summary>
        /// <typeparam name="TStep">The type of WorkflowStep to register.</typeparam>
        /// <param name="services">The IServiceCollection to register the WorkflowStep to.</param>
        /// <param name="lifetime">The lifetime to use when registering the WorkflowStep.</param>
        public static IServiceCollection AddWorkflowStep<TStep>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TStep : class, IWorkflowStep
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Add(new ServiceDescriptor(typeof(TStep), typeof(TStep), lifetime));

            return services;
        }

        /// <summary>
        /// Adds a transient Workflow of type TWorkflow to the IOC container using a factory.
        /// </summary>
        /// <typeparam name="TWorkflow">The type of Workflow to register.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to.</param>
        /// <param name="func">The factory used to create the Workflow.</param>
        public static IServiceCollection AddWorkflow<TWorkflow, TResult>(this IServiceCollection services, Func<TWorkflow> func)
            where TWorkflow : class, IWorkflow<TResult>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (func == null) throw new ArgumentNullException(nameof(func));

            services.AddTransient<IWorkflowRunner<TWorkflow, TResult>, WorkflowRunner<TWorkflow, TResult>>();

            return services.AddTransient<TWorkflow>(x => func.Invoke());
        }

        /// <summary>
        /// Adds a transient Workflow service type with an implementation type to the IOC container using a factory.
        /// </summary>
        /// <typeparam name="TWorkflow">The Workflow service type to register.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <typeparam name="TImplementation">The implementation Workflow type to resolve to.</typeparam>
        /// <param name="services">The IServiceCollection to register the Workflow to.</param>
        /// <param name="func">The factory used to create the Workflow implementation.</param>
        public static IServiceCollection AddWorkflow<TWorkflow, TResult, TImplementation>(this IServiceCollection services, Func<TImplementation> func)
            where TWorkflow : class, IWorkflow<TResult>
            where TImplementation : class, TWorkflow
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (func == null) throw new ArgumentNullException(nameof(func));

            return services.AddTransient<TWorkflow, TImplementation>(x => func.Invoke());
        }

        /// <summary>
        /// Adds a transient WorkflowScheduler to the IOC container.
        /// </summary>
        /// <typeparam name="TWorkflow">The type of Workflow the WorkflowScheduler uses.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <param name="services">The IServiceCollection to register the WorkflowScheduler to.</param>
        /// <param name="func">The factory used to create the WorkflowScheduler.</param>
        public static IServiceCollection AddWorkflowScheduler<TWorkflow, TResult>(this IServiceCollection services, Func<WorkflowScheduler<TWorkflow, TResult>> func)
            where TWorkflow : class, IWorkflow<TResult>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (func == null) throw new ArgumentNullException(nameof(func));

            return services.AddTransient(x => func.Invoke());
        }

        /// <summary>
        /// Adds a hosted Workflow scheduler that resolves each Workflow execution from a fresh dependency injection scope.
        /// </summary>
        /// <typeparam name="TWorkflow">The type of Workflow to schedule.</typeparam>
        /// <typeparam name="TResult">The type of result produced by the Workflow.</typeparam>
        /// <param name="services">The IServiceCollection to register the hosted Workflow to.</param>
        /// <param name="configuration">The scheduler configuration.</param>
        public static IServiceCollection AddHostedWorkflow<TWorkflow, TResult>(
            this IServiceCollection services,
            Action<WorkflowSchedulerConfiguration<TResult>> configuration)
            where TWorkflow : class, IWorkflow<TResult>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddWorkflow<TWorkflow, TResult>();
            services.AddSingleton(configuration);
            services.AddHostedService<HostedWorkflowService<TWorkflow, TResult>>();

            return services;
        }
    }
}
