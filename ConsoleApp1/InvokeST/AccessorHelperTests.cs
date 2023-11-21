using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raindrops.Shared.InvokeST;
using Raindrops.SharedTests.InvokeST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raindrops.Shared.InvokeST.Tests
{
    [TestClass()]
    public class AccessorHelperTests
    {
        [TestMethod()]
        public void TryGetValueTest()
        {
            TestClassA testClassA = new TestClassA();
            TestStructC testStructC = new TestStructC() { Name = "Name", Number = 100 };
            Assert.IsTrue(testClassA.TryGetValue<string>(nameof(TestClassA.P_obj_string_wr), out _));
            Assert.IsTrue(testClassA.TryGetValue<int>(nameof(TestClassA.P_numebr_int_wr), out _));
            Assert.IsTrue(testClassA.TryGetValue<DateTime>(nameof(TestClassA.P_struct_datetime_wr), out _));

            Assert.IsTrue(testStructC.TryGetValue<string>(nameof(TestStructC.Name), out _));
            Assert.IsTrue(testStructC.TryGetValue<int>(nameof(TestStructC.Number), out _));
        }

        [TestMethod()]
        public void CreateKeyTest()
        {
            string p1 = AccessorHelper.CreateKey("Set", nameof(TestClassA.P_number_int_r), typeof(TestClassA), 0);
            string p2 = AccessorHelper.CreateKey("Get", nameof(TestClassA.P_number_int_r), typeof(TestClassA), 0);
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p2);
            Assert.AreNotEqual(p1, p2);
        }

        [TestMethod()]
        public void ClearCacheTest()
        {
            TestClassA testClassA = new TestClassA();
            testClassA.TryGetValue<string>(nameof(TestClassA.P_obj_string_wr), out _);
            Assert.IsTrue(AccessorHelper.s_map.Count > 0);
            AccessorHelper.ClearCache(typeof(TestClassA).Assembly);
            Assert.IsTrue(AccessorHelper.s_map.Count == 0);
        }

        [TestMethod()]
        public void GetGetHandlerTest()
        {
            ConcurrentDictionary<string, Delegate> dic = AccessorHelper.s_map.GetOrAdd(typeof(TestClassA), (k) => new ConcurrentDictionary<string, Delegate>());
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_obj_string_wr)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_obj_string_r)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_obj_string)));

            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_numebr_int_wr)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_number_int_r)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_number_int)));

            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_struct_datetime_wr)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_struct_datetime_r)));
            Assert.IsNotNull(AccessorHelper.GetGetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_struct_datetime)));
        }

        [TestMethod()]
        public void GetSetHandlerTest()
        {
            ConcurrentDictionary<string, Delegate> dic = AccessorHelper.s_map.GetOrAdd(typeof(TestClassA), (k) => new ConcurrentDictionary<string, Delegate>());
            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_obj_string_wr)));
            Assert.IsNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_obj_string_r)));
            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_obj_string)));

            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_numebr_int_wr)));
            Assert.IsNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_number_int_r)));
            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_number_int)));

            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_struct_datetime_wr)));
            Assert.IsNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.P_struct_datetime_r)));
            Assert.IsNotNull(AccessorHelper.GetSetHandler<int>(dic, typeof(TestClassA), nameof(TestClassA.F_struct_datetime)));
        }

        [TestMethod()]
        public void TryGetValueTest1()
        {
            TestClassA testClassA = new TestClassA();
            object testStructC = new TestStructC() { Name = "Name", Number = 100 };

            Assert.IsTrue(testClassA.TryGetValue<string>(typeof(TestClassA), nameof(TestClassA.P_obj_string_wr), out string value_1));
            Assert.AreEqual(value_1, "Hello");
            Assert.IsTrue(testStructC.TryGetValue(typeof(TestStructC), nameof(TestStructC.Name), out string value_2));
            Assert.AreEqual(value_2, "Name");
            Assert.IsTrue(testStructC.TryGetValue(typeof(TestStructC), nameof(TestStructC.Number), out int value_3));
            Assert.AreEqual(value_3, 100);
        }

        [TestMethod()]
        public void TryGetValueTest2()
        {
            Assert.IsTrue(AccessorHelper.TryGetValue(null, typeof(TestClassA), nameof(TestClassA.S_P_obj_string_wr), out string value_1));
            Assert.AreEqual(value_1, "Hello");
            object testStructC = new TestStructC() { Name = "Name", Number = 100 };
            Assert.IsTrue(AccessorHelper.TryGetValue(testStructC, typeof(TestStructC), nameof(TestStructC.S_Name), out string value_2));
            Assert.AreEqual(value_2, "Hello");
        }

        [TestMethod()]
        public void TrySetValueTest()
        {
            object testStructC = new TestStructC() { Name = string.Empty, Number = 0 };
            Assert.IsTrue(AccessorHelper.TrySetValue(testStructC, typeof(TestStructC), nameof(TestStructC.Name), "World"));
            Assert.AreEqual(((TestStructC)testStructC).Name, "World");
            Assert.IsTrue(AccessorHelper.TrySetValue(testStructC, typeof(TestStructC), nameof(TestStructC.Number), 1000));
            Assert.AreEqual(((TestStructC)testStructC).Number, 1000);
        }

        [TestMethod()]
        public void TrySetValueTest1()
        {
            string ori = TestStructC.S_Name;
            Assert.IsTrue(AccessorHelper.TrySetValue(null, typeof(TestStructC), nameof(TestStructC.S_Name), "World"));
            Assert.AreEqual(TestStructC.S_Name, "World");
            TestStructC.S_Name = ori;
        }
    }
}
