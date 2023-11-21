using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public class ContentElement : CpfObject
    {
        /// <summary>
        /// 获取此元素的逻辑树中的父级。
        /// </summary>
        [NotCpfProperty]
        public CpfObject Parent { get; internal set; }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            CpfObject p;
            if (pm.PropertyName != nameof(Parent) && (pm is UIPropertyMetadataAttribute) && ((UIPropertyMetadataAttribute)pm).Inherits && (p = Parent) != null && p.HasProperty(pm.PropertyName))
            {
                return p.GetValue(pm.PropertyName);
            }
            return base.OnGetDefaultValue(pm);
        }
    }
}
