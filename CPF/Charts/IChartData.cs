using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Charts
{
    /// <summary>
    /// 图表数据
    /// </summary>
    public interface IChartData : System.ComponentModel.INotifyPropertyChanged, IFormatData
    {
        /// <summary>
        /// 获取数据表示的填充
        /// </summary>
        ViewFill Fill { get; }

        /// <summary>
        /// 数据线的名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 绘制图形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="startIndex">数据开始</param>
        /// <param name="length">数据长度</param>
        /// <param name="rectangle">绘制范围</param>
        /// <param name="maxValue">Y轴最大值</param>
        /// <param name="minValue">Y轴最小值</param>
        /// <param name="rect"></param>
        /// <param name="renderScaling"></param>
        void Paint(DrawingContext graphics, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling);
        /// <summary>
        /// 绘制图形背景
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="startIndex">数据开始</param>
        /// <param name="length">数据长度</param>
        /// <param name="rectangle"></param>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="rect"></param>
        /// <param name="renderScaling"></param>
        void PaintBackground(DrawingContext graphics, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <returns></returns>
        double GetValue(int index);
        /// <summary>
        /// 获取该范围内数据的最大值
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        double GetMaxValue(int startIndex, int length);
        /// <summary>
        /// 获取该范围内数据的最小值
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        double GetMinValue(int startIndex, int length);
        /// <summary>
        /// 数据量
        /// </summary>
        int DataCount { get; }
        /// <summary>
        /// 设置所属的Chart控件
        /// </summary>
        void SetOwnerChart(Chart chart);
        ///// <summary>
        ///// 定义数值格式化方式
        ///// </summary>
        //string Format { get; set; }
    }
}
