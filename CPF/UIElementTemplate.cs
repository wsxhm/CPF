using System;
using System.Collections.Generic;
using System.Text;
using CPF.Reflection;
using System.Reflection;

namespace CPF
{
    /// <summary>
    /// UI元素模板，可以是UIElement的实例或者Type用type的话可以完全克隆出新元素
    /// </summary>
    public class UIElementTemplate
    {
        UIElement element;
        ///// <summary>
        ///// 获取UI模板元素实例
        ///// </summary>
        //public UIElement Element
        //{
        //    get
        //    {
        //        return element;
        //    }
        //}
        /// <summary>
        /// 创建模板元素
        /// </summary>
        /// <returns></returns>
        public UIElement CreateElement()
        {
            if (constructor != null)
            {
                return (UIElement)constructor.FastInvoke();
            }
            var ele = element.Clone() as UIElement;
            var control = ele as Controls.Control;
            if (control != null && (element as Controls.Control).IsInitialized)
            {
                control.IsInitialized = true;
            }
            return ele;
        }
        ///// <summary>
        ///// 应用模板，将属性和触发器等等拷贝过去
        ///// </summary>
        ///// <param name="element"></param>
        //public void ApplyTemplate(UIElement element)
        //{
        //    element.template = this.element;
        //}

        ///// <summary>
        ///// 应用模板，将属性和触发器等等拷贝过去
        ///// </summary>
        ///// <param name="element"></param>
        //public void CancelApplyTemplate(UIElement element)
        //{
        //    element.template = null;
        //}
        //Type type;
        ConstructorInfo constructor;
        public UIElementTemplate(UIElement element)
        {
            //var control = element as Controls.Control;
            //if (control != null && !control.IsInitialized)
            //{
            //    control.Initialize();
            //}
            this.element = element;
        }
        public UIElementTemplate(Type type)
        {
            var t = type;
            var tt = typeof(UIElement);
            if (t != tt && !t.IsSubclassOf(tt))
            {
                throw new Exception("模板类型必须继承自UIElement");
            }
            constructor = type.GetConstructor(new Type[] { });
            if (constructor == null)
            {
                throw new Exception("模板类型必须包含无参构造函数");
            }
        }


        public static implicit operator UIElementTemplate(UIElement element)
        {
            return new UIElementTemplate(element);
        }
        public static implicit operator UIElementTemplate(Type element)
        {
            return new UIElementTemplate(element);
        }
    }
    /// <summary>
    /// UI元素模板，可以是Type或者UIElement的实例，用type的话可以完全克隆出新元素。限定模板基类
    /// </summary>
    /// <typeparam name="T">模板基类</typeparam>
    public class UIElementTemplate<T> : UIElementTemplate where T : UIElement
    {
        /// <summary>
        /// 创建模板元素
        /// </summary>
        /// <returns></returns>
        public new T CreateElement()
        {
            var ele = base.CreateElement() as T;
            //var control = ele as CPF.Controls.Control;
            //if (control != null)
            //{
            //    control.IsInitialized = true;
            //}
            return ele;
        }
        ///// <summary>
        ///// 获取UI模板元素实例
        ///// </summary>
        //public new T Element
        //{
        //    get
        //    {
        //        return (T)base.Element;
        //    }
        //}

        public UIElementTemplate(T element) : base(element)
        {
            var t = element.GetType();
            var tt = typeof(T);
            if (t != tt && !t.IsSubclassOf(tt))
            {
                throw new Exception("UIElement模板类型必须继承自" + tt.Name);
            }
        }
        public UIElementTemplate(Type element) : base(element)
        {
            var t = element;
            var tt = typeof(T);
            if (t != tt && !t.IsSubclassOf(tt))
            {
                throw new Exception("UIElement模板类型必须继承自" + tt.Name);
            }
        }

        public static implicit operator UIElementTemplate<T>(T element)
        {
            return new UIElementTemplate<T>(element);
        }

        public static implicit operator UIElementTemplate<T>(Type element)
        {
            return new UIElementTemplate<T>(element);
        }

    }
}
