using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF
{
    /// <summary>
    /// 自动切换List 和 Dictionary，如果4条记录以下的，比直接使用Dictionary稍微省一丢丢内存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HybridDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private const int CutoverPoint = 4;

        List<KeyValue<TKey, TValue>> list;
        Dictionary<TKey, TValue> dic;

        public TValue this[TKey key]
        {
            get
            {
                if (dic != null)
                {
                    return dic[key];
                }
                else if (list != null)
                {
                    //foreach (var item in list)
                    //{
                    //    if (item.Key.Equals(key))
                    //    {
                    //        return item.Value;
                    //    }
                    //}
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        if (item.Key.Equals(key))
                        {
                            return item.Value;
                            //return true;
                        }
                    }
                }
                throw new ArgumentNullException("没有元素");
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                if (dic != null)
                {
                    return dic.Keys;
                }
                //return list.Select(a => a.Key);
                throw new NotImplementedException();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                if (dic != null)
                {
                    return dic.Values;
                }
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                if (dic != null)
                {
                    return dic.Count;
                }
                else if (list != null)
                {
                    return list.Count;
                }
                return 0;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private void ChangeOver()
        {
            dic = new Dictionary<TKey, TValue>();
            foreach (var item in list)
            {
                dic.Add(item.Key, item.Value);
            }
            list = null;
        }

        public void Add(TKey key, TValue value)
        {
            //Add(new KeyValuePair<TKey, TValue>(key, value));
            if (dic != null)
            {
                dic.Add(key, value);
            }
            else
            {
                if (list == null)
                {
                    list = new List<KeyValue<TKey, TValue>>();
                    list.Add(new KeyValue<TKey, TValue>(key, value));
                }
                else
                {
                    if (list.Count + 1 >= CutoverPoint)
                    {
                        ChangeOver();
                        dic.Add(key, value);
                    }
                    else
                    {
                        list.Add(new KeyValue<TKey, TValue>(key, value));
                    }
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (dic != null)
            {
                dic.Clear();
                dic = null;
            }
            if (list != null)
            {
                list.Clear();
                list = null;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (dic != null)
            {
                return dic.Contains(item);
            }
            else if (list != null)
            {
                return list.Contains(new KeyValue<TKey, TValue>(item.Key, item.Value));
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (dic != null)
            {
                return dic.ContainsKey(key);
            }
            else if (list != null)
            {
                //return list.Any(a => a.Key.Equals(key));
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item.Key.Equals(key))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            //throw new NotImplementedException();
            if (dic != null)
            {
                (dic as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
            }
            else
            {
                //list.CopyTo(array, arrayIndex);
                int id = 0;
                for (int i = arrayIndex; i < list.Count; i++)
                {
                    var item = list[i];
                    array[id] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
                    id++;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (dic != null)
            {
                foreach (var item in dic)
                {
                    yield return item;
                }
            }
            if (list == null)
            {
                list = new List<KeyValue<TKey, TValue>>();
            }
            foreach (var item in list)
            {
                yield return new KeyValuePair<TKey, TValue>(item.Key, item.Value);
            }
        }

        public bool Remove(TKey key)
        {
            if (dic != null)
            {
                return dic.Remove(key);
            }
            else if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item.Key.Equals(key))
                    {
                        list.RemoveAt(i);
                        return true;
                    }
                }
                //foreach (var item in list)
                //{
                //    if (item.Key.Equals(key))
                //    {
                //        list.Remove(item);
                //        return true;
                //    }
                //}
                return false;
            }
            else
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key不能为null");
                }
                return false;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dic != null)
            {
                return dic.TryGetValue(key, out value);
            }
            if (list != null)
            {
                //foreach (var item in list)
                //{
                //    if (item.Key.Equals(key))
                //    {
                //        value = item.Value;
                //        return true;
                //    }
                //}
                //var length = list.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item.Key.Equals(key))
                    {
                        value = item.Value;
                        return true;
                    }
                }
            }

            value = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (dic != null)
            {
                foreach (var item in dic)
                {
                    yield return item;
                }
            }
            if (list == null)
            {
                list = new List<KeyValue<TKey, TValue>>();
            }
            foreach (var item in list)
            {
                yield return new KeyValuePair<TKey, TValue>(item.Key, item.Value);
            }
            //return list.GetEnumerator();
        }

    }
    [Serializable]
    struct KeyValue<TKey, TValue>
    {
        public KeyValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key;

        public TValue Value;

        public override bool Equals(object obj)
        {
            if (obj is KeyValue<TKey, TValue> key)
            {
                return key.Key.Equal(Key) && key.Value.Equal(Value);
            }
            return false;
            //return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            var v = 0;
            if (Value != null)
            {
                v = Value.GetHashCode();
            }
            var k = 0;
            if (Key != null)
            {
                k = Key.GetHashCode();
            }
            return v ^ k;
            //return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append('[');
            if (Key != null)
            {
                s.Append(Key.ToString());
            }
            s.Append(", ");
            if (Value != null)
            {
                s.Append(Value.ToString());
            }
            s.Append(']');
            return s.ToString();
        }
    }
}
