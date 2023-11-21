using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Android
{
    public class CpfAndroidApp : CPF.Platform.IApp
    {
        public bool IsMain { get; set; }

        public event EventHandler Closed;

        public void Close()
        {
            Closed(this, EventArgs.Empty);
            Java.Lang.JavaSystem.Exit(0);
        }

        public void Show()
        {

        }
    }
}