using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class ItemData : CPF.CpfObject
    {
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Introduce
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }

    public class TestClass
    {
        public string test { get; set; }
        public override string ToString()
        {
            return test;
            //return base.ToString();
        }
    }
}
