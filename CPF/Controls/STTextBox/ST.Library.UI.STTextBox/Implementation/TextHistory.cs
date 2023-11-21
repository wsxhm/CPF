using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public class TextHistory : ITextHistory
    {
        private int m_nIndex;
        private int m_nCount;
        private int m_nMax;
        private TextHistoryRecord[][] m_arr;

        public TextHistory(int nCount) {
            m_nMax = nCount;
            m_arr = new TextHistoryRecord[nCount][];
        }

        public void SetHistory(TextHistoryRecord[] histories) {
            if (m_nIndex == m_nMax) {
                for (int i = 1; i < m_arr.Length; i++) {
                    m_arr[i - 1] = m_arr[i];
                }
                m_nIndex--;
            }
            m_arr[m_nIndex++] = histories;
            m_nCount = m_nIndex;
        }

        public TextHistoryRecord[] GetUndo() {
            if (m_nIndex == 0) { //not have history
                return null;
            }
            return m_arr[--m_nIndex];
        }

        public TextHistoryRecord[] GetRedo() {
            if (m_nIndex == m_nCount) {
                return null;
            }
            return m_arr[m_nIndex++];
        }

        public void Clear() {
            m_nIndex = m_nCount = 0;
        }
    }
}
