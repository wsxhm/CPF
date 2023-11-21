using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
//using System.Threading.Tasks;

namespace CPF.Input
{
    public interface IClipboard : IDataObject
    {
        void SetData(params ValueTuple<DataFormat, object>[] data);

        void Clear();
    }
}
