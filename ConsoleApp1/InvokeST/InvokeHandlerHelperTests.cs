using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raindrops.Shared.InvokeST;
using Raindrops.SharedTests.InvokeST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Raindrops.Shared.InvokeST.Tests
{
    [TestClass()]
    public class InvokeHandlerHelperTests
    {
        private readonly BindingFlags _flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private readonly TestClassMethod _testClassMethod;
        private readonly TestClassA _testClassA;
        private readonly TestClassB _testClassB;
        public InvokeHandlerHelperTests()
        {
            _testClassMethod = new TestClassMethod
            {
                Sid = 100
            };
            _testClassA = new TestClassA();
            _testClassB = new TestClassB();

            TestClassA.S_F_obj_string = "Hello";
            TestClassA.S_F_number_int = 100;
            TestClassA.S_F_struct_datetime = DateTime.FromBinary(0);

            TestClassA.S_P_obj_string_wr = "Hello";
            TestClassA.S_P_numebr_int_wr = 100;
            TestClassA.S_P_struct_datetime_wr = DateTime.FromBinary(0);

            TestClassB.s_f_obj_string = "Hello";
            TestClassB.s_f_number_int = 100;
            TestClassB.s_f_struct_datetime = DateTime.FromBinary(0);

            TestClassB.S_P_obj_string_wr = "Hello";
            TestClassB.S_P_numebr_int_wr = 100;
            TestClassB.S_P_struct_datetime_wr = DateTime.FromBinary(0);
        }

        public InvokeHandler S_string_handler;
        public InvokeHandler S_int_handler;
        public InvokeHandler S_struct_handler;
        public InvokeHandler D_string_handler;
        public InvokeHandler D_int_handler;
        public InvokeHandler D_struct_handler;

        [TestMethod()]
        public void CreateMethodInvokerTest()
        {
            S_string_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.S_objTest), _flag)); Assert.IsNotNull(S_string_handler);
            S_int_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.S_numberTest), _flag)); Assert.IsNotNull(S_int_handler);
            S_struct_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.S_StructTest), _flag)); Assert.IsNotNull(S_struct_handler);

            D_string_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.objTest), _flag)); Assert.IsNotNull(D_string_handler);
            D_int_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.numberTest), _flag)); Assert.IsNotNull(D_int_handler);
            D_struct_handler = InvokeHandlerHelper.CreateMethodInvoker(typeof(TestClassMethod).GetMethod(nameof(TestClassMethod.StructTest), _flag)); Assert.IsNotNull(D_struct_handler);
        }
        [TestMethod()]
        public void InvokeHandler_Static_Object_InvokerTest()
        {
            string str1 = "Hello";
            string str2 = "World";
            string str_out = string.Empty;
            string str_ref = string.Empty;
            if (S_string_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { str1, str2, str_out, str_ref };
            S_string_handler(null, parameters);
            Assert.AreEqual(parameters[0], "Hello");
            Assert.AreEqual(parameters[1], "World");
            Assert.AreEqual(parameters[2], str1 + str2);
            Assert.AreEqual(parameters[3], str1 + str2);
        }
        [TestMethod()]
        public void InvokeHandler_Static_Number_InvokerTest()
        {
            int int0 = 99;
            int int1 = 1;
            int int_out = 0;
            int int_ref = 0;
            if (S_int_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { int0, int1, int_out, int_ref };
            S_int_handler(null, parameters);
            Assert.AreEqual(parameters[0], 99);
            Assert.AreEqual(parameters[1], 1);
            Assert.AreEqual(parameters[2], 100);
            Assert.AreEqual(parameters[3], 100);
        }
        [TestMethod()]
        public void InvokeHandler_Static_Struct_InvokerTest()
        {
            TestStruct struct0 = new TestStruct() { Number = 99 };
            TestStruct struct1 = new TestStruct() { Number = 1 };
            TestStruct struct_out = new TestStruct() { Number = 100 };
            TestStruct struct_ref = new TestStruct() { Number = 100 };
            if (S_struct_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { struct0, struct1, struct_out, struct_ref };
            S_struct_handler(null, parameters);
            Assert.AreEqual(((TestStruct)parameters[0]).Number, 99);
            Assert.AreEqual(((TestStruct)parameters[1]).Number, 1);
            Assert.AreEqual(((TestStruct)parameters[2]).Number, 100);
            Assert.AreEqual(((TestStruct)parameters[3]).Number, 100);
        }
        [TestMethod()]
        public void InvokeHandler_Object_InvokerTest()
        {
            string str1 = "Hello";
            string str2 = "World";
            string str_out = string.Empty;
            string str_ref = string.Empty;
            if (D_string_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { str1, str2, str_out, str_ref };
            D_string_handler(_testClassMethod, parameters);
            Assert.AreEqual(parameters[0], "Hello");
            Assert.AreEqual(parameters[1], "World");
            Assert.AreEqual(parameters[2], str1 + str2 + _testClassMethod.Sid);
            Assert.AreEqual(parameters[3], str1 + str2 + _testClassMethod.Sid);
        }
        [TestMethod()]
        public void InvokeHandler_Number_InvokerTest()
        {
            int int0 = 99;
            int int1 = 1;
            int int_out = 0;
            int int_ref = 0;
            if (D_int_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { int0, int1, int_out, int_ref };
            D_int_handler(_testClassMethod, parameters);
            Assert.AreEqual(parameters[0], 99);
            Assert.AreEqual(parameters[1], 1);
            Assert.AreEqual(parameters[2], 100 + _testClassMethod.Sid);
            Assert.AreEqual(parameters[3], 100 + _testClassMethod.Sid);
        }
        [TestMethod()]
        public void InvokeHandler_Struct_InvokerTest()
        {
            TestStruct struct0 = new TestStruct() { Number = 99 };
            TestStruct struct1 = new TestStruct() { Number = 1 };
            TestStruct struct_out = new TestStruct() { Number = 100 };
            TestStruct struct_ref = new TestStruct() { Number = 100 };
            if (D_struct_handler == null) CreateMethodInvokerTest();
            object[] parameters = new object[] { struct0, struct1, struct_out, struct_ref };
            D_struct_handler(_testClassMethod, parameters);
            Assert.AreEqual(((TestStruct)parameters[0]).Number, 99);
            Assert.AreEqual(((TestStruct)parameters[1]).Number, 1);
            Assert.AreEqual(((TestStruct)parameters[2]).Number, 100 + _testClassMethod.Sid);
            Assert.AreEqual(((TestStruct)parameters[3]).Number, 100 + _testClassMethod.Sid);
        }

        //ClassA
        GetHandler<string> _a_t_P_obj_string_wr_gethandler;
        GetHandler<string> _a_t_P_obj_string_r_gethandler;
        GetHandler<string> _a_t_F_obj_string_gethandler;

        GetHandler<long> _a_t_P_number_int_wr_gethandler;
        GetHandler<uint> _a_t_P_number_int_r_gethandler;
        GetHandler<double> _a_t_F_number_int_gethandler;

        GetHandler<DateTime> _a_t_P_struct_datetime_wr_gethandler;
        GetHandler<DateTime> _a_t_P_struct_datetime_r_gethandler;
        GetHandler<DateTime> _a_t_F_struct_datetime_gethandler;

        GetHandler<string> _a_t_S_P_obj_string_wr_gethandler;
        GetHandler<string> _a_t_S_P_obj_string_r_gethandler;
        GetHandler<string> _a_t_S_F_obj_string_gethandler;

        GetHandler<long> _a_t_S_P_number_int_wr_gethandler;
        GetHandler<uint> _a_t_S_P_number_int_r_gethandler;
        GetHandler<double> _a_t_S_F_number_int_gethandler;

        GetHandler<DateTime> _a_t_S_P_struct_datetime_wr_gethandler;
        GetHandler<DateTime> _a_t_S_P_struct_datetime_r_gethandler;
        GetHandler<DateTime> _a_t_S_F_struct_datetime_gethandler;
        [TestMethod()]
        public void CreateGetHandlerTest1()
        {
            _a_t_P_obj_string_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_obj_string_wr), _flag));
            Assert.IsNotNull(_a_t_P_obj_string_wr_gethandler);
            _a_t_P_obj_string_r_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_obj_string_r), _flag));
            Assert.IsNotNull(_a_t_P_obj_string_r_gethandler);
            _a_t_F_obj_string_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetField(nameof(TestClassA.F_obj_string), _flag));
            Assert.IsNotNull(_a_t_F_obj_string_gethandler);

            _a_t_P_number_int_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<long>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_numebr_int_wr), _flag));
            Assert.IsNotNull(_a_t_P_number_int_wr_gethandler);
            _a_t_P_number_int_r_gethandler = InvokeHandlerHelper.CreateGetHandler<uint>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_number_int_r), _flag));
            Assert.IsNotNull(_a_t_P_number_int_r_gethandler);
            _a_t_F_number_int_gethandler = InvokeHandlerHelper.CreateGetHandler<double>(typeof(TestClassA).GetField(nameof(TestClassA.F_number_int), _flag));
            Assert.IsNotNull(_a_t_F_number_int_gethandler);

            _a_t_P_struct_datetime_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_struct_datetime_wr), _flag));
            Assert.IsNotNull(_a_t_P_struct_datetime_wr_gethandler);
            _a_t_P_struct_datetime_r_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_struct_datetime_r), _flag));
            Assert.IsNotNull(_a_t_P_struct_datetime_r_gethandler);
            _a_t_F_struct_datetime_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetField(nameof(TestClassA.F_struct_datetime), _flag));
            Assert.IsNotNull(_a_t_F_struct_datetime_gethandler);

            //Static
            _a_t_S_P_obj_string_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_obj_string_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_obj_string_wr_gethandler);
            _a_t_S_P_obj_string_r_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_obj_string_r), _flag));
            Assert.IsNotNull(_a_t_S_P_obj_string_r_gethandler);
            _a_t_S_F_obj_string_gethandler = InvokeHandlerHelper.CreateGetHandler<string>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_obj_string), _flag));
            Assert.IsNotNull(_a_t_S_F_obj_string_gethandler);

            _a_t_S_P_number_int_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<long>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_numebr_int_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_number_int_wr_gethandler);
            _a_t_S_P_number_int_r_gethandler = InvokeHandlerHelper.CreateGetHandler<uint>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_number_int_r), _flag));
            Assert.IsNotNull(_a_t_S_P_number_int_r_gethandler);
            _a_t_S_F_number_int_gethandler = InvokeHandlerHelper.CreateGetHandler<double>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_number_int), _flag));
            Assert.IsNotNull(_a_t_S_F_number_int_gethandler);

            _a_t_S_P_struct_datetime_wr_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_struct_datetime_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_struct_datetime_wr_gethandler);
            _a_t_S_P_struct_datetime_r_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_struct_datetime_r), _flag));
            Assert.IsNotNull(_a_t_S_P_struct_datetime_r_gethandler);
            _a_t_S_F_struct_datetime_gethandler = InvokeHandlerHelper.CreateGetHandler<DateTime>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_struct_datetime), _flag));
            Assert.IsNotNull(_a_t_S_F_struct_datetime_gethandler);
        }
        [TestMethod()]
        public void TGetHandler_Obj_InvokeGetTest()
        {
            if (_a_t_P_obj_string_wr_gethandler == null || _a_t_P_obj_string_r_gethandler == null || _a_t_F_obj_string_gethandler == null)
                CreateGetHandlerTest1();
            Assert.AreEqual(_a_t_P_obj_string_wr_gethandler(_testClassA), "Hello");
            Assert.AreEqual(_a_t_P_obj_string_r_gethandler(_testClassA), "Hello");
            Assert.AreEqual(_a_t_F_obj_string_gethandler(_testClassA), "Hello");
        }
        [TestMethod()]
        public void TGetHandler_Number_InvokeGetTest()
        {
            if (_a_t_P_number_int_wr_gethandler == null || _a_t_P_number_int_r_gethandler == null || _a_t_F_number_int_gethandler == null)
                CreateGetHandlerTest1();
            Assert.IsTrue(_a_t_P_number_int_wr_gethandler(_testClassA) == 100);
            Assert.IsTrue(_a_t_P_number_int_r_gethandler(_testClassA) == 100);
            Assert.IsTrue(_a_t_F_number_int_gethandler(_testClassA) == 100);
        }
        [TestMethod()]
        public void TGetHandler_Struct_InvokeGetTest()
        {
            if (_a_t_P_struct_datetime_wr_gethandler == null || _a_t_P_struct_datetime_r_gethandler == null || _a_t_F_struct_datetime_gethandler == null)
                CreateGetHandlerTest1();
            DateTime dateTime = DateTime.FromBinary(0);
            Assert.IsTrue(_a_t_P_struct_datetime_wr_gethandler(_testClassA) == dateTime);
            Assert.IsTrue(_a_t_P_struct_datetime_r_gethandler(_testClassA) == dateTime);
            Assert.IsTrue(_a_t_F_struct_datetime_gethandler(_testClassA) == dateTime);
        }
        [TestMethod()]
        public void TGetHandler_Static_Obj_InvokeGetTest()
        {
            if (_a_t_S_P_obj_string_wr_gethandler == null || _a_t_S_P_obj_string_r_gethandler == null || _a_t_S_F_obj_string_gethandler == null)
                CreateGetHandlerTest1();
            Assert.AreEqual(_a_t_S_P_obj_string_wr_gethandler(null), "Hello");
            Assert.AreEqual(_a_t_S_P_obj_string_r_gethandler(null), "Hello");
            Assert.AreEqual(_a_t_S_F_obj_string_gethandler(null), "Hello");
        }
        [TestMethod()]
        public void TGetHandler_Static_Number_InvokeGetTest()
        {
            if (_a_t_S_P_number_int_wr_gethandler == null || _a_t_S_P_number_int_r_gethandler == null || _a_t_S_F_number_int_gethandler == null)
                CreateGetHandlerTest1();
            Assert.IsTrue(_a_t_S_P_number_int_wr_gethandler(null) == 100);
            Assert.IsTrue(_a_t_S_P_number_int_r_gethandler(null) == 100);
            Assert.IsTrue(_a_t_S_F_number_int_gethandler(null) == 100);
        }
        [TestMethod()]
        public void TGetHandler_Static_Struct_InvokeGetTest()
        {
            if (_a_t_S_P_struct_datetime_wr_gethandler == null || _a_t_S_P_struct_datetime_r_gethandler == null || _a_t_S_F_struct_datetime_gethandler == null)
                CreateGetHandlerTest1();
            DateTime dateTime = DateTime.FromBinary(0);
            Assert.IsTrue(_a_t_S_P_struct_datetime_wr_gethandler(null) == dateTime);
            Assert.IsTrue(_a_t_S_P_struct_datetime_r_gethandler(null) == dateTime);
            Assert.IsTrue(_a_t_S_F_struct_datetime_gethandler(null) == dateTime);
        }

        SetHandler<string> _a_t_P_obj_string_wr_sethandler;
        SetHandler<string> _a_t_P_obj_string_r_sethandler;
        SetHandler<string> _a_t_F_obj_string_sethandler;

        SetHandler<long> _a_t_P_number_int_wr_sethandler;
        SetHandler<uint> _a_t_P_number_int_r_sethandler;
        SetHandler<double> _a_t_F_number_int_sethandler;

        SetHandler<DateTime> _a_t_P_struct_datetime_wr_sethandler;
        SetHandler<DateTime> _a_t_P_struct_datetime_r_sethandler;
        SetHandler<DateTime> _a_t_F_struct_datetime_sethandler;

        SetHandler<string> _a_t_S_P_obj_string_wr_sethandler;
        SetHandler<string> _a_t_S_P_obj_string_r_sethandler;
        SetHandler<string> _a_t_S_F_obj_string_sethandler;

        SetHandler<long> _a_t_S_P_number_int_wr_sethandler;
        SetHandler<uint> _a_t_S_P_number_int_r_sethandler;
        SetHandler<double> _a_t_S_F_number_int_sethandler;

        SetHandler<DateTime> _a_t_S_P_struct_datetime_wr_sethandler;
        SetHandler<DateTime> _a_t_S_P_struct_datetime_r_sethandler;
        SetHandler<DateTime> _a_t_S_F_struct_datetime_sethandler;
        [TestMethod()]
        public void CreateSetHandlerTest1()
        {
            _a_t_P_obj_string_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_obj_string_wr), _flag));
            Assert.IsNotNull(_a_t_P_obj_string_wr_sethandler);
            _a_t_P_obj_string_r_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_obj_string_r), _flag));
            Assert.IsNull(_a_t_P_obj_string_r_sethandler);
            _a_t_F_obj_string_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetField(nameof(TestClassA.F_obj_string), _flag));
            Assert.IsNotNull(_a_t_F_obj_string_sethandler);

            _a_t_P_number_int_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<long>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_numebr_int_wr), _flag));
            Assert.IsNotNull(_a_t_P_number_int_wr_sethandler);
            _a_t_P_number_int_r_sethandler = InvokeHandlerHelper.CreateSetHandler<uint>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_number_int_r), _flag));
            Assert.IsNull(_a_t_P_number_int_r_sethandler);
            _a_t_F_number_int_sethandler = InvokeHandlerHelper.CreateSetHandler<double>(typeof(TestClassA).GetField(nameof(TestClassA.F_number_int), _flag));
            Assert.IsNotNull(_a_t_F_number_int_sethandler);

            _a_t_P_struct_datetime_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_struct_datetime_wr), _flag));
            Assert.IsNotNull(_a_t_P_struct_datetime_wr_sethandler);
            _a_t_P_struct_datetime_r_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.P_struct_datetime_r), _flag));
            Assert.IsNull(_a_t_P_struct_datetime_r_sethandler);
            _a_t_F_struct_datetime_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetField(nameof(TestClassA.F_struct_datetime), _flag));
            Assert.IsNotNull(_a_t_F_struct_datetime_sethandler);

            //Static
            _a_t_S_P_obj_string_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_obj_string_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_obj_string_wr_sethandler);
            _a_t_S_P_obj_string_r_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_obj_string_r), _flag));
            Assert.IsNull(_a_t_S_P_obj_string_r_sethandler);
            _a_t_S_F_obj_string_sethandler = InvokeHandlerHelper.CreateSetHandler<string>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_obj_string), _flag));

            _a_t_S_P_number_int_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<long>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_numebr_int_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_number_int_wr_sethandler);
            _a_t_S_P_number_int_r_sethandler = InvokeHandlerHelper.CreateSetHandler<uint>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_number_int_r), _flag));
            Assert.IsNull(_a_t_S_P_number_int_r_sethandler);
            _a_t_S_F_number_int_sethandler = InvokeHandlerHelper.CreateSetHandler<double>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_number_int), _flag));
            Assert.IsNotNull(_a_t_S_F_number_int_sethandler);

            _a_t_S_P_struct_datetime_wr_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_struct_datetime_wr), _flag));
            Assert.IsNotNull(_a_t_S_P_struct_datetime_wr_sethandler);
            _a_t_S_P_struct_datetime_r_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetProperty(nameof(TestClassA.S_P_struct_datetime_r), _flag));
            Assert.IsNull(_a_t_S_P_struct_datetime_r_sethandler);
            _a_t_S_F_struct_datetime_sethandler = InvokeHandlerHelper.CreateSetHandler<DateTime>(typeof(TestClassA).GetField(nameof(TestClassA.S_F_struct_datetime), _flag));
            Assert.IsNotNull(_a_t_S_F_struct_datetime_sethandler);
        }
        [TestMethod()]
        public void TSetHandler_Obj_InvokeSetTest()
        {
            if (_a_t_P_obj_string_wr_sethandler == null || _a_t_F_obj_string_sethandler == null)
                CreateSetHandlerTest1();
            string ori1 = _testClassA.P_obj_string_wr;
            string ori2 = _testClassA.F_obj_string;
            _a_t_P_obj_string_wr_sethandler(_testClassA, string.Empty);
            _a_t_F_obj_string_sethandler(_testClassA, string.Empty);
            Assert.AreEqual(_testClassA.P_obj_string_wr, string.Empty);
            Assert.AreEqual(_testClassA.F_obj_string, string.Empty);
            _testClassA.P_obj_string_wr = ori1;
            _testClassA.F_obj_string = ori2;
        }
        [TestMethod()]
        public void TSetHandler_Number_InvokeSetTest()
        {
            if (_a_t_P_number_int_wr_sethandler == null || _a_t_F_number_int_sethandler == null)
                CreateSetHandlerTest1();
            int ori1 = _testClassA.P_numebr_int_wr;
            int ori2 = _testClassA.F_number_int;
            _a_t_P_number_int_wr_sethandler(_testClassA, 0);
            _a_t_F_number_int_sethandler(_testClassA, 0);
            Assert.IsTrue(_testClassA.P_numebr_int_wr == 0);
            Assert.IsTrue(_testClassA.F_number_int == 0);
            _testClassA.P_numebr_int_wr = ori1;
            _testClassA.F_number_int = ori2;
        }
        [TestMethod()]
        public void TSetHandler_Struct_InvokeSetTest()
        {
            if (_a_t_P_struct_datetime_wr_sethandler == null || _a_t_F_struct_datetime_sethandler == null)
                CreateSetHandlerTest1();
            DateTime dateTime = DateTime.FromBinary(1);
            DateTime ori1 = _testClassA.P_struct_datetime_wr;
            DateTime ori2 = _testClassA.F_struct_datetime;
            _a_t_P_struct_datetime_wr_sethandler(_testClassA, dateTime);
            _a_t_F_struct_datetime_sethandler(_testClassA, dateTime);
            Assert.IsTrue(_testClassA.P_struct_datetime_wr == dateTime);
            Assert.IsTrue(_testClassA.F_struct_datetime == dateTime);
            _testClassA.P_struct_datetime_wr = ori1;
            _testClassA.F_struct_datetime = ori2;
        }
        [TestMethod()]
        public void TSetHandler_Static_Obj_InvokeSetTest()
        {
            if (_a_t_S_P_obj_string_wr_sethandler == null || _a_t_S_F_obj_string_sethandler == null)
                CreateSetHandlerTest1();
            string ori1 = TestClassA.S_P_obj_string_wr;
            string ori2 = TestClassA.S_F_obj_string;
            _a_t_S_P_obj_string_wr_sethandler(null, string.Empty);
            _a_t_S_F_obj_string_sethandler(null, string.Empty);
            Assert.AreEqual(TestClassA.S_P_obj_string_wr, string.Empty);
            Assert.AreEqual(TestClassA.S_F_obj_string, string.Empty);
            TestClassA.S_P_obj_string_wr = ori1;
            TestClassA.S_F_obj_string = ori2;
        }
        [TestMethod()]
        public void TSetHandler_Static_Number_InvokeSetTest()
        {
            if (_a_t_S_P_number_int_wr_sethandler == null || _a_t_S_F_number_int_sethandler == null)
                CreateSetHandlerTest1();
            int ori1 = TestClassA.S_P_numebr_int_wr;
            int ori2 = TestClassA.S_F_number_int;
            _a_t_S_P_number_int_wr_sethandler(null, 0);
            _a_t_S_F_number_int_sethandler(null, 0);
            Assert.IsTrue(TestClassA.S_P_numebr_int_wr == 0);
            Assert.IsTrue(TestClassA.S_F_number_int == 0);
            TestClassA.S_P_numebr_int_wr = ori1;
            TestClassA.S_F_number_int = ori2;
        }
        [TestMethod()]
        public void TSetHandler_Static_Struct_InvokeSetTest()
        {
            if (_a_t_S_P_struct_datetime_wr_sethandler == null || _a_t_S_F_struct_datetime_sethandler == null)
                CreateSetHandlerTest1();
            DateTime dateTime = DateTime.FromBinary(1);
            DateTime ori1 = TestClassA.S_P_struct_datetime_wr;
            DateTime ori2 = TestClassA.S_F_struct_datetime;
            _a_t_S_P_struct_datetime_wr_sethandler(null, dateTime);
            _a_t_S_F_struct_datetime_sethandler(null, dateTime);
            Assert.IsTrue(TestClassA.S_P_struct_datetime_wr == dateTime);
            Assert.IsTrue(TestClassA.S_F_struct_datetime == dateTime);
            TestClassA.S_P_struct_datetime_wr = ori1;
            TestClassA.S_F_struct_datetime = ori2;
        }
    }
}
