using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 定义颜色渐变
    /// </summary>
    public struct GradientStop
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="position">偏移位置 0-1</param>
        public GradientStop(Color color, float position)
        {
            this.color = color;
            this.position = position;
        }

        Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        float position;
        /// <summary>
        /// 颜色偏移位置 0-1
        /// </summary>
        public float Position
        {
            get { return position; }
            set { position = value; }
        }

        public override string ToString()
        {
            return color.ToString() + " " + position;
        }
    }
}
