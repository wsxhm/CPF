using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ST.Library.UI.STTextBox
{
    public class KeyWordStyleMonitor : TextStyleMonitor
    {
        private Regex m_reg_keywords;
        private Regex m_reg_escape = new Regex(@"([\|\[\]\(\)\.\*\+\?\{\}\^\$\\])");

        private Dictionary<string, TextStyle> m_dic = new Dictionary<string, TextStyle>();
        private List<TextStyleRange> m_lst = new List<TextStyleRange>();

        public override void Init(string strText) {
            m_lst.Clear();
            int nIndex = 0;
            var ms = m_reg_keywords.Matches(strText);
            foreach (Match m in ms) {
                if (nIndex != m.Index) {
                    m_lst.Add(new TextStyleRange() {
                        Index = nIndex,
                        Length = m.Index - nIndex
                    });
                }
                m_lst.Add(new TextStyleRange() {
                    Index = m.Index,
                    Length = m.Length,
                    Style = m_dic[m.Value]
                });
                nIndex = m.Index + m.Length;
            }
            if (nIndex < strText.Length) {
                m_lst.Add(new TextStyleRange() {
                    Index = nIndex,
                    Length = strText.Length - nIndex
                });
            }
        }

        public void Add(string strKeyWord, TextStyle style) {
            if (m_dic.ContainsKey(strKeyWord)) {
                m_dic[strKeyWord] = style;
            } else {
                m_dic.Add(strKeyWord, style);
            }
            this.InitRegex();
        }

        public void AddRangle(string[] strKeyWords, TextStyle style) {
            foreach (var v in strKeyWords) {
                if (m_dic.ContainsKey(v)) {
                    m_dic[v] = style;
                } else {
                    m_dic.Add(v, style);
                }
            }
            this.InitRegex();
        }

        public TextStyle Get(string strKeyWord) {
            if (m_dic.ContainsKey(strKeyWord)) {
                return m_dic[strKeyWord];
            }
            return TextStyle.Empty;
        }

        public bool Remove(string strKeyWord) {
            if (!m_dic.ContainsKey(strKeyWord)) {
                return false;
            }
            m_dic.Remove(strKeyWord);
            this.InitRegex();
            return true;
        }

        private void InitRegex() {
            var arr = m_dic.Keys.ToArray();
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = m_reg_escape.Replace(arr[i], "\\$1");
            }
            m_reg_keywords = new Regex("\\b(" + string.Join("|", arr) + ")\\b");
        }

        public override void OnSelectionChanged(TextManager textManager, int nStart, int nLen) { }

        public override void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs) {
            this.Init(textManager.GetText());
        }

        public override TextStyleRange GetStyleFromCharIndex(int nIndex) {
            return TextStyleMonitor.GetStyleFromCharIndex(nIndex, m_lst);
        }
    }
}
