using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace CPF.Reflection
{
    public static class EmitConvertMap
    {
        internal static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConvertItem>> s_map;
        internal static readonly Type[] s_numbers;
        static EmitConvertMap()
        {
            s_map = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConvertItem>>();
            s_numbers = new Type[10] {
                typeof(sbyte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(byte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(float),
                typeof(double),
            };
            Init();
        }
        private static bool IsUnsignedNumber(Type type)
        {
            return type == typeof(byte)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong);
        }
        private static bool IsSignedNumber(Type type)
        {
            return type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(long);
        }
        private static void Init()
        {
            OpCode[] numberConverts = new OpCode[10] {
                OpCodes.Conv_I1,
                OpCodes.Conv_I2,
                OpCodes.Conv_I4,
                OpCodes.Conv_I8,
                OpCodes.Conv_U1,
                OpCodes.Conv_U2,
                OpCodes.Conv_U4,
                OpCodes.Conv_U8,
                OpCodes.Conv_R4,
                OpCodes.Conv_R8,
            };
            ConcurrentDictionary<Type, ConvertItem> dic;
            //Single
            MethodInfo cucMethodInfo = typeof(CultureInfo).GetProperty(nameof(CultureInfo.CurrentUICulture)).GetGetMethod();
            for (int i = 0; i < s_numbers.Length; i++)
            {
                int sSize = Marshal.SizeOf(s_numbers[i]) * 8;
                dic = s_map.GetOrAdd(s_numbers[i], new ConcurrentDictionary<Type, ConvertItem>());
                //Number to Number
                for (int j = 0; j < s_numbers.Length; j++)
                {
                    int k = (i + j) % s_numbers.Length;
                    if (s_numbers[i] == s_numbers[k])
                        continue;
                    int lost = GetLost(s_numbers[i], s_numbers[k]);
                    if (s_numbers[k] == typeof(double) && IsUnsignedNumber(s_numbers[i]))
                        continue;
                    if (s_numbers[k] == typeof(float) && IsUnsignedNumber(s_numbers[i]))
                    {
                        dic.TryAdd(s_numbers[k], new ConvertItem(OpCodes.Conv_R_Un, lost, 1, k));
                        continue;
                    }
                    dic.TryAdd(s_numbers[k], new ConvertItem(numberConverts[k], lost, 1, k));
                }
                //Number to Boolen
                dic.TryAdd(typeof(bool), new ConvertItem((il) =>
                {
                    il.Emit(OpCodes.Call, cucMethodInfo);
                    il.Emit(OpCodes.Callvirt, typeof(IConvertible).GetMethod(nameof(IConvertible.ToBoolean)));
                }, sSize - 1, 4, s_numbers.Length));
                AnalyzeConverter(s_numbers[i]);
            }
            //Decimal <-> Number
            AnalyzeConverter(typeof(decimal));
            AnalyzeConverter(typeof(IntPtr));
            AnalyzeConverter(typeof(UIntPtr));

            //DateTime
            AnalyzeConverter(typeof(DateTime));

            //String
            AnalyzeConverter(typeof(string));
        }
        private static void AddConverter(Type sourceType, Type targetType, ConvertItem convertItem)
        {
            ConcurrentDictionary<Type, ConvertItem> dic = s_map.GetOrAdd(sourceType, new ConcurrentDictionary<Type, ConvertItem>());
            convertItem.Order = dic.Count;
            dic.TryAdd(targetType, convertItem);
        }
        private static void UpdateConverter(Type sourceType, Type targetType, ConvertItem convertItem)
        {
            ConcurrentDictionary<Type, ConvertItem> dic = s_map.GetOrAdd(sourceType, new ConcurrentDictionary<Type, ConvertItem>());
            convertItem.Order = dic.Count;
            dic.AddOrUpdate(targetType, convertItem, (k, v) => convertItem);
        }
        private static int GetSize(Type type)
        {
            return type == typeof(bool) ? 1 : Marshal.SizeOf(type) * 8;
        }
        static Type dateTime = typeof(DateTime);
        private static int GetLost(Type source, Type target)
        {
            try
            {
                if (source.IsValueType && target.IsValueType)
                {
                    if (source == dateTime || target == dateTime)
                    {
                        return 0;
                    }
                    int sSize = GetSize(source);
                    int tSize = GetSize(target);
                    int rSize = tSize / 2 < sSize && (IsUnsignedNumber(source) && IsSignedNumber(target)) || (IsSignedNumber(source) && IsUnsignedNumber(target)) ? 1 : 0;
                    return (tSize >= sSize ? 0 : sSize - tSize) + rSize;
                }
            }
            catch (Exception e)
            {

            }
            return 0;
        }
        public static bool SearchConvertItem(Type source, Type target, out ConvertItem convertItem)
        {
            convertItem = default;
            if (s_map.TryGetValue(source, out ConcurrentDictionary<Type, ConvertItem> dic))
            {
                return dic.TryGetValue(target, out convertItem);
            }
            return false;
        }
        public static void AnalyzeConverter(Type type)
        {
            if (type.IsByRef) throw new ArgumentException(nameof(type));

            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                //implicit
                if (methodInfo.IsSpecialName)
                {
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    if (parameterInfos.Length == 1)
                    {
                        Type cType = parameterInfos[0].ParameterType;
                        if (cType.IsByRef)
                            continue;
                        if (methodInfo.ReturnType == type && cType != type)
                        {
                            AddConverter(cType, type, new ConvertItem((il) => il.Emit(OpCodes.Call, methodInfo), GetLost(cType, type), 2));
                        }
                        else if (methodInfo.ReturnType != type && cType == type)
                        {
                            AddConverter(type, methodInfo.ReturnType, new ConvertItem((il) => il.Emit(OpCodes.Call, methodInfo), GetLost(type, methodInfo.ReturnType), 2));
                        }
                    }
                }
                //Parse
                else if (methodInfo.Name == "Parse" && methodInfo.ReturnType == type)
                {
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    Type cType = parameterInfos[0].ParameterType;
                    if (parameterInfos.Length == 1 && !cType.IsByRef)
                    {
                        AddConverter(cType, type, new ConvertItem((il) => il.Emit(OpCodes.Call, methodInfo), GetLost(cType, type), 2));
                    }
                }
            }

            // IConvertible
            if (typeof(IConvertible).IsAssignableFrom(type))
            {
                Type[] numbers = new Type[10] {
                    typeof(sbyte),
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(bool),
                    typeof(string)
                };
                MethodInfo[] funcs = new MethodInfo[10] {
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToSByte)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToInt16)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToInt32)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToInt64)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToByte)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToUInt16)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToUInt32)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToUInt64)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToBoolean)),
                    typeof(IConvertible).GetMethod(nameof(IConvertible.ToString))
                };

                MethodInfo cucMethodInfo = typeof(CultureInfo).GetProperty(nameof(CultureInfo.CurrentUICulture)).GetGetMethod();
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (type == numbers[i])
                        continue;
                    MethodInfo methodInfo = funcs[i];
                    AddConverter(type, numbers[i], new ConvertItem((il) =>
                    {
                        il.Emit(OpCodes.Call, cucMethodInfo);
                        il.Emit(OpCodes.Callvirt, methodInfo);
                    }, GetLost(type, numbers[i]), 4));
                }
            }
        }
        public static bool SearchConvertPath(Type source, Type target, out SearchResult searchResult)
        {
            searchResult = default;
            if (!s_map.TryGetValue(source, out ConcurrentDictionary<Type, ConvertItem> value) || value.Keys.Count == 0)
                return false;

            List<SearchNode> nodes = new List<SearchNode>();
            List<Type> logs = new List<Type>();
            foreach (KeyValuePair<Type, ConvertItem> kv in value.ToArray().OrderByDescending(a => a.Value.Order))
            {
                int lost = GetLost(source, kv.Key);
                nodes.Add(new SearchNode()
                {
                    ParentIndex = -1,
                    Target = kv.Key,
                    ConvertItem = kv.Value,
                    ConsumptionWeight = kv.Value.ConsumptionWeight,
                    Inherit = lost > 0 ? kv.Key : source,
                    LostWeight = lost,
                    Deep = 1
                });
            }
            int currentDeep = 1;
            for (int i = 0; i < nodes.Count; i++)
            {
                SearchNode current = nodes[i];

                if (searchResult != null)
                {
                    if (searchResult.LostWeight < current.LostWeight)
                        continue;
                    if (searchResult.LostWeight == current.LostWeight)
                    {
                        if (searchResult.Length < current.Deep)
                            continue;
                        if (searchResult.Length == current.Deep)
                        {
                            if (searchResult.ConsumptionWeight <= current.ConsumptionWeight)
                                continue;
                        }
                    }
                }

                if (current.Target == target)
                {
                    KeyValuePair<Type, ConvertItem>[] convertItems = new KeyValuePair<Type, ConvertItem>[current.Deep];
                    SearchNode t = current;
                    for (int k = 0; k < convertItems.Length; k++)
                    {
                        convertItems[convertItems.Length - k - 1] = new KeyValuePair<Type, ConvertItem>(t.Target, t.ConvertItem);
                        if (t.ParentIndex >= 0) t = nodes[t.ParentIndex];
                    }

                    searchResult = new SearchResult()
                    {
                        ConsumptionWeight = current.ConsumptionWeight,
                        LostWeight = current.LostWeight,
                        Items = convertItems
                    };
                }
                else
                {
                    if (s_map.TryGetValue(current.Target, out ConcurrentDictionary<Type, ConvertItem> cur))
                    {
                        int deep = current.Deep + 1;
                        if (currentDeep < deep)
                        {
                            logs.AddRange(nodes.Where(x => x.Deep == currentDeep).Select(y => y.Target));
                            currentDeep = deep;
                        }
                        foreach (KeyValuePair<Type, ConvertItem> kv in cur.ToArray().OrderByDescending(a => a.Value.Order))
                        {
                            if (logs.Contains(kv.Key))
                                continue;
                            int lost = GetLost(current.Inherit, kv.Key);
                            nodes.Add(new SearchNode()
                            {
                                ParentIndex = i,
                                Target = kv.Key,
                                ConvertItem = kv.Value,
                                ConsumptionWeight = current.ConsumptionWeight + kv.Value.ConsumptionWeight,
                                Inherit = lost > 0 ? current.Inherit : kv.Key,
                                LostWeight = current.LostWeight + lost,
                                Deep = deep
                            });
                        }
                    }
                }
            }

            return searchResult != default;
        }
    }
}
