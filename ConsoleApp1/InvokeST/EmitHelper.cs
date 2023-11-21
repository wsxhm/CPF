using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Raindrops.Shared.InvokeST.ConvertMap;

namespace Raindrops.Shared.InvokeST
{
    public static class EmitHelper
    {
        private static readonly BindingFlags s_flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public static void PushNumber(this ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }
        public static void PushArgument(this ILGenerator il, int index)
        {
            if (index == 0)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            else if (index == 1)
            {
                il.Emit(OpCodes.Ldarg_1);
            }
            else if (index == 2)
            {
                il.Emit(OpCodes.Ldarg_2);
            }
            else if (index == 3)
            {
                il.Emit(OpCodes.Ldarg_3);
            }
            else if (index <= byte.MaxValue)
            {
                il.Emit(OpCodes.Ldarg_S, index);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, index);
            }
        }
        public static void PushLocal(this ILGenerator il, int index)
        {
            if (index == 0)
            {
                il.Emit(OpCodes.Ldloc_0);
            }
            else if (index == 1)
            {
                il.Emit(OpCodes.Ldloc_1);
            }
            else if (index == 2)
            {
                il.Emit(OpCodes.Ldloc_2);
            }
            else if (index == 3)
            {
                il.Emit(OpCodes.Ldloc_3);
            }
            else if (index <= byte.MaxValue)
            {
                il.Emit(OpCodes.Ldarg_S, index);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, index);
            }
        }
        public static void PopLocal(this ILGenerator il, int index)
        {
            if (index == 0)
            {
                il.Emit(OpCodes.Stloc_0);
            }
            else if (index == 1)
            {
                il.Emit(OpCodes.Stloc_1);
            }
            else if (index == 2)
            {
                il.Emit(OpCodes.Stloc_2);
            }
            else if (index == 3)
            {
                il.Emit(OpCodes.Stloc_3);
            }
            else if (index <= byte.MaxValue)
            {
                il.Emit(OpCodes.Stloc_S, index);
            }
            else
            {
                il.Emit(OpCodes.Stloc, index);
            }
        }
        public static void PushLocalRef(this ILGenerator il, int index)
        {
            if (index <= byte.MaxValue)
            {
                il.Emit(OpCodes.Ldloca_S, index);
            }
            else
            {
                il.Emit(OpCodes.Ldloca, index);
            }
        }
        public static void PushLocal(this ILGenerator il, LocalBuilder localBuilder)
        {
            PushLocal(il, localBuilder.LocalIndex);
        }
        public static void PushLocalRef(this ILGenerator il, LocalBuilder localBuilder)
        {
            PushLocalRef(il, localBuilder.LocalIndex);
        }
        public static void PopLocal(this ILGenerator il, LocalBuilder localBuilder)
        {
            PopLocal(il, localBuilder.LocalIndex);
        }
        public static void UnRef(this ILGenerator il, Type reftype)
        {
            if (reftype.IsByRef)
            {
                Type elementType = reftype.GetElementType();
                switch (Type.GetTypeCode(elementType))
                {
                    case TypeCode.Empty:
                        return;
                    case TypeCode.DBNull:
                    case TypeCode.String:
                        il.Emit(OpCodes.Ldind_Ref);
                        break;
                    case TypeCode.SByte:
                        il.Emit(OpCodes.Ldind_I1);
                        break;
                    case TypeCode.Int16:
                        il.Emit(OpCodes.Ldind_I2);
                        break;
                    case TypeCode.Int32:
                        il.Emit(OpCodes.Ldind_I4);
                        break;
                    case TypeCode.Int64:
                        il.Emit(OpCodes.Ldind_I8);
                        break;
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                        il.Emit(OpCodes.Ldind_U1);
                        break;
                    case TypeCode.UInt16:
                    case TypeCode.Char:
                        il.Emit(OpCodes.Ldind_U2);
                        break;
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldind_U4);
                        break;
                    case TypeCode.UInt64:
                        //***
                        il.Emit(OpCodes.Ldind_I8);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldind_R4);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldind_R8);
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        il.Emit(OpCodes.Ldobj, elementType);
                        break;
                    case TypeCode.Object:
                        if (elementType == typeof(IntPtr) || elementType == typeof(UIntPtr))
                        {
                            il.Emit(OpCodes.Ldind_I);
                            break;
                        }
                        else if (elementType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldobj, elementType);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldind_Ref);
                        }
                        break;
                }
            }
        }
        public static void UnBox(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }
        public static void TransferRef(this ILGenerator il, Type refType)
        {
            if (refType.IsByRef)
            {
                Type elementType = refType.GetElementType();
                switch (Type.GetTypeCode(elementType))
                {
                    case TypeCode.Empty:
                        return;
                    case TypeCode.DBNull:
                    case TypeCode.String:
                        il.Emit(OpCodes.Stind_Ref);
                        break;
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        il.Emit(OpCodes.Stind_I1);
                        break;
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Char:
                        il.Emit(OpCodes.Stind_I2);
                        break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Stind_I4);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Stind_I8);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Stind_R4);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Stind_R8);
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        il.Emit(OpCodes.Stobj, elementType);
                        break;
                    case TypeCode.Object:
                        if (elementType == typeof(IntPtr) || elementType == typeof(UIntPtr))
                        {
                            il.Emit(OpCodes.Stind_I);
                            break;
                        }
                        else if (elementType.IsValueType)
                        {
                            il.Emit(OpCodes.Stobj, elementType);
                        }
                        else
                        {
                            il.Emit(OpCodes.Stind_Ref);
                        }
                        break;
                }
            }
        }
        public static void CreateDefault(this ILGenerator il, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                    return;
                case TypeCode.DBNull:
                case TypeCode.String:
                    il.Emit(OpCodes.Ldnull);
                    break;
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Conv_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8);
                    break;
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.Object:
                    if (type == typeof(IntPtr))
                    {
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Conv_I);
                    }
                    else if (type.IsValueType)
                    {
                        if (TryGetFieldInfo(type, nameof(decimal.Zero), out FieldInfo fieldInfo) && fieldInfo.IsStatic)
                        {
                            il.Emit(OpCodes.Ldsfld, fieldInfo);
                        }
                        else
                        {
                            LocalBuilder temp = il.DeclareLocal(type);
                            il.Emit(OpCodes.Ldloca_S, temp.LocalIndex);
                            il.Emit(OpCodes.Initobj, type);
                            il.Emit(OpCodes.Ldloc, temp.LocalIndex);
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    break;
            }
        }
        public static void Convert(this ILGenerator il, ConvertItem convertItem)
        {
            if (convertItem == null) throw new ArgumentNullException(nameof(convertItem));
            if (convertItem.OpCode.HasValue)
            {
                il.Emit(convertItem.OpCode.Value);
            }
            else
            {
                convertItem.Action(il);
            }
        }
        public static void Convert(this ILGenerator il, Type source, Type target)
        {
            if (source == target)
                return;

            if (source == typeof(object) || target == typeof(object))
            {
                if (EmitConvertMap.SearchConvertItem(source, target, out ConvertItem convertItem))
                {
                    il.Convert(convertItem);
                }
                else
                {
                    if (source == typeof(object))
                    {
                        if (target != typeof(object))
                        {
                            il.UnBox(target);
                        }
                    }
                    else
                    {
                        il.BoxIfNeeded(source);
                    }
                }
                return;
            }

            if (EmitConvertMap.SearchConvertPath(source, target, out SearchResult searchResult))
            {
                foreach (ConvertItem convertItem in searchResult.Items.Select(x => x.Value))
                {
                    il.Convert(convertItem);
                }
                return;
            }
            throw new InvalidCastException($"{source.Name}-{target.Name}");
        }
        public static bool TryGetFieldInfo(Type type, string name, out FieldInfo fieldInfo)
        {
            foreach (FieldInfo info in type.GetFields(s_flags))
                if (info.Name == name)
                {
                    fieldInfo = info;
                    return true;
                }
            fieldInfo = default;
            return false;
        }
        public static void BoxIfNeeded(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Box, type);
        }
        public static void BoxIfNeeded(this ILGenerator il, LocalBuilder localBuilder)
            => BoxIfNeeded(il, localBuilder.LocalType);
        public static void Call(this ILGenerator il, MethodInfo methodInfo)
        {
            if (methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType)
                il.Emit(OpCodes.Call, methodInfo);
            else
                il.Emit(OpCodes.Callvirt, methodInfo);
        }
        public static void PushField(this ILGenerator il, FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, fieldInfo);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, fieldInfo);
            }
        }
        public static void PopField(this ILGenerator il, FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic)
            {
                il.Emit(OpCodes.Stsfld, fieldInfo);
            }
            else
            {
                il.Emit(OpCodes.Stfld, fieldInfo);
            }
        }
        public static void Ret(this ILGenerator il)
            => il.Emit(OpCodes.Ret);
        public static LocalBuilder PushThis(this ILGenerator il, Type objType)
        {
            il.Emit(OpCodes.Ldarg_0);
            if (objType.IsValueType)
            {
                il.UnBox(objType);
                LocalBuilder localBuilder = il.DeclareLocal(objType);
                il.PopLocal(localBuilder);
                il.PushLocalRef(localBuilder);
                return localBuilder;
            }
            else
            {
                return null;
            }
        }
    }
}
