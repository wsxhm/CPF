using CPF.Controls;
using CPF.Platform;
using CPF.Skia;
using CPF.Toolkit.Dialogs;
using CPF.Windows;

namespace CPF.Toolkit.Demo
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
                  (OperatingSystemType.Windows, new WindowsPlatform(false), new SkiaDrawingFactory { })
                  , (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory { UseGPU = false })
                  , (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory { UseGPU = false })
                  );

            Application.Run(ViewManager.View<TestMdiView>());
        }
    }
}
