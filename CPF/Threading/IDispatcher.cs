using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Threading
{
    public interface IDispatcher
    {
        bool CheckAccess();

        void VerifyAccess();

        void Invoke(Action callback);

        void BeginInvoke(Action callback);
    }
}
