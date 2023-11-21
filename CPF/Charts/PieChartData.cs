using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Charts
{
    public class PieChartData : CpfObject, IFormatData
    {
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public double Value
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        public ViewFill Fill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        public string Format
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }

    public interface IFormatData
    {
        /// <summary>
        /// 定义数值格式化方式
        /// </summary>
        string Format { get; set; }
    }
}
