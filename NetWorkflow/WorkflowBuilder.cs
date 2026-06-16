using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class WorkflowBuilder : IWorkflowBuilder, IDisposable
    {
        protected WorkflowBuilder _next;

        private bool _disposedValue;

        public object Result { get; protected set; }

        public IWorkflowBuilderNext<TOut> StartWith<TOut>(Expression<Func<IWorkflowStep<TOut>>> func)
        {
            _next = new WorkflowExecutionNode<object, TOut>(new StartStepExecutor<TOut>(func));

            return (IWorkflowBuilderNext<TOut>)_next;
        }

        public virtual object Run(object args, CancellationToken token = default)
        {
            return RunAsync(args, token).GetAwaiter().GetResult();
        }

        public virtual ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            return _next?.RunAsync(args, token) ?? new ValueTask<object>((object)null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _next?.Dispose();

                    Result = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }

    internal class WorkflowExecutionNode<TIn, TOut> : WorkflowBuilder, IWorkflowBuilderNext<TIn, TOut>
    {
        private readonly IWorkflowExecutor _executor;

        private bool _disposedValue;

        internal WorkflowExecutionNode(IWorkflowExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public IWorkflowBuilderNext<IEnumerable<TNext>> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TOut, TNext>>>> func)
        {
            _next = new WorkflowExecutionNode<TOut, IEnumerable<TNext>>(new ParallelStepExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<IEnumerable<TNext>>)_next;
        }

        public IWorkflowBuilderNext<TOut, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TOut, TNext>>> func)
        {
            _next = new WorkflowExecutionNode<TOut, TNext>(new SyncStepExecutor<TOut, TNext>(func));

            return (IWorkflowBuilderNext<TOut, TNext>)_next;
        }

        public IWorkflowBuilderConditional<TOut> If(Expression<Func<TOut, bool>> func)
        {
            _next = new ConditionalWorkflowBuilder<TOut>(new ConditionalExecutor<TOut>(func));

            return (IWorkflowBuilderConditional<TOut>)_next;
        }

        public override async ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            Result = await _executor.RunAsync(args, token).ConfigureAwait(false);

            if (_next == null) return Result;

            return await _next.RunAsync(Result, token).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _executor.Dispose();
                }

                _disposedValue = true;

                base.Dispose(disposing);
            }
        }
    }

    internal class ConditionalWorkflowBuilder<TIn> : WorkflowBuilder,
        IWorkflowBuilderConditional<TIn>,
        IWorkflowBuilderConditionalNext<TIn>,
        IWorkflowBuilderConditionalFinal<TIn>,
        IWorkflowBuilderConditionalEnd
    {
        private readonly ConditionalExecutor<TIn> _executor;

        private bool _disposedValue;

        public ConditionalWorkflowBuilder(ConditionalExecutor<TIn> executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public IWorkflowBuilderConditionalNext<TIn> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            _executor.Append(new SyncStepExecutor<TIn, TNext>(func));

            return this;
        }

        public IWorkflowBuilderConditional<TIn> ElseIf(Expression<Func<TIn, bool>> func)
        {
            _executor.Append(func);

            return this;
        }

        public IWorkflowBuilderConditionalFinal<TIn> Else()
        {
            _executor.Append(args => true);

            return this;
        }

        public IWorkflowBuilderNext<object> EndIf()
        {
            _next = new WorkflowExecutionNode<object, object>(new PassThroughExecutor());

            return (IWorkflowBuilderNext<object>)_next;
        }

        public IWorkflowBuilderConditionalNext<TIn> Stop()
        {
            _executor.Stop();

            return this;
        }

        public IWorkflowBuilderConditionalNext<TIn> Throw(Expression<Func<Exception>> func)
        {
            _executor.OnExceptionDo(func);

            return this;
        }

        public override async ValueTask<object> RunAsync(object args, CancellationToken token = default)
        {
            Result = await _executor.RunAsync(args, token).ConfigureAwait(false);

            if (_next == null) return Result;

            return await _next.RunAsync(Result, token).ConfigureAwait(false);
        }

        IWorkflowBuilderConditionalEnd IWorkflowBuilderConditionalFinal<TIn>.Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func)
        {
            Do(func);

            return this;
        }

        IWorkflowBuilderConditionalEnd IWorkflowBuilderConditionalFinal<TIn>.Stop()
        {
            Stop();

            return this;
        }

        IWorkflowBuilderConditionalEnd IWorkflowBuilderConditionalFinal<TIn>.Throw(Expression<Func<Exception>> func)
        {
            Throw(func);

            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _executor.Dispose();
                }

                _disposedValue = true;

                base.Dispose(disposing);
            }
        }
    }
}
