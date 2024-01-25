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
            this.Dialog.Alert("确定删除所选的文件吗？", "确定删除", DialogType.Warn, "", "取消", "删除");
        }

        //protected override void OnClosing(ClosingEventArgs e)
        //{
        //    e.Cancel = true;
        //    base.OnClosing(e);
        //}
    }
}
