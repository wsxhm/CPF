using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

namespace CPF.Android
{
    interface ISurfaceView : IViewImpl, View.IOnTouchListener, View.IOnKeyListener
    {
        CpfView CpfView { get; }

        Resources Resources { get; }

        void GetWindowVisibleDisplayFrame(Rect outRect);

        CPF.Controls.View Root { get; }

        void OnPaint();

        GeneralView GeneralView { get; }
    }
}