using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF
{
    /// <summary>
    /// 字符串快速检索数据，不要使用，测试的
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class TrieTree<TValue> : IEnumerable<KeyValuePair<string, TValue>>
    {
        LinkedList<KeyValuePair<string, TValue>> list = new LinkedList<KeyValuePair<string, TValue>>();
        public int Count
        {
            get { return list.Count; }
        }

        TrieTreeNode<TValue>[] Nodes = new TrieTreeNode<TValue>[256];
        /// <summary>
        /// 只能是数字、字母和_
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, TValue value)
        {
            Add(new KeyValuePair<string, TValue>(key, value));
        }
        /// <summary>
        /// 只能是数字、字母和_
        /// </summary>
        /// <param name="keyValue"></param>
        public void Add(KeyValuePair<string, TValue> keyValue)
        {
            var key = keyValue.Key;
            var value = keyValue.Value;
            TrieTreeNode<TValue>[] nodes = Nodes;
            unsafe
            {
                int len = key.Length * 2;
                if (len != 0)
                {
                    fixed (char* dest = key)
                    {
                        var bs = (byte*)dest;
                        for (int i = 0; i < len; i++)
                        {
                            var c = bs[i];
                            TrieTreeNode<TValue> node = nodes[c];
                            if (node == null)
                            {
                                node = new TrieTreeNode<TValue>();
                                //node.Key = c;
                                nodes[c] = node;
                            }
                            if (i == len - 1)
                            {
                                if (node.HasValue)
                                {
                                    throw new Exception("存在键值" + key);
                                }
                                node.Value = value;
                                node.HasValue = true;
                            }
                            else
                            {
                                if (node.Nodes == null)
                                {
                                    node.Nodes = new TrieTreeNode<TValue>[256];
                                }
                                nodes = node.Nodes;
                            }
                        }
                    }
                }
            }
            list.AddLast(keyValue);
        }

        public bool ContainsKey(string key)
        {
            TrieTreeNode<TValue>[] nodes = Nodes;
            var len = key.Length;
            for (int i = 0; i < len; i++)
            {
                var c = key[i];
                if (c >= '.' && c <= 'z')
                {
                    TrieTreeNode<TValue> node = nodes[c - '.'];
                    if (node == null)
                    {
                        break;
                    }
                    if (i == len - 1)
                    {
                        return node.HasValue;
                    }
                    else
                    {
                        nodes = node.Nodes;
                        if (nodes == null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //throw new Exception("只能是数字、字母和_");
                    return false;
                }
            }
            return false;
        }

        public void Remove(string key)
        {
            TrieTreeNode<TValue>[] nodes = Nodes;
            var len = key.Length;
            TValue value = default;
            for (int i = 0; i < len; i++)
            {
                var c = key[i];
                if (c >= '.' && c <= 'z')
                {
                    TrieTreeNode<TValue> node = nodes[c - '.'];
                    if (node == null)
                    {
                        node = new TrieTreeNode<TValue>();
                        //node.Key = c;
                        nodes[c - '.'] = node;
                    }
                    if (i == len - 1)
                    {
                        if (node.HasValue)
                        {
                            value = node.Value;
                            node.HasValue = false;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (node.Nodes == null)
                        {
                            //node.Nodes = new TrieTreeNode<TValue>[77];
                            return;
                        }
                        nodes = node.Nodes;
                    }
                }
                else
                {
                    return;
                }
            }
            var n = list.FirstOrDefault(a => a.Key == key);
            list.Remove(n);
        }
        /// <summary>
        /// 只能是数字、字母和_
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out TValue value)
        {
            TrieTreeNode<TValue>[] nodes = this.Nodes;
            //int len = key.Length;
            //var str = key.ToCharArray();
            unsafe
            {
                int len = key.Length * 2;
                if (len != 0)
                {
                    fixed (char* dest = key)
                    {
                        var bs = (byte*)dest;
                        int i = 0;
                        while (i < len)
                        {
                            var c = bs[i];
                            TrieTreeNode<TValue> node = nodes[c];
                            if (node == null)
                            {
                                break;
                            }
                            nodes = node.Nodes;
                            if (nodes == null)
                            {
                                break;
                            }
                            i++;
                        }
                        if (i == len)
                        {
                            var node = nodes[len - 1];
                            if (node != null && node.HasValue)
                            {
                                value = node.Value;
                                return true;
                            }
                        }
                    }
                }
            }
            value = default(TValue);
            return false;
        }

        public void Clear()
        {
            Nodes = new TrieTreeNode<TValue>[256];
            list.Clear();
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }

    class TrieTreeNode<TValue>
    {
        public TrieTreeNode<TValue>[] Nodes;

        public TValue Value;

        public bool HasValue;
        //public char Key;
    }

}
