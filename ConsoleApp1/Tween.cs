using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Animator
{
    public delegate double DllcallBack(double t, double b, double c, double d);
    //隐藏式实现例子
    public interface ITween
    {
        DllcallBack this[int index]
        {
            get;
        }
        double easeIn(double t, double b, double c, double d);
        double easeOut(double t, double b, double c, double d);
        double easeInOut(double t, double b, double c, double d);
    }
    public static class Tween
    {
        public enum AnimateMode : uint
        {
            Quad,
            Cubic,
            Quart,
            Quint,
            Sine,
            Expo,
            Circ,
            Elastic,
            Back,
            Bounce
        }
        /// <summary>
        /// 创建缓动函数
        /// </summary>
        /// <param name="mode">缓动模式</param>
        /// <param name="timespan">时间</param>
        /// <param name="from">初始值</param>
        /// <param name="to">终止值</param>
        /// <param name="Step">步长</param>
        /// <returns></returns>
        public static ITween Easing_create(AnimateMode mode)
        {

            switch (mode)
            {
                case AnimateMode.Quad:
                    return new Tween.Quad();
                case AnimateMode.Cubic:
                    return new Tween.Cubic();
                case AnimateMode.Quart:
                    return new Tween.Quart();
                case AnimateMode.Quint:
                    return new Tween.Quint();
                case AnimateMode.Sine:
                    return new Tween.Sine();
                case AnimateMode.Expo:
                    return new Tween.Expo();
                case AnimateMode.Circ:
                    return new Tween.Circ();
                case AnimateMode.Elastic:
                    return new Tween.Elastic();
                case AnimateMode.Back:
                    return new Tween.Back();
                case AnimateMode.Bounce:
                    return new Tween.Bounce();
                default:
                    return new Tween.Bounce();
            }
        }

        public static float Linear(int timespan, int from, int to, int Step) {
            return to * timespan / Step + from;
        }
        public class Quad : ITween
        {
            public DllcallBack this[int i]
            {
                get {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            
            public double easeIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return -c * (t /= d) * (t - 2) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * t * t + b;
                return -c / 2 * ((--t) * (t - 2) - 1) + b;
            }
            
        }
        public class Cubic: ITween
        {
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return c * ((t = t / d - 1) * t * t + 1) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
                return c / 2 * ((t -= 2) * t * t + 2) + b;
            }
        }
        public class Quart : ITween
        {
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t * t + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return -c * ((t = t / d - 1) * t * t * t - 1) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
                return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
            }
        }
        public class Quint : ITween
        {
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t * t * t + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
                return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
            }
        }
        public class Sine : ITween
        {
            
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return c * Math.Sin(t / d * (Math.PI / 2)) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                return -c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b;
            }
        }
        public class Expo : ITween
        {
            
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if (t == 0) return b;
                if (t == d) return b + c;
                if ((t /= d / 2) < 1) return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;
                return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;
            }
        }
        public class Circ : ITween
        {
            
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1) return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;
                return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
            }
        }
        public class Elastic : ITween
        {
            
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                if (t == 0) return b; if ((t /= d) == 1) return b + c;
                double p = d * .3;
                var a = c; var s = p / 4;
                return -(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                if (t == 0) return b; if ((t /= d) == 1) return b + c; var p = d * .3;
                var a = c; var s = p / 4;
                return (a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; var p = d * (.3 * 1.5);
                var a = c; var s = p / 4;
                if (t < 1) return -.5 * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
                return a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
            }
        }
        public class Back : ITween
        {
           
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                var s = 1.70158;
                return c * (t /= d) * t * ((s + 1) * t - s) + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                var s = 1.70158;
                return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                var s = 1.70158;
                if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
                return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
            }
        }
        public class Bounce : ITween
        {
           
            public DllcallBack this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 1:
                            return easeIn;
                        case 2:
                            return easeIn;
                        case 3:
                            return easeIn;
                    }
                    return easeOut;
                }
            }
            public double easeIn(double t, double b, double c, double d)
            {
                return c - easeOut(d - t, 0, c, d) + b;
            }
            public double easeOut(double t, double b, double c, double d)
            {
                if ((t /= d) < (1 / 2.75))
                {
                    return c * (7.5625 * t * t) + b;
                }
                else if (t < (2 / 2.75))
                {
                    return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
                }
                else if (t < (2.5 / 2.75))
                {
                    return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
                }
                else
                {
                    return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
                }
            }
            public double easeInOut(double t, double b, double c, double d)
            {
                if (t < d / 2) return easeIn(t * 2, 0, c, d) * .5 + b;
                else return easeOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
            }
        }
    }
}
