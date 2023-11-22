using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit.Dialogs
{
    public interface IDialogService
    {
        string Alert(string text, string title, DialogType dialogType, string defaultButton, params string[] buttons);
        void Alert(string text);
        void Sucess(string text);
        void Error(string text);
        void Warn(string text);
        string Ask(string text);
    }

    public class DialogService : IDialogService
    {
        public DialogService(Window owner)
        {
            this.owner = owner;
        }
        Window owner;

        public string Alert(string text, string title, DialogType dialogType, string defaultButton, params string[] buttons)
        {
            var view = new DialogView(text, title, dialogType, defaultButton, buttons);
            view.MarginLeft = this.owner.MarginLeft.Value - this.owner.Width.Value / 2;
            view.MarginTop = this.owner.MarginTop.Value - this.owner.Height.Value / 2;
            return view.ShowDialogSync(this.owner)?.ToString();
        }

        public void Alert(string text)
        {
            this.Alert(text, "消息", DialogType.None, "确定", "确定");
        }

        public string Ask(string text)
        {
            return this.Alert(text, "询问", DialogType.Ask, "确定", "确定", "取消");
        }

        public void Error(string text)
        {
            this.Alert(text, "错误", DialogType.Error, defaultButton: "确定", "确定");
        }

        public void Sucess(string text)
        {
            this.Alert(text, "成功", DialogType.Sucess, "确定", "确定");
        }

        public void Warn(string text)
        {
            this.Alert(text, "警告", DialogType.Warn, "确定", "确定");
        }
    }

    public enum DialogType
    {
        None,
        Sucess,
        Error,
        Ask,
        Warn
    }
}
