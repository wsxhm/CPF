using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Android
{
    class SoftKeyboardListner : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {

        private const int DefaultKeyboardHeightDP = 100;
        private static readonly int EstimatedKeyboardDP = DefaultKeyboardHeightDP + (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop ? 48 : 0);

        private readonly ISurfaceView _host;
        public bool _wasKeyboard;

        public SoftKeyboardListner(View view)
        {
            _host = view as ISurfaceView;
        }

        public IViewImpl viewImpl = null;
        int top = 0;
        public void OnGlobalLayout()
        {
            int estimatedKeyboardHeight = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip,
                EstimatedKeyboardDP, _host.Resources.DisplayMetrics);

            var rect = new global::Android.Graphics.Rect();
            _host.GetWindowVisibleDisplayFrame(rect);

            int heightDiff = (int)_host.Screen.Bounds.Height - (rect.Bottom);
            var isKeyboard = heightDiff >= estimatedKeyboardHeight;

            if (isKeyboard != _wasKeyboard)
            {
                if (GeneralView.editor != null && isKeyboard)
                {
                    var ele = GeneralView.editor as UIElement;
                    var p = ele.PointToScreen(new Drawing.Point(ele.ActualSize.Width, ele.ActualSize.Height));
                    var offset = p.Y - (_host.Screen.Bounds.Height - heightDiff);
                    if (offset > 0)
                    {
                        viewImpl = ele.Root.ViewImpl;
                        top = viewImpl.Position.Y;
                        viewImpl.Position = new PixelPoint(viewImpl.Position.X, top - (int)offset);
                    }
                }
                else
                {
                    //_host.Top = top;
                    if (viewImpl != null)
                    {
                        viewImpl.Position = new PixelPoint(viewImpl.Position.X, top);
                        viewImpl = null;
                    }
                }
            }
            _wasKeyboard = isKeyboard;
        }
    }
}