using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// TreeViewItem的内容模板
    /// </summary>
    [Description("TreeViewItem的内容模板"), Browsable(false)]
    public class TreeViewContentTemplate : ContentTemplate
    {
        /// <summary>
        /// 当前的TreeViewItem
        /// </summary>
        public TreeViewItem TreeViewItem
        {
            get { return GetValue<TreeViewItem>(); }
            set { SetValue(value); }
        }

        protected override void OnAttachedToVisualTree()
        {
            var parent = Parent;
            while (parent != null)
            {
                if (parent is TreeViewItem viewItem)
                {
                    TreeViewItem = viewItem;
                    break;
                }
                parent = parent.Parent;
            }
            base.OnAttachedToVisualTree();
        }
    }
}
