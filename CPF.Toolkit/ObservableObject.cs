using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CPF.Toolkit
{
    public class ObservableObject : INotifyPropertyChanged
    {
        readonly ConcurrentDictionary<string, object> Propertys = new ConcurrentDictionary<string, object> { };
        public event PropertyChangedEventHandler PropertyChanged;


        public T GetValue<T>(T defaultValue = default, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new Exception("propertyName不能为空");
            }
            if (!this.Propertys.ContainsKey(propertyName))
            {
                this.Propertys.TryAdd(propertyName, defaultValue);
            }
            return (T)this.Propertys[propertyName];
        }

        public bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new Exception("propertyName不能为空");
            }

            if (!this.Propertys.ContainsKey(propertyName))
            {
                this.Propertys.TryAdd(propertyName, value);
            }
            else
            {
                var v = this.GetValue<T>(propertyName: propertyName);
                if (EqualityComparer<T>.Default.Equals(value, v)) return false;
                this.Propertys[propertyName] = value;
            }
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        public void SetValue<T>(T value, Action<T> action, [CallerMemberName] string propertyName = null)
        {
            if (this.SetValue(value, propertyName))
            {
                action.Invoke(value);
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
