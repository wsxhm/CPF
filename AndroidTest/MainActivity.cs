using Android.App;
using Android.OS;
//using Android.Runtime;
//using AndroidX.AppCompat.App;
using CPF.Platform;

namespace AndroidTest
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : CPF.Android.CpfActivity
    {
        static MainActivity()
        {
            CPF.Platform.Application.Initialize((OperatingSystemType.Android, new CPF.Android.AndroidPlatform(), new CPF.Skia.SkiaDrawingFactory { ClearType = true, UseGPU = true }));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Window.SetSoftInputMode(SoftInput.AdjustResize);
            base.OnCreate(savedInstanceState);
            ClassLibrary1.Class1.CreateNativeControl = () => new global::Android.Widget.Button(this) { Text = "原生控件" };
            SetContentView(new CPF.Android.CpfView(new ClassLibrary1.Class1()));
            //this.Window.AddContentView();

        }

    }
}