using CPF.Platform;
using CPF.Skia;
using CPF.Windows;
using System;

namespace CPFApplication1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
                (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory())
                , (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory())//如果需要支持Mac才需要
                , (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
            );
            Application.Run(new Window1());
        }
    }
}
