using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public class SelectionStyleMonitor : ITextStyleMonitor
    {
        public TextStyle Style { get; set; }

        private static WordSplitter m_spliter = new WordSplitter();
        private List<TextStyleRange> m_lst = new List<TextStyleRange>();

        public SelectionStyleMonitor() {
            this.Style = new TextStyle() {
                BackColor = Color.FromArgb(50, 255, 0, 0)
            };
        }

        public void Init(string strText) {
            //throw new NotImplementedException();
        }

        public void OnSelectionChanged(TextManager textManager, int nStart, int nLen) {
            //throw new NotImplementedException()
            m_lst.Clear();
            m_lst.Add(new TextStyleRange() {
                Index = 0,
                Length = textManager.TextLength,
                Style = TextStyle.Empty
            });
            if (nLen == 0) return;
            var line = textManager.GetLineFromCharIndex(nStart);
            string strKey = string.Empty;
            m_spliter.Each(line.RawString, nStart - line.IndexOfFirstChar, (str, ns, nl) => {
                if (ns + line.IndexOfFirstChar == nStart && nLen == nl) {
                    strKey = str.Substring(ns, nl);
                }
                return false;
            });
            if (!string.IsNullOrEmpty(strKey))  {
                string strText = textManager.GetText();
                List<TextStyleRange> lst = new List<TextStyleRange>();
                foreach (Match m in Regex.Matches(strText, "\\b" + strKey + "\\b")) {
                    lst.Add(new TextStyleRange() {
                        Index = m.Index,
                        Length = m.Length,
                        Style = this.Style
                    });
                }
                m_lst = TextStyleMonitor.FillDefaultStyle(lst, strText.Length, TextStyle.Empty);
            }
        }

        public void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs) {
            //throw new NotImplementedException();
        }

        public TextStyleRange GetStyleFromCharIndex(int nIndex) {
            return TextStyleMonitor.GetStyleFromCharIndex(nIndex, m_lst);
        }
    }
}
