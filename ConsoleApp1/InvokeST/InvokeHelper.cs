using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Raindrops.Shared.InvokeST
{
    public delegate void SetHandler<T>(object target, T value);
    public delegate T GetHandler<T>(object target);
    public delegate object InvokeHandler(object target, object[] paramters);
    public delegate void SetRefHandler<T, V>(ref T target, V value);
    public static class InvokeHandlerHelper
    {
        public static InvokeHandler CreateMethodInvoker(this MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module, true);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            ParameterInfo[] parameters = methodInfo.GetParameters();
            var parametersTypes = new Type[parameters.Length];
            LocalBuilder[] locclBuilders = new LocalBuilder[parametersTypes.Length];
            for (int i = 0; i < parametersTypes.Length; i++)
            {
                Type ptype = parameters[i].ParameterType;
                if (ptype.IsByRef)
                    ptype = ptype.GetElementType();
                locclBuilders[i] = iLGenerator.DeclareLocal(ptype, true);
                parametersTypes[i] = ptype;
            }
            for (int i = 0; i < parametersTypes.Length; i++)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.PushNumber(i);
                iLGenerator.Emit(OpCodes.Ldelem_Ref);
                iLGenerator.UnBox(parametersTypes[i]);
                iLGenerator.Emit(OpCodes.Stloc, locclBuilders[i]);
            }
            if (!methodInfo.IsStatic)
            {
                LoadThis(iLGenerator, methodInfo.DeclaringType);
            }

            for (int i = 0; i < parametersTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                    iLGenerator.Emit(OpCodes.Ldloca_S, locclBuilders[i]);
                else
                    iLGenerator.Emit(OpCodes.Ldloc, locclBuilders[i]);
            }
            iLGenerator.Call(methodInfo);

            if (methodInfo.ReturnType == typeof(void))
                iLGenerator.Emit(OpCodes.Ldnull);
            else
                iLGenerator.BoxIfNeeded(methodInfo.ReturnType);

            for (int i = 0; i < parametersTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    iLGenerator.PushNumber(i);
                    iLGenerator.Emit(OpCodes.Ldloc, locclBuilders[i]);
                    iLGenerator.BoxIfNeeded(locclBuilders[i]);
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }
            iLGenerator.Ret();
            return (InvokeHandler)dynamicMethod.CreateDelegate(typeof(InvokeHandler));
        }
        public static GetHandler<T> CreateGetHandler<T>(this PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
                return null;
            DynamicMethod dm = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(object) }, propertyInfo.DeclaringType.Module, true);
            ILGenerator il = dm.GetILGenerator();
            MethodInfo mt = propertyInfo.GetGetMethod(true);
            if (!mt.IsStatic)
            {
                il.PushThis(mt.DeclaringType);
            }
            il.Call(mt);
            il.Convert(mt.ReturnType, typeof(T));
            il.Ret();
            return dm.CreateDelegate(typeof(GetHandler<T>)) as GetHandler<T>;
        }
        public static GetHandler<T> CreateGetHandler<T>(this FieldInfo fieldInfo)
        {
            DynamicMethod dm = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(object) }, fieldInfo.DeclaringType.Module, true);
            ILGenerator il = dm.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                il.PushThis(fieldInfo.DeclaringType);
            }
            il.PushField(fieldInfo);
            il.Convert(fieldInfo.FieldType, typeof(T));
            il.Ret();
            return dm.CreateDelegate(typeof(GetHandler<T>)) as GetHandler<T>;
        }
        public static SetHandler<T> CreateSetHandler<T>(this PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
                return null;
            DynamicMethod dm = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(T) }, propertyInfo.DeclaringType.Module, true);
            ILGenerator il = dm.GetILGenerator();
            MethodInfo mt = propertyInfo.GetSetMethod(true);
            if (!mt.IsStatic)
            {
                LoadThis(il, mt.DeclaringType);
            }
            il.PushArgument(1);
            il.Convert(typeof(T), propertyInfo.PropertyType);
            il.Call(mt);
            il.Ret();
            return dm.CreateDelegate(typeof(SetHandler<T>)) as SetHandler<T>;
        }
        public static void LoadThis(ILGenerator iLGenerator, Type thisType)
        {
            if (thisType.IsValueType)
            {
                //生成一个loc.0钉住引用
                LocalBuilder fixedloc = iLGenerator.DeclareLocal(typeof(object), true);
                iLGenerator.PushArgument(0);
                iLGenerator.PopLocal(fixedloc);

                //由于传入的是object 则堆栈顶部是指向堆的指针
                iLGenerator.PushLocal(fixedloc);
                //压入一个IntPtr大小
                iLGenerator.PushNumber(IntPtr.Size);

                MethodInfo addition = typeof(IntPtr).GetMethod(nameof(IntPtr.Add), BindingFlags.Public | BindingFlags.Static);
                //此时堆栈
                //IntPtr.Size
                //native int to Object
                //调用Add方法,让native int 越过一个单位
                iLGenerator.Call(addition);
                //此时堆栈
                //native int to Struct
            }
            else
            {
                iLGenerator.PushThis(thisType);
            }
        }
        public static SetHandler<T> CreateSetHandler<T>(this FieldInfo fieldInfo)
        {
            DynamicMethod dm = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(T) }, fieldInfo.DeclaringType.Module, true);
            ILGenerator il = dm.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                LoadThis(il, fieldInfo.DeclaringType);
            }
            il.PushArgument(1);
            il.Convert(typeof(T), fieldInfo.FieldType);
            il.PopField(fieldInfo);
            il.Ret();
            return dm.CreateDelegate(typeof(SetHandler<T>)) as SetHandler<T>;
        }
    }
}
