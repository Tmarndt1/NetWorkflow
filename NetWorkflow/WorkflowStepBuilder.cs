
namespace NetWorkflow
{
    public abstract class WorkflowStepBuilder
    {
        internal WorkflowStepBuilder? Next { get; set; }
    }

    #region With Params

    public class WorkflowStepBuilder<TContext, Tin, Tout> : WorkflowStepBuilder, IWorkflowStepBuilder<TContext, Tin, Tout>
    {
        private readonly TContext _context;

        private readonly Func<WorkflowStep<Tin, Tout>> _func;

        public WorkflowStepBuilder(Func<WorkflowStep<Tin, Tout>> func, TContext context)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));

            _context = context; 
        }

        public Tout Run(Tin args, CancellationToken token = default) => _func.Invoke().Run(args, token);

        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<WorkflowStep<Tout, TNext>> func)
        {
            Next = new WorkflowStepBuilder<TContext, Tout, TNext>(func, _context);

            return (IWorkflowStepBuilder<TContext, Tout, TNext>)Next;
        }

        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<TContext, WorkflowStep<Tout, TNext>> func)
        {
            Next = new WorkflowStepBuilder<TContext, Tout, TNext>(() => func.Invoke(_context), _context);

            return (IWorkflowStepBuilder<TContext, Tout, TNext>)Next;
        }
    }

    #endregion

    #region No Params

    public class WorkflowStepBuilder<TContext, Tout> : WorkflowStepBuilder, IWorkflowStepBuilder<TContext, Tout>
    {
        private readonly TContext _context;

        private readonly Func<WorkflowStep<Tout>> _func;

        public WorkflowStepBuilder(Func<WorkflowStep<Tout>> func, TContext context)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            _func = func;

            _context = context;
        }

        public Tout Run(CancellationToken token = default) => _func.Invoke().Run(token);

        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<WorkflowStep<Tout, TNext>> func)
        {
            Next = new WorkflowStepBuilder<TContext, Tout, TNext>(func, _context);

            return (IWorkflowStepBuilder<TContext, Tout, TNext>)Next;
        }

        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<TContext, WorkflowStep<Tout, TNext>> func)
        {
            Next = new WorkflowStepBuilder<TContext, Tout, TNext>(() => func.Invoke(_context), _context);

            return (IWorkflowStepBuilder<TContext, Tout, TNext>)Next;
        }
    }

    #endregion
}
