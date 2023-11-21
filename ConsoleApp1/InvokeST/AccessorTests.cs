using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raindrops.Shared.InvokeST;
using Raindrops.SharedTests.InvokeST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raindrops.Shared.InvokeST.Tests
{
    [TestClass()]
    public class AccessorTests
    {
        private TestClassA _testClassA;
        private Accessor _accessor;
        [TestMethod()]
        public void AccessorTest()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(_testClassA);
        }

        [TestMethod()]
        public void AccessorTest1()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(null, typeof(TestClassA));
        }

        [TestMethod()]
        public void GetValueTest()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(_testClassA);

            Assert.AreEqual(_accessor.GetValue<string>(nameof(TestClassA.F_obj_string)), "Hello");
            Assert.AreEqual(_accessor.GetValue<int>(nameof(TestClassA.F_number_int)), 100);
            Assert.AreEqual(_accessor.GetValue<DateTime>(nameof(TestClassA.F_struct_datetime)), DateTime.FromBinary(0));


            Assert.AreEqual(_accessor.GetValue<string>(nameof(TestClassA.P_obj_string_wr)), "Hello");
            Assert.AreEqual(_accessor.GetValue<int>(nameof(TestClassA.P_numebr_int_wr)), 100);
            Assert.AreEqual(_accessor.GetValue<DateTime>(nameof(TestClassA.P_struct_datetime_wr)), DateTime.FromBinary(0));

            //Struct to Sting
            //Assert.AreEqual(_accessor.GetValue<string>(nameof(TestClassA.P_struct_datetime_wr)), DateTime.FromBinary(0).ToString());
            Accessor accessor = new Accessor(DateTime.Now);
            accessor.GetValue<object>(nameof(DateTime.Ticks));

            //Static
            Assert.AreEqual(_accessor.GetValue<string>(nameof(TestClassA.S_F_obj_string)), "Hello");
            Assert.AreEqual(_accessor.GetValue<int>(nameof(TestClassA.S_F_number_int)), 100);
            Assert.AreEqual(_accessor.GetValue<DateTime>(nameof(TestClassA.S_F_struct_datetime)), DateTime.FromBinary(0));

            Assert.AreEqual(_accessor.GetValue<string>(nameof(TestClassA.S_P_obj_string_wr)), "Hello");
            Assert.AreEqual(_accessor.GetValue<int>(nameof(TestClassA.S_P_numebr_int_wr)), 100);
            Assert.AreEqual(_accessor.GetValue<DateTime>(nameof(TestClassA.S_P_struct_datetime_wr)), DateTime.FromBinary(0));
        }

        [TestMethod()]
        public void SetValueTest()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(_testClassA);

            //obj
            _accessor.SetValue(nameof(TestClassA.P_obj_string_wr), "World");
            Assert.AreEqual(_testClassA.P_obj_string_wr, "World");
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.P_obj_string_r)), "World"));
            _accessor.SetValue(nameof(TestClassA.F_obj_string), "World");
            Assert.AreEqual(_testClassA.F_obj_string, "World");

            //number
            _accessor.SetValue(nameof(TestClassA.P_numebr_int_wr), 101);
            Assert.AreEqual(_testClassA.P_numebr_int_wr, 101);
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.P_number_int_r)), 101));
            _accessor.SetValue(nameof(TestClassA.F_number_int), 101);
            Assert.AreEqual(_testClassA.F_number_int, 101);

            //Struct
            _accessor.SetValue(nameof(TestClassA.P_struct_datetime_wr), DateTime.FromBinary(1));
            Assert.AreEqual(_testClassA.P_struct_datetime_wr, DateTime.FromBinary(1));
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.P_struct_datetime_r)), DateTime.FromBinary(1)));
            _accessor.SetValue(nameof(TestClassA.F_struct_datetime), DateTime.FromBinary(1));
            Assert.AreEqual(_testClassA.F_struct_datetime, DateTime.FromBinary(1));

            //Static
            //obj
            _accessor.SetValue(nameof(TestClassA.S_P_obj_string_wr), "World");
            Assert.AreEqual(TestClassA.S_P_obj_string_wr, "World");
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.S_P_obj_string_r)), "World"));
            _accessor.SetValue(nameof(TestClassA.S_F_obj_string), "World");
            Assert.AreEqual(TestClassA.S_F_obj_string, "World");

            //number
            _accessor.SetValue(nameof(TestClassA.S_P_numebr_int_wr), 101);
            Assert.AreEqual(TestClassA.S_P_numebr_int_wr, 101);
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.S_P_number_int_r)), 101));
            _accessor.SetValue(nameof(TestClassA.S_F_number_int), 101);
            Assert.AreEqual(TestClassA.S_F_number_int, 101);

            //Struct
            _accessor.SetValue(nameof(TestClassA.S_P_struct_datetime_wr), DateTime.FromBinary(1));
            Assert.AreEqual(TestClassA.S_P_struct_datetime_wr, DateTime.FromBinary(1));
            Assert.ThrowsException<ArgumentException>(() => _accessor.SetValue((nameof(TestClassA.S_P_struct_datetime_r)), DateTime.FromBinary(1)));
            _accessor.SetValue(nameof(TestClassA.S_F_struct_datetime), DateTime.FromBinary(1));
            Assert.AreEqual(TestClassA.S_F_struct_datetime, DateTime.FromBinary(1));
        }

        [TestMethod()]
        public void TryGetValueTest()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(_testClassA);
            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.P_obj_string_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.P_obj_string_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.F_obj_string), out _));

            Assert.IsTrue(_accessor.TryGetValue<int>(nameof(TestClassA.P_numebr_int_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<double>(nameof(TestClassA.P_number_int_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<long>(nameof(TestClassA.F_number_int), out _));

            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.P_struct_datetime_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.P_struct_datetime_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.F_struct_datetime), out _));

            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.S_P_obj_string_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.S_P_obj_string_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<string>(nameof(TestClassA.S_F_obj_string), out _));

            Assert.IsTrue(_accessor.TryGetValue<int>(nameof(TestClassA.S_P_numebr_int_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<double>(nameof(TestClassA.S_P_number_int_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<long>(nameof(TestClassA.S_F_number_int), out _));

            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.S_P_struct_datetime_wr), out _));
            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.S_P_struct_datetime_r), out _));
            Assert.IsTrue(_accessor.TryGetValue<DateTime>(nameof(TestClassA.S_F_struct_datetime), out _));
        }

        [TestMethod()]
        public void TrySetValueTest()
        {
            _testClassA = new TestClassA();
            _accessor = new Accessor(_testClassA);
            Assert.IsTrue(_accessor.TrySetValue<string>(nameof(TestClassA.P_obj_string_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<string>(nameof(TestClassA.P_obj_string_r), default));
            Assert.IsTrue(_accessor.TrySetValue<string>(nameof(TestClassA.F_obj_string), default));

            Assert.IsTrue(_accessor.TrySetValue<int>(nameof(TestClassA.P_numebr_int_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<double>(nameof(TestClassA.P_number_int_r), default));
            Assert.IsTrue(_accessor.TrySetValue<long>(nameof(TestClassA.F_number_int), default));

            Assert.IsTrue(_accessor.TrySetValue<DateTime>(nameof(TestClassA.P_struct_datetime_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<DateTime>(nameof(TestClassA.P_struct_datetime_r), default));
            Assert.IsTrue(_accessor.TrySetValue<DateTime>(nameof(TestClassA.F_struct_datetime), default));

            Assert.IsTrue(_accessor.TrySetValue<string>(nameof(TestClassA.S_P_obj_string_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<string>(nameof(TestClassA.S_P_obj_string_r), default));
            Assert.IsTrue(_accessor.TrySetValue<string>(nameof(TestClassA.S_F_obj_string), default));

            Assert.IsTrue(_accessor.TrySetValue<int>(nameof(TestClassA.S_P_numebr_int_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<double>(nameof(TestClassA.S_P_number_int_r), default));
            Assert.IsTrue(_accessor.TrySetValue<long>(nameof(TestClassA.S_F_number_int), default));

            Assert.IsTrue(_accessor.TrySetValue<DateTime>(nameof(TestClassA.S_P_struct_datetime_wr), default));
            Assert.IsFalse(_accessor.TrySetValue<DateTime>(nameof(TestClassA.S_P_struct_datetime_r), default));
            Assert.IsTrue(_accessor.TrySetValue<DateTime>(nameof(TestClassA.S_F_struct_datetime), default));
        }
    }
}
