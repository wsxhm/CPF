using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raindrops.SharedTests.InvokeST
{
    public struct TestStruct
    {
        public int Number;
    }
    public class TestClassMethod
    {
        public int Sid { get; set; }
        public static string S_objTest(string obj_string, in string obj_string_in, out string obj_string_out, ref string obj_string_ref)
        {
            obj_string += obj_string_in;
            obj_string_out = obj_string;
            obj_string_ref = obj_string;
            return obj_string;
        }
        public static int S_numberTest(int number_int, in int number_int_in, out int number_int_out, ref int number_int_ref)
        {
            number_int += number_int_in;
            number_int_out = number_int;
            number_int_ref = number_int;
            return number_int;
        }
        public static TestStruct S_StructTest(TestStruct struct_int, in TestStruct struct_int_in, out TestStruct struct_int_out, ref TestStruct struct_int_ref)
        {
            struct_int.Number += struct_int_in.Number;
            struct_int_out = struct_int;
            struct_int_ref = struct_int;
            return struct_int;
        }
        public string objTest(string obj_string, in string obj_string_in, out string obj_string_out, ref string obj_string_ref)
        {
            obj_string = obj_string + obj_string_in + Sid;
            obj_string_out = obj_string;
            obj_string_ref = obj_string;
            return obj_string;
        }
        public int numberTest(int number_int, in int number_int_in, out int number_int_out, ref int number_int_ref)
        {
            number_int = number_int + number_int_in + Sid;
            number_int_out = number_int;
            number_int_ref = number_int;
            return number_int;
        }
        public TestStruct StructTest(TestStruct struct_int, in TestStruct struct_int_in, out TestStruct struct_int_out, ref TestStruct struct_int_ref)
        {
            struct_int.Number = struct_int.Number + struct_int_in.Number + Sid;
            struct_int_out = struct_int;
            struct_int_ref = struct_int;
            return struct_int;
        }
    }
    public class TestClassA
    {
        public string P_obj_string_wr { get; set; } = "Hello";
        public string P_obj_string_r { get; } = "Hello";
        public string F_obj_string = "Hello";
        public int P_numebr_int_wr { get; set; } = 100;
        public int P_number_int_r { get; } = 100;
        public int F_number_int = 100;
        public DateTime P_struct_datetime_wr { get; set; } = DateTime.FromBinary(0);
        public DateTime P_struct_datetime_r { get; } = DateTime.FromBinary(0);
        public DateTime F_struct_datetime = DateTime.FromBinary(0);
        public static string S_P_obj_string_wr { get; set; } = "Hello";
        public static string S_P_obj_string_r { get; } = "Hello";
        public static string S_F_obj_string = "Hello";
        public static int S_P_numebr_int_wr { get; set; } = 100;
        public static int S_P_number_int_r { get; } = 100;
        public static int S_F_number_int = 100;
        public static DateTime S_P_struct_datetime_wr { get; set; } = DateTime.FromBinary(0);
        public static DateTime S_P_struct_datetime_r { get; } = DateTime.FromBinary(0);
        public static DateTime S_F_struct_datetime = DateTime.FromBinary(0);
    }
    internal struct TestStructC
    {
        public static string S_Name = "Hello";
        public string Name;
        public int Number;
    }
    internal class TestClassB
    {
        internal string P_obj_string_wr { get; set; } = "Hello";
        internal string P_obj_string_r { get; } = "Hello";
        internal string _f_obj_string = "Hello";
        internal int P_numebr_int_wr { get; set; } = 100;
        internal int P_number_int_r { get; } = 100;
        internal int _f_number_int = 100;
        internal DateTime P_struct_datetime_wr { get; set; } = DateTime.FromBinary(0);
        internal DateTime P_struct_datetime_r { get; } = DateTime.FromBinary(0);
        internal DateTime _f_struct_datetime = DateTime.FromBinary(0);
        internal static string S_P_obj_string_wr { get; set; } = "Hello";
        internal static string S_P_obj_string_r { get; } = "Hello";
        internal static string s_f_obj_string = "Hello";
        internal static int S_P_numebr_int_wr { get; set; } = 100;
        internal static int S_P_number_int_r { get; } = 100;
        internal static int s_f_number_int = 100;
        internal static DateTime S_P_struct_datetime_wr { get; set; } = DateTime.FromBinary(0);
        internal static DateTime S_P_struct_datetime_r { get; } = DateTime.FromBinary(0);
        internal static DateTime s_f_struct_datetime = DateTime.FromBinary(0);
    }
}
