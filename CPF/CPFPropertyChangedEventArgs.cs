using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CPF
{

    /// <summary>
    /// 由对象池管理，请勿在属性变化事件之外保留引用
    /// </summary>
    public class CPFPropertyChangedEventArgs : EventArgs, IDisposable
    {
        private CPFPropertyChangedEventArgs(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute metadataAttribute)
        {
            this.propertyMetadata = metadataAttribute;
            this.propertyName = propertyName;
            this.newValue = NewValue;
            this.oldValue = oldValue;
        }

        static ConcurrentQueue<CPFPropertyChangedEventArgs> propertyChangedEventArgPool = new ConcurrentQueue<CPFPropertyChangedEventArgs>();
        /// <summary>
        /// 创建一个可回收的CPFPropertyChangedEventArgs
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="metadataAttribute"></param>
        /// <returns></returns>
        public static CPFPropertyChangedEventArgs Create(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute metadataAttribute)
        {
            if (!propertyChangedEventArgPool.TryDequeue(out var args))
            {
                return new CPFPropertyChangedEventArgs(propertyName, oldValue, newValue, metadataAttribute);
            }
            args.propertyMetadata = metadataAttribute;
            args.newValue = newValue;
            args.oldValue = oldValue;
            args.propertyName = propertyName;
            args.isDisposed = false;
            return args;
        }

        string propertyName;
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName
        {
            get
            {
                if (isDisposed)
                {
                    throw new Exception("对象已被回收，无法访问");
                }
                return propertyName;
            }
            //set { propertyName = value; }
        }
        object oldValue;
        /// <summary>
        /// 旧值
        /// </summary>
        public object OldValue
        {
            get
            {
                if (isDisposed)
                {
                    throw new Exception("对象已被回收，无法访问");
                }
                return oldValue;
            }
            //set { oldValue = value; }
        }
        object newValue;
        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue
        {
            get
            {
                if (isDisposed)
                {
                    throw new Exception("对象已被回收，无法访问");
                }
                return newValue;
            }
            //set { newValue = value; }
        }
        PropertyMetadataAttribute propertyMetadata;
        /// <summary>
        /// 属性元数据
        /// </summary>
        public PropertyMetadataAttribute PropertyMetadata
        {
            get
            {
                if (isDisposed)
                {
                    throw new Exception("对象已被回收，无法访问");
                }
                return propertyMetadata;
            }
            //set { propertyMetadata = value; }
        }
        /// <summary>
        /// 是否被回收，被回收了的，不能在外边保留引用
        /// </summary>
        public bool IsDisposed { get => isDisposed; }

        bool isDisposed;

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Dispose()
        {
            propertyName = null;
            oldValue = null;
            newValue = null;
            propertyMetadata = null;
            propertyChangedEventArgPool.Enqueue(this);
            isDisposed = true;
        }
    }

}
