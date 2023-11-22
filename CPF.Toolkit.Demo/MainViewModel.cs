using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Toolkit.Demo
{
    internal class MainViewModel : ViewModelBase
    {
        bool isClose = false;
        public void Test()
        {
            if (this.Dialog.Ask("确定要关闭吗") == "确定")
            {
                this.isClose = true;
                this.Close();
            }
        }

        protected override void OnClose(ClosingEventArgs e)
        {
            e.Cancel = !this.isClose;
            base.OnClose(e);
        }

        public async void LoadingTest()
        {
            await this.ShowLoading(Task.Delay(3000));
            this.Dialog.Sucess("test");

            //var result = await this.ShowLoading(async () =>
            //{
            //    await Task.Delay(5000);
            //    return "test";
            //});
            //this.Dialog.Sucess(result);
        }
    }
}
