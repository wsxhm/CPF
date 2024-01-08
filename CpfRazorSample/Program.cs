using CPF.Platform;
using CPF.Skia;
using CPF.Windows;
using Microsoft.Extensions.Hosting;
using System;
using CPF.Razor;
using ComponentWrapperGenerator;

namespace CpfRazorSample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
                (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory())
            //, (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory())//如果需要支持Mac才需要
            //, (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
            );

            var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // Register app-specific services
                //services.AddSingleton<AppState>();
            })
            .Build();

            var window = new CPF.Controls.Window { Width = 500, Height = 500, Background = null };
            host.AddComponent<Test>(window);
            Application.Run(window);


        }

        static void Create()
        {
            var settings = new GeneratorSettings
            {
                FileHeader = @"//CPF自动生成.
            ",
                RootNamespace = "CPF.Razor.Controls",
            };

            var type = typeof(CPF.UIElement);
            var viewType = typeof(CPF.Controls.View);
            var types = type.Assembly.GetTypes();
            CpfGenerateWrapperForType(type, settings, "");
            //CpfGenerateWrapperForType(typeof(CPF.Controls.WindowFrame), settings, "");
            foreach (var item in types)
            {
                if (item.IsPublic && item.IsSubclassOf(type) && !item.IsAbstract && !item.IsGenericType && !item.IsSubclassOf(viewType) && item.GetConstructor(Array.Empty<Type>()) != null)
                {
                    var brow = item.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), true);
                    if (brow != null && brow.Length > 0 && !(brow[0] as System.ComponentModel.BrowsableAttribute).Browsable)
                    {
                        continue;
                    }
                    CpfGenerateWrapperForType(item, settings, "");
                }
            }
        }

        private static void CpfGenerateWrapperForType(Type typeToGenerate, GeneratorSettings settings, string outputFolder)
        {
            var generator = new CpfComponentWrapperGenerator(settings);
            generator.GenerateComponentWrapper(typeToGenerate, outputFolder);
            Console.WriteLine();
        }
    }
}
