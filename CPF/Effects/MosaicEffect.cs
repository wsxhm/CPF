using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Effects
{
    /// <summary>
    /// 马赛克特效
    /// </summary>
    public class MosaicEffect : Effect
    {
        /// <summary>
        /// 马赛克特效
        /// </summary>
        public MosaicEffect() { }
        /// <summary>
        /// 马赛克大小
        /// </summary>
        [PropertyMetadata(3)]
        public int Size { get { return GetValue<int>(); } set { SetValue(value); } }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            var val = Size;
            int iWidth = bitmap.Width;
            int iHeight = bitmap.Height;
            int stdR, stdG, stdB, stdA;
            stdR = 0;
            stdG = 0;
            stdB = 0;
            stdA = 255;
            int Stride = iWidth * 4;
            unsafe
            {
                using (var lk = bitmap.Lock())
                {
                    byte* point = (byte*)lk.DataPointer;
                    for (int i = 0; i < iHeight; i++)
                    {
                        for (int j = 0; j < iWidth; j++)
                        {
                            if (i % val == 0)
                            {
                                if (j % val == 0)
                                {
                                    stdA = point[3];
                                    stdR = point[2];
                                    stdG = point[1];
                                    stdB = point[0];
                                }
                                else
                                {
                                    point[0] = (byte)stdB;
                                    point[1] = (byte)stdG;
                                    point[2] = (byte)stdR;
                                    point[3] = (byte)stdA;
                                }
                            }
                            else
                            {
                                //复制上一行  
                                byte* pTemp = point - Stride;
                                point[0] = pTemp[0];
                                point[1] = pTemp[1];
                                point[2] = pTemp[2];
                                point[3] = pTemp[3];
                            }
                            point += 4;
                        }
                        point += Stride - iWidth * 4;
                    }
                }
            }
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            dc.DrawImage(bitmap, rect, rect);
        }
    }
}
