using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 定义一个缓动动画时间变换接口
    /// </summary>
    public interface IEase
    {
        float EaseInCore(float normalizedTime);
    }
}
