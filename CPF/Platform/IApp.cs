using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    public interface IApp
    {
        bool IsMain { get; set; }

        event EventHandler Closed;

        void Close();

        void Show();
    }
}
