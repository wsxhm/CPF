using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using CPF;
using CPF.Animation;
using System.Linq.Expressions;
using System.ComponentModel;
using CPF.Reflection;
using System.Collections;

namespace CPF.Styling
{
    /// <summary>
    /// 触发器
    /// </summary>
    public class Trigger
    {
        /// <summary>
        /// 触发器
        /// </summary>
        public Trigger()
        {
            PropertyConditions = DefaultPropertyConditions;
        }
        /// <summary>
        /// 触发器，如果属性的类型是bool，而且条件是属性值为true，PropertyConditions可以不设置
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="TargetRelation"></param>
        /// <param name="PropertyConditions"></param>
        public Trigger(string Property, Relation TargetRelation, Func<object, bool> PropertyConditions = default)
        {
            this.Property = Property;
            this.TargetRelation = TargetRelation;
            if (PropertyConditions != null)
            {
                this.PropertyConditions = PropertyConditions;
            }
            else
            {
                this.PropertyConditions = DefaultPropertyConditions;
            }
        }

        bool DefaultPropertyConditions(object v)
        {
            return (bool)v;
        }

        /// <summary>
        /// 满足条件之后播放的动画
        /// </summary>
        public Storyboard Animation { get; set; }

        /// <summary>
        /// 条件属性
        /// </summary>
        public string Property { get; set; }
        ///// <summary>
        ///// 条件值
        ///// </summary>
        //public object Value { get; set; }
        ///// <summary>
        ///// 运算条件
        ///// </summary>
        //public Conditions Condition { get; set; } = Conditions.Equals;

        /// <summary>
        /// 属性条件，参数是属性值，返回条件结果
        /// </summary>
        public Func<object, bool> PropertyConditions { get; set; }

        internal bool Condition(UIElement element)
        {
            if (PropertyConditions != null)
            {
                if (Property.Contains('.'))
                {
                    if (element.attachedValues != null && element.attachedValues.TryGetValue(Property, out var v))
                    {
                        return PropertyConditions(v);
                    }
                }
                else
                {
                    return PropertyConditions(element.GetValue(Property));
                }
            }
            return false;
        }
        Dictionary<string, object> propertyAndValues = new Dictionary<string, object>();
        /// <summary>
        /// 设置的属性名和值
        /// </summary>
        public Dictionary<string, object> Setters
        {
            get
            {
                return propertyAndValues;
            }
        }
        /// <summary>
        /// 相对位置元素，用来设置值或者动画
        /// </summary>
        public Relation TargetRelation { get; set; }
        //public void SetStyle(DependencyObject obj)
        //{
        //    foreach (var item in propertyAndValues)
        //    {
        //        obj.SetValue(item.Key, item.Value);
        //    }
        //}
        /// <summary>
        /// 动画持续时间
        /// </summary>
        public TimeSpan AnimationDuration
        {
            get;
            set;
        } = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// 动画播放次数，0为无限循环
        /// </summary>
        public uint AnimationIterationCount
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// 动画结束之后的行为
        /// </summary>
        public EndBehavior AnimationEndBehavior
        {
            get;
            set;
        } = EndBehavior.Recovery;

        internal Style Style;
        internal HybridDictionary<CpfObject, List<string>> SetPropertys;
    }

    //public enum Conditions
    //{
    //    NotEqual,
    //    Equals,
    //    //LessThan,
    //    //LessThanOrEqual,
    //    //GreaterThan,
    //    //GreaterThanOrEqual,
    //}
}
