using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Toolkit.Input
{
    public interface IAsyncCommand
    {
        Task ExecutionTask { get; }
        bool IsRunning { get; }
        Task ExecuteAsync();
    }
}
