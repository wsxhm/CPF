using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 路由事件
    /// </summary>
    public class RoutedEventArgs : EventArgs
    {
        public RoutedEventArgs()
        {
            this.Handled = false;
        }

        public RoutedEventArgs(object source)
        {
            this.OriginalSource = source;
            this.Handled = false;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示针对路由事件（在其经过路由时）的事件处理的当前状态。
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// 在父类进行任何可能的 Source 调整之前，获取由纯命中测试确定的原始报告源。
        /// </summary>
        public object OriginalSource
        {
            get;
            protected set;
        }

        /// <summary>
        /// 重设事件触发源
        /// </summary>
        /// <param name="source"></param>
        public void OverrideSource(object source)
        {
            this.OriginalSource = source;
        }
        ///// <summary>
        ///// 触发者
        ///// </summary>
        //public object Sender { get;internal set; }
        ///// <summary>
        /////     Invokes the generic handler with the
        /////     appropriate arguments
        ///// </summary>
        ///// <remarks>
        /////     Is meant to be overridden by sub-classes of
        /////     <see cref="RoutedEventArgs"/> to provide
        /////     more efficient invocation of their delegate
        ///// </remarks>
        ///// <param name="genericHandler">
        /////     Generic Handler to be invoked
        ///// </param>
        ///// <param name="genericTarget">
        /////     Target on whom the Handler will be invoked
        ///// </param>
        //protected virtual void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        //{
        //    if (genericHandler == null)
        //    {
        //        throw new ArgumentNullException("genericHandler");
        //    }

        //    if (genericTarget == null)
        //    {
        //        throw new ArgumentNullException("genericTarget");
        //    }

        //    //if (genericHandler is RoutedEventHandler)
        //    //{
        //    //    ((RoutedEventHandler)genericHandler)(genericTarget, this);
        //    //}
        //    //else
        //    if (genericHandler is EventHandler<RoutedEventArgs>)
        //    {
        //        ((EventHandler<RoutedEventArgs>)genericHandler)(genericTarget, this);
        //    }
        //    else
        //    {
        //        // Restricted Action - reflection permission required
        //        genericHandler.DynamicInvoke(new object[] { genericTarget, this });
        //    }
        //}

        //internal void InvokeHandler(Delegate handler, object target)
        //{
        //    InvokeEventHandler(handler, target);
        //}
    }

    //public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);
}
