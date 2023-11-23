using CPF.Controls;
using CPF.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class AsyncButton : Button
    {

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Triggers.Add(nameof(IsEnabled), Relation.Me, null, (nameof(Background), "224,224,224"));
        }

        public IAsyncCommand Command { get; set; }

        protected override async void OnClick(RoutedEventArgs e)
        {
            this.IsEnabled = false;
            base.OnClick(e);
            if (this.Command != null)
            {
                await this.Command.ExecuteAsync();
            }
            this.IsEnabled = true;
        }
    }
}
