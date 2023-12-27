using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using CPF.Drawing;
using CPF.Platform;
using CPF.Styling;
using CPF.Animation;
using CPF.Reflection;
using CPF.Design;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Concurrent;

namespace CPF.Controls
{
    /// <summary>
    /// 基础视图
    /// </summary>
    [Description("基础视图"), Browsable(false)]
    public abstract class View : Control, ITopLevel
    {
        static ConcurrentDictionary<View, View> views = new ConcurrentDictionary<View, View>();
        /// <summary>
        /// 当前有效的所有视图
        /// </summary>
        public static IEnumerable<View> Views { get => views.Values; }

        IViewImpl viewImpl;
        List<(RadioButtonGroupAttribute group, PropertyInfo property)> radioButtonGroup;
        HybridDictionary<string, RadioButtonGroupAttribute> radioButtonGroupProperty;
        /// <summary>
        /// 这种方式不会调用CreateView();
        /// </summary>
        /// <param name="view"></param>
        public View(IViewImpl view)
        {
            views.TryAdd(this, this);
            IsRoot = true;
            Root = this;
            viewImpl = view;
            viewImpl.PositionChanged = PositionChanged;
            viewImpl.Resized = SizeChanged;
            viewImpl.ScalingChanged = ScalingChanged;
            viewImpl.Activated = Activated;
            viewImpl.Deactivated = Deactivated;
            viewImpl.SetRoot(this);
            var ps = Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in ps)
            {
                var a = item.GetCustomAttributes(typeof(RadioButtonGroupAttribute), true);
                if (a != null && a.Length > 0 && a[0] is RadioButtonGroupAttribute radio)
                {
                    if (radioButtonGroup == null)
                    {
                        radioButtonGroup = new List<(RadioButtonGroupAttribute group, PropertyInfo property)>();
                        radioButtonGroupProperty = new HybridDictionary<string, RadioButtonGroupAttribute>();
                    }
                    radioButtonGroupProperty.Add(item.Name, radio);
                    radioButtonGroup.Add((radio, item));
                }
            }
        }
        public View()
        {
            views.TryAdd(this, this);
            IsRoot = true;
            Root = this;
            viewImpl = CreateView();
            viewImpl.PositionChanged = PositionChanged;
            viewImpl.Resized = SizeChanged;
            viewImpl.ScalingChanged = ScalingChanged;
            viewImpl.Activated = Activated;
            viewImpl.Deactivated = Deactivated;
            viewImpl.SetRoot(this);
            var ps = Type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in ps)
            {
                var a = item.GetCustomAttributes(typeof(RadioButtonGroupAttribute), true);
                if (a != null && a.Length > 0 && a[0] is RadioButtonGroupAttribute radio)
                {
                    if (radioButtonGroup == null)
                    {
                        radioButtonGroup = new List<(RadioButtonGroupAttribute group, PropertyInfo property)>();
                        radioButtonGroupProperty = new HybridDictionary<string, RadioButtonGroupAttribute>();
                    }
                    radioButtonGroupProperty.Add(item.Name, radio);
                    radioButtonGroup.Add((radio, item));
                }
            }
            styles.CollectionChanged += Styles_CollectionChanged;
        }
        protected abstract IViewImpl CreateView();
        //{
        //    return Application.GetRuntimePlatform().CreateView();
        //}
        //internal HybridDictionary<string, UIElement> nameDic = new HybridDictionary<string, UIElement>();
        //public View(IViewImpl viewImpl)
        //{
        //    IsRoot = true;
        //    Root = this;
        //    this.viewImpl = viewImpl;
        //    viewImpl.PositionChanged = PositionChanged;
        //    viewImpl.Resized = SizeChanged;
        //    viewImpl.Deactivated = Deactivated;
        //    viewImpl.SetRoot(this);
        //}
        public IViewImpl ViewImpl { get { return viewImpl; } }


        NamedPipe namedPipe;

        internal void ConnnetDev(string id)
        {
            if (namedPipe != null)
            {
                namedPipe.Dispose();
                namedPipe = null;
            }
            namedPipe = new NamedPipe(id, false);
            namedPipe.AcceptMessage += NamedPipe_AcceptMessage;
            try
            {
                namedPipe.WaitForConnection(2000, true);
            }
            catch (Exception E)
            {
                MessageBox.Show("连接开发者工具失败：" + E.Message);
            }
        }

        private void NamedPipe_AcceptMessage(NamedPipe pipe, string data, bool conn)
        {
            if (!conn)
            {
                pipe.Dispose();
                if (pipe == namedPipe)
                {
                    renderRectElement = null;
                    Invoke(Invalidate);
                    namedPipe = null;
                }
                return;
            }
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            var root = Root;
            var cm = CommandMessage<object>.DeserializeWithString(data);
            if (cm == null)
            {
                return;
            }
            Invoke(() =>
            {
                if (cm.MessageType == MessageType.GetChildren)
                {
                    IList<ElementTreeNode> list;
                    list = GetNodeList(cm.Data == null ? 0 : CommandMessage<long>.DeserializeWithString(data).Data);
                    SendData(new CommandMessage<IList<ElementTreeNode>> { MessageType = MessageType.Children, Data = list });
                }
                else if (cm.MessageType == MessageType.GetProperties)
                {
                    var list = new List<CPFPropertyInfo>();
                    var ele = CommandMessage<UIElement>.GetObject(CommandMessage<long>.DeserializeWithString(data).Data);
                    if (ele != null)
                    {
                        var ps = ele.Type.GetProperties();
                        foreach (var item in ps)
                        {
                            try
                            {
#if NET40
                                if (item.CanRead && item.GetGetMethod().GetParameters().Length == 0)
                                {
                                    var v = item.GetValue(ele, null);
#else
                                if (item.CanRead && item.GetMethod.GetParameters().Length == 0)
                                {
                                    var v = item.GetValue(ele);
#endif
                                    list.Add(new CPFPropertyInfo { Name = item.Name, Value = v == null ? "" : v.ToString(), IsReadOnly = !item.CanWrite, TypeName = item.PropertyType.FullName, GCHandle = ele.GetIntPtr().ToInt64() });
                                }
                                else if (!item.CanRead)
                                {
                                    list.Add(new CPFPropertyInfo { Name = item.Name, Value = "无法读取", IsReadOnly = true, TypeName = item.PropertyType.FullName, GCHandle = ele.GetIntPtr().ToInt64() });
                                }
                            }
                            catch (Exception e)
                            {
                                list.Add(new CPFPropertyInfo { Name = item.Name, Value = e.ToString(), IsReadOnly = true, TypeName = item.PropertyType.FullName, GCHandle = ele.GetIntPtr().ToInt64() });
                            }
                        }
                        if (ele.attachedValues != null)
                        {
                            foreach (var item in ele.attachedValues)
                            {
                                list.Add(new CPFPropertyInfo { Name = item.Key, Value = item.Value == null ? "" : item.Value.ToString(), IsReadOnly = true, TypeName = item.Value != null ? item.Value.GetType().FullName : "", GCHandle = ele.GetIntPtr().ToInt64() });
                            }
                        }

                        SendData(new CommandMessage<List<CPFPropertyInfo>> { MessageType = MessageType.Properties, Data = list });
                    }
                    SetDrawRenderRectangle(ele);
                }
                else if (cm.MessageType == MessageType.FindElement)
                {
                    //var p = CommandMessage<Point>.DeserializeWithString(data).Data;
                    //var bounds = Bounds;
                    var mp = MouseDevice.Location;
                    Point p = new Point(mp.X, mp.Y);
                    var po = Position;
                    p = new Point(p.X - po.X, p.Y - po.Y);
                    var list = root.HitTest(p / LayoutScaling);
                    var e = list.FirstOrDefault();
                    SetDrawRenderRectangle(e);
                    if (e != null)
                    {
                        var parent = e.Parent;
                        List<UIElement> ps = new List<UIElement>();
                        while (parent != null)
                        {
                            ps.Add(parent);
                            parent = parent.Parent;
                        }
                        Collection<ElementTreeNode> nodes = new Collection<ElementTreeNode>();
                        var treeRoot = nodes;
                        var node = CreateNode(root);
                        nodes.Add(node);
                        nodes = node.Nodes;
                        nodes.AddRange(root.GetChildren().Select(CreateNode));
                        for (int i = ps.Count - 2; i >= 0; i--)
                        {
                            var n = ps[i];
                            nodes = nodes.First(a => a.GCHandle == n.GetIntPtr().ToInt64()).Nodes;
                            nodes.AddRange(n.GetChildren().Select(CreateNode));
                        }
                        SendData(new CommandMessage<IList<ElementTreeNode>> { MessageType = MessageType.ElementTree, Data = treeRoot });
                        SendData(new CommandMessage<long> { MessageType = MessageType.ShowSelectNode, Data = e.GetIntPtr().ToInt64() });
                    }
                }
                else if (cm.MessageType == MessageType.ClearEvent)
                {
                    var p = CommandMessage<Collection<ElementTreeNode>>.DeserializeWithString(data).Data;
                    ClearEvent(p);
                }
                else if (cm.MessageType == MessageType.SetValue)
                {
                    var p = CommandMessage<SetPropertyValue>.DeserializeWithString(data).Data;
                    var ui = CommandMessage<UIElement>.GetObject(p.GCHandle);
                    if (ui != null)
                    {
                        try
                        {
                            var type = Type.GetType(p.TypeName);
                            if (type == null)
                            {
                                type = typeof(TextBox).Assembly.GetType(p.TypeName);
                            }
                            var v = p.Value.ConvertTo(type);
                            var pinfo = ui.Type.GetProperty(p.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (pinfo != null && pinfo.GetSetMethod() != null)
                            {
                                pinfo.SetValue(ui, v, null);
                            }
                            else if (ui.HasProperty(p.Name))
                            {
                                ui.SetValue(v, p.Name);
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + (e.InnerException == null ? "" : (":" + e.InnerException)));
                        }
                    }
                }
                else if (cm.MessageType == MessageType.GetPropertyInfo)
                {
                    var p = CommandMessage<GetPropertyInfo>.DeserializeWithString(data).Data;
                    var ui = CommandMessage<UIElement>.GetObject(p.GCHandle);
                    if (ui != null)
                    {
                        try
                        {
                            var type = ui.Type;
                            string info = p.PropertyName;
                            var prop = type.GetProperty(p.PropertyName);
                            if (prop != null)
                            {
                                info += "\n" + prop.PropertyType.FullName;
                                var pm = ui.GetPropertyMetadata(p.PropertyName);
                                if (pm != null)
                                {
                                    info += "\n" + "依赖属性";

                                    var pv = ui.GetEffectiveValue(pm);
                                    if (pv != null)
                                    {
                                        if (pv.AnimationValue != null && pv.AnimationValue.Count > 0)
                                        {
                                            //var value = pv.AnimationValue[pv.AnimationValue.Count - 1];
                                            info += "\n" + "来自动画";
                                        }
                                        else if (pv.TriggerValue != null && pv.TriggerValue.Count > 0)
                                        {
                                            var value = pv.TriggerValue[pv.TriggerValue.Count - 1];
                                            info += "\n" + "来自触发器";
                                            if (value.Trigger != null && value.Trigger.Style != null)
                                            {
                                                info += "\n" + "来自样式";
                                                info += "\n" + "行号：" + value.Trigger.Style.Line;
                                                if (!string.IsNullOrWhiteSpace(value.Trigger.Style.Url))
                                                {
                                                    info += "\n" + "Url：" + value.Trigger.Style.Url;
                                                }
                                            }
                                        }
                                        else if (pv.styleValues != null && pv.styleValues.Count > 0)
                                        {
                                            var value = pv.styleValues[pv.styleValues.Count - 1];
                                            info += "\n" + "来自样式";
                                            if (value.StyleValue != null && value.StyleValue != null)
                                            {
                                                info += "\n" + "行号：" + value.StyleValue.Style.Line;
                                                if (!string.IsNullOrWhiteSpace(value.StyleValue.Style.Url))
                                                {
                                                    info += "\n" + "Url：" + value.StyleValue.Style.Url;
                                                }
                                            }
                                        }
                                        else if (pv.LocalValue.HasValue)
                                        {
                                            info += "\n" + "来自属性赋值";
                                        }
                                    }
                                    else
                                    {
                                        info += "\n" + "来自默认值或者继承父元素值";
                                    }
                                }
                                else
                                {
                                    info += "\n" + "普通属性";
                                }
                            }
                            else if (p.PropertyName.Contains("."))
                            {
                                info += "\n附加属性";
                            }
                            SendData(new CommandMessage<string> { MessageType = MessageType.GetPropertyInfo, Data = info });
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + (e.InnerException == null ? "" : (":" + e.InnerException)));
                        }
                    }
                }
            });
        }

        void SendData<T>(T data)
        {
            var by = CommandMessage<T>.SerializeToString(data);
            if (namedPipe != null)
            {
                namedPipe.WriteString(by);
            }
        }

        void GetNodeTree(ElementTreeNode node, UIElement element)
        {
            node.Name = element.Type.ToString();
            var name = element.Name;
            if (!string.IsNullOrEmpty(name))
            {
                node.Name = node.Name + "[" + name + "]";
            }
            node.Visibility = element.Visibility;
            node.GCHandle = element.GetIntPtr().ToInt64();
            node.Nodes = new Collection<ElementTreeNode>();
            foreach (var item in element.GetChildren())
            {
                var n = new ElementTreeNode();
                GetNodeTree(n, item);
                node.Nodes.Add(n);
            }
        }

        ElementTreeNode CreateNode(UIElement element)
        {
            return new ElementTreeNode
            {
                GCHandle = element.GetIntPtr().ToInt64(),
                Name = GetName(element),
                Visibility = element.Visibility,
                Nodes = new Collection<ElementTreeNode>()
            };
        }
        IList<ElementTreeNode> GetNodeList(long GCHandle)
        {
            if (GCHandle != 0)
            {
                UIElement el = CommandMessage<UIElement>.GetObject(GCHandle);
                if (el != null)
                {
                    el.UIElementAdded -= El_UIElementAdded;
                    el.UIElementAdded += El_UIElementAdded;
                    el.UIElementRemoved -= El_UIElementRemoved;
                    el.UIElementRemoved += El_UIElementRemoved;
                    return el.GetChildren().Select(CreateNode).ToList();
                }
                else
                {
                    return new ElementTreeNode[0];
                }
            }
            else
            {
                return new List<ElementTreeNode>() { CreateNode(Root) };
            }
        }
        //IntPtr target;
        private void El_UIElementAdded(object sender, UIElementAddedEventArgs e)
        {
            SendData(new CommandMessage<ValueTuple<long, ElementTreeNode>>
            {
                MessageType = MessageType.AddChild,
                Data = ((sender as UIElement).GetIntPtr().ToInt64(),
                new ElementTreeNode
                {
                    GCHandle = e.Element.GetIntPtr().ToInt64(),
                    Name = GetName(e.Element),
                    Visibility = e.Element.Visibility,
                    Nodes = new Collection<ElementTreeNode>()
                })
            });
        }
        private void El_UIElementRemoved(object sender, UIElementRemovedEventArgs e)
        {
            SendData(new CommandMessage<ValueTuple<long, ElementTreeNode>>
            {
                MessageType = MessageType.RemoveChild,
                Data = ((sender as UIElement).GetIntPtr().ToInt64(),
                new ElementTreeNode
                {
                    GCHandle = e.Element.GetIntPtr().ToInt64(),
                    Name = GetName(e.Element),
                    Visibility = e.Element.Visibility,
                    Nodes = new Collection<ElementTreeNode>()
                })
            });
        }

        void ClearEvent(Collection<ElementTreeNode> nodes)
        {
            foreach (var item in nodes)
            {
                UIElement el = CommandMessage<UIElement>.GetObject(item.GCHandle);
                el.UIElementAdded -= El_UIElementAdded;
                el.UIElementRemoved -= El_UIElementRemoved;
                ClearEvent(item.Nodes);
            }
        }

        string GetName(UIElement element)
        {
            var Name = element.Type.ToString();
            var name = element.Name;
            if (!string.IsNullOrEmpty(name))
            {
                Name = Name + "[" + name + "]";
            }
            return Name;
        }

        UIElement renderRectElement;
        internal void SetDrawRenderRectangle(UIElement element)
        {
            if (element != null && element.Root == Root)
            {
                renderRectElement = element;
            }
            else
            {
                renderRectElement = null;
            }
            Root.Invalidate();
        }

        #region InputManage
        InputManager current;

        public InputManager InputManager
        {
            get
            {
                if (current == null)
                {
                    current = new InputManager(this);
                }
                return current;
            }
        }
        #endregion
        /// <summary>
        /// 渲染缩放比例
        /// </summary>
        public virtual float RenderScaling { get { return viewImpl.RenderScaling; } }
        /// <summary>
        /// 布局坐标缩放，用于窗体在屏幕和鼠标位置的定位
        /// </summary>
        public virtual float LayoutScaling { get { return viewImpl.LayoutScaling; } }

        public bool CanActivate { get { return viewImpl.CanActivate; } set { viewImpl.CanActivate = value; } }

        internal bool offset;
        protected virtual void PositionChanged(PixelPoint pos)
        {
            //System.Diagnostics.Debug.WriteLine(pos);
            var scr = Screen;
            var p = new Point(pos.X - scr.WorkingArea.X, pos.Y - scr.WorkingArea.Y);
            this.VisualOffset = p / ViewImpl.LayoutScaling;
            offset = true;
            MarginLeft = VisualOffset.X;
            MarginTop = VisualOffset.Y;
            MarginBottom = "auto";
            MarginRight = "auto";
            Position = pos;
            offset = false;
        }
        [PropertyChanged(nameof(Position))]
        void OnPositionChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!offset)
            {
                viewImpl.Position = (PixelPoint)newValue;
            }
        }

        /// <summary>
        /// 视图位置，像素坐标
        /// </summary>
        public PixelPoint Position { get { return GetValue<PixelPoint>(); } set { SetValue(value); } }

        void SizeChanged(Size size)
        {
            //System.Diagnostics.Debug.WriteLine(size + ":" + ActualSize);
            if (ActualSize != size)
            {
                Width = size.Width;
                Height = size.Height;
            }
        }
        void ScalingChanged()
        {
            InvalidateMeasure();
            Invalidate();
        }
        void Activated()
        {
            //if (this is Popup)
            //{
            //    Debug.WriteLine("Activated");
            //}
            if (!IsKeyboardFocusWithin)
            {
                Focus();
            }
        }

        void Deactivated()
        {
            //if (this is Popup)
            //{
            //    Debug.WriteLine("Deactivated");
            //}
            var fe = InputManager.KeyboardDevice.FocusedElement;
            if (fe != null)
            {
                fe.InnerLostFocus();
                var p = fe.Parent;
                while (p != null)
                {
                    p.InnerLostFocus();
                    p = p.Parent;
                }
                InputManager.KeyboardDevice.SetFocus(null);
            }
            InputManager.MouseDevice.Capture(null);
            //else
            //{
            //    this.InnerLostFocus();
            //}
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            InputManager.MouseDevice.Capture(null);
            if (viewImpl.CanActivate)
            {
                viewImpl.Activate();
            }
            base.OnGotFocus(e);
        }

        //public DpiScale GetDpi()
        //{
        //    return viewImpl.GetDpi();
        //}
        public override Point PointToScreen(Point point)
        {
            return viewImpl.PointToScreen(point);
        }

        public void Invalidate(in Rect rect)
        {
            viewImpl.Invalidate(rect);
        }

        Cursor cursor;
        public void SetCursor(Cursor cursor)
        {
            if (cursor != this.cursor)
            {
                this.cursor = cursor;
                viewImpl.SetCursor(cursor);
            }
        }

        internal void Capture()
        {
            viewImpl.Capture();
        }

        LayoutManager layoutManager;
        public LayoutManager LayoutManager
        {
            get
            {
                if (layoutManager == null)
                {
                    layoutManager = new LayoutManager(this);
                }
                return layoutManager;
            }
        }

        internal void ReleaseCapture()
        {
            viewImpl.ReleaseCapture();
        }

        public void InvalidateMeasure(UIElement control)
        {
            //if (IsDisposed || IsDisposing)
            //{
            //    return;
            //}
            LayoutManager.InvalidateMeasure(control);
        }

        public void InvalidateArrange(UIElement control)
        {
            //if (IsDisposed || IsDisposing)
            //{
            //    return;
            //}
            LayoutManager.InvalidateArrange(control);
        }


        // public bool EnabledIME { get { return viewImpl.EnabledIME; } set { viewImpl.EnabledIME = value; } }

        public void SetIMEEnable(bool value)
        {
            viewImpl.SetIMEEnable(value);
        }

        //public void SetIMEPosition(Point point)
        //{
        //    viewImpl.SetIMEPosition(point);
        //}

        /// <summary>
        /// 渲染整个视图
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rect">剪辑区域</param>
        public void RenderView(DrawingContext dc, Rect rect)
        {
            //LayoutManager.ExecuteLayoutPass();
            var scaling = this.RenderScaling;
            Matrix old = dc.Transform;
            Matrix m = old;
            m.ScalePrepend(scaling, scaling);
            dc.Transform = m;
            viewRect = new Rect(new Point(), ActualSize);
            var rc = new Rect(rect.Location / scaling, rect.Size / scaling);
            dc.PushClip(rc);
            dc.Clear(Color.Transparent);
            RenderUIElement(dc, LayoutManager.VisibleUIElements, rc);
            dc.PopClip();
            dc.Transform = old;

            if (renderRectElement != null)
            {
                dc.PushClip(rect);
                var rect1 = renderRectElement.GetContentBounds();
                var points = new List<Point>();
                points.Add(new Point());
                points.Add(new Point((float)Math.Round(rect1.Width * scaling) / scaling, 0));
                points.Add(new Point((float)Math.Round(rect1.Width * scaling) / scaling, (float)Math.Round(rect1.Height * scaling) / scaling));
                points.Add(new Point(0, (float)Math.Round(rect1.Height * scaling) / scaling));
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = renderRectElement.TransformPoint(points[i]);
                }
                var p = renderRectElement.Parent;
                while (p != null && !p.IsRoot)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        points[i] = p.TransformPoint(points[i]);
                    }
                    p = p.Parent;
                }

                using (var brush = new SolidColorBrush(Color.FromRgb(255, 0, 0)))
                {
                    var stroke = new Stroke(1);
                    for (int i = 0; i < points.Count; i++)
                    {
                        dc.DrawLine(stroke, brush, points[i] * scaling, points[i == 3 ? 0 : (i + 1)] * scaling);
                    }
                }

                dc.PopClip();
            }
        }

        Rect viewRect;
        public void RenderUIElement(DrawingContext dc, VisibleUIElement element, in Rect r, in bool drawEffect = false, in Point effectOffset = default(Point))
        {
            var ele = element.Element;
            if (ele.Visibility != Visibility.Visible || ele.Root == null)
            {
                return;
            }
            if (r.IntersectsWith(ele.RenderBounds))
            {
                //var old = dc.AntialiasMode;
                dc.AntialiasMode = ele.IsAntiAlias ? AntialiasMode.AntiAlias : AntialiasMode.Default;
                if (!drawEffect)
                {
                    var effect = ele.Effect;
                    if (effect != null)
                    {
                        var s = ele.RenderBounds;
                        s.Intersect(viewRect);
                        var ss = new Size((s.Width + 2) * RenderScaling, (s.Height + 2) * RenderScaling);
                        var off = new Point((s.X - 1) * RenderScaling, (s.Y - 1) * RenderScaling);
                        using (var effectBitmap = new Bitmap((int)ss.Width, (int)ss.Height))
                        {
                            using (var effectDc = DrawingContext.FromBitmap(effectBitmap))
                            {
                                effectDc.Clear(Color.Transparent);
                                var rb = ele.RenderBounds;
                                rb = new Rect(new Point(rb.X - s.X - 1, rb.Y - s.Y - 1) * RenderScaling, new Size(rb.Width + 2, rb.Height + 2) * RenderScaling);
                                effectDc.PushClip(rb);
                                effectDc.AntialiasMode = dc.AntialiasMode;
                                var old = dc.Transform;
                                var eff = old;
                                eff.Translate(-off.X + effectOffset.X, -off.Y + effectOffset.Y);
                                effectDc.Transform = eff;
                                //element.Element.Render(dcc);
                                RenderUIElement(effectDc, element, r, true, off);
                                var offset = Matrix.Identity;
                                offset.Translate(off.X - effectOffset.X, off.Y - effectOffset.Y);
                                //effectOffset = off;
                                dc.Transform = offset;
                                dc.PushClip(new Rect(0, 0, ss.Width, ss.Height));
                                effect.DoEffect(dc, effectBitmap);
                                //dc.DrawImage(effectBitmap, rb, rb);
                                dc.PopClip();
                                dc.Transform = old;
                                effectDc.PopClip();
                            }
                        }
                        return;
                    }
                }
                ele.Render(dc);
            }
            //foreach (var item in element.Children)
            var length = element.Children.Count;
            for (int i = 0; i < length; i++)
            {
                var item = element.Children[i];
                Rect rect = item.Element.GetContentBounds();
                Matrix old = dc.Transform;
                Matrix m = old;
                var op = item.Element.RenderTransformOrigin;
                m.TranslatePrepend(rect.X + op.X.GetActualValue(rect.Width), rect.Y + op.Y.GetActualValue(rect.Height));
                m.Prepend(item.Element.RenderTransform.Value);
                m.TranslatePrepend(-op.X.GetActualValue(rect.Width), -op.Y.GetActualValue(rect.Height));
                dc.Transform = m;
                if (item.Element.ClipToBounds)
                {
                    dc.PushClip(new Rect(0, 0, rect.Width, rect.Height));
                    //item.Element.Render(dc);
                    RenderUIElement(dc, item, r, false, effectOffset);
                    dc.PopClip();
                }
                else
                {
                    RenderUIElement(dc, item, r, false, effectOffset);
                }

                dc.Transform = old;
            }
        }

        public override bool HitTestCore(Point point)
        {
            if (VisualClip == null)
            {
                return new Rect(ActualOffset, ActualSize).Contains(point);
            }
            else
            {
                Point offset = VisualOffset;
                point.Offset(-offset.X, -offset.Y);
                return VisualClip.Contains(point);
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Visibility), new UIPropertyMetadataAttribute(Visibility.Collapsed, UIPropertyOptions.AffectsArrange));
            overridePropertys.Override(nameof(MarginBottom), new PropertyMetadataAttribute(typeof(FloatField), "Auto"));
            overridePropertys.Override(nameof(MarginLeft), new PropertyMetadataAttribute(typeof(FloatField), "Auto"));
            overridePropertys.Override(nameof(MarginRight), new PropertyMetadataAttribute(typeof(FloatField), "Auto"));
            overridePropertys.Override(nameof(MarginTop), new PropertyMetadataAttribute(typeof(FloatField), "Auto"));
            //overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender));
            //overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender));
        }

        //[PropertyChanged(nameof(Visibility))]
        //void RegisterVisibility(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        //{
        //    viewImpl.SetVisible((Visibility)newValue == Visibility.Visible);
        //}

        [PropertyChanged(nameof(MarginBottom))]
        [PropertyChanged(nameof(MarginLeft))]
        [PropertyChanged(nameof(MarginRight))]
        [PropertyChanged(nameof(MarginTop))]
        void RegisterMargin(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!offset)
            {
                if ((!MarginTop.IsAuto && !MarginBottom.IsAuto) || (!MarginLeft.IsAuto && !MarginRight.IsAuto))
                {
                    InvalidateMeasure();
                }
                else
                {
                    InvalidateArrange();
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            if (propertyName == nameof(Visibility))
            {
                InputManager.TouchDevice.ClearPoints();
                viewImpl.SetVisible((Visibility)newValue == Visibility.Visible);
            }
            if (!onChecked && radioButtonGroupProperty != null && radiobuttons != null && radioButtonGroupProperty.TryGetValue(propertyName, out var group))
            {
                if (newValue == null)
                {
                    foreach (var item in radiobuttons)
                    {
                        if (item.GroupName == group.GroupName)
                        {
                            item.IsChecked = false;
                        }
                    }
                }
                else
                {
                    foreach (var item in radiobuttons)
                    {
                        if (item.GroupName == group.GroupName)
                        {
                            item.IsChecked = (group.ValueFromDataContext && item.DataContext.Equal(newValue)) || (!group.ValueFromDataContext && item.Content.Equal(newValue));
                        }
                    }
                }
            }
            //else if (propertyName == nameof(MarginBottom) || propertyName == nameof(MarginLeft) || propertyName == nameof(MarginRight) || propertyName == nameof(MarginTop))
            //{
            //    if (!offset)
            //    {
            //        if ((!MarginTop.IsAuto && !MarginBottom.IsAuto) || (!MarginLeft.IsAuto && !MarginRight.IsAuto))
            //        {
            //            InvalidateMeasure();
            //        }
            //        else
            //        {
            //            InvalidateArrange();
            //        }
            //    }
            //}
        }

        List<RadioButton> radiobuttons;
        /// <summary>
        /// 当前视图内RadioButton的值
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public object GetRadioButtonValue(string GroupName)
        {
            if (radiobuttons != null)
            {
                return radiobuttons.Where(a => a.GroupName == GroupName && a.IsChecked == true).Select(a => a.Content).FirstOrDefault();
            }
            return null;
        }

        bool onChecked = false;
        internal void CheckedRadioButton(RadioButton radioButton)
        {
            onChecked = true;
            if (radiobuttons != null)
            {
                var gn = radioButton.GroupName;
                foreach (var item in radiobuttons.Where(a => a.GroupName == gn && a.IsChecked == true && a != radioButton))
                {
                    item.IsChecked = false;
                }
                if (radioButtonGroup != null)
                {
                    foreach (var item in radioButtonGroup)
                    {
                        if (item.group.GroupName == gn)
                        {
                            if (item.group.ValueFromDataContext)
                            {
                                item.property.FastSetValue(this, radioButton.DataContext);
                            }
                            else
                            {
                                item.property.FastSetValue(this, radioButton.Content);
                            }
                        }
                    }
                }
            }
            onChecked = false;
        }
        internal void RegisterRadioButton(RadioButton radioButton)
        {
            if (radiobuttons == null)
            {
                radiobuttons = new List<RadioButton>();
            }
            if (radioButton.IsChecked == true)
            {
                if (radioButtonGroup != null)
                {
                    var gn = radioButton.GroupName;
                    foreach (var item in radioButtonGroup)
                    {
                        if (item.group.GroupName == gn)
                        {
                            if (item.group.ValueFromDataContext)
                            {
                                item.property.FastSetValue(this, radioButton.DataContext);
                            }
                            else
                            {
                                item.property.FastSetValue(this, radioButton.Content);
                            }
                        }
                    }
                }
            }
            radiobuttons.Add(radioButton);
        }

        internal void UnRegisterRadioButton(RadioButton radioButton)
        {
            if (radioButton.IsChecked == true)
            {
                if (radioButtonGroup != null)
                {
                    var gn = radioButton.GroupName;
                    foreach (var item in radioButtonGroup)
                    {
                        if (item.group.GroupName == gn)
                        {
                            //item.property.FastSetValue(this, null);
                        }
                    }
                }
            }
            if (radiobuttons != null)
            {
                radiobuttons.Remove(radioButton);
            }
        }
        StyleSheet styleSheet;
        ///// <summary>
        ///// 解析了的css样式，这里不能动态切换样式
        ///// </summary>
        //public StyleSheet StyleSheet
        //{
        //    get
        //    {
        //        if (styleSheet == null)
        //        {
        //            styleSheet = new StyleSheet();
        //        }
        //        return styleSheet;
        //    }
        //}
        Styles styles = new Styles();
        /// <summary>
        /// 样式集合
        /// </summary>
        [NotCpfProperty]
        public Styles Styles
        {
            get { return styles; }
        }
        /// <summary>
        /// 清除原来样式，拷贝view样式过来
        /// </summary>
        /// <param name="view"></param>
        public void LoadStyle(View view)
        {
            isLoadStyle = true;
            this.ClearStyleValues();
            if (this.triggers != null)
            {
                foreach (var trigger in this.triggers.Where(a => a.Style != null).ToArray())
                {
                    this.triggers.Remove(trigger);
                }
            }
            foreach (var item in Find<UIElement>())
            {
                item.ClearStyleValues();
                if (item.triggers != null)
                {
                    foreach (var trigger in item.triggers.Where(a => a.Style != null).ToArray())
                    {
                        item.triggers.Remove(trigger);
                    }
                }
            }
            styles.Clear();
            styles.AddRange(view.styles);
            if (this.animations != null)
            {
                this.animations.Clear();
            }
            else if (view.animations != null)
            {
                this.animations = new HybridDictionary<string, KeyframesRule>();
            }

            if (view.animations != null)
            {
                foreach (var item in view.animations)
                {
                    animations.Add(item);
                }
            }
            styles.Update();
            foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
            {
                SetStyle(item);
            }

            isLoadStyle = false;
        }

        /// <summary>
        /// css样式，比如 Button{Background:rgb(0,142,244);} #Test{Background:rgb(0,142,244);}
        /// </summary>
        /// <param name="css"></param>
        /// <param name="append">附加还是替换</param>
        /// <param name="url">用于开发者工具记录的文件路径，可以不设置</param>
        public void LoadStyle(string css, bool append = false, string url = null)
        {
            if (!string.IsNullOrEmpty(css))
            {
                css = css.TrimStart((char)65279);
            }
            var parser = new Parser();
            if (append && styleSheet != null)
            {
                var s = parser.Parse(css, url);
                //foreach (var item in s.KeyframeDirectives)
                //{
                //    animations.Add(item.Identifier, item);
                //}
                //foreach (var style in s.StyleRules)
                //{
                //    SetStyle(this, style);
                //}
                //foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                //{
                //    foreach (var style in s.StyleRules)
                //    {
                //        SetStyle(item, style);
                //    }
                //}
                if (s.KeyframeDirectives.Count() > 0 && animations == null)
                {
                    animations = new HybridDictionary<string, KeyframesRule>();
                }
                if (animations != null)
                {
                    foreach (var item in s.KeyframeDirectives)
                    {
                        animations.Add(item.Identifier, item);
                    }
                }

                ConvertStyle(s);

                foreach (var item in s.Errors)
                {
                    styleSheet.Errors.Add(item);
                }
                foreach (var item in s.Rules)
                {
                    styleSheet.Rules.Add(item);
                }
            }
            else
            {
                isLoadStyle = true;
                var ss = parser.Parse(css, url);
                if (styles.Count > 0)
                {
                    this.ClearStyleValues();
                    if (this.triggers != null)
                    {
                        foreach (var trigger in this.triggers.Where(a => a.Style != null).ToArray())
                        {
                            this.triggers.Remove(trigger);
                        }
                    }
                    foreach (var item in Find<UIElement>())
                    {
                        item.ClearStyleValues();
                        if (item.triggers != null)
                        {
                            foreach (var trigger in item.triggers.Where(a => a.Style != null).ToArray())
                            {
                                item.triggers.Remove(trigger);
                            }
                        }
                    }
                }
                styles.Clear();
                styleSheet = ss;
                if (styleSheet.KeyframeDirectives.Count() > 0 && animations == null)
                {
                    animations = new HybridDictionary<string, KeyframesRule>();
                }
                if (animations != null)
                {
                    animations.Clear();
                    foreach (var item in styleSheet.KeyframeDirectives)
                    {
                        animations.Add(item.Identifier, item);
                    }
                }

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //foreach (var style in styleSheet.StyleRules)
                //{
                //    SetStyle(this, style);
                //}
                //foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                //{
                //    foreach (var style in styleSheet.StyleRules)
                //    {
                //        SetStyle(item, style);
                //    }
                //}
                //Debug.WriteLine(stopwatch.ElapsedMilliseconds);

                ConvertStyle(styleSheet);

                styles.Update();
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                {
                    SetStyle(item);
                    //foreach (var style in Styles)
                    //{
                    //    SetStyle(item, style);
                    //}
                }
                //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                //Debug.WriteLine("个数" + Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized).Count());

            }
            if (styleSheet != null)
            {
                foreach (var item in styleSheet.FontFaceDirectives)
                {
                    if (item.Src.StartsWith("url("))
                    {
                        Application.LoadFont(item.Src.Substring(4, item.Src.Length - 5), item.FontFamily.Trim('\'', '"'));
                    }
                    else
                    {
                        Application.LoadFont(item.Src, item.FontFamily.Trim('\'', '"'));
                    }
                }
            }
            isLoadStyle = false;
        }

        private void ConvertStyle(StyleSheet styleSheet)
        {
            foreach (var item in styleSheet.StyleRules)
            {
                var selector2 = item.Selector;
                SelectorRelation selector = null;
                selector = CreateSelector(selector2, selector);
                if (selector is Selector selector1)
                {
                    Style style = new Style(selector1);
                    style.Line = item.Line;
                    style.Url = styleSheet.Url;
                    //style.StyleRule = item;
                    foreach (var pv in item.Declarations)
                    {
                        if (pv.Term == null)
                        {
                            Debug.WriteLine(styleSheet.Url + " 在第" + pv.Line + "行:" + pv.Name + "的值无效");
                            continue;
                        }
                        var lower = pv.Name.ToLower();
                        switch (lower)
                        {
                            case "animation-name":
                                style.animation_name = pv.Term.GetValue();
                                break;
                            case "animation-duration":
                                {
                                    var c = pv.Term.GetValue().ToLower();
                                    try
                                    {
                                        if (c.EndsWith("ms"))
                                        {
                                            style.AnimationDuration = TimeSpan.FromMilliseconds(double.Parse(c.TrimEnd('s', 'm')));
                                        }
                                        else if (c.EndsWith("s"))
                                        {
                                            style.AnimationDuration = TimeSpan.FromSeconds(double.Parse(c.TrimEnd('s')));
                                        }
                                        else
                                        {
                                            style.AnimationDuration = TimeSpan.FromMilliseconds(double.Parse(c));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("不支持的时间格式:" + c + "，如果是俄语环境，请尝试 System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(\"en-US\");", e);
                                    }

                                    break;
                                }
                            case "animation-iteration-count":
                                {
                                    var c = pv.Term.GetValue();
                                    if (c == "infinite")
                                    {
                                        style.AnimationIterationCount = 0;
                                    }
                                    else
                                    {
                                        if (uint.TryParse(c, out var animation_iteration_count))
                                        {
                                            style.AnimationIterationCount = animation_iteration_count;
                                        }
                                        else
                                        {
                                            style.AnimationIterationCount = 1;
                                        }
                                    }

                                    break;
                                }
                            case "animation-timing-function":
                                break;
                            case "animation-fill-mode":
                                {
                                    var c = pv.Term.GetValue();
                                    if (c == "none")
                                    {
                                        style.AnimationEndBehavior = EndBehavior.Recovery;
                                    }
                                    else if (c == "forwards")
                                    {
                                        style.AnimationEndBehavior = EndBehavior.Reservations;
                                    }
                                    else
                                    {
                                        throw new Exception("不支持animation-fill-mode的" + c);
                                    }
                                    break;
                                }
                            default:
                                style.Setters.Add(pv.Name, pv.Term.GetValue(), pv.Important);
                                break;
                        }
                    }
                    styles.Add(style);
                }
            }
        }

        bool isLoadStyle;
        private void Styles_CollectionChanged(object sender, CollectionChangedEventArgs<Style> e)
        {
            if (isLoadStyle)
            {
                if (e.Action == CollectionChangedAction.Add || e.Action == CollectionChangedAction.Replace)
                {
                    e.NewItem.Index = e.Index;
                }
                return;
            }
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.Index = e.Index;
                    SetStyle(this, e.NewItem);
                    foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                    {
                        SetStyle(item, e.NewItem);
                    }
                    break;
                case CollectionChangedAction.Remove:
                    this.ClearStyleValue(e.OldItem);
                    if (this.triggers != null)
                    {
                        foreach (var trigger in this.triggers.Where(a => a.Style == e.OldItem).ToArray())
                        {
                            this.triggers.Remove(trigger);
                        }
                    }
                    foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                    {
                        item.ClearStyleValue(e.OldItem);
                        if (item.triggers != null)
                        {
                            foreach (var trigger in item.triggers.Where(a => a.Style == e.OldItem).ToArray())
                            {
                                item.triggers.Remove(trigger);
                            }
                        }
                    }
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.Index = e.Index;
                    this.ClearStyleValue(e.OldItem);
                    if (this.triggers != null)
                    {
                        foreach (var trigger in this.triggers.Where(a => a.Style == e.OldItem).ToArray())
                        {
                            this.triggers.Remove(trigger);
                        }
                    }
                    foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                    {
                        item.ClearStyleValue(e.OldItem);
                        if (item.triggers != null)
                        {
                            foreach (var trigger in item.triggers.Where(a => a.Style == e.OldItem).ToArray())
                            {
                                item.triggers.Remove(trigger);
                            }
                        }
                    }
                    SetStyle(this, e.NewItem);
                    foreach (var item in Find<UIElement>().Where(a => !(a is Control control) || control.IsInitialized))
                    {
                        SetStyle(item, e.NewItem);
                    }
                    break;
                case CollectionChangedAction.Sort:
                    break;
                default:
                    break;
            }
        }

        internal void SetStyle(UIElement item)
        {
            if (styles.typeNameStyles.TryGetValue(item.Type.Name, out var list))
            {
                foreach (var style in list)
                {
                    SetStyle(item, style);
                }
            }
            if (!string.IsNullOrWhiteSpace(item.Name) && styles.nameStyles.TryGetValue(item.Name, out var list1))
            {
                foreach (var style in list1)
                {
                    SetStyle(item, style);
                }
            }
            if (item.classes != null && item.classes.Count > 0)
            {
                foreach (var c in item.classes)
                {
                    if (styles.classStyles.TryGetValue(c, out var list2))
                    {
                        foreach (var style in list2)
                        {
                            SetStyle(item, style);
                        }
                    }
                }
            }
            foreach (var style in styles.other)
            {
                SetStyle(item, style);
            }
        }

        private void SetStyle(UIElement item, Style style)
        {
            if (style.Selector is OrSelector or)
            {
                foreach (var selectItem in or.Selectors)
                {
                    SelectStyle(item, style, selectItem);
                }
            }
            else
            {
                SelectStyle(item, style, style.Selector);
            }
        }

        private void SelectStyle(UIElement item, Style style, Selector selector)
        {
            PropertyEqualsSelector propertyEquals = null;
            UIElement peElement = null;
            if (IsSelect(selector, item, ref propertyEquals, ref peElement))//style.Selector.Select(item)
            {
                var prev = selector.Prev;
                if (prev == null)
                {
                    //System.Diagnostics.Debug.WriteLine(item);
                    if (propertyEquals != null)
                    {
                        AddTrigger(peElement, style, propertyEquals, selector, item);
                    }
                    else
                    {
                        SetValue(item, style);
                    }
                    //continue;
                }
                else
                {
                    var element = item;
                    while (prev != null)
                    {
                        if (prev is Selector selector1)
                        {
                            if (IsSelect(selector1, element, ref propertyEquals, ref peElement))//selector1.Select(element)
                            {
                                if (prev.Prev == null)
                                {
                                    //System.Diagnostics.Debug.WriteLine(element);
                                    if (propertyEquals != null)
                                    {
                                        AddTrigger(peElement, style, propertyEquals, selector, item);
                                    }
                                    else
                                    {
                                        SetValue(item, style);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (prev is ChildSelector)
                        {
                            element = element.Parent;
                        }
                        else
                        {
                            var p = element.Parent;
                            var select = prev.Prev as Selector;
                            while (p != null)
                            {
                                if (IsSelect(select, p, ref propertyEquals, ref peElement))//select.Select(p)
                                {
                                    if (select.Prev == null)
                                    {
                                        //System.Diagnostics.Debug.WriteLine(element);
                                        if (propertyEquals != null)
                                        {
                                            AddTrigger(peElement, style, propertyEquals, selector, item);
                                        }
                                        else
                                        {
                                            SetValue(item, style);
                                        }
                                    }
                                    element = p;
                                    break;
                                }
                                p = p.Parent;
                            }
                            if (p == null)
                            {
                                break;
                            }
                            prev = prev.Prev;
                        }
                        prev = prev.Prev;
                    }
                }

            }
        }

        bool IsSelect(Selector selector, UIElement element, ref PropertyEqualsSelector propertyEquals, ref UIElement peElement)
        {
            if (selector is PropertyEqualsSelector property)
            {
                propertyEquals = property;
                peElement = element;
                if (property.HasDot)
                {
                    return true;
                }
                var r = element.GetPropertyMetadata(property.Property);
                if (r != null)
                {
                    if (!(property.Prev is Selector selector1 && selector1.Select(element)))
                    {
                        return false;
                    }
                    if (!property.HasValue && !string.IsNullOrEmpty(property.ValueString))
                    {
                        property.HasValue = true;
                        property.Value = property.ValueString.ConvertTo(r.PropertyType);
                    }

                    return true;
                }
                return false;
            }
            return selector.Select(element);
        }

        private static SelectorRelation CreateSelector(BaseSelector selector2, SelectorRelation selector)
        {
            if (selector2 is SimpleSelector simple)
            {
                selector = CreateSimpleSelector(simple, selector);
            }
            else if (selector2 is AggregateSelectorList aggregate1)
            {
                switch (aggregate1.Delimiter)
                {
                    case "":
                        foreach (var a in aggregate1)
                        {
                            selector = CreateSelector(a, selector);
                        }
                        break;
                    case ",":
                        List<Selector> selectors = new List<Selector>();
                        foreach (var a in aggregate1)
                        {
                            selectors.Add((Selector)CreateSelector(a, null));
                        }
                        selector = new OrSelector(selectors);
                        break;
                    default:
                        break;
                }

            }
            else if (selector2 is ComplexSelector combinatorSelectors)
            {
                for (int i = 0; i < combinatorSelectors.Length; i++)
                {
                    var s = combinatorSelectors[i];
                    switch (s.Delimiter)
                    {
                        case Combinator.Child:
                            selector = CreateSelector(s.Selector, selector);
                            if (i != combinatorSelectors.Length - 1)
                            {
                                selector = ((Selector)selector).Child();
                            }
                            break;
                        case Combinator.Descendent:
                            selector = CreateSelector(s.Selector, selector);
                            if (i != combinatorSelectors.Length - 1)
                            {
                                selector = ((Selector)selector).Descendant();
                            }
                            break;
                        case Combinator.AdjacentSibling:
                        case Combinator.Sibling:
                        case Combinator.Namespace:
                            break;
                    }
                }
            }

            return selector;
        }

        private static SelectorRelation CreateSimpleSelector(SimpleSelector simple, SelectorRelation selector)
        {
            if (simple.value != null)
            {
                if (selector == null)
                {
                    selector = new PropertyEqualsSelector(simple.text, simple.value) { ValueString = simple.value, HasValue = false };
                }
                else
                {
                    selector = selector.PropertyEquals(simple.text, simple.value);
                    var propertyEqualsSelector = selector as PropertyEqualsSelector;
                    propertyEqualsSelector.HasValue = false;
                    propertyEqualsSelector.ValueString = simple.value;
                }
            }
            else
            {
                switch (simple.SimpleSelectorType)
                {
                    case SimpleSelectorType.All:
                        selector = new AllSelector();
                        break;
                    case SimpleSelectorType.Class://.class.class
                        var temp = simple.text.Split('.');
                        foreach (var c in temp)
                        {
                            if (!string.IsNullOrEmpty(c))
                            {
                                if (selector == null)
                                {
                                    selector = new ClassSelector(c);
                                }
                                else
                                {
                                    selector = selector.Class(c);
                                }
                            }
                        }
                        break;
                    case SimpleSelectorType.Id:
                        if (selector != null)
                        {
                            selector = selector.Name(simple.text);
                        }
                        else
                        {
                            selector = new NameSelector(simple.text);
                        }
                        break;
                    case SimpleSelectorType.Type://type type.class.class   type#id
                        StringBuilder sb = new StringBuilder();
                        char tc = (char)0;
                        foreach (var c in simple._code)
                        {
                            if (c == '.' || c == '#')
                            {
                                if (selector == null)
                                {
                                    selector = new TypeNameSelector(sb.ToString());
                                }
                                else
                                {
                                    switch (tc)
                                    {
                                        case '.':
                                            selector = selector.Class(sb.ToString());
                                            break;
                                        case '#':
                                            selector = selector.Name(sb.ToString());
                                            break;
                                        default:
                                            selector = selector.OfType(sb.ToString());
                                            break;
                                    }
                                }
                                sb.Clear();
                                tc = c;
                            }
                            else
                            {
                                sb.Append(c);
                            }
                        }
                        if (sb.Length > 0)
                        {
                            if (selector == null)
                            {
                                selector = new TypeNameSelector(sb.ToString());
                            }
                            else
                            {
                                switch (tc)
                                {
                                    case '.':
                                        selector = selector.Class(sb.ToString());
                                        break;
                                    case '#':
                                        selector = selector.Name(sb.ToString());
                                        break;
                                    default:
                                        selector = selector.OfType(sb.ToString());
                                        break;
                                }
                            }
                        }
                        break;
                    case SimpleSelectorType.PseudoElement:
                    case SimpleSelectorType.PseudoClass:
                    case SimpleSelectorType.AttributeUnmatched:
                    case SimpleSelectorType.AttributeMatch:
                    case SimpleSelectorType.AttributeNegatedMatch:
                    case SimpleSelectorType.AttributeSpaceSeparated:
                    case SimpleSelectorType.AttributeStartsWith:
                    case SimpleSelectorType.AttributeEndsWith:
                    case SimpleSelectorType.AttributeContains:
                    case SimpleSelectorType.AttributeDashSeparated:
                    default:
                        break;
                }

            }
            return selector;
        }

        /// <summary>
        /// 内部元素初始化，内部所有元素都会调用这个，可以用来代替CSS实现复杂的属性或者模板设置，重写必须保留base.OnElementInitialize。设置Control的Template来替换模板，就需要在base.OnElementInitialize之前，对于非Control的，元素复用的话，每次添加到元素树的时候都会触发
        /// </summary>
        internal protected virtual void OnElementInitialize(UIElement element)
        {
            if (element is Control control)
            {
                control.Initialize();
            }
            else
            {
                element.LoadStyle();
            }
        }
        ///// <summary>
        ///// 对于初始化过的元素被移除的时候进行终结操作，比如在OnElementInitialize绑定的事件，可以在这里取消绑定
        ///// </summary>
        ///// <param name="element"></param>
        //internal protected virtual void OnElementFinalize(UIElement element)
        //{

        //}
        internal List<(Delegate, UIElement)> afterStyles;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (afterStyles != null)
            {
                foreach (var item in afterStyles)
                {
                    item.Item1.Method.FastInvoke(item.Item1.Target, item.Item2);
                }
                afterStyles.Clear();
            }
        }

        /// <summary>
        /// 加载样式文件，可以是内嵌，或者在线或者本地的，文件需要是utf8编码
        /// </summary>
        /// <param name="path"></param>
        /// <param name="append"></param>
        public void LoadStyleFile(string path, bool append = false)
        {
            ResourceManager.GetText(path, a =>
            {
                if (!string.IsNullOrWhiteSpace(a))
                {
                    Invoke(() =>
                    {
                        LoadStyle(a, append, path);
                    });
                }
            });
        }

        HybridDictionary<string, KeyframesRule> animations;

        //internal void SetStyle(UIElement item, StyleRule style)
        //{
        //    if (style.Selector.Select(item))
        //    {
        //        if (style.Selector is AggregateSelectorList selectorList)
        //        {
        //            if (selectorList.Delimiter == "")
        //            {
        //                if (selectorList[1] is SimpleSelector simpleSelector && simpleSelector.value != null)
        //                {
        //                    AddTrigger(item, style, simpleSelector);
        //                }
        //                else
        //                {
        //                    SetValue(item, style);
        //                }
        //            }
        //            else if (selectorList.Delimiter == ",")
        //            {
        //                var asl = selectorList.FirstOrDefault(a => a is AggregateSelectorList && (a as AggregateSelectorList).Delimiter == "");
        //                if (asl != null && asl.Select(item))
        //                {
        //                    var simpleSelector = ((AggregateSelectorList)asl)[1] as SimpleSelector;
        //                    AddTrigger(item, style, simpleSelector);
        //                }
        //                else
        //                {
        //                    var simpleSelector = selectorList.FirstOrDefault(a => a is SimpleSelector) as SimpleSelector;
        //                    if (simpleSelector != null && simpleSelector.value != null && simpleSelector.Select(item))
        //                    {
        //                        AddTrigger(item, style, simpleSelector);
        //                    }
        //                    else
        //                    {
        //                        SetValue(item, style);
        //                    }
        //                }
        //            }
        //        }
        //        else if (style.Selector is ComplexSelector combinatorSelectors)
        //        {
        //            if (combinatorSelectors[0].Selector is SimpleSelector simpleSelector)
        //            {
        //                if (combinatorSelectors[1].Selector is AggregateSelectorList selectorList1)
        //                {
        //                    if (selectorList1.Delimiter == "")
        //                    {
        //                        if (selectorList1[1] is SimpleSelector simpleSelector1 && simpleSelector1.value != null)
        //                        {
        //                            if (combinatorSelectors.Length == 2)
        //                            {
        //                                AddTrigger(item, style, simpleSelector1);
        //                            }
        //                            else
        //                            {
        //                                var parent = item.Parent;
        //                                while (parent != null)
        //                                {
        //                                    if (selectorList1.Select(parent))
        //                                    {
        //                                        break;
        //                                    }
        //                                    parent = parent.Parent;
        //                                }
        //                                if (parent != null)
        //                                {
        //                                    ComplexSelector selectors = new ComplexSelector();
        //                                    for (int i = 1; i < combinatorSelectors.Length; i++)
        //                                    {
        //                                        selectors.AppendSelector(combinatorSelectors[i].Selector, combinatorSelectors[i].Delimiter);
        //                                    }

        //                                    //selectors.AppendSelector(combinatorSelectors[2].Selector, combinatorSelectors[2].Delimiter);
        //                                    AddTrigger(parent, style, simpleSelector1, Relation.Me.Find(a => selectors.Select(a)));
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SetValue(item, style);
        //                        }
        //                    }
        //                    else if (selectorList1.Delimiter == ",")
        //                    {
        //                        var asl = selectorList1.FirstOrDefault(a => a is AggregateSelectorList && (a as AggregateSelectorList).Delimiter == "");
        //                        if (asl != null && asl.Select(item))
        //                        {
        //                            var simpleSelector1 = ((AggregateSelectorList)asl)[1] as SimpleSelector;
        //                            AddTrigger(item, style, simpleSelector1);
        //                        }
        //                        else
        //                        {
        //                            var simpleSelector1 = selectorList1.FirstOrDefault(a => a is SimpleSelector) as SimpleSelector;
        //                            if (simpleSelector1 != null && simpleSelector1.value != null && simpleSelector1.Select(item))
        //                            {
        //                                AddTrigger(item, style, simpleSelector1);
        //                            }
        //                            else
        //                            {
        //                                SetValue(item, style);
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (combinatorSelectors[1].Selector is SimpleSelector simpleSelector2)
        //                {
        //                    if (simpleSelector2.value != null)
        //                    {
        //                        AddTrigger(item, style, simpleSelector2);
        //                    }
        //                    else
        //                    {
        //                        SetValue(item, style);
        //                    }
        //                }
        //            }
        //            else if (combinatorSelectors[0].Selector is AggregateSelectorList selectorList1)
        //            {//设置给祖先触发器
        //                if (selectorList1.Delimiter != "")
        //                {
        //                    throw new Exception("不支持" + style.Selector);
        //                }
        //                else
        //                {
        //                    var parent = item.Parent;
        //                    while (parent != null)
        //                    {
        //                        if (selectorList1.Select(parent))
        //                        {
        //                            break;
        //                        }
        //                        parent = parent.Parent;
        //                    }
        //                    if (parent != null)
        //                    {
        //                        //if (combinatorSelectors.Length > 2)
        //                        //{
        //                        ////ComplexSelector selectors = new ComplexSelector();
        //                        ////for (int i = 1; i < combinatorSelectors.Length; i++)
        //                        ////{
        //                        ////    selectors.AppendSelector(combinatorSelectors[i].Selector, combinatorSelectors[i].Delimiter);
        //                        ////}
        //                        if (combinatorSelectors.Count(a => a.Delimiter == Combinator.Child) == combinatorSelectors.Length)
        //                        {
        //                            var relation = Relation.Me;
        //                            for (int i = 1; i < combinatorSelectors.Length; i++)
        //                            {
        //                                var selector = combinatorSelectors[i].Selector;
        //                                relation = relation.Children(a => selector.Select(a));
        //                            }
        //                            AddTrigger(parent, style, selectorList1[1] as SimpleSelector, relation);
        //                        }
        //                        else
        //                        {
        //                            AddTrigger(parent, style, selectorList1[1] as SimpleSelector, Relation.Me.Find(a => combinatorSelectors.Select(a)));
        //                        }
        //                        //}
        //                        //else
        //                        //{
        //                        //    var sss = combinatorSelectors[1].Selector;
        //                        //    if (combinatorSelectors[0].Delimiter == Combinator.Child)
        //                        //    {
        //                        //        var ss = combinatorSelectors[1].Selector;
        //                        //        AddTrigger(item.Parent, style, selectorList1[1] as SimpleSelector, Relation.Me.Children(a => ss.Select(a)));
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        AddTrigger(parent, style, selectorList1[1] as SimpleSelector, Relation.Me.Find(a => sss.Select(a)));
        //                        //    }

        //                        //}
        //                    }

        //                    //switch (combinatorSelectors[0].Delimiter)
        //                    //{
        //                    //    case Combinator.Child:
        //                    //        var ss = combinatorSelectors[1].Selector;
        //                    //        AddTrigger(item.Parent, style, selectorList1[1] as SimpleSelector, Relation.Me.Children(a => ss.Select(a)));
        //                    //        break;
        //                    //    case Combinator.Descendent:
        //                    //        var parent = item.Parent;
        //                    //        while (parent != null)
        //                    //        {
        //                    //            if (selectorList1[0].Select(parent))
        //                    //            {
        //                    //                break;
        //                    //            }
        //                    //            parent = parent.Parent;
        //                    //        }

        //                    //        var sss = combinatorSelectors[1].Selector;
        //                    //        AddTrigger(parent, style, selectorList1[1] as SimpleSelector, Relation.Me.Find(a => sss.Select(a)));
        //                    //        break;
        //                    //    default:
        //                    //        throw new Exception("不支持" + style.Selector);
        //                    //}
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("不支持" + combinatorSelectors[0].Selector);
        //            }
        //        }
        //        else if (style.Selector is SimpleSelector simpleSelector && simpleSelector.value != null)
        //        {
        //            AddTrigger(item, style, simpleSelector);
        //        }
        //        else
        //        {
        //            SetValue(item, style);
        //        }
        //    }
        //}

        //static Type keyframe = typeof(KeyFrame<>);
        static Type ViewFillType = typeof(ViewFill);
        static Type SolidColorFillType = typeof(SolidColorFill);

        bool AddTrigger(UIElement pItem, Style style, PropertyEqualsSelector propertyEqualsSelector, Selector selector, UIElement target)
        {
            Trigger t;
            if (pItem.triggers != null && (t = pItem.triggers.FirstOrDefault(a => a.Style == style)) != null)
            {
                if (!propertyEqualsSelector.HasDot && propertyEqualsSelector.Select(pItem))
                {
                    foreach (var setter in style.Setters)
                    {
                        if (target.HasProperty(setter.Key))
                        {
                            if (!setter.Value.HasValue)
                            {
                                setter.Value.HasValue = true;
                                setter.Value.Value = setter.Value.CssValue.ConvertTo(target.GetPropertyMetadata(setter.Key).PropertyType);
                            }
                            target.SetTriggerValue(setter.Key, t, setter.Value);
                        }
                        else
                        {
                            try
                            {
                                var property = target.Type.GetProperty(setter.Key);
                                if (property != null)
                                {
                                    if (!setter.Value.HasValue)
                                    {
                                        setter.Value.HasValue = true;
                                        setter.Value.Value = setter.Value.CssValue.ConvertTo(property.PropertyType);
                                    }
                                    property.FastSetValue(target, setter.Value.Value);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("设置样式时出错：" + style.Selector + " 属性名：" + setter.Key + "  " + e.Message);
                            }
                        }
                    }
                }
                return false;
            }
            //var p = item.GetPropertyMetadata(propertyEqualsSelector.Property);
            //if (p == null)
            //{
            //    Debug.WriteLine(item + "未找到属性：" + propertyEqualsSelector.Property);
            //}
            object value = propertyEqualsSelector.Value;
            Func<object, bool> pc;
            if (propertyEqualsSelector.HasDot)
            {
                pc = a =>
                {
                    if (a != null)
                    {
                        if (!propertyEqualsSelector.HasValue && !string.IsNullOrEmpty(propertyEqualsSelector.ValueString))
                        {
                            propertyEqualsSelector.HasValue = true;
                            propertyEqualsSelector.Value = propertyEqualsSelector.ValueString.ConvertTo(a.GetType());
                        }
                        return a.Equals(propertyEqualsSelector.Value);
                    }
                    return propertyEqualsSelector.ValueString == "null";
                };
            }
            else
            {
                pc = a => (a != null && a.Equals(value) || (a == null && value == null));
            }
            t = new Trigger
            {
                Property = propertyEqualsSelector.Property,
                PropertyConditions = pc,
                TargetRelation = style.GetRelation(selector),
                Style = style,
                AnimationDuration = style.AnimationDuration,
                AnimationEndBehavior = style.AnimationEndBehavior,
                AnimationIterationCount = style.AnimationIterationCount
            };
            foreach (var setter in style.Setters)
            {
                t.Setters.Add(setter.Key, setter.Value);
            }
            string animation_name = style.animation_name;
            if (!string.IsNullOrWhiteSpace(animation_name) && animations != null)
            {
                if (animations.TryGetValue(animation_name, out KeyframesRule kf))
                {
                    var story = new Storyboard();
                    foreach (KeyframeRule k in kf.Declarations)
                    {
                        var timeline = new Timeline(k.Timeline);
                        foreach (var v in k.Declarations)
                        {
                            var pm = pItem.GetPropertyMetadata(v.Name);
                            if (pm != null)
                            {
                                var pt = pm.PropertyType;
                                //var type = keyframe.MakeGenericType(pt == ViewFillType ? SolidColorFillType : pt);
                                var type = KeyFrame<int>.KeyFrameTypes[pt == ViewFillType ? SolidColorFillType : pt];
                                var kk = type();
                                kk.Property = v.Name;
                                kk.SetValue("Value", v.Term.GetValue().ConvertTo(pt));
                                timeline.KeyFrames.Add(kk);
                            }
                        }
                        story.Timelines.Add(timeline);
                    }
                    t.Animation = story;
                }
            }
            pItem.Triggers.Add(t);
            return true;
        }

        //private void AddTrigger(UIElement item, StyleRule style, SimpleSelector simpleSelector, Relation relation = null)
        //{
        //    if (item.triggers != null && item.triggers.FirstOrDefault(a => a.Style == style) != null)
        //    {
        //        return;
        //    }
        //    var p = item.GetPropertyMetadata(simpleSelector.text);
        //    if (p == null)
        //    {
        //        Debug.WriteLine(item + "未找到属性：" + simpleSelector.text);
        //    }
        //    object value = null;
        //    //try
        //    //{
        //    value = simpleSelector.value.ConvertTo(p.PropertyType);
        //    //}
        //    //catch (Exception)
        //    //{ }
        //    Trigger t = null;
        //    switch (simpleSelector.SimpleSelectorType)
        //    {
        //        case SimpleSelectorType.AttributeMatch:
        //            t = new Trigger
        //            {
        //                Property = simpleSelector.text,
        //                PropertyConditions = a => (a != null && a.Equals(value) || (a == null && value == null)),
        //                TargetRelation = relation,
        //                Style = style,
        //            };
        //            break;
        //        case SimpleSelectorType.AttributeNegatedMatch:
        //            t = new Trigger
        //            {
        //                Property = simpleSelector.text,
        //                PropertyConditions = a => !((a != null && a.Equals(value) || (a == null && value == null))),
        //                TargetRelation = relation,
        //                Style = style,
        //            };
        //            break;
        //        default:
        //            throw new Exception("暂时不支持" + simpleSelector.ToString());
        //    }

        //    string animation_name = null;
        //    TimeSpan animation_duration = TimeSpan.FromSeconds(0.5);
        //    uint animation_iteration_count = 1;
        //    foreach (var sdv in style.Declarations)
        //    {
        //        var lower = sdv.Name.ToLower();
        //        //if (item.HasProperty(sdv.Name))
        //        //{
        //        t.Setters.Add(sdv.Name, sdv.Term.GetValue());
        //        //}
        //        if (lower == "animation-name")
        //        {
        //            animation_name = sdv.Term.GetValue();
        //        }
        //        else if (lower == "animation-duration")
        //        {
        //            var c = sdv.Term.GetValue().ToLower();
        //            try
        //            {
        //                if (c.EndsWith("ms"))
        //                {
        //                    animation_duration = TimeSpan.FromMilliseconds(double.Parse(c.TrimEnd('s', 'm')));
        //                }
        //                else if (c.EndsWith("s"))
        //                {
        //                    animation_duration = TimeSpan.FromSeconds(double.Parse(c.TrimEnd('s')));
        //                }
        //                else
        //                {
        //                    animation_duration = TimeSpan.FromMilliseconds(double.Parse(c));
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                throw new Exception("不支持的时间格式:" + c);
        //            }
        //        }
        //        else if (lower == "animation-iteration-count")
        //        {
        //            var c = sdv.Term.GetValue();
        //            if (c == "infinite")
        //            {
        //                animation_iteration_count = 0;
        //            }
        //            else
        //            {
        //                if (!uint.TryParse(c, out animation_iteration_count))
        //                {
        //                    animation_iteration_count = 1;
        //                }
        //            }
        //        }
        //        else if (lower == "animation-timing-function")
        //        {

        //        }
        //        else if (lower == "animation-fill-mode")
        //        {
        //            var c = sdv.Term.GetValue();
        //            if (c == "none")
        //            {
        //                t.AnimationEndBehavior = EndBehavior.Recovery;
        //            }
        //            else if (c == "forwards")
        //            {
        //                t.AnimationEndBehavior = EndBehavior.Reservations;
        //            }
        //            else
        //            {
        //                throw new Exception("不支持animation-fill-mode的" + c);
        //            }
        //        }
        //    }
        //    if (!string.IsNullOrWhiteSpace(animation_name))
        //    {
        //        if (animations.TryGetValue(animation_name, out KeyframesRule kf))
        //        {
        //            var story = new Storyboard();
        //            foreach (KeyframeRule k in kf.Declarations)
        //            {
        //                var timeline = new Timeline(k.Timeline);
        //                foreach (var v in k.Declarations)
        //                {
        //                    var pm = item.GetPropertyMetadata(v.Name);
        //                    if (pm != null)
        //                    {
        //                        var pt = pm.PropertyType;
        //                        //if (pt == ViewFillType)
        //                        //{
        //                        //    pt = SolidColorFillType;
        //                        //}
        //                        var type = keyframe.MakeGenericType(pt == ViewFillType ? SolidColorFillType : pt);
        //                        var kk = type.GetConstructor(new Type[] { }).FastInvoke() as KeyFrame;
        //                        kk.Property = v.Name;
        //                        //type.GetProperty("Value").FastSetValue(kk, v.Term.GetValue().ConvertTo(pt));
        //                        kk.SetValue("Value", v.Term.GetValue().ConvertTo(pt));
        //                        timeline.KeyFrames.Add(kk);
        //                    }
        //                }
        //                story.Timelines.Add(timeline);
        //            }
        //            t.Animation = story;
        //        }
        //    }
        //    t.AnimationDuration = animation_duration;
        //    t.AnimationIterationCount = animation_iteration_count;
        //    item.Triggers.Add(t);
        //}


        private static void SetValue(UIElement item, Style style)
        {
            foreach (var setter in style.Setters)
            {
                if (item.HasProperty(setter.Key))
                {
                    //if (!setter.Value.HasValue)
                    //{
                    //    setter.Value.HasValue = true;
                    //    setter.Value.Value = setter.Value.CssValue.ConvertTo(item.GetPropertyMetadata(setter.Key).PropertyType);
                    //}
                    setter.Value.ConvertValue(item.GetPropertyMetadata(setter.Key).PropertyType);
                    item.SetStyleValue(setter.Key, setter.Value);
                }
                else
                {
                    try
                    {
                        var property = item.Type.GetProperty(setter.Key);
                        if (property != null)
                        {
                            //if (!setter.Value.HasValue)
                            //{
                            //    setter.Value.HasValue = true;
                            //    setter.Value.Value = setter.Value.CssValue.ConvertTo(property.PropertyType);
                            //}
                            setter.Value.ConvertValue(property.PropertyType);
                            property.FastSetValue(item, setter.Value.Value);
                        }
                        else
                        {
                            var index = setter.Key.IndexOf('-');
                            if (index > 0)
                            {
                                property = item.Type.GetProperty(setter.Key.Substring(0, index));
                                if (property != null)
                                {
                                    var temp = setter.Key.Split('-');
                                    var lIndex = 0;
                                    if (temp.Length > 2 && (temp[1] == "*" || int.TryParse(temp[1], out lIndex)))
                                    {
                                        var value = property.FastGetValue(item);
                                        if (value is IEnumerable enumerable)
                                        {
                                            if (temp[1] == "*")
                                            {
                                                foreach (var it in enumerable)
                                                {
                                                    if (it != null)
                                                    {
                                                        property = it.GetType().GetProperty(temp[2]);
                                                        if (property != null)
                                                        {
                                                            setter.Value.ConvertValue(property.PropertyType);
                                                            property.FastSetValue(it, setter.Value.Value);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var it = enumerable.OfType<object>().ElementAtOrDefault(lIndex);
                                                if (it != null)
                                                {
                                                    property = it.GetType().GetProperty(temp[2]);
                                                    if (property != null)
                                                    {
                                                        //if (!setter.Value.HasValue)
                                                        //{
                                                        //    setter.Value.HasValue = true;
                                                        //    setter.Value.Value = setter.Value.CssValue.ConvertTo(property.PropertyType);
                                                        //}
                                                        setter.Value.ConvertValue(property.PropertyType);
                                                        property.FastSetValue(it, setter.Value.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var value = property.FastGetValue(item);
                                        if (value != null)
                                        {
                                            property = value.GetType().GetProperty(temp[1]);
                                            if (property != null)
                                            {
                                                //if (!setter.Value.HasValue)
                                                //{
                                                //    setter.Value.HasValue = true;
                                                //    setter.Value.Value = setter.Value.CssValue.ConvertTo(property.PropertyType);
                                                //}
                                                setter.Value.ConvertValue(property.PropertyType);
                                                property.FastSetValue(value, setter.Value.Value);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("设置样式时出错：" + style.Selector + " 属性名：" + setter.Key + "  " + e.Message);
                    }
                }
            }
            //foreach (var sdv in style.Declarations)
            //{
            //    if (item.HasProperty(sdv.Name))
            //    {
            //        try
            //        {
            //            item.SetStyleValue(sdv.Name, style, sdv, sdv.Value.HasValue ? sdv.Value.Value : sdv.Term.GetValue());
            //        }
            //        catch (Exception e)
            //        {
            //            Debug.WriteLine("设置样式时出错：" + sdv + " " + e.Message);
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {
            //            var property = item.Type.GetProperty(sdv.Name);
            //            if (property != null)
            //            {
            //                var value = sdv.Value.HasValue ? sdv.Value.Value : sdv.Term.GetValue();
            //                if (value != null && !sdv.Value.HasValue)
            //                {
            //                    var vType = value.GetType();
            //                    if (vType != property.PropertyType && !property.PropertyType.IsAssignableFrom(vType))
            //                    {
            //                        value = value.ConvertTo(property.PropertyType);
            //                    }
            //                    sdv.Value = new EffectiveValueEntry { HasValue = true, Value = value };
            //                }
            //                else
            //                {
            //                    value = sdv.Value.Value;
            //                }
            //                property.FastSetValue(item, value);
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            Debug.WriteLine("设置样式时出错：" + sdv + " " + e.Message);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 子元素
        /// </summary>
        [NotCpfProperty]
        public virtual new UIElementCollection Children
        {
            get
            {
                return base.Children;
            }
        }
        /// <summary>
        /// 视图所在的屏幕
        /// </summary>
        [NotCpfProperty]
        public virtual Screen Screen { get { return viewImpl.Screen; } }


        ///// <summary>
        ///// 背景填充
        ///// </summary>
        //[UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        //public ViewFill Background
        //{
        //    get { return (ViewFill)GetValue(); }
        //    set { SetValue(value); }
        //}

        //protected override void OnRender(DrawingContext dc)
        //{
        //    var back = Background;
        //    var size = ActualSize;
        //    if (back != null && size.Width > 0 && size.Height > 0)
        //    {
        //        using (var b = back.CreateBrush(new Rect(new Point(), size), dc))
        //        {
        //            dc.FillRectangle(b, new Rect(0, 0, size.Width, size.Height));
        //        }
        //    }
        //}
        protected override void Dispose(bool disposing)
        {
            views.TryRemove(this, out _);
            base.Dispose(disposing);
        }
    }
    /// <summary>
    /// 绑定RadioButton分组的值，只能在View里使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RadioButtonGroupAttribute : Attribute
    {
        /// <summary>
        /// 绑定RadioButton分组的值，只能在View里使用，默认是用RadioButton的Content作为绑定值
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="valueFromDataContext">用RadioButton的DataContext作为绑定的值，默认是用RadioButton的Content</param>
        public RadioButtonGroupAttribute(string groupName, bool valueFromDataContext = false)
        {
            this.GroupName = groupName;
            this.ValueFromDataContext = valueFromDataContext;
        }
        public string GroupName { get; }
        /// <summary>
        /// 用RadioButton的DataContext作为绑定的值，默认是用RadioButton的Content
        /// </summary>
        public bool ValueFromDataContext { get; }
    }
}
