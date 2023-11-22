using CPF;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class IPlugin : CpfObject, IDisposable
    {
        /// <summary>
        /// 当自动执行时,此属性指示可重试次数
        /// </summary>
        public int MaxTryCount { get { return GetValue<int>(); } set { SetValue(value); } }
        ///// <summary>
        ///// 插件状态
        ///// </summary>
        //public PluginState State { get { return GetValue<PluginState>(); } set { SetValue(value); } }
        /// <summary>
        /// 最后消息
        /// </summary>
        public string LastMessage { get { return GetValue<string>(); } set { SetValue(value); } }
        /// <summary>
        /// 插件名
        /// </summary>
        public string PluginName { get { return GetValue<string>(); } set { SetValue(value); } }
        /// <summary>
        /// 关闭插件 
        /// </summary>
        public virtual void Close()
        {

        }
    }

    public class TestPlugin : IPlugin
    {
        public override void Close()
        {
            base.Close();
        }
    }
}
