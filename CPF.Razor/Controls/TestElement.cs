using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Razor.Controls
{
    public class TestElement : Microsoft.AspNetCore.Components.IComponent, IHandleEvent, IHandleAfterRender, ICustomTypeDescriptor
    {
        [Parameter]
        public string Test { get; set; }

        public void Attach(RenderHandle renderHandle)
        {
            throw new NotImplementedException();
        }

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            return "TestElement";
        }

        public string GetComponentName()
        {
            return "GetComponentName";
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(new CpfPropertyDescriptor[] { new CpfPropertyDescriptor("Pro1", false, typeof(string), null) });
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        public Task HandleEventAsync(EventCallbackWorkItem item, object arg)
        {
            throw new NotImplementedException();
        }

        public Task OnAfterRenderAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            throw new NotImplementedException();
        }
    }
    class CpfPropertyDescriptor : PropertyDescriptor
    {
        public CpfPropertyDescriptor(string name, bool readOnly, Type type, Attribute[] attributes) : base(name, attributes)
        {
            isreadonly = readOnly;
            pType = type;
        }
        public string FileTypes { get; set; }
        public bool IsAttached { get; set; }
        public bool IsDependency { get; set; }

        bool isreadonly;
        Type pType;
        public override Type ComponentType => typeof(TestElement);

        public override bool IsReadOnly => isreadonly;

        public override Type PropertyType => pType;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            if (component is UIElement element)
            {
                return element.GetPropretyValue(Name);
            }
            return null;
        }

        public override void ResetValue(object component)
        {
            //if (component is UIElement element)
            //{
            //    element.ResetValue(Name);
            //}
        }

        public override void SetValue(object component, object value)
        {
            if (component is UIElement element)
            {
                element.SetPropretyValue(Name, value);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
