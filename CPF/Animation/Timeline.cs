using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 时间轴
    /// </summary>
    public class Timeline
    {
        /// <summary>
        /// 初始化时间轴，设置到达关键帧的目标 Value 的百分比时间0-1
        /// </summary>
        /// <param name="keyTime"></param>
        public Timeline(float keyTime)
        {
            if (keyTime < 0 || keyTime > 1)
            {
                throw new Exception("KeyTime必须0-1");
            }
            KeyTime = keyTime;
        }

        /// <summary>
        ///  获取或设置应到达关键帧的目标 Value 的百分比时间。 0-1
        /// </summary>
        public float KeyTime
        {
            get;
            set;
        }

        List<KeyFrame> keyFrames = new List<KeyFrame>();
        /// <summary>
        /// 关键帧
        /// </summary>
        public List<KeyFrame> KeyFrames
        {
            get
            {
                return keyFrames;
            }
        }
    }
}
