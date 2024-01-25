using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit.Demo
{
    internal class MainViewModel : ViewModelBase
    {
        protected override void OnLoaded()
        {

        }


        public void Test()
        {
            this.Close();
        }

        //protected override void OnClosing(ClosingEventArgs e)
        //{
        //    e.Cancel = true;
        //    base.OnClosing(e);
        //}
    }
}
