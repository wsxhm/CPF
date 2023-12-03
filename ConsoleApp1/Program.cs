#if !Net4
using CPF.Skia;
#endif
using CPF.Platform;
using CPF.Windows;
using System;
using System.IO;
using System.Linq;
using SDL2;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Threading;
//using CPF.Cef;
//using LibVLCSharp.Shared;
using CPF.Controls;
using CPF.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        unsafe static void Main(string[] args)
        {
            //#if !Net4
            //            OpenGlTest openGlTest = new OpenGlTest();
            //            openGlTest.Run();
            //#endif
            //try
            //{

            Application.Initialize(
#if Net4
               (OperatingSystemType.Windows, new WindowsPlatform(), new CPF.GDIPlus.GDIPlusDrawingFactory { ClearType = true })
#else
            (OperatingSystemType.Windows, new WindowsPlatform(false), new SkiaDrawingFactory 
            {
                //#if NETCOREAPP3_1_OR_GREATER
                UseGPU = true
                //#endif
            })
            , (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory { UseGPU = false })
            , (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory { UseGPU = true })
#endif
            );

            //for (int i = 0; i < 5; i++)
            //{
            //    //AutoResetEvent autoEvent = new AutoResetEvent(false);
            //    Stopwatch stopwatch = new Stopwatch();
            //    stopwatch.Start();
            //    //autoEvent.WaitOne(2);
            //    //Thread.SpinWait(100000);
            //    //SpinWait.SpinUntil(() => false, 2);
            //    //Thread.Sleep(1);
            ////    Debug.WriteLine(Thread.Yield());
            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            //}
            //CPF.Cef.CefRuntime.Load();

            //var mainArgs = new CPF.Cef.CpfCefMainArgs(args);
            //var app = new CPF.Cef.CpfCefApp();
            //var exitCode = CPF.Cef.CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            //if (exitCode != -1)
            //{
            //    return;
            //}

            //CPF.Cef.CefRuntime.Initialize(mainArgs, new CPF.Cef.CefSettings(), app, IntPtr.Zero);
            //Application.Run(new Window3());
            //CPF.Cef.CefRuntime.Shutdown();

            //Application.Run(new VideoPlayTest());
            //Application.BaseScale = 1.25f;
            //var img = (CPF.Drawing.Image)@"res://()（）.jpg";
            //var s= img.SaveToStream(CPF.Drawing.ImageFormat.Jpeg);

            //#if !NET40
            //Function(1, AppDomain.CurrentDomain.ToString());
            //#endif

            ////A.Add("a", (CPF.CpfObject c) => c.Type.Name);
            ////////Application.BaseScale = 1.5f;
            //////CPF.Animation.Storyboard.FrameRate = 250;
            var model = new MainModel();
            ////Thread.Sleep(10000);
            ////Application.AllowDeveloperTool = false;
            ////Application.DisablePopupClose = true;
            

            //data aa = new data();
            //aa.test.test.test.Name = "11111";
            //model.Test1.test = aa;

            //var test1 = new TextBlock
            //{
            //    [nameof(TextBlock.Text)] = new CPF.Obx<MainModel>(a => a.Test1.test.test.test.test.Name),
            //};
            //test1.DataContext=model;

            //aa = new data();
            //aa.test.test.test.Name = "666666";
            //model.Test1.test = aa;
            Application.Run(new Window2 { DataContext = model, CommandContext = model });

            //Application.Run(new Window
            //{
            //    Background = CPF.Drawing.Color.Red,
            //    Width = 300,
            //    Height = 300,
            //    Children = {
            //        new Panel {
            //            Width = 200,
            //            Height = 200,
            //            Background = CPF.Drawing.Color.Yellow,
            //            Children =
            //            {
            //                new Panel
            //                {
            //                    Width=100,
            //                    Height=100,
            //                    Background="#0f0",
            //                    AllowDrop = true,
            //                    Children =
            //                    {
            //                        new TextBox{ Width=100,Height=30 }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //});

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    Debug.WriteLine(e);
            //}
        }
        //#if !NET40
        static void Function(int a, string b, [CallerArgumentExpression("a")] string c = null, [CallerArgumentExpression("b")] string d = null)
        {
            Debug.WriteLine($"Called with value {a} from expression '{c}'");
            Debug.WriteLine($"Called with value {b} from expression '{d}'");
        }
        //#endif
    }

}
class tr : TextWriter
{
    public override Encoding Encoding => Encoding.Unicode;

    public override void Write(string value)
    {
        base.Write(value);
    }
}

class A
{

    public static void Add<S>(string propertyName, Expression<Func<S, object>> source)
    {
        if (source.Body.NodeType == ExpressionType.MemberAccess)
        {
            var body = source.Body as MemberExpression;
            Debug.WriteLine(body.Member.Name);
            if (body.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var body1 = body.Expression as MemberExpression;
                Debug.WriteLine(body1.Member.Name);
                if (body1.Expression.NodeType == ExpressionType.Parameter)
                {
                    var p = body1.Expression as ParameterExpression;
                    Debug.WriteLine(p.Name);
                }
            }
            else
            {

            }
        }

    }
}

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices
{

    /// <summary>
    /// Allows capturing of the expressions passed to a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.CallerArgumentExpressionAttribute" /> class.
        /// </summary>
        /// <param name="parameterName">The name of the targeted parameter.</param>
        public CallerArgumentExpressionAttribute(string parameterName) => this.ParameterName = parameterName;

        /// <summary>
        /// Gets the target parameter name of the <c>CallerArgumentExpression</c>.
        /// </summary>
        /// <returns>
        /// The name of the targeted parameter of the <c>CallerArgumentExpression</c>.
        /// </returns>
        public string ParameterName { get; }
    }
}
#endif