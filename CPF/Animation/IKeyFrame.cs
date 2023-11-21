using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 定义一个关键帧接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IKeyFrame<T>
    {
        /// <summary>
        /// 获取或设置应到达关键帧的目标 Value 的时间。
        /// </summary>
        float KeyTime { get; set; }
        /// <summary>
        /// 获取或设置关键帧的目标值。
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// 获取插值
        /// </summary>
        /// <param name="keyFrameProgress">0-1</param>
        /// <returns></returns>
        T InterpolateValue(T baseValue, float keyFrameProgress);
    }
}
