//#if NET6_0
[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(ConsoleApp1.HotReloadManager))]
//#endif
#if !NET6_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    //
    // 摘要:
    //     Indicates that certain members on a specified System.Type are accessed dynamically,
    //     for example, through System.Reflection.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
    public sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        //
        // 摘要:
        //     Gets the System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes that
        //     specifies the type of dynamically accessed members.
        public DynamicallyAccessedMemberTypes MemberTypes
        {
            get
            {
                throw null;
            }
        }

        //
        // 摘要:
        //     Initializes a new instance of the System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute
        //     class with the specified member types.
        //
        // 参数:
        //   memberTypes:
        //     The types of the dynamically accessed members.
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
        }
    }

    //
    // 摘要:
    //     Specifies the types of members that are dynamically accessed. This enumeration
    //     has a System.FlagsAttribute attribute that allows a bitwise combination of its
    //     member values.
    [Flags]
    public enum DynamicallyAccessedMemberTypes
    {
        //
        // 摘要:
        //     Specifies all members.
        All = -1,
        //
        // 摘要:
        //     Specifies no members.
        None = 0x0,
        //
        // 摘要:
        //     Specifies the default, parameterless public constructor.
        PublicParameterlessConstructor = 0x1,
        //
        // 摘要:
        //     Specifies all public constructors.
        PublicConstructors = 0x3,
        //
        // 摘要:
        //     Specifies all non-public constructors.
        NonPublicConstructors = 0x4,
        //
        // 摘要:
        //     Specifies all public methods.
        PublicMethods = 0x8,
        //
        // 摘要:
        //     Specifies all non-public methods.
        NonPublicMethods = 0x10,
        //
        // 摘要:
        //     Specifies all public fields.
        PublicFields = 0x20,
        //
        // 摘要:
        //     Specifies all non-public fields.
        NonPublicFields = 0x40,
        //
        // 摘要:
        //     Specifies all public nested types.
        PublicNestedTypes = 0x80,
        //
        // 摘要:
        //     Specifies all non-public nested types.
        NonPublicNestedTypes = 0x100,
        //
        // 摘要:
        //     Specifies all public properties.
        PublicProperties = 0x200,
        //
        // 摘要:
        //     Specifies all non-public properties.
        NonPublicProperties = 0x400,
        //
        // 摘要:
        //     Specifies all public events.
        PublicEvents = 0x800,
        //
        // 摘要:
        //     Specifies all non-public events.
        NonPublicEvents = 0x1000,
        //
        // 摘要:
        //     Specifies all interfaces implemented by the type.
        Interfaces = 0x2000
    }
}

namespace System.Reflection.Metadata
{
    using System.Diagnostics.CodeAnalysis;
    //
    // 摘要:
    //     Indicates that a type that should receive notifications of metadata updates.
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class MetadataUpdateHandlerAttribute : Attribute
    {
        //
        // 摘要:
        //     Gets the type that handles metadata updates and that should be notified when
        //     any occur.
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type HandlerType
        {
            get
            {
                throw null;
            }
        }

        //
        // 摘要:
        //     Initializes the attribute.
        //
        // 参数:
        //   handlerType:
        //     A type that handles metadata updates and that should be notified when any occur.
        public MetadataUpdateHandlerAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type handlerType)
        {
        }
    }
}
#endif
namespace ConsoleApp1
{
    using System;

    internal static class HotReloadManager
    {
        public static void ClearCache(Type[] types)
        {

        }

        public static void UpdateApplication(Type[] types)
        {
            CPF.Controls.MessageBox.Show("test" + types.Length);
        }
    }
}