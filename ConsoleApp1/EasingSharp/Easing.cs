using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.EasingSharp
{
    public static class Easing
    {
        /// <summary>
        /// Asynchronously ease between two values and run a callback on each step.
        /// </summary>
        /// <param name="callback">The callback to run on each tick</param>
        /// <param name="easing">缓动类型.</param>
        /// <param name="start">起始值</param>
        /// <param name="end">结束值</param>
        /// <param name="time">执行时间毫秒.</param>
        public static void Ease(Action<double> callback, EasingTypes easing, double start, double end, int time)
        {
            var timeThread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var nextFrame = (double)stopwatch.ElapsedMilliseconds;
                var wait = 1000 / 60;
                var startTick = (int)stopwatch.ElapsedMilliseconds;
                while (stopwatch.ElapsedMilliseconds - startTick <= time)
                    if (stopwatch.ElapsedMilliseconds >= nextFrame)
                    {
                        var x = ((float)stopwatch.ElapsedMilliseconds - startTick) / time;
                        if (x > 1) x = 1;
                        double eas = 0;
                        switch (easing)
                        {
                            case EasingTypes.Linear: eas = EasingActions.Linear(x); break;
                            case EasingTypes.EaseInSine: eas = EasingActions.EaseInSine(x); break;
                            case EasingTypes.EaseOutSine: eas = EasingActions.EaseOutSine(x); break;
                            case EasingTypes.EaseInOutSine: eas = EasingActions.EaseInOutSine(x); break;
                            case EasingTypes.EaseInQuad: eas = EasingActions.EaseInQuad(x); break;
                            case EasingTypes.EaseOutQuad: eas = EasingActions.EaseOutQuad(x); break;
                            case EasingTypes.EaseInOutQuad: eas = EasingActions.EaseInOutQuad(x); break;
                            case EasingTypes.EaseInCubic: eas = EasingActions.EaseInCubic(x); break;
                            case EasingTypes.EaseOutCubic: eas = EasingActions.EaseOutCubic(x); break;
                            case EasingTypes.EaseInOutCubic: eas = EasingActions.EaseInOutCubic(x); break;
                            case EasingTypes.EaseInQuart: eas = EasingActions.EaseInQuart(x); break;
                            case EasingTypes.EaseOutQuart: eas = EasingActions.EaseOutQuart(x); break;
                            case EasingTypes.EaseInOutQuart: eas = EasingActions.EaseInOutQuart(x); break;
                            case EasingTypes.EaseInQuint: eas = EasingActions.EaseInQuint(x); break;
                            case EasingTypes.EaseOutQuint: eas = EasingActions.EaseOutQuint(x); break;
                            case EasingTypes.EaseInOutQuint: eas = EasingActions.EaseInOutQuint(x); break;
                            case EasingTypes.EaseInExpo: eas = EasingActions.EaseInExpo(x); break;
                            case EasingTypes.EaseOutExpo: eas = EasingActions.EaseOutExpo(x); break;
                            case EasingTypes.EaseInOutExpo: eas = EasingActions.EaseInOutExpo(x); break;
                            case EasingTypes.EaseInCirc: eas = EasingActions.EaseInCirc(x); break;
                            case EasingTypes.EaseOutCirc: eas = EasingActions.EaseOutCirc(x); break;
                            case EasingTypes.EaseInOutCirc: eas = EasingActions.EaseInOutCirc(x); break;
                            case EasingTypes.EaseInBack: eas = EasingActions.EaseInBack(x); break;
                            case EasingTypes.EaseOutBack: eas = EasingActions.EaseOutBack(x); break;
                            case EasingTypes.EaseInOutBack: eas = EasingActions.EaseInOutBack(x); break;
                            case EasingTypes.EaseInElastic: eas = EasingActions.EaseInElastic(x); break;
                            case EasingTypes.EaseOutElastic: eas = EasingActions.EaseOutElastic(x); break;
                            case EasingTypes.EaseInOutElastic: eas = EasingActions.EaseInOutElastic(x); break;
                            case EasingTypes.EaseInBounce: eas = EasingActions.EaseInBounce(x); break;
                            case EasingTypes.EaseOutBounce: eas = EasingActions.EaseOutBounce(x); break;
                            case EasingTypes.EaseInOutBounce: eas = EasingActions.EaseInOutBounce(x); break;
                        };

                        callback(start + eas * (end - start));
                        nextFrame += wait;
                    }

                callback(end);
            })
            { IsBackground = true, Name = "测试" };
            timeThread.Start();
        }
    }
}
