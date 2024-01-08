using CPF.Platform;
using CPF.Skia;
using CPF.Windows;
using Microsoft.Extensions.Hosting;
using System;
using CPF.Razor;

namespace CpfRazorSample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
                (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory())
                , (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory())//如果需要支持Mac才需要
                , (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
            );

            var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // Register app-specific services
                //services.AddSingleton<AppState>();
            })
            .Build();

            var window = new CPF.Controls.Window();
            host.AddComponent<Test>(window);
            Application.Run(window);
        }
    }
}
