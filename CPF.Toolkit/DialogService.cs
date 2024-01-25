using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public interface IDialog
    {
        IDialogService Dialog { get; set; }
    }

    public interface IDialogService
    {
        string Alert(string text, string title, DialogType dialogType, string defaultButton, params string[] buttons);
        void Alert(string text);
        void Sucess(string text);
        void Error(string text);
        void Warn(string text);
        bool Ask(string text);
    }

    public class DialogService : IDialogService
    {
        public DialogService(Window owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
        Window owner;

        public string Alert(string text, string title, DialogType dialogType, string defaultButton, params string[] buttons)
        {
            var view = new DialogView(text, title, dialogType, defaultButton, buttons);
            var result = view.ShowDialogSync(owner);
            return result?.ToString();
        }

        public void Alert(string text)
        {
            this.Alert(text, "消息", DialogType.None, "确定", "确定");
        }

        public bool Ask(string text)
        {
            return this.Alert(text, "询问", DialogType.Ask, "确定", "确定", "取消") == "确定";
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
}
