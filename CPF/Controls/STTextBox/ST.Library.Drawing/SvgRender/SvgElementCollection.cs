using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ST.Library.Drawing.SvgRender
{
    public sealed class SvgElementCollection : IEnumerable
    {
        public int Count { get; private set; }
        private SvgElement[] m_arr;

        internal SvgElementCollection(int capacity) {
            m_arr = new SvgElement[capacity];
        }

        public SvgElement this[int nIndex] {
            get {
                if (nIndex < 0 || nIndex >= this.Count) {
                    throw new ArgumentOutOfRangeException("nIndex");
                }
                return m_arr[nIndex]; 
            }
        }

        internal void Add(SvgElement ele) {
            this.EnsureSpace(1);
            m_arr[this.Count++] = ele;
        }

        private void EnsureSpace(int nCount) {
            if (this.Count + nCount <= m_arr.Length) {
                return;
            }
            SvgElement[] temp = new SvgElement[Math.Max(m_arr.Length << 1, this.Count + nCount)];
            m_arr.CopyTo(temp, 0);
            m_arr = temp;
        }

        public IEnumerator<SvgElement> GetEnumerator() {
            for (int i = 0; i < this.Count; i++) {
                yield return m_arr[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
