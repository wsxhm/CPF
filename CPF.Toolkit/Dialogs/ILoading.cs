using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Toolkit.Dialogs
{
    public interface ILoading
    {
        event Func<string, Task, Task<object>> ShowLoadingFunc;
        event Func<string, Task,Task> ShowLoading;
    }
}
