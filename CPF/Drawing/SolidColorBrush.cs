using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF.Drawing
{
    /// <summary>
    /// 纯色笔刷
    /// </summary>
    public class SolidColorBrush : Brush, Design.ISerializerCode
    {
        /// <summary>
        /// 纯色笔刷
        /// </summary>
        /// <param name="color"></param>
        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        public Color Color
        {
            get;
            private set;
        }

        public string GetCreationCode()
        {
            return $"\"new SolidColorBrush({Color})\"";
        }

        public override string ToString()
        {
            return Color.ToString();
        }

        //protected override void Disposing()
        //{
        //    //usings.Remove(this);
        //    disposes.Add(this);
        //}

        internal static SolidColorBrush Create(Color color)
        {
            SolidColorBrush sb;
            //if (disposes.Count == 0)
            //{
            sb = new SolidColorBrush(color);
            //}
            //else
            //{
            //    sb = disposes[disposes.Count - 1];
            //    disposes.RemoveAt(disposes.Count - 1);
            //    sb.Color = color;
            //    sb.IsDisposed = false;
            //}
            return sb;
        }

        //static List<SolidColorBrush> disposes = new List<SolidColorBrush>();

    }
}
