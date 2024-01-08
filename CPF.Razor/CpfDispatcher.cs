using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Razor
{
    public class CpfDispatcher : Dispatcher
    {
        public override bool CheckAccess()
        {
            return CPF.Threading.Dispatcher.MainThread.CheckAccess();
        }

        public override Task InvokeAsync(Action workItem)
        {
            return Task.Run(() =>
            {
                CPF.Threading.Dispatcher.MainThread.Invoke(workItem);
            });
        }

        public override Task InvokeAsync(Func<Task> workItem)
        {
            return Task.Run(() =>
            {
                var task = Task.CompletedTask;
                CPF.Threading.Dispatcher.MainThread.Invoke(() => { task = workItem(); });
                return task;
            });
        }

        public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
        {
            return Task.Run(() =>
            {
                TResult result = default;
                CPF.Threading.Dispatcher.MainThread.Invoke(() => { result = workItem(); });
                return result;
            });
        }

        public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
        {
            return Task.Run(() =>
            {
                TResult result = default;
                CPF.Threading.Dispatcher.MainThread.Invoke(async () => { result = await workItem(); });
                return result;
            });
        }
    }
}
