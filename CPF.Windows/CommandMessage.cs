﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using CPF.Json;

namespace CPF.Windows
{
    [Serializable]
    public class CommandMessage<T>
    {
        public MessageType MessageType { get; set; }

        public T Data { get; set; }


        /// <summary>
        /// 将二进制数据反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommandMessage<T> DeserializeWithBinary(byte[] data)
        {
            var json = Encoding.Unicode.GetString(data);
            var obj = JsonSerializer.ToObject<CommandMessage<T>>(json);
            return obj;
        }

        /// <summary>
        /// 将对象序列化为二进制数据 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(T obj)
        {
            var json = JsonSerializer.ToJson<T>(obj);
            var data = Encoding.Unicode.GetBytes(json);
            return data;
        }


        public static T GetObject(long gchandle)
        {
            if (IntPtr.Size == 4)
            {
                return (T)GCHandle.FromIntPtr((IntPtr)(int)gchandle).Target;
            }
            else
            {
                return (T)GCHandle.FromIntPtr((IntPtr)gchandle).Target;
            }
        }
    }

    public enum MessageType : byte
    {
        GetChildren,
        Children,
        AddChild,
        RemoveChild,
        GetProperties,
        Properties,
        FindElement,
        ClearEvent,
        ElementTree,
        ShowSelectNode,
        SetValue,
    }
    [Serializable]
    public class ElementTreeNode
    {
        public long GCHandle { get; set; }

        public Visibility Visibility { get; set; }

        public string Name { get; set; }

        public Collection<ElementTreeNode> Nodes { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return Name;
            }
            return base.ToString();
        }
    }

    [Serializable]
    public class CPFPropertyInfo
    {
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public string Value { get; set; }
        public string TypeName { get; set; }
        public long GCHandle { get; set; }
    }

    [Serializable]
    public class SetPropertyValue
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public long GCHandle { get; set; }
    }
}
