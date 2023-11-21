using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public class TransformCollection : CpfObject, IList,
    ICollection, IEnumerable, IList<Transform>, ICollection<Transform>,
    IEnumerable<Transform>
    {
        List<Transform> list = new List<Transform>();

        object IList.this[int index]
        {
            get
            {
                return list[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Transform this[int index]
        {
            get
            {
                return list[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return list;
            }
        }

        public void Add(Transform item)
        {
            list.Add(item);
        }

        int IList.Add(object value)
        {
            list.Add((Transform)value);
            return list.Count - 1;
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(Transform item)
        {
            return list.Contains(item);
        }

        bool IList.Contains(object value)
        {
            return list.Contains((Transform)value);
        }

        public void CopyTo(Transform[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(Transform item)
        {
            return list.IndexOf(item);
        }

        int IList.IndexOf(object value)
        {
            return list.IndexOf((Transform)value);
        }

        public void Insert(int index, Transform item)
        {
            list.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            list.Insert(index, (Transform)value);
        }

        public bool Remove(Transform item)
        {
            return list.Remove(item);
        }

        public void Remove(object value)
        {
            list.Remove((Transform)value);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
