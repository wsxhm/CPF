using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示控件的文本标签。
    /// </summary>
    [Description("表示控件的文本标签。")]
    [DefaultProperty(nameof(Text))]
    public class Label : Control
    {
        [PropertyMetadata("")]
        public string Text
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            Children.Add(new TextBlock
            {
                MaxWidth = "100%",
                MaxHeight = "100%",
                Bindings =
                {
                    {nameof(Text),nameof(Text),this }
                }
            });
        }

    }
}
