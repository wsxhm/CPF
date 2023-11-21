using CPF.Animation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Styling
{
    public class Style
    {
        public Style(Selector selector)
        {
            Selector = selector;
            propertyAndValues = new Setter(this);
        }

        //public StyleRule StyleRule { get; set; }
        /// <summary>
        /// 来源的URL
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int Line { get; internal set; }

        /// <summary>
        /// 选择器
        /// </summary>
        public Selector Selector { get; private set; }

        Setter propertyAndValues;
        /// <summary>
        /// 设置的属性名和值
        /// </summary>
        public Setter Setters
        {
            get
            {
                return propertyAndValues;
            }
        }

        /// <summary>
        /// 满足条件之后播放的动画
        /// </summary>
        public Storyboard Animation { get; set; }

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
        Relation relation;
        internal Relation GetRelation(Selector _selector = null)
        {
            if (relation == null)
            {
                SelectorRelation pre = this.Selector;
                if (_selector != null)
                {
                    pre = _selector;
                }
                bool has = false;
                List<SelectorRelation> relations = new List<SelectorRelation>();
                while (pre != null)
                {
                    if (pre is PropertyEqualsSelector)
                    {
                        has = true;
                        break;
                    }
                    relations.Add(pre);
                    pre = pre.Prev;
                }
                if (has && relations.Count != 0)
                {
                    relation = Relation.Me;
                    var last = -1;
                    for (int i = relations.Count - 1; i >= 0; i--)
                    {
                        if (!(relations[i] is Selector))
                        {
                            last = i + 1;
                            break;
                        }
                    }
                    if (last >= 0)
                    {
                        relations.RemoveRange(last, relations.Count - last);
                    }

                    for (int i = relations.Count - 1; i >= 0; i--)
                    {
                        if (relations[i] is ChildSelector child)
                        {
                            List<Selector> selectors = new List<Selector>();
                            i--;
                            while (i >= 0)
                            {
                                if (relations[i] is Selector selector)
                                {
                                    selectors.Add(selector);
                                }
                                else
                                {
                                    i++;
                                    break;
                                }
                                i--;
                            }
                            relation = relation.Children(a =>
                            {
                                if (selectors.Count == 0)
                                {
                                    return false;
                                }
                                foreach (var item in selectors)
                                {
                                    if (!item.Select(a))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            });
                        }
                        else if (relations[i] is DescendantSelector descendant)
                        {
                            List<Selector> selectors = new List<Selector>();
                            i--;
                            while (i >= 0)
                            {
                                if (relations[i] is Selector selector)
                                {
                                    selectors.Add(selector);
                                }
                                else
                                {
                                    i++;
                                    break;
                                }
                                i--;
                            }
                            relation = relation.Find(a =>
                            {
                                if (selectors.Count == 0)
                                {
                                    return false;
                                }
                                foreach (var item in selectors)
                                {
                                    if (!item.Select(a))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            });
                        }
                    }
                    //relation = relation.Find(a =>
                    //{
                    //    if (relations.Count == 0)
                    //    {
                    //        return false;
                    //    }
                    //    var element = a;
                    //    for (int i = 0; i < relations.Count; i++)
                    //    //for (int i = relations.Count - 1; i >= 0; i--)
                    //    {
                    //        if (relations[i] is ChildSelector child)
                    //        {
                    //            element = a.Parent;
                    //        }
                    //        else if (relations[i] is DescendantSelector descendant)
                    //        {
                    //            var p = element.Parent;
                    //            i++;
                    //            Selector select = relations[i] as Selector;
                    //            while (p != null)
                    //            {
                    //                if (select.Select(p))
                    //                {
                    //                    if (i == relations.Count - 1)
                    //                    {
                    //                        return true;
                    //                    }
                    //                    break;
                    //                }
                    //                p = p.Parent;
                    //            }
                    //            if (i == relations.Count - 1)
                    //            {
                    //                return false;
                    //            }
                    //        }
                    //        else if (relations[i] is Selector selector)
                    //        {
                    //            if (!selector.Select(element))
                    //            {
                    //                return false;
                    //            }
                    //            else
                    //            {
                    //                if (i == relations.Count - 1)
                    //                {
                    //                    return true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    return false;
                    //});
                }
                else
                {
                    relation = Relation.Me;
                }
            }
            return relation;
        }
        internal string animation_name;
        internal int Index;
    }

    public class StyleValue
    {
        string cssValue;
        public string CssValue
        {
            get
            {
                if (cssValue == null)
                {
                    return Value == null ? "null" : Value.ToString();
                }
                return cssValue;
            }
            set { cssValue = value; }
        }
        //Type valueType;
        //public Type ValueType
        //{
        //    get
        //    {
        //        if (valueType == null && Value != null)
        //        {
        //            valueType = Value.GetType();
        //        }
        //        return valueType;
        //    }
        //    set
        //    {
        //        valueType = value;
        //    }
        //}
        /// <summary>
        /// Value值是否有效
        /// </summary>
        public bool HasValue { get; set; }
        /// <summary>
        /// 是否高优先级
        /// </summary>
        public bool IsImportant { get; set; }

        public object Value { get; set; }

        public Style Style { get; internal set; }

        internal void ConvertValue(Type valueType)
        {
            if (!HasValue)
            {
                HasValue = true;
                if (Value != null)
                {
                    Value = Value.ConvertTo(valueType);
                }
                else if (!string.IsNullOrEmpty(CssValue))
                {
                    Value = CssValue.ConvertTo(valueType);
                }
            }
        }

    }

    public class Setter : Dictionary<string, StyleValue>
    {
        Style style;
        public Setter(Style style)
        {
            this.style = style;
        }

        /// <summary>
        /// 添加设置的属性值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="important"></param>
        public void Add(string property, object value, bool important = false)
        {
            base.Add(property, new StyleValue { Value = value, IsImportant = important, Style = style });
        }
        /// <summary>
        /// 添加设置的属性值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="cssValue">字符串自动根据元素的属性类型转换，不同元素的话属性类型必须一致</param>
        /// <param name="important"></param>
        public void Add(string property, string cssValue, bool important = false)
        {
            base.Add(property, new StyleValue { CssValue = cssValue, IsImportant = important, Style = style });
        }
    }
}
