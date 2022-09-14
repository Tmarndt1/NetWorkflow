
using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    public class WorkflowBuilder<TContext> : IWorkflowBuilder<TContext>
    {
        protected readonly TContext _context;

        protected WorkflowBuilder<TContext>? _next;

        public WorkflowBuilder(TContext context)
        {
            _context = context;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<WorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, WorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _next?.Run(token);
    }

    public class WorkflowBuilder<TContext, Tout> : WorkflowBuilder<TContext>, IWorkflowBuilderNext<TContext, Tout>
    {
        protected readonly WorkflowExecutor<TContext> _executor;
        
        public WorkflowBuilder(WorkflowExecutor<TContext> executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<WorkflowStep<Tout, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout, TNext>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<TContext, WorkflowStep<Tout, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout, TNext>)_next;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? results = _executor.Run(args, token);

            if (_next == null) return results;

            return _next?.Run(results);
        }
    }

    public class WorkflowBuilder<TContext, Tin, Tout> : WorkflowBuilder<TContext, Tout>, IWorkflowBuilderNext<TContext, Tin, Tout>
    {
        public WorkflowBuilder(WorkflowExecutor<TContext> executor, TContext context) : base(executor, context) { }
    }
}

