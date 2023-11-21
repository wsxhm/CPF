using CPF.Animation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF.Styling
{
    /// <summary>
    /// 选择器
    /// </summary>
    public abstract class Selector : SelectorRelation
    {
        /// <summary>
        /// 判断元素是否符合选择器标准
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract bool Select(UIElement element);

        /// <summary>
        /// 直接子元素
        /// </summary>
        /// <returns></returns>
        public SelectorRelation Child()
        {
            return new ChildSelector { Prev = this };
        }
        /// <summary>
        /// 子代以及所有后代
        /// </summary>
        /// <returns></returns>
        public SelectorRelation Descendant()
        {
            return new DescendantSelector { Prev = this };
        }
    }
    /// <summary>
    /// 选择器关系
    /// </summary>
    public abstract class SelectorRelation
    {
        /// <summary>
        /// 上一个选择器
        /// </summary>
        public SelectorRelation Prev { get; internal set; }

        //public virtual SelectorTypes SelectorType { get { return SelectorTypes.Selector; } }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var selector = this;
            while (selector != null)
            {
                sb.Insert(0, selector.GetString());
                selector = selector.Prev;
            }
            return sb.ToString();
        }

        protected abstract string GetString();

        /// <summary>
        /// 是否包含类名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Selector Class(string name)
        {
            return new ClassSelector(name) { Prev = this };
        }

        ///// <summary>
        ///// Returns a selector which matches a type or a derived type.
        ///// </summary>
        ///// <param name="previous">The previous selector.</param>
        ///// <param name="type">The type.</param>
        ///// <returns>The selector.</returns>
        //public static Selector Is(this Selector previous, Type type)
        //{
        //    Contract.Requires<ArgumentNullException>(type != null);

        //    return TypeNameAndClassSelector.Is(previous, type);
        //}

        ///// <summary>
        ///// Returns a selector which matches a type or a derived type.
        ///// </summary>
        ///// <typeparam name="T">The type.</typeparam>
        ///// <param name="previous">The previous selector.</param>
        ///// <returns>The selector.</returns>
        //public static Selector Is<T>(this Selector previous) where T : IStyleable
        //{
        //    return previous.Is(typeof(T));
        //}
        /// <summary>
        /// 元素的Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Selector Name(string name)
        {
            return new NameSelector(name) { Prev = this };
        }

        ///// <summary>
        ///// Returns a selector which inverts the results of selector argument.
        ///// </summary>
        ///// <param name="previous">The previous selector.</param>
        ///// <param name="argument">The selector to be not-ed.</param>
        ///// <returns>The selector.</returns>
        //public static Selector Not(this Selector previous, Func<Selector, Selector> argument)
        //{
        //    return new NotSelector(previous, argument(null));
        //}

        ///// <summary>
        ///// Returns a selector which inverts the results of selector argument.
        ///// </summary>
        ///// <param name="previous">The previous selector.</param>
        ///// <param name="argument">The selector to be not-ed.</param>
        ///// <returns>The selector.</returns>
        //public static Selector Not(this Selector previous, Selector argument)
        //{
        //    return new NotSelector(previous, argument);
        //}
        /// <summary>
        /// 不包括派生类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Selector OfType(Type type)
        {
            return new TypeSelector(type) { Prev = this };
        }
        /// <summary>
        /// 不包括派生类
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Selector OfType(string typeName)
        {
            return new TypeNameSelector(typeName) { Prev = this };
        }
        /// <summary>
        /// 不包括派生类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Selector OfType<T>() where T : UIElement
        {
            return OfType(typeof(T));
        }
        /// <summary>
        /// 满足其中一个
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static Selector Or(params Selector[] selectors)
        {
            return new OrSelector(selectors);
        }
        /// <summary>
        /// 满足其中一个
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static Selector Or(IEnumerable<Selector> selectors)
        {
            return new OrSelector(selectors);
        }
        /// <summary>
        /// 值等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Selector PropertyEquals<T>(string property, T value)
        {
            var pre = Prev;
            while (pre != null)
            {
                if (pre is PropertyEqualsSelector)
                {
                    throw new Exception("只能有一个PropertyEqualsSelector");
                }
                pre = pre.Prev;
            }
            return new PropertyEqualsSelector(property, value) { Prev = this };
        }
        /// <summary>
        /// 值等于
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Selector PropertyEquals(string property, object value)
        {
            var pre = Prev;
            while (pre != null)
            {
                if (pre is PropertyEqualsSelector)
                {
                    throw new Exception("只能有一个PropertyEqualsSelector");
                }
                pre = pre.Prev;
            }
            return new PropertyEqualsSelector(property, value) { Prev = this };
        }
        ///// <summary>
        ///// 不等于
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="property"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public Selector PropertyNotEquals<T>(string property, T value)
        //{
        //    var pre = Prev;
        //    while (pre != null)
        //    {
        //        if (pre is PropertyEqualsSelector)
        //        {
        //            throw new Exception("只能有一个PropertyEqualsSelector");
        //        }
        //        pre = pre.Prev;
        //    }
        //    return new PropertyNotEqualsSelector(property, value) { Prev = this };
        //}
        /// <summary>
        /// 包含属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public Selector HasProperty(string property)
        {
            return new HasPropertySelector(property) { Prev = this };
        }
    }

    //public enum SelectorTypes : byte
    //{
    //    Selector,
    //    Child,
    //    Descendant
    //}

    //class StartSelector : SelectorRelation
    //{

    //}

    public class ChildSelector : SelectorRelation
    {
        //public override SelectorTypes SelectorType => SelectorTypes.Child;
        protected override string GetString()
        {
            return ">";
        }
    }
    public class DescendantSelector : SelectorRelation
    {
        //public override SelectorTypes SelectorType => SelectorTypes.Descendant;
        protected override string GetString()
        {
            return " ";
        }
    }

    public class AllSelector : Selector
    {
        public override bool Select(UIElement element)
        {
            return true;
        }
        protected override string GetString()
        {
            return "*";
        }
    }
    public class ClassSelector : Selector
    {
        /// <summary>
        /// 是否包含类名
        /// </summary>
        /// <param name="className"></param>
        public ClassSelector(string className)
        {
            @class = className;
        }
        string @class;
        public string ClassName
        {
            get { return @class; }
            set { @class = value; }
        }
        public override bool Select(UIElement element)
        {
            return element.Classes.Contains(@class);
        }
        protected override string GetString()
        {
            return "." + @class;
        }
    }

    public class NameSelector : Selector
    {
        /// <summary>
        /// 元素Name属性
        /// </summary>
        /// <param name="name"></param>
        public NameSelector(string name)
        {
            this.name = name;
        }
        string name;
        public string ElementName
        {
            get { return name; }
            set { name = value; }
        }
        public override bool Select(UIElement element)
        {
            return element.Name == name;
        }
        protected override string GetString()
        {
            return "#" + name;
        }
    }

    public class TypeSelector : Selector
    {
        /// <summary>
        /// 类型，不包含派生类
        /// </summary>
        /// <param name="type"></param>
        public TypeSelector(Type type)
        {
            this.type = type;
        }
        Type type;
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
        public override bool Select(UIElement element)
        {
            return element.Type == type;
        }
        protected override string GetString()
        {
            return type.Name;
        }
    }

    public class TypeNameSelector : Selector
    {
        public TypeNameSelector(string typeName)
        {
            this.typeName = typeName;
        }
        string typeName;
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
        public override bool Select(UIElement element)
        {
            return element.Type.Name == typeName;
        }
        protected override string GetString()
        {
            return typeName;
        }
    }

    /// <summary>
    /// 属性值等于
    /// </summary>
    public class PropertyEqualsSelector : Selector
    {
        /// <summary>
        /// 属性值等于
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public PropertyEqualsSelector(string property, object value)
        {
            this.Property = property;
            this.value = value;
            HasValue = true;
        }
        string property;
        public string Property
        {
            get { return property; }
            set
            {
                property = value;
                HasDot = property.Contains('.');
            }
        }

        internal bool HasDot;

        object value;
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        internal string ValueString;
        internal bool HasValue;
        public override bool Select(UIElement element)
        {
            if (HasDot)
            {
                return true;
            }
            var property = element.GetPropertyMetadata(this.property);
            if (property == null)
            {
                return false;
            }
            if (!HasValue && !string.IsNullOrEmpty(ValueString))
            {
                HasValue = true;
                Value = ValueString.ConvertTo(property.PropertyType);
            }
            return element.GetValue(this.property).Equal(value);
        }

        protected override string GetString()
        {
            return $"[{Property}={(value == null ? "null" : value.ToString())}]";
        }
    }
    ///// <summary>
    ///// 属性值不等于
    ///// </summary>
    //public class PropertyNotEqualsSelector : PropertyEqualsSelector
    //{
    //    /// <summary>
    //    /// 属性值不等于
    //    /// </summary>
    //    /// <param name="property"></param>
    //    /// <param name="value"></param>
    //    public PropertyNotEqualsSelector(string property, object value) : base(property, value)
    //    {
    //    }
    //    public override bool Select(UIElement element)
    //    {
    //        return !element.GetPropretyValue(Property).Equals(Value);
    //    }
    //}
    /// <summary>
    /// 有属性
    /// </summary>
    public class HasPropertySelector : Selector
    {
        public HasPropertySelector(string property)
        {
            this.property = property;
        }
        string property;
        public string Property
        {
            get { return property; }
            set { property = value; }
        }
        public override bool Select(UIElement element)
        {
            var h = element.HasProperty(property);
            if (!h)
            {
                return element.Type.GetProperty(property) != null;
            }
            return h;
        }

        protected override string GetString()
        {
            return $"[{property}]";
        }
    }
    /// <summary>
    /// 满足其中一个
    /// </summary>
    public class OrSelector : Selector
    {
        public OrSelector(IEnumerable<Selector> selectors)
        {
            this.selectors = selectors;
        }

        IEnumerable<Selector> selectors;
        public IEnumerable<Selector> Selectors
        {
            get { return selectors; }
        }

        public override bool Select(UIElement element)
        {
            bool r = false;
            foreach (var item in selectors)
            {
                r = r || item.Select(element);
            }
            return r;
        }

        protected override string GetString()
        {
            return string.Join(",", selectors.Select(a => a.ToString()));
        }
    }

}
