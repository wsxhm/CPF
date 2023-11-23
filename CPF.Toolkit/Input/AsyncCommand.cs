using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPF.Toolkit.Input
{
    public class AsyncCommand : IAsyncCommand
    {
        public AsyncCommand(Func<Task> execute)
        {
            this.execute = execute;
        }

        private readonly Func<Task> execute;
        private Task executionTask;

        public Task ExecutionTask
        {
            get => this.executionTask;
            private set
            {
                if (ReferenceEquals(this.executionTask, value))
                {
                    return;
                }

                this.executionTask = value;
                bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;
                if (isAlreadyCompletedOrNull)
                {
                    return;
                }
            }
        }

        public bool IsRunning => ExecutionTask is { IsCompleted: false };


        public Task ExecuteAsync()
        {
            Task executionTask = ExecutionTask;
            if (this.execute is not null)
            {
                executionTask = ExecutionTask = this.execute();
            }
            return executionTask;
        }
    }
}
