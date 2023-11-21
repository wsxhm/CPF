using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Platform
{
    public class Screen
    {

#if Net4
        private static IList<Screen> _allScreens;
        public static IList<Screen> AllScreens
#else
        //private static IReadOnlyList<Screen> _allScreens;
        public static IReadOnlyList<Screen> AllScreens
#endif
        {
            get
            {
                //if (_allScreens == null)
                //{
                //    _allScreens = Application.GetRuntimePlatform().GetAllScreen();
                //}
                //return _allScreens;
                return Application.GetRuntimePlatform().GetAllScreen();
            }
        }
        /// <summary>
        /// 获取显示的边界。单位为像素
        /// </summary>
        public Rect Bounds { get; }
        /// <summary>
        ///  获取显示的工作区域。 工作区域是显示内容中，不包括任务栏、 停靠的窗口和已停靠的工具条的桌面区域。单位为像素
        /// </summary>
        public Rect WorkingArea { get; }
        /// <summary>
        /// 是否是主屏
        /// </summary>
        public bool Primary { get; }
        ///// <summary>
        ///// 设备分辨率
        ///// </summary>
        //public PixelSize DeviceResolution
        //{
        //    get;
        //}

        public Screen(Rect bounds, Rect workingArea, bool primary)
        {
            this.Bounds = bounds;
            this.WorkingArea = workingArea;
            this.Primary = primary;
            //DeviceResolution = deviceResolution;
        }
        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <returns></returns>
        public virtual Bitmap Screenshot()
        {
            throw new NotImplementedException();
        }
    }
}
