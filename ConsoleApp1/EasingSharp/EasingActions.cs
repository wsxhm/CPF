using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.EasingSharp
{
    public static class EasingActions
    {
        public const double DEFAULT_EASE = 1.70158;
        public const double ELASTIC_EASE = 2 * Math.PI / 3;
        public const double ELASTIC_INOUT_EASE = 2 * Math.PI / 4.5;

        public static double EaseInSine(float x) => 1 - Math.Cos(x * Math.PI / 2);

        public static double EaseOutSine(float x) => Math.Sin(x * Math.PI / 2);

        public static double EaseInOutSine(float x) => -(Math.Cos(Math.PI * x) - 1) / 2;

        public static double EaseInQuad(float x) => x * x;

        public static double EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

        public static double EaseInOutQuad(float x) => x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2;

        public static double EaseInCubic(float x) => x * x * x;

        public static double EaseOutCubic(float x) => 1 - Math.Pow(1 - x, 3);

        public static double EaseInOutCubic(float x) =>
            x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;

        public static double EaseInQuart(float x) => x * x * x * x;

        public static double EaseOutQuart(float x) => 1 - Math.Pow(1 - x, 4);

        public static double EaseInOutQuart(float x) =>
            x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2;

        public static double EaseInQuint(float x) => x * x * x * x * x;

        public static double EaseOutQuint(float x) => 1 - Math.Pow(1 - x, 5);

        public static double EaseInOutQuint(float x) =>
            x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;

        public static double EaseInExpo(float x) => x <= 0 ? 0 : Math.Pow(2, 10 * x - 10);

        public static double EaseOutExpo(float x) => x >= 1 ? 1 : 1 - Math.Pow(2, -10 * x);

        public static double EaseInOutExpo(float x) =>
            x <= 0
                ? 0
                : x >= 1
                    ? 1
                    : x < 0.5
                        ? Math.Pow(2, 20 * x - 10) / 2
                        : (2 - Math.Pow(2, -20 * x + 10)) / 2;

        public static double EaseInCirc(float x) => 1 - Math.Sqrt(1 - Math.Pow(x, 2));

        public static double EaseOutCirc(float x) => Math.Sqrt(1 - Math.Pow(x - 1, 2));

        public static double EaseInOutCirc(float x) =>
            x < 0.5
                ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
                : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;

        public static double EaseInBack(float x) => (DEFAULT_EASE + 1) * x * x * x - DEFAULT_EASE * x * x;

        public static double EaseOutBack(float x) =>
            1 + (DEFAULT_EASE + 1) * Math.Pow(x - 1, 3) + DEFAULT_EASE * Math.Pow(x - 1, 2);

        public static double EaseInOutBack(float x)
        {
            var c2 = DEFAULT_EASE * 1.525;

            return x < 0.5
                ? Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2
                : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        public static double EaseInElastic(float x) =>
            x <= 0
                ? 0
                : x >= 1
                    ? 1
                    : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * ELASTIC_EASE);

        public static double EaseOutElastic(float x) =>
            x <= 0
                ? 0
                : x >= 1
                    ? 1
                    : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * ELASTIC_EASE) + 1;

        public static double EaseInOutElastic(float x) =>
            x <= 0
                ? 0
                : x >= 1
                    ? 1
                    : x < 0.5
                        ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * ELASTIC_INOUT_EASE)) / 2
                        : Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * ELASTIC_INOUT_EASE) / 2 + 1;

        public static double EaseInBounce(float x) => 1 - EaseOutBounce(1 - x);

        public static double EaseOutBounce(float x)
        {
            var n1 = 7.5625;
            var d1 = 2.75;

            if (x < 1 / d1)
                return n1 * x * x;
            else if (x < 2 / d1)
                return n1 * (x -= (float)1.5 / (float)d1) * x + 0.75;
            else if (x < 2.5 / d1)
                return n1 * (x -= (float)2.25 / (float)d1) * x + 0.9375;
            else
                return n1 * (x -= (float)2.625 / (float)d1) * x + 0.984375;
        }

        public static double EaseInOutBounce(float x) =>
            x < 0.5
                ? (1 - EaseOutBounce(1 - 2 * x)) / 2
                : (1 + EaseOutBounce(2 * x - 1)) / 2;

        public static double Linear(float x) => x;
    }
}
