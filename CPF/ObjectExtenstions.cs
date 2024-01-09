using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CPF.Reflection;
using System.Linq;
using System.Data;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using CPF.Threading;
using CPF.Controls;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CPF
{
    /// <summary>
    /// Object数据转换扩展
    /// </summary>
    public static class ObjectExtenstions
    {
        /// <summary>
        /// 分配一个变量，用来在绑定的时候使用，比如 new Button{Content="test"}.Assign(out var btn)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T Assign<T>(this T o, out T v)
        {
            v = o;
            return o;
        }

        /// <summary>
        /// 观察事件或者属性变化，用于Reactive 开发模式，代替 Observable.FromEventPattern
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="source"></param>
        /// <param name="eventName">事件名或者属性名</param>
        /// <returns></returns>
        public static IObservable<EventObserver<TArgs, TSender>> Observe<TSender, TArgs>(this TSender source, string eventName) where TSender : CpfObject where TArgs : EventArgs
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException("eventName");
            }

            return new EO<TArgs, TSender> { Observable = source, eventName = eventName };
        }

        /// <summary>
        /// 设置控件模板，就是对Control的Template 属性进行泛型包装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static T SetTemplate<T>(this T control, Action<T, UIElementCollection> action) where T : Control
        {
            control.Template = (ctl, children) =>
            {
                action(ctl as T, children);
            };
            return control;
        }

        /// <summary>
        /// 延迟设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="obj"></param>
        /// <param name="timeSpan"></param>
        /// <param name="expression">a=>a.Property</param>
        /// <param name="value"></param>
        public static void Delay<T, V>(this T obj, TimeSpan timeSpan, Expression<Func<T, V>> expression, V value)
        {
            MemberExpression member = (MemberExpression)expression.Body;
            var property = member.Member.Name;
            DispatcherTimer timer = new DispatcherTimer { Interval = timeSpan, IsEnabled = true };
            timer.Tick += delegate
            {
                timer.Dispose();
                obj.SetPropretyValue(property, value);
            };
        }
        /// <summary>
        /// 延迟操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timeSpan"></param>
        /// <param name="action"></param>
        /// <param name="count">循环次数</param>
        public static void Delay(this object obj, TimeSpan timeSpan, Action action, uint count = 1)
        {
            if (action == null || count == 0)
            {
                return;
            }
            int c = 0;
            DispatcherTimer timer = new DispatcherTimer { Interval = timeSpan, IsEnabled = true };
            timer.Tick += delegate
            {
                c++;
                if (c >= count)
                {
                    timer.Dispose();
                }
                action();
            };
        }
        /// <summary>
        /// 延迟操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="timeSpan"></param>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T Delay<T>(this T obj, TimeSpan timeSpan, Action<T> action, uint count = 1)
        {
            if (action == null || count == 0)
            {
                return obj;
            }
            int c = 0;
            DispatcherTimer timer = new DispatcherTimer { Interval = timeSpan, IsEnabled = true };
            timer.Tick += delegate
            {
                c++;
                if (c >= count)
                {
                    timer.Dispose();
                }
                action(obj);
            };
            return obj;
        }

        public static string GetCreationCode(this object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            var type = obj.GetType();
            if (type.IsEnum)
            {
                return type.Name + "." + Enum.GetName(type, obj);
            }
            if (obj is string str)
            {
                return $"\"{str.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r")}\"";
            }
            if (obj is Design.ISerializerCode code)
            {
                return code.GetCreationCode();
            }
            if (obj is int || obj is double || obj is byte || obj is short || obj is uint || obj is ulong || obj is decimal || obj is ushort)
            {
                return obj.ToString();
            }
            if (obj is char c)
            {
                if (c == '\'')
                {
                    return "'\\''";
                }
                return "'" + obj.ToString() + "'";
            }
            if (obj is float)
            {
                return ((float)obj).ToString("0.#") + "f";
            }
            if (type.GetMethod("op_Implicit", new Type[] { typeof(string) }) != null)
            {
                return $"\"{obj}\"";
            }
            if (obj is bool)
            {
                return ((bool)obj) ? "true" : "false";
            }
            if (obj is DateTime)
            {
                var date = (DateTime)obj;
                return $"new DateTime({date.Ticks},DateTimeKind.{date.Kind})";
            }
            return $"new {type.Name}()";
        }

        //internal static T ListFirstOrDefault<T>(this IList<T> list, Func<T, bool> func)
        //{
        //    var c = list.Count;
        //    for (int i = 0; i < c; i++)
        //    {
        //        var item = list[i];
        //        if (func(item))
        //        {
        //            return item;
        //        }
        //    }
        //    return default;
        //}

        /// <summary>
        /// 转成IList用于Items属性设置
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IList ToItems(this DataTable dataTable)
        {
            return new DataRows(dataTable);
        }
        /// <summary>
        /// 必须是DataTable转换为IList的才行
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(this IList list)
        {
            if (list is DataRows dataRows)
            {
                return dataRows.DataTable;
            }
            throw new Exception("必须是DataTable.ToItems()获取的IList才能使用GetDataTable()");
        }

        /// <summary>
        /// 转成IList用于Items属性设置
        /// </summary>
        /// <returns></returns>
        public static IList ToItems(this IEnumerable list)
        {
            return new Enumerable(list);
        }
        /// <summary>
        /// 是否大约相等 Math.Abs(f1 - f2) &lt; 0.0001
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool Equal(this float f1, float f2)
        {
            return Math.Abs(f1 - f2) < 0.0001;
        }

        /// <summary>
        /// 对象是否相等
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool Equal(this object obj1, object obj2)
        {
            return (obj1 == obj2) || (obj1 != null && obj1.Equals(obj2));
        }

        /// <summary>
        /// 字符串转具体的数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Parse(this Type type, string value)
        {
            object v = null;
            var op_Implicit = type.GetMethod("op_Implicit", new Type[] { typeof(string) });
            if (op_Implicit != null)
            {
                v = op_Implicit.FastInvoke(null, value);
            }
            else
            {
                var Parse = type.GetMethod("Parse", new Type[] { typeof(string) });
                if (Parse != null)
                {
                    v = Parse.FastInvoke(null, value);
                }
                else
                {
                    v = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
                }
            }
            return v;
        }
        /// <summary>
        /// 针对Grid添加控件元素，并设置所在的单元格位置
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="element"></param>
        /// <param name="col">列</param>
        /// <param name="row">行</param>
        /// <param name="colSpan">跨列</param>
        /// <param name="rowSpan">跨行</param>
        public static T Add<T>(this UIElementCollection elements, T element, int col = 0, int row = 0, int colSpan = 1, int rowSpan = 1) where T : UIElement
        {
            elements.Add(element);
            var grid = elements.Owner as Controls.Grid;
            if (grid != null)
            {
                if (col > 0)
                {
                    Controls.Grid.ColumnIndex(element, col);
                }
                if (row > 0)
                {
                    Controls.Grid.RowIndex(element, row);
                }
                if (colSpan > 1)
                {
                    Controls.Grid.ColumnSpan(element, colSpan);
                }
                if (rowSpan > 1)
                {
                    Controls.Grid.RowSpan(element, rowSpan);
                }
            }
            else
            {
                throw new Exception("不是Grid控件");
            }
            return element;
        }
        /// <summary>
        /// 添加Point
        /// </summary>
        /// <param name="points"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Add(this CPF.Collection<Drawing.Point> points, float x, float y)
        {
            points.Add(new Drawing.Point(x, y));
        }

        public static T[] ToArray<T>(this System.Collections.ObjectModel.ObservableCollection<T> list)
        {
            var a = new T[list.Count];
            list.CopyTo(a, 0);
            return a;
        }
        /// <summary>
        /// 获取附加属性的名称
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <param name="attached"></param>
        /// <returns></returns>
        public static string GetAttachedPropertyName<Value>(this Attached<Value> attached)
        {
            var type = attached.Target.GetType();
            var field = type.GetField("propertyName");
            return field.FastGetValue(attached.Target).ToString();
        }

        internal static readonly HashSet<Type> ConvertTypes = new HashSet<Type>() {
            //typeof(System.Empty),
            //typeof(Object),
            typeof(System.DBNull),
            typeof(Boolean),
            typeof(Char),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
           // typeof(Object), //TypeCode is discontinuous so we need a placeholder.
            typeof(String)
        };

        static Type strType = typeof(string);
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertTo(this object obj, Type type)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                if (ConvertTypes.Contains(type))
                {
                    return Convert.ChangeType(obj, type);
                }
            }
            catch (Exception e)
            {
                throw new Exception(obj.GetType() + "无法转换成" + type, e);
            }
            var vType = obj.GetType();
            if (vType == type || vType.IsSubclassOf(type) || (type.IsInterface && type.IsAssignableFrom(vType)))
            {
                return obj;
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, obj.ToString());
                }
                catch (Exception e)
                {
                    throw new Exception(vType + "无法转换成" + type, e);
                }
            }
            var parse = type.GetMethod("op_Implicit", new Type[] { vType });
            if (parse == null)
            {
                if (type.Name == "Nullable`1")
                {
                    if (vType == strType && "null".Equals(obj))
                    {
                        return null;
                    }
                    //var sType = type.GenericTypeArguments[0];
                    var sType = type.GetGenericArguments()[0];
                    var v = ConvertTo(obj, sType);
                    return type.GetConstructor(new Type[] { sType }).FastInvoke(v);
                }
                throw new Exception(vType + "无法转换成" + type);
            }
            else
            {
                try
                {
                    if (vType == strType && "null".Equals(obj))
                    {
                        return null;
                    }
                    return parse.FastInvoke(null, obj);
                }
                catch (Exception e)
                {
                    throw new Exception(vType + "无法转换成" + type, e);
                }
            }
        }
        /// <summary>
        /// 获取对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropretyValue(this object obj, string propertyName)
        {
            //if (obj == null)
            //{
            //    return null;
            //}
            if (obj is CpfObject cpf)
            {
                if (cpf.HasProperty(propertyName))
                {
                    return cpf.GetValue(propertyName);
                }
            }
            //var p = obj.GetType().GetProperty(propertyName);
            //if (p == null)
            //{
            //    throw new Exception("未找到属性：" + propertyName);
            //}
            //return p.FastGetValue(obj);
            return obj.GetValue(propertyName);
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropretyValue(this object obj, string propertyName, object value)
        {
            var b = obj as CpfObject;
            if (b != null)
            {
                if (!b.SetValue(value, propertyName))
                {
                    b.SetValue(propertyName, value);
                }
            }
            else
            {
                obj.SetValue(propertyName, value);
            }
        }
        //public static void Add(this IList list)
        //{

        //}
        /// <summary>
        /// 定义一个加载动画，你可以将耗时操作放到work委托里，可以异步等待返回一个值。里面可以执行多个分段的任务，并且刷新Message。
        /// var r = await this.ShowLoading("开始加载...",a =>
        ///    {
        ///        System.Threading.Thread.Sleep(1000);
        ///        a.Message = "加载组件1...";
        ///        System.Threading.Thread.Sleep(1000);
        ///        a.Message = "加载组件2...";
        ///        System.Threading.Thread.Sleep(1000);
        ///        return "结果";
        ///    });
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="message"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static async Task<T> ShowLoading<T>(this UIElement root, string message, Func<LoadingBox, T> work)
        {
            var loadingBox = new LoadingBox { Message = message };
            var layer = new LayerDialog
            {
                Name = "loadingDialog",
                Content = loadingBox,
                ShowCloseButton = false,
                Background = null,
            };
            layer.ShowDialog(root);
            return await
#if NET40
                TaskEx
#else
                Task
#endif
                .Run(() =>
                {
                    var r = work(loadingBox);
                    loadingBox.Invoke(layer.CloseDialog);
                    return r;
                });
        }
        /// <summary>
        /// 定义一个加载动画，你可以将耗时操作放到work委托里，可以异步等待返回一个值。里面可以执行多个分段的任务，并且刷新Message。
        ///  await this.ShowLoading("开始加载...",a =>
        ///    {
        ///        System.Threading.Thread.Sleep(1000);
        ///        a.Message = "加载组件1...";
        ///        System.Threading.Thread.Sleep(1000);
        ///        a.Message = "加载组件2...";
        ///        System.Threading.Thread.Sleep(1000);
        ///    });
        /// </summary>
        /// <param name="root"></param>
        /// <param name="message"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static async Task ShowLoading(this UIElement root, string message, Action<LoadingBox> work)
        {
            var loadingBox = new LoadingBox { Message = message };
            var layer = new LayerDialog
            {
                Name = "loadingDialog",
                Content = loadingBox,
                ShowCloseButton = false,
                Background = null,
            };
            layer.ShowDialog(root);
            await
#if NET40
                TaskEx
#else
                Task
#endif
                .Run(() =>
                {
                    work(loadingBox);
                    loadingBox.Invoke(layer.CloseDialog);
                });
        }
        /// <summary>
        /// 循环创建子元素，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panel"></param>
        /// <param name="count">循环次数</param>
        /// <param name="func">循环中的索引，返回创建结果</param>
        /// <returns></returns>
        public static T LoopCreate<T>(this T panel, int count, Func<int, UIElement> func) where T : Panel
        {
            for (int i = 0; i < count; i++)
            {
                var e = func(i);
                if (e != null)
                {
                    panel.Children.Add(e);
                }
            }
            return panel;
        }

        /// <summary>
        /// 循环创建子元素，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panel"></param>
        /// <param name="count">循环次数</param>
        /// <param name="func">循环中的索引，当前容器，返回创建结果</param>
        /// <returns></returns>
        public static T LoopCreate<T>(this T panel, int count, Func<int, T, UIElement> func) where T : Panel
        {
            for (int i = 0; i < count; i++)
            {
                var e = func(i, panel);
                if (e != null)
                {
                    panel.Children.Add(e);
                }
            }
            return panel;
        }
        /// <summary>
        /// 一般用来设置元素属性，初始化阶段比css优先级高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T AfterStyle<T>(this T element, Action<T> action) where T : UIElement
        {
            if (element.Root != null && element.Root.IsInitialized)
            {
                if (element is Control control && control.IsInitialized)
                {
                    action(element);
                }
                else
                {
                    element.afterStyle = action;
                }
            }
            else
            {
                element.afterStyle = action;
            }
            return element;
        }
    }

    class Enumerable : IList
    {
        IEnumerable list;
        public Enumerable(IEnumerable list)
        {
            this.list = list;
        }

        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    class DataRows : IList, INotifyCollectionChanged
    {
        DataTable dataTable;
        Dictionary<DataRow, DataRowObject> rows = new Dictionary<DataRow, DataRowObject>();
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public DataRows(DataTable dataTable)
        {
            this.dataTable = dataTable;
            foreach (DataRow item in dataTable.Rows)
            {
                rows.Add(item, new DataRowObject(item));
            }
            dataTable.RowChanged += DataTable_RowChanged;
            dataTable.RowDeleting += DataTable_RowDeleting;
            dataTable.TableClearing += DataTable_TableClearing;
            dataTable.ColumnChanged += DataTable_ColumnChanged;
            dataTable.RowDeleted += DataTable_RowDeleted;
        }

        private void DataTable_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            rows.Remove(e.Row);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.Row, deleteIndex));
        }

        private void DataTable_TableClearing(object sender, DataTableClearEventArgs e)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void DataTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (rows.TryGetValue(e.Row, out var row))
            {
                row.NotifyPropertyChanged(e.Column.ColumnName);
            }
        }

        int deleteIndex = -1;
        private void DataTable_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            deleteIndex = dataTable.Rows.IndexOf(e.Row);
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Add:
                    int index;
                    if (e.Row != dataTable.Rows[dataTable.Rows.Count - 1])
                    {
                        index = dataTable.Rows.IndexOf(e.Row);
                    }
                    else
                    {
                        index = dataTable.Rows.Count - 1;
                    }
                    var r = new DataRowObject(e.Row);
                    rows.Add(e.Row, r);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, r, index));
                    break;
                case DataRowAction.Change:
                    break;
                case DataRowAction.ChangeCurrentAndOriginal:
                    break;
                case DataRowAction.ChangeOriginal:
                    break;
                case DataRowAction.Commit:
                    break;
                case DataRowAction.Delete:
                    break;
                case DataRowAction.Nothing:
                    break;
                case DataRowAction.Rollback:
                    break;
                default:
                    break;
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Events[nameof(CollectionChanged)]?.Invoke(this, e);
            //if (CollectionChanged != null)
            //{
            //    CollectionChanged(this, e);
            //}
        }

        public DataRowObject this[int index]
        {
            get
            {
                return rows[dataTable.Rows[index]];
            }
            set => throw new NotImplementedException();
        }
        object IList.this[int index] { get { return rows[dataTable.Rows[index]]; } set => throw new NotImplementedException(); }

        public int Count { get { return dataTable.Rows.Count; } }

        public bool IsReadOnly { get { return dataTable.Rows.IsReadOnly; } }

        public bool IsFixedSize { get { return false; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return dataTable.Rows.SyncRoot; } }

        WeakEventHandlerList events;
        /// <summary>
        /// 事件列表，用于优化事件订阅内存
        /// </summary>
        protected WeakEventHandlerList Events
        {
            get
            {
                if (events == null)
                {
                    events = new WeakEventHandlerList();
                }
                return events;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { Events.AddHandler(value); }
            remove { Events.RemoveHandler(value); }
        }

        public void Add(DataRow item)
        {
            dataTable.Rows.Add(item);
        }

        int IList.Add(object value)
        {
            dataTable.Rows.Add((DataRow)value);
            return dataTable.Rows.Count - 1;
        }

        public void Clear()
        {
            dataTable.Rows.Clear();
        }

        public bool Contains(DataRow item)
        {
            return dataTable.Rows.Contains(item.ItemArray);
        }

        public bool Contains(object value)
        {
            return dataTable.Rows.Contains(value);
        }

        public void CopyTo(DataRow[] array, int arrayIndex)
        {
            dataTable.Rows.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            dataTable.Rows.CopyTo(array, index);
        }

        int IList.IndexOf(object value)
        {
            if (value is DataRowObject dataRowObject)
            {
                return dataTable.Rows.IndexOf(dataRowObject.row);
            }
            else
            {
                return dataTable.Rows.IndexOf((DataRow)value);
            }
        }

        public void Insert(int index, DataRow item)
        {
            dataTable.Rows.InsertAt(item, index);
        }

        public void Insert(int index, object value)
        {
            dataTable.Rows.InsertAt((DataRow)value, index);
        }

        public void Remove(DataRow item)
        {
            dataTable.Rows.Remove(item);
        }

        public void Remove(object value)
        {
            dataTable.Rows.Remove((DataRow)value);
        }

        public void RemoveAt(int index)
        {
            dataTable.Rows.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in rows.Values)
            {
                yield return item;
            }
        }
    }

    class DataRowObject : CpfObject
    {
        public DataRow row;
        public DataRowObject(DataRow row)
        {
            this.row = row;
        }

        public override object GetValue([CallerMemberName] string propertyName = null)
        {
            //if (row.Table.Columns.Contains(propertyName))
            //{
            return row[propertyName];
            //}
            //return base.GetValue(propertyName);
        }

        public override bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            //if (row.Table.Columns.Contains(propertyName))
            //{
            row[propertyName] = value;
            return true;
            //}
            //return base.SetValue(value, propertyName);
        }

        //internal void Raise(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = (PropertyChangedEventHandler)Events["INotifyPropertyChanged"];
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public override bool HasProperty(string propertyName)
        {
            return row.Table.Columns.Contains(propertyName);
        }

        public override PropertyMetadataAttribute GetPropertyMetadata(string propertyName)
        {
            //return base.GetPropertyMetadata(propertyName);
            return new PropertyMetadataAttribute() { PropertyName = propertyName, PropertyType = row.Table.Columns[propertyName].DataType };
        }

        public override IEnumerable<PropertyMetadataAttribute> GetProperties()
        {
            List<PropertyMetadataAttribute> list = new List<PropertyMetadataAttribute>();
            foreach (DataColumn item in row.Table.Columns)
            {
                list.Add(new PropertyMetadataAttribute { PropertyName = item.ColumnName, PropertyType = item.DataType, });
            }
            return list;
        }
    }
}