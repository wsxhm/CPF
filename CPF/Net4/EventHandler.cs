#if Net4
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF
{
    public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);
}
#endif