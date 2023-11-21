using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Input;
using CPF.Controls;
using CPF.Drawing;

namespace CPF.Platform
{
    /// <summary>
    /// 表示一个独立的页面，比如一个托管控件
    /// </summary>
    public interface IViewImpl : IDisposable
    {

        Screen Screen { get; }

        void SetRoot(View view);
        /// <summary>
        /// 将指定工作区点的位置计算成屏幕坐标。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Point PointToScreen(Point point);
        /// <summary>
        /// 将指定屏幕点的位置计算成工作区坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Point PointToClient(Point point);

        //Size Size { get; set; }

        void Invalidate(in Rect rect);

        //void Invalidate(UIElement element);

        //VisibleUIElement VisibleUIElements { get; }
        //DpiScale GetDpi();
        float RenderScaling { get; }

        float LayoutScaling { get; }

        /// <summary>
        /// 捕获鼠标
        /// </summary>
        void Capture();
        /// <summary>
        /// 释放捕获的鼠标
        /// </summary>
        void ReleaseCapture();

        Action ScalingChanged { get; set; }

        Action<Size> Resized { get; set; }

        Action<PixelPoint> PositionChanged { get; set; }

        Action Activated { get; set; }
        Action Deactivated { get; set; }

        void Activate();

        void SetVisible(bool visible);
        bool CanActivate { get; set; }

        void SetCursor(Cursor cursor);

        void SetIMEEnable(bool enable);
        /// <summary>
        /// 设置中文输入法位置
        /// </summary>
        /// <param name="point"></param>
        void SetIMEPosition(Point point);


        PixelPoint Position { get; set; }
    }
}
