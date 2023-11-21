using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using CPF.Threading;
using CPF;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CPF.Animation
{
    /// <summary>
    /// 一个容器时间线，该时间线为子动画提供对象和属性确定信息。
    /// </summary>
    [ToolboxItem(false)]
    public class Storyboard : CpfObject, IComponent
    {
        static byte frameRate = 100;
        /// <summary>
        /// 动画帧率，默认100，由于定时器不够精确，帧率没那么准
        /// </summary>
        public static byte FrameRate
        {
            get { return frameRate; }
            set
            {
                frameRate = value;
                if (frameRate == 0)
                {
                    throw new Exception("FrameRate帧率不能为0");
                }
            }
        }

        Collection<Timeline> timelines = new Collection<Timeline>();

        Dictionary<CpfObject, PlayStates> playstates = new Dictionary<CpfObject, PlayStates>();

        public Storyboard()
        {
            //timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 需要按时间的从小到大顺序添加
        /// </summary>
        [NotCpfProperty]
        public Collection<Timeline> Timelines
        {
            get
            {
                return timelines;
            }
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="duration">持续时间</param>
        /// <param name="iterationCount">动画播放次数，0为无限循环</param>
        /// <param name="endBehavior">动画结束之后的行为</param>
        public void Start(CpfObject obj, TimeSpan duration, uint iterationCount = 1, EndBehavior endBehavior = EndBehavior.Recovery)
        {
            PlayStates p;
            if (!playstates.TryGetValue(obj, out p))
            {
                p = new PlayStates();
                p.Storyboard = this;
                p.Target = obj;
                p.Duration = duration;
                p.IterationCount = iterationCount;
                p.EndBehavior = endBehavior;
                playstates.Add(obj, p);
            }
            p.Start();

        }

        internal void Start(UIElement owner, CpfObject obj, TimeSpan duration, uint iterationCount, EndBehavior endBehavior, Styling.Trigger trigger)
        {
            PlayStates p;
            if (!playstates.TryGetValue(obj, out p))
            {
                p = new PlayStates();
                p.Storyboard = this;
                p.Target = obj;
                p.Duration = duration;
                p.IterationCount = iterationCount;
                p.EndBehavior = endBehavior;
                p.Trigger = trigger;
                p.TriggerOwner = owner;
                playstates.Add(obj, p);
            }
            p.Start();
        }

        public void Stop(CpfObject obj)
        {
            PlayStates p;
            if (playstates.TryGetValue(obj, out p))
            {
                p.Stop();
            }
        }
        /// <summary>
        /// 重置播放状态
        /// </summary>
        public void Reset(CpfObject obj)
        {
            PlayStates p;
            if (playstates.TryGetValue(obj, out p))
            {
                p.Reset();
            }
        }


        /// <summary>
        /// 移除动画关联的对象
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(CpfObject obj)
        {
            PlayStates p;
            if (playstates.TryGetValue(obj, out p))
            {
                p.Dispose();
                playstates.Remove(obj);
            }
        }


        ISite IComponent.Site
        {
            get;
            set;
        }

        /// <summary>
        /// 获取当前动画时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TimeSpan GetCurrentTime(CpfObject obj)
        {
            PlayStates p;
            if (playstates.TryGetValue(obj, out p))
            {
                return p.CurrentTime;
            }
            return new TimeSpan();
        }
        /// <summary>
        /// 获取关联的对象动画是否暂停
        /// </summary>
        /// <returns></returns>
        public bool GetIsPaused(CpfObject obj)
        {
            PlayStates p;
            if (playstates.TryGetValue(obj, out p))
            {
                return !p.IsPlaying;
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var item in playstates)
            {
                item.Value.Dispose();
            }
            playstates.Clear();
            timelines.Clear();
            //if (Disposed != null)
            //{
            //    Disposed(this, EventArgs.Empty);
            //}
            this.RaiseEvent(EventArgs.Empty, nameof(Disposed));
        }

        public event EventHandler<StoryboardCompletedEventArgs> Completed
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        public event EventHandler Disposed
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnCompleted(StoryboardCompletedEventArgs e)
        {
            Remove(e.Target);
            this.RaiseEvent(e, nameof(Completed));
            //if (Completed != null)
            //{
            //    Completed(this, e);
            //}
        }

        class PlayStates : IDisposable
        {
            public Styling.Trigger Trigger { get; set; }
            public UIElement TriggerOwner { get; set; }
            public PlayStates()
            {
                timer.Tick += Timer_Tick;
            }

            /// <summary>
            /// 动画持续时间
            /// </summary>
            public TimeSpan Duration
            {
                get;
                set;
            }

            /// <summary>
            /// 动画播放次数，0为无限循环
            /// </summary>
            public uint IterationCount
            {
                get;
                set;
            } = 1;

            /// <summary>
            /// 动画结束之后的行为
            /// </summary>
            public EndBehavior EndBehavior
            {
                get;
                set;
            } = EndBehavior.Recovery;
            /// <summary>
            /// 需要应用动画的目标对象
            /// </summary>
            public CpfObject Target { get; set; }
            Stopwatch watch = new Stopwatch();
            Dictionary<string, object> oldState = new Dictionary<string, object>();
            Dictionary<string, object> lastState = new Dictionary<string, object>();
            int index = 0;
            int count = 1;
            DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000f / frameRate) };
            public Storyboard Storyboard { get; set; }
            //int cc;
            private void Timer_Tick(object sender, EventArgs e)
            {
                //Debug.WriteLine(cc++);
                var duration = Duration;
                if (Target == null || Storyboard.timelines.Count == 0)
                {
                    Stop();
                    return;
                }
                if (CurrentTime.CompareTo(duration) >= 0)
                {
                    if (IterationCount == 0)
                    {
                        Reset();
                        Start();
                    }
                    else if (IterationCount > count)
                    {
                        count++;
                        index = 0;
                        watch.Stop();
                        watch.Reset();
                        RecoverValues();
                        Start();
                    }
                    else
                    {
                        Stop();

                        count = 1;
                        index = 0;
                        watch.Reset();

                        if (EndBehavior == EndBehavior.Recovery)
                        {
                            RecoverValues();
                        }
                        else
                        {
                            foreach (var k in Storyboard.timelines[Storyboard.timelines.Count - 1].KeyFrames)//设置到最后的目标值
                            {
                                var last = k.GetValue();
                                //Debug.WriteLine(last);
                                SetValue(k.Property, last);
                                ClearAnimationValue(k.Property);
                            }
                        }
                        Storyboard.OnCompleted(new StoryboardCompletedEventArgs { Target = Target });
                        return;
                    }
                }
                double c = CurrentTime.TotalMilliseconds;//当前时间
                int i = 0;
                foreach (Timeline item in Storyboard.timelines)
                {
                    double t = item.KeyTime * duration.TotalMilliseconds;//到的毫秒数
                    if (c < t)
                    {
                        if (i > 0)
                        {
                            c = c - Storyboard.timelines[i - 1].KeyTime * duration.TotalMilliseconds;
                            t = t - Storyboard.timelines[i - 1].KeyTime * duration.TotalMilliseconds;
                        }
                        if (index != i)
                        {
                            foreach (var k in Storyboard.timelines[index].KeyFrames)//切换时间轴的时候把上一次的目标值设置过去
                            {
                                SetAnimationValue(k.Property, k.GetValue());
                            }
                            index = i;
                            foreach (var k in Storyboard.timelines[i].KeyFrames)//保存开始动画的时候的基础值
                            {
                                lastState.SetValue(k.Property, GetValue(k.Property));
                            }
                        }
                        foreach (var frame in item.KeyFrames)
                        {
                            double pro = c / t;
                            var dep = frame.Property;
                            object baseValue = lastState.GetValue(dep);
                            object value = frame.InterpolateValue(baseValue, (float)pro);
                            SetAnimationValue(dep, value);
                        }
                        break;
                    }
                    i++;
                }
            }

            CpfObject GetObject(string property, out string p)
            {
                if (property.IndexOf('.') > 0)
                {
                    var temp = property.Split('.');
                    var dp = Target.GetValue(temp[0]) as CpfObject;
                    if (dp == null)
                    {
                        throw new Exception(temp[0] + "为null或不是" + typeof(CpfObject));
                    }
                    p = temp[1];
                    return dp;
                }
                else
                {
                    p = property;
                    return Target;
                }
            }
            private void SetAnimationValue(string property, object value)
            {
                GetObject(property, out string p).SetAnimationValue(p, Storyboard, value);
            }

            private void ClearAnimationValue(string property)
            {
                GetObject(property, out string p).ClearAnimationValue(Storyboard, p);
            }

            private void SetValue(string property, object value)
            {
                if (Trigger != null)
                {
                    var obj = GetObject(property, out string p);
                    if (Trigger.Condition(TriggerOwner))
                    {
                        obj.SetTriggerValue(p, Trigger, value);
                    }
                }
                else
                {
                    GetObject(property, out string p).SetValue(value, p);
                }
            }

            object GetValue(string property)
            {
                return GetObject(property, out string p).GetValue(p);
            }

            public void Start()
            {
                if (Storyboard.timelines.Count == 0)
                {
                    return;
                }
                if (!IsPlaying)
                {
                    foreach (var item in Storyboard.timelines[0].KeyFrames)//保存开始动画的时候的基础值
                    {
                        lastState.SetValue(item.Property, GetValue(item.Property));
                    }
                    foreach (Timeline t in Storyboard.timelines)//保存所有初始值
                    {
                        foreach (var item in t.KeyFrames)
                        {
                            oldState.SetValue(item.Property, GetValue(item.Property));
                        }
                    }
                }
                else
                {
                    Reset();
                }
                timer.Start();
                watch.Start();
                Timer_Tick(null, null);
            }

            /// <summary>
            /// 重置播放状态
            /// </summary>
            public void Reset()
            {
                count = 1;
                index = 0;
                watch.Stop();
                watch.Reset();
                timer.Stop();
                RecoverValues();
            }

            private void RecoverValues()
            {
                foreach (Timeline t in Storyboard.timelines)//恢复所有初始值
                {
                    foreach (var item in t.KeyFrames)
                    {
                        SetAnimationValue(item.Property, oldState.GetValue(item.Property));
                        ClearAnimationValue(item.Property);
                    }
                }
            }

            public bool IsPlaying
            {
                get { return watch.IsRunning; }
            }

            public void Stop()
            {
                timer.Stop();
                watch.Stop();
            }

            //private DependencyProperty GetProperty(string name)
            //{
            //    return DependencyProperty.GetProperty(Target.ThisType, name);
            //}
            /// <summary>
            /// 当前动画所在的时间
            /// </summary>
            public TimeSpan CurrentTime
            {
                get
                {
                    //return new TimeSpan((int)watch.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond);
                    return watch.Elapsed;
                }
            }
            public void Dispose()
            {
                timer.Dispose();
                //oldState.Dispose();
                //lastState.Dispose();
                watch.Stop();
                foreach (Timeline t in Storyboard.timelines)
                {
                    foreach (var item in t.KeyFrames)
                    {
                        ClearAnimationValue(item.Property);
                    }
                }
            }
        }
    }
    ///// <summary>
    ///// 动画播放状态
    ///// </summary>
    //public enum PlayState : byte
    //{
    //    Paused = 0,
    //    Running = 1
    //}
    /// <summary>
    /// 动画播放结束之后的行为
    /// </summary>
    public enum EndBehavior : byte
    {
        /// <summary>
        /// 恢复为开始动画之前的状态
        /// </summary>
        Recovery,
        /// <summary>
        /// 保留动画的属性变化
        /// </summary>
        Reservations,
    }
    public static class ObjEx
    {
        //public static void SetValue(this object obj, string pn, object value)
        //{
        //    var type = obj.GetType();
        //    var p = type.GetProperty(pn);
        //    if (p == null)
        //    {
        //        throw new Exception("未找到" + pn + "属性");
        //    }
        //    p.FastSetValue(obj, value);
        //}

        //public static object GetValue(this object obj, string pn)
        //{
        //    var type = obj.GetType();
        //    var p = type.GetProperty(pn);
        //    if (p == null)
        //    {
        //        throw new Exception("未找到" + pn + "属性");
        //    }
        //    return p.FastGetValue(obj);
        //}

        internal static void SetValue(this Dictionary<string, object> obj, string pn, object value)
        {
            if (obj.ContainsKey(pn))
            {
                obj.Remove(pn);
            }
            obj.Add(pn, value);
        }
        internal static object GetValue(this Dictionary<string, object> obj, string pn)
        {
            if (obj.ContainsKey(pn))
            {
                return obj[pn];
            }
            throw new Exception("未找到属性值");
        }

        static List<TransitionState> storyboards = new List<TransitionState>();
        /// <summary>
        /// 对属性值动画过渡到目标值，目标值必须显式转换，比如FloatField的"100%"，必须写成(FloatField)"100%",不能直接用"100%"
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <typeparam name="S">执行动画的对象类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="targetValue"></param>
        /// <param name="duration"></param>
        /// <param name="ease"></param>
        /// <param name="animateMode"></param>
        /// <param name="completed">如果循环次数是多次的话，那就会触发多次</param>
        /// <param name="iterationCount">为0的话是一直播放，不断循环</param>
        public static S TransitionValue<T, S>(this S obj, string propertyName, T targetValue, TimeSpan duration, IEase ease = null, AnimateMode animateMode = AnimateMode.Linear, Action completed = null, uint iterationCount = 1) where S : CpfObject
        {
            var sb = storyboards.Find(a => a.Object == obj && a.PropertyName == propertyName);
            if (sb != null)
            {
                sb.Storyboard.Stop(obj);
                sb.Storyboard.Remove(obj);
                storyboards.Remove(sb);
                sb.Storyboard.Dispose();
            }
            Storyboard storyboard = new Storyboard
            {
                //Duration = duration,
                Timelines = {
                    new Timeline(1) {
                        KeyFrames = {
                            new KeyFrame<T> {
                                Property = propertyName,
                                Value = targetValue,
                                Ease = ease,
                                AnimateMode = animateMode
                            }
                        }
                    }
                },
                //EndBehavior = EndBehavior.Reservations,
                //IterationCount = iterationCount,
            };
            var tran = new TransitionState { Object = obj, Storyboard = storyboard, PropertyName = propertyName, completed = completed };
            storyboard.Completed += tran.Completed;
            storyboards.Add(tran);
            storyboard.Start(obj, duration, iterationCount, EndBehavior.Reservations);
            //if (sb != null)
            //{
            //    sb.Storyboard.Remove(obj);
            //}
            return obj;
        }
        /// <summary>
        /// 对属性值动画过渡到目标值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression">a=>a.Property</param>
        /// <param name="targetValue"></param>
        /// <param name="duration"></param>
        /// <param name="ease"></param>
        /// <param name="animateMode"></param>
        /// <param name="completed">如果循环次数是多次的话，那就会触发多次</param>
        /// <param name="iterationCount">为0的话是一直播放，不断循环</param>
        /// <returns></returns>
        public static S TransitionValue<T, S>(this S obj, Expression<Func<S, T>> expression, T targetValue, TimeSpan duration, IEase ease = null, AnimateMode animateMode = AnimateMode.Linear, Action completed = null, uint iterationCount = 1) where S : CpfObject
        {
            MemberExpression member = (MemberExpression)expression.Body;
            var property = member.Member.Name;
            TransitionValue(obj, property, targetValue, duration, ease, animateMode, completed, iterationCount);
            return obj;
        }
        ///// <summary>
        ///// 对属性值动画过渡到目标值，目标值必须显式转换，比如FloatField的"100%"，必须写成(FloatField)"100%",不能直接用"100%"
        ///// </summary>
        ///// <typeparam name="T">属性值类型</typeparam>
        ///// <param name="obj"></param>
        ///// <param name="propertyName"></param>
        ///// <param name="targetValue"></param>
        ///// <param name="duration"></param>
        ///// <param name="ease"></param>
        ///// <param name="animateMode"></param>
        ///// <param name="completed">如果循环次数是多次的话，那就会触发多次</param>
        ///// <param name="iterationCount">为0的话是一直播放，不断循环</param>
        //public static void TransitionValue<T>(this CpfObject obj, string propertyName, T targetValue, TimeSpan duration, IEase ease = null, AnimateMode animateMode = AnimateMode.Linear, Action completed = null, uint iterationCount = 1)
        //{
        //    TransitionValue<T, CpfObject>(obj, propertyName, targetValue, duration, ease, animateMode, completed, iterationCount);
        //}

        class TransitionState
        {
            public Storyboard Storyboard;
            public string PropertyName;
            public CpfObject Object;
            public Action completed;
            public void Completed(object s, StoryboardCompletedEventArgs eventArgs)
            {
                if (completed != null)
                {
                    completed();
                }
                var story = s as Storyboard;
                var ss = storyboards.Find(a => a.Storyboard == story);
                storyboards.Remove(ss);
                if (ss != null)
                {
                    story.Remove(ss.Object);
                }
                story.Dispose();
            }
        }
    }


    public class StoryboardCompletedEventArgs : EventArgs
    {
        public CpfObject Target { get; set; }
    }
}
