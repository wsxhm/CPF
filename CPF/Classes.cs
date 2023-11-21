using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF
{
    public class Classes : IEnumerable<string>
    {
        public void Add(string c)
        {
            cs.Add(c);
        }
        public void Remove(string c)
        {
            cs.Remove(c);
        }
        public bool Contains(string c)
        {
            return cs.Contains(c);
        }

        public void Clear()
        {
            cs.Clear();
        }

        public int Count
        {
            get { return cs.Count; }
        }

        HashSet<string> cs = new HashSet<string>();

        public override string ToString()
        {
            return string.Join(",", cs.ToArray());
        }

        public IEnumerator<string> GetEnumerator()
        {
            return cs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cs.GetEnumerator();
        }

        public static implicit operator Classes(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                return new Classes();
            }
            var tem = n.Split(',');
            try
            {
                var c = new Classes();
                foreach (var item in tem)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        c.Add(item);
                    }
                }
                return c;
            }
            catch (Exception)
            {
                throw new Exception("Classes 字符串格式错误 :" + n);
            }
        }

    }
}
