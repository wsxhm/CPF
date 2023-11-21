using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;
using CPF.Shapes;
using CPF.Styling;
using CPF.Animation;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 左右切换的按钮
    /// </summary>
    [Description("左右切换的按钮")]
    public class Switch : ToggleButton
    {
        protected override void InitializeComponent()
        {
            IsAntiAlias = true;
            MinWidth = 20;
            MinHeight = 10;
            CornerRadius = "12";
            Children.Add(new Ellipse
            {
                MarginLeft = IsChecked == true ? FloatField.Auto : 2,
                MarginRight = IsChecked == true ? 2 : FloatField.Auto,
                Fill = "#fff",
                StrokeFill = null,
                PresenterFor = this,
                Name = "switchCore",
                Bindings =
                {
                    {
                        nameof(Size),
                        nameof(ActualSize),
                        this,
                        BindingMode.OneWay,
                        (Size s)=>new SizeField(s.Height-4,s.Height-4)
                    }
                },
            });
            Bindings.Add(nameof(Background), nameof(OffColor), this, BindingMode.OneWay, a => IsChecked == true ? OnColor : OffColor);
            Bindings.Add(nameof(Background), nameof(OnColor), this, BindingMode.OneWay, a => IsChecked == true ? OnColor : OffColor);
            Bindings.Add(nameof(CornerRadius), nameof(ActualSize), this, BindingMode.OneWay, (Size s) => (CornerRadius)(s.Height / 2).ToString());
        }

        protected override void OnChecked(EventArgs e)
        {
            base.OnChecked(e);
            if (!IsInitialized)
            {
                return;
            }
            var switchCore = FindPresenterByName<UIElement>("switchCore");
            if (switchCore)
            {
                switchCore.TransitionValue(a => a.MarginLeft, ActualSize.Width - switchCore.ActualSize.Width - 2, TimeSpan.FromSeconds(0.2), completed: () =>
                 {
                     if (IsChecked == true)
                     {
                         switchCore.MarginLeft = "auto";
                         switchCore.MarginRight = 2;
                     }
                 });
            }
            this.TransitionValue(a => a.Background, OnColor, TimeSpan.FromSeconds(0.2));
        }

        protected override void OnUnchecked(EventArgs e)
        {
            base.OnUnchecked(e);
            var switchCore = FindPresenterByName<UIElement>("switchCore");
            if (switchCore)
            {
                if (!switchCore.MarginRight.IsAuto)
                {
                    switchCore.MarginLeft = ActualSize.Width - switchCore.ActualSize.Width - 2;
                    switchCore.MarginRight = "auto";
                }
                switchCore.TransitionValue(a => a.MarginLeft, 2, TimeSpan.FromSeconds(0.2));
            }
            this.TransitionValue(a => a.Background, OffColor, TimeSpan.FromSeconds(0.2));
        }
        /// <summary>
        /// 打开时候显示的背景色
        /// </summary>
        [PropertyMetadata(typeof(Color), "64,158,255"), Description("打开时候显示的背景色")]
        public Color OnColor
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 关闭时候显示的背景色
        /// </summary>
        [PropertyMetadata(typeof(Color), "220,223,230"), Description("关闭时候显示的背景色")]
        public Color OffColor
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }
    }
}
