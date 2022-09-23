
using NetWorkflow.Interfaces;
using System.Linq.Expressions;

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

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<IWorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, IWorkflowStep<Tout>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout>(new WorkflowExecutor<TContext, Tout>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_next;
        }

        public virtual object? Run(object? args, CancellationToken token = default) => _next?.Run(token);
    }

    public class WorkflowBuilder<TContext, Tout> : WorkflowBuilder<TContext>, 
        IWorkflowBuilderNext<TContext, Tout>
    {
        internal readonly IWorkflowExecutor _executor;
        
        public WorkflowBuilder(IWorkflowExecutor executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<IWorkflowStepAsync<Tout, TNext>>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext[]>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, TNext[]>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<IWorkflowStep<Tout, TNext>>> func)
        {
            _next = new WorkflowBuilder<TContext, Tout, TNext>(new WorkflowExecutor<TContext, TNext>(func, _context), _context);

            return (IWorkflowBuilderNext<TContext, Tout, TNext>)_next;
        }

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<TContext, IWorkflowStep<Tout, TNext>>> func)
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

        public IWorkflowBuilderConditional<TContext, Tout> If(Expression<Func<Tout, bool>> func)
        {
            _next = new WorkflowBuilderConditional<TContext, Tout>(new WorkflowExecutorConditional<Tout>(func), _context);

            return (IWorkflowBuilderConditional<TContext, Tout>)_next;
        }
    }

    public class WorkflowBuilder<TContext, Tin, Tout> : WorkflowBuilder<TContext, Tout>, IWorkflowBuilderNext<TContext, Tin, Tout>
    {
        public WorkflowBuilder(IWorkflowExecutor executor, TContext context) : base(executor, context) { }
    }

    public class WorkflowBuilderConditional<TContext, Tin> : WorkflowBuilder<TContext>, IWorkflowBuilderConditional<TContext, Tin>, IWorkflowBuilderConditionalNext<TContext, Tin>
    {
        private readonly WorkflowExecutorConditional<Tin> _executor;

        public WorkflowBuilderConditional(WorkflowExecutorConditional<Tin> executor, TContext context) : base(context)
        {
            _executor = executor;
        }

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<IWorkflowStep<Tin, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<TContext, IWorkflowStep<Tin, TNext>>> func)
        {
            _executor.Append(new WorkflowExecutor<TContext, TNext>(func, _context));

            return this;
        }

        public IWorkflowBuilderConditional<TContext, Tin> ElseIf(Expression<Func<Tin, bool>> func)
        {
            _executor.Append(func);

            return this;
        }

        public IWorkflowBuilderConditional<TContext, Tin> Else()
        {
            _executor.Append(args => true);

            return this;
        }

        public IWorkflowBuilderNext<TContext, object> EndIf()
        {
            _next = new WorkflowBuilder<TContext, object>(new WorkflowExecutorNext(), _context);

            return (IWorkflowBuilderNext<TContext, object>)_next;
        }

        public IWorkflowBuilderConditionalNext<TContext, Tin> Stop()
        {
            _executor.SetStop();

            return this;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            object? results = _executor.Run(args, token);

            if (_executor.Stopped || _next == null) return results;

            return _next?.Run(results);
        }
    }
}

