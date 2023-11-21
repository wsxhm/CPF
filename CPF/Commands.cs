using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CPF
{
    /// <summary>
    /// 绑定的命令集合
    /// </summary>
    public class Commands : IEnumerable
    {
        internal HybridDictionary<string, List<Command>> commands = new HybridDictionary<string, List<Command>>();

        CpfObject owner;
        public Commands(CpfObject cpfObject)
        {
            owner = cpfObject;
        }

        /// <summary>
        /// 添加处理命令，命令方法在CommandContext或者其他对象上
        /// </summary>
        /// <param name="eventName">触发的事件名或者属性名</param>
        /// <param name="methodName">触发事件之后调用的方法名</param>
        /// <param name="obj">方法所在的对象如果为null，在是CommandContext</param>
        /// <param name="ps">方法参数，如果参数是CommandParameter则可以获取对应的事件数据或者属性值</param>
        public void Add(string eventName, string methodName, object obj = null, params object[] ps)
        {
            List<Command> list;
            if (!commands.TryGetValue(eventName, out list))
            {
                list = new List<Command>();
                commands.Add(eventName, list);
            }
            //if (list.FirstOrDefault(a => a.MethodName == methodName && a.Obj == obj) != null)
            //{
            //    throw new Exception("已经存在相同处理命令");
            //}
            list.Add(new Command { MethodName = methodName, Params = ps, Target = obj == null ? null : new WeakReference(obj) });
        }
        ///// <summary>
        ///// 添加处理命令，命令方法在自己获取父级UI元素层上
        ///// </summary>
        ///// <param name="eventName"></param>
        ///// <param name="methodName">触发事件之后调用的方法名</param>
        ///// <param name="relation">UI元素关系，如果为null，则是CommandContext对象</param>
        ///// <param name="ps">方法参数，如果参数是CommandParameter则可以获取对应的事件数据或者属性值</param>
        //public void Add(string eventName, string methodName, Relation relation, params object[] ps)
        //{
        //    List<Command> list;
        //    if (!commands.TryGetValue(eventName, out list))
        //    {
        //        list = new List<Command>();
        //        commands.Add(eventName, list);
        //    }
        //    if (relation == null)
        //    {
        //        list.Add(new Command { MethodName = methodName, Params = ps, Target = null });
        //    }
        //    else
        //    {
        //        list.Add(new Command { MethodName = methodName, Params = ps, Relation = relation });
        //    }
        //}
        /// <summary>
        /// 添加处理命令，初始化的时候查找相对元素并绑定
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="methodName"></param>
        /// <param name="find"></param>
        /// <param name="ps"></param>
        public void Add(string eventName, string methodName, Func<UIElement, UIElement> find, params object[] ps)
        {
            List<Command> list;
            if (!commands.TryGetValue(eventName, out list))
            {
                list = new List<Command>();
                commands.Add(eventName, list);
            }
            if (find == null)
            {
                list.Add(new Command { MethodName = methodName, Params = ps, Target = null });
            }
            else
            {
                Threading.Dispatcher.MainThread.BeginInvoke(() =>
                {
                    var obj = find(owner as UIElement);
                    list.Add(new Command { MethodName = methodName, Params = ps, Target = obj == null ? null : new WeakReference(obj) });
                });
            }
        }

        /// <summary>
        /// 添加处理命令
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action">参数是事件数据EventArgs或者属性的CPFPropertyChangedEventArgs</param>
        public void Add(string eventName, Action<CpfObject, object> action)
        {
            List<Command> list;
            if (!commands.TryGetValue(eventName, out list))
            {
                list = new List<Command>();
                commands.Add(eventName, list);
            }
            list.Add(new Command { Action = action });
        }

        /// <summary>
        /// KeyValuePair&lt;string, List&lt;Command&gt;&gt;
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return commands.GetEnumerator();
        }
        /// <summary>
        /// 清空所有命令绑定，一般不建议调用
        /// </summary>
        public void Clear()
        {
            commands.Clear();
        }
    }


    public class Command
    {
        public string MethodName;
        public WeakReference Target;
        public object[] Params;
        //public Relation Relation;
        public Action<CpfObject, object> Action;
    }

}
