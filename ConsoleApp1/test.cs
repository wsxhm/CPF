using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{

    [CPF.Design.DesignerLoadStyle("res://$safeprojectname$/Stylesheet1.css")]//用于设计的时候加载样式
    public class test : Control
    {
        public float Valeft
        {
            set; get;
        } = 0f;
        public void Animation(test button) {
            //new Animation_easing(button, 2000, 10, 0, 500);
            if (Valeft == 500)
            {
                EasingSharp.Easing.Ease((v) =>
                {
                    Invoke(() =>
                    {
                        Valeft = (float)v;
                    });
                    Invalidate();
                }, EasingSharp.EasingTypes.EaseOutBounce, 500, 0, 2000);
            }
            else {
                EasingSharp.Easing.Ease((v) =>
                {
                    Invoke(() => {
                        Valeft = (float)v;
                    });
                    Invalidate();
                }, EasingSharp.EasingTypes.EaseOutBounce, 0, 500, 2000);
            }
            
        }
        //模板定义
        protected override void InitializeComponent()
        {
            Background = "0,122,204";
        }
        protected override void OnRender(DrawingContext dc)
        {
            var rect1 = new Rect
            {
                X = Valeft,
                Y = 1,
                Width = 50,
                Height = 50,
            };
            //dc.DrawRectangle("#fff", "1", rect1);
            dc.FillRectangle("46,46,46", rect1);
            //base.OnRender(dc);
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif
    }
}
