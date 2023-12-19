using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough [width] [Solid/Dash/Dot/DashDot/DashDotDot] [color]
    /// </summary>
    [TypeConverter(typeof(StringConverter))]
    public struct TextDecoration
    {
        public Stroke Stroke { get; set; }

        public Brush Brush { get; set; }

        public TextDecorationLocation Location { get; set; }


        /// <summary>
        /// 字符串格式： overline/Underline/Strikethrough [width] [Solid/Dash/Dot/DashDot/DashDotDot] [color]
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator TextDecoration(string n)
        {
            if (string.IsNullOrWhiteSpace(n) || n.ToLower() == "none")
            {
                return default;
            }
            var temp = n.Split(' ');
            var td = new TextDecoration();
            //if (temp[0].ToLower() == "none")
            //{
            //    return td;
            //}
            td.Location = (TextDecorationLocation)Enum.Parse(typeof(TextDecorationLocation), temp[0], true);
            Stroke stroke;
            if (temp.Length > 1 && !string.IsNullOrWhiteSpace(temp[1]))
            {
                stroke = temp[1];
            }
            else
            {
                stroke = "1";
            }
            if (temp.Length > 2 && !string.IsNullOrWhiteSpace(temp[2]))
            {
                stroke.DashStyle = (DashStyles)Enum.Parse(typeof(DashStyles), temp[2], true);
            }
            td.Stroke = stroke;
            if (temp.Length > 3 && !string.IsNullOrWhiteSpace(temp[3]))
            {
                td.Brush = temp[3];
            }
            else
            {
                td.Brush = "#000";
            }
            return td;
        }

        public override string ToString()
        {
            return (Location + " " + Stroke.ToString().Replace(',', ' ') + " " + (Brush == null ? "" : Brush.ToString())).Trim();
        }
    }


    public enum TextDecorationLocation : byte
    {
        None,
        /// <summary>
        /// 下划线的垂直位置。
        /// </summary>
        Underline = 1,
        /// <summary>
        /// 上划线的垂直位置。
        /// </summary>
        OverLine = 2,
        /// <summary>
        /// 删除线的垂直位置。
        /// </summary>
        Strikethrough = 4,

    }
}
