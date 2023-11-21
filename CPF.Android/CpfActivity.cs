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
using static Java.Lang.Thread;
using Java.Lang;
using System.Threading.Tasks;
using static CPF.Android.FileSaveFragment;
using Android.Content.PM;

namespace CPF.Android
{
    public class CpfActivity : Activity, IUncaughtExceptionHandler//, FileSaveCallbacks
    {
        public CpfActivity()
        {
            Java.Lang.Thread.DefaultUncaughtExceptionHandler = this;
            if (CurrentActivity == null)
            {
                CurrentActivity = this;
                CPF.Platform.Application.Run(new CpfAndroidApp());
            }
        }

        /// <summary>
        /// 当前活动的Activity
        /// </summary>
        public static Activity CurrentActivity
        {
            get;
            private set;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        }
        protected override void OnResume()
        {
            CurrentActivity = this;
            base.OnResume();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (CurrentActivity == this)
            {
                CurrentActivity = null;
            }
        }
        /// <summary>
        /// 申请弹窗权限，可以弹到桌面，需要添加SYSTEM_ALERT_WINDOW 权限
        /// </summary>
        public void ApplyDrawOverLays()
        {
            Intent intent = new Intent();
            intent.SetAction(global::Android.Provider.Settings.ActionManageOverlayPermission);
            intent.SetData(global::Android.Net.Uri.Parse("package:" + PackageName));
            StartActivity(intent);
        }


        public virtual void UncaughtException(Thread t, Throwable e)
        {
            Console.WriteLine("java异常：线程：" + t.Name + " 堆栈：" + e.StackTrace);
        }

        //internal TaskCompletionSource<string[]> fileName;
        //protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        //{
        //    if (requestCode == 20)
        //    {
        //        var uri = data?.Data as global::Android.Net.Uri;
        //        if (fileName != null)
        //        {
        //            if (uri == null)
        //            {
        //                fileName.SetResult(new string[] {});
        //            }
        //            else
        //            {
        //                fileName.SetResult(new string[] { uri.Path });
        //            }
        //        }
        //    }
        //    base.OnActivityResult(requestCode, resultCode, data);
        //}

        //public bool onCanSave(string absolutePath, string fileName)
        //{
        //    return true;
        //}

        //public void onConfirmSave(string absolutePath, string fileName)
        //{
           
        //}

        public CpfView Main { get; set; }
    }
}