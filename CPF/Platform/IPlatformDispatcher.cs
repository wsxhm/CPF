using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    /// <summary>
    /// 主线程调度接口
    /// </summary>
    public interface IPlatformDispatcher
    {
        void Invoke(Action callback);

        void BeginInvoke(Action callback);
    }
}
