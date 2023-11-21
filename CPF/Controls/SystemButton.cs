using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个系统按钮
    /// </summary>
    public class SystemButton : ButtonBase
    {
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(false));
        }
    }
}
