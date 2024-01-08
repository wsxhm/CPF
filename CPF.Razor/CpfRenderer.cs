using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
//using Microsoft.MobileBlazorBindings.Core;

namespace CPF.Razor
{
    public class CpfRenderer : NativeComponentRenderer
    {
        public CpfRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
        }

        protected override void HandleException(Exception exception)
        {
            //MessageBox.Show(exception?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine(exception?.Message);
        }

        protected override ElementManager CreateNativeControlManager()
        {
            return new CpfElementManager();
        }

        public override Dispatcher Dispatcher => new CpfDispatcher();
    }
}
