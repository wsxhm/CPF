using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public class LinkStyleMonitor : ITextStyleMonitor
    {
        private Regex LinkRegex { get; set; }
        public TextStyle Style { get; set; }

        private List<TextStyleRange> m_lst;

        public LinkStyleMonitor() {
            this.LinkRegex = new Regex(@"https?://[a-zA-Z0-9-./#?&%+_]+");
            this.Style = new TextStyle() {
                ForeColor = Color.Blue,
                UnderLineColor = Color.Blue,
                FontStyle = FontStyle.Underline
            };
        }

        public void Init(string strText) {
            List<TextStyleRange> lst = new List<TextStyleRange>();
            foreach (Match m in this.LinkRegex.Matches(strText)) {
                lst.Add(new TextStyleRange() {
                    Index = m.Index,
                    Length = m.Length,
                    Style = this.Style
                });
            }
            m_lst = TextStyleMonitor.FillDefaultStyle(lst, strText.Length, TextStyle.Empty);
        }

        public void OnSelectionChanged(TextManager textManager, int nStart, int nLen) { }

        public void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs) {
            this.Init(textManager.GetText());
        }

        public TextStyleRange GetStyleFromCharIndex(int nIndex) {
            return TextStyleMonitor.GetStyleFromCharIndex(nIndex, m_lst);
        }
    }
}
