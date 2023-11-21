using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ST.Library.UI.STTextBox
{
    public class TextManager : IEnumerable<TextLine>
    {
        public int LineCount {
            get { return m_nLineCount; }
        }

        public int TextLength {
            get { return m_nTextLength; }
        }

        public TextLine this[int nIndex] {
            get {
                if (nIndex < 0 || nIndex >= m_nLineCount) {
                    throw new ArgumentOutOfRangeException("nIndex");
                }
                return m_lines[nIndex];
            }
        }

        private int m_nLineCount;
        private int m_nTextLength;
        private TextLine[] m_lines;

        public TextManager() {
            m_lines = new TextLine[2];
            m_lines[0] = new TextLine();
            m_nLineCount++;
        }

        public event TextManagerLineEventHandler LineAdded;
        public event TextManagerLineEventHandler LineRemoved;
        public event TextManagerLineEventHandler LineChanged;
        public event TextManagerTextEventHandler TextChanged;
        public event TextManagerTextEventHandler TextStartChange;
        public event EventHandler LineCountChanged;

        #region Event

        protected virtual void OnTextStartChange(TextManagerTextEventArgs e) {
            if (this.TextStartChange != null) {
                this.TextStartChange(this, e);
            }
        }

        protected virtual void OnLineAdded(TextManagerLineEventArgs e) {
            if (this.LineAdded != null) {
                this.LineAdded(this, e);
            }
        }

        protected virtual void OnRemoveLine(TextManagerLineEventArgs e) {
            if (this.LineRemoved != null) {
                this.LineRemoved(this, e);
            }
        }

        protected virtual void OnTextChanged(TextManagerTextEventArgs e) {
            if (this.TextChanged != null) {
                this.TextChanged(this, e);
            }
        }

        protected virtual void OnLineChanged(TextManagerLineEventArgs e) {
            if (this.LineChanged != null) {
                this.LineChanged(this, e);
            }
        }

        protected virtual void OnLineCountChanged(EventArgs e) {
            if (this.LineCountChanged != null) {
                this.LineCountChanged(this, e);
            }
        }

        #endregion

        public string GetText() {
            return this.GetStringBuilder(0, m_nTextLength).ToString();
        }

        public string GetText(int nIndex, int nLen) {
            return this.GetStringBuilder(nIndex, nLen).ToString();
        }

        public StringBuilder GetStringBuilder() {
            return this.GetStringBuilder(0, m_nTextLength);
        }

        public StringBuilder GetStringBuilder(int nIndex, int nLen) {
            StringBuilder sb = new StringBuilder();
            if (nLen == 0) {
                return sb;
            }
            int nIndexStart = this.GetLineIndexFromCharIndex(nIndex);
            int nIndexEnd = this.GetLineIndexFromCharIndex(nIndex + nLen);
            TextLine stLineStart = this[nIndexStart];
            TextLine stLineEnd = this[nIndexEnd];
            if (stLineStart == stLineEnd) {
                sb.Append(stLineStart.RawString.Substring(nIndex - stLineStart.IndexOfFirstChar, nLen));
                return sb;
            }
            int nLenAdded = 0;
            sb.Append(stLineStart.RawString.Substring(nIndex - stLineStart.IndexOfFirstChar));
            nLenAdded += sb.Length;
            for (int i = nIndexStart + 1; i < nIndexEnd; i++) {
                sb.Append(this[i].RawString);
                nLenAdded += this[i].RawString.Length;
            }
            sb.Append(stLineEnd.RawString.Substring(0, nLen - nLenAdded));
            return sb;
        }

        public TextHistoryRecord SetText(string strText) {
            return this.SetText(0, m_nTextLength, strText);
        }

        public TextHistoryRecord SetText(int nIndex, string strText) {
            return this.SetText(nIndex, 0, strText);
        }
        /// <summary>
        /// Modify the source text
        /// </summary>
        /// <param name="nIndex">The index from source string</param>
        /// <param name="nLen">The length of the text to be deleted</param>
        /// <param name="strText">The text that will be add</param>
        public TextHistoryRecord SetText(int nIndex, int nLen, string strText) {
            return this.SetText(nIndex, nLen, strText, false);
        }

        public TextHistoryRecord[] RunHistory(TextHistoryRecord[] histories) {
            if (histories == null || histories.Length == 0) {
                return new TextHistoryRecord[0];
            }
            int nLineCount = m_nLineCount;
            this.OnTextStartChange(new TextManagerTextEventArgs(new List<TextHistoryRecord>(histories)));
            var ret = new TextHistoryRecord[histories.Length];
            for (int i = 0; i < histories.Length; i++) {
                var h = histories[i];
                ret[i] = this.SetText(h.Index, h.OldText.Length, h.NewText, true);
            }
            if (m_nLineCount != nLineCount) {
                this.OnLineCountChanged(EventArgs.Empty);
            }
            this.OnTextChanged(new TextManagerTextEventArgs(new List<TextHistoryRecord>(ret)));
            return ret;
        }

        public int Clear() {
            int nRet = m_nTextLength;
            m_lines = new TextLine[2];
            m_lines[0] = new TextLine();
            m_nLineCount = 1;
            m_nTextLength = 0;
            return nRet;
        }

        public TextLine GetLineFromCharIndex(int nIndex) {
            nIndex = this.GetLineIndexFromCharIndex(nIndex);
            return m_lines[nIndex];
        }

        public int GetLineIndexFromCharIndex(int nIndex) {
            if (m_nLineCount == 0) {
                return -1;
            }
            if (nIndex <= 0) {
                return 0;
            }
            if (nIndex >= m_lines[m_nLineCount - 1].IndexOfFirstChar + m_lines[m_nLineCount - 1].RawString.Length) {
                return m_nLineCount - 1;
            }
            int nLeft = 0, nRight = m_nLineCount - 1, nMid = 0;
            while (nLeft <= nRight) {
                nMid = (nRight + nLeft) >> 1;
                if (nIndex >= m_lines[nMid].IndexOfFirstChar + m_lines[nMid].RawString.Length) {
                    nLeft = nMid + 1;
                } else if (nIndex < m_lines[nMid].IndexOfFirstChar) {
                    nRight = nMid - 1;
                } else {
                    return nMid;
                }
            }
            return -1;
        }

        public int GetLineIndexFromLine(TextLine line) {
            int nLeft = 0, nRight = m_nLineCount - 1, nMid = 0;
            while (nLeft <= nRight) {
                nMid = (nLeft + nRight) >> 1;
                if (line.IndexOfFirstChar < m_lines[nMid].IndexOfFirstChar) {
                    nRight = nMid - 1;
                } else if (line.IndexOfFirstChar > m_lines[nMid].IndexOfFirstChar) {
                    nLeft = nMid + 1;
                } else {
                    if (m_lines[nMid] == line) {
                        return nMid;
                    }
                    return -1;
                }
            }
            return -1;
        }

        public TextHistoryRecord[] InsertAtStart(int nLineIndex, int nLineCount, string strText) {
            if (nLineIndex < 0) {
                nLineCount += nLineCount;
                nLineIndex = 0;
            }
            if (nLineIndex >= m_nLineCount) {
                return new TextHistoryRecord[0];
            }
            if (nLineIndex + nLineCount > m_nLineCount) {
                nLineCount = m_nLineCount - nLineIndex;
            }
            TextHistoryRecord[] lst = new TextHistoryRecord[nLineCount];
            this.OnTextStartChange(new TextManagerTextEventArgs(new List<TextHistoryRecord>(lst)));
            int nAddCounter = 0;
            for (int i = 0; i < nLineCount; i++) {
                var line = m_lines[nLineIndex + i];
                line.RawString = strText + line.RawString;
                line.IndexOfFirstChar += nAddCounter;
                var thr = new TextHistoryRecord() {
                    Index = line.IndexOfFirstChar,
                    OldText = string.Empty,
                    NewText = strText
                };
                lst[i] = thr;
                this.OnLineChanged(new TextManagerLineEventArgs(nLineIndex + i, line, thr));
                nAddCounter += strText.Length;
            }
            for (int i = nLineIndex + nLineCount; i < m_nLineCount; i++) {
                m_lines[i].IndexOfFirstChar += nAddCounter;
            }
            m_nTextLength += nAddCounter;
            this.OnTextChanged(new TextManagerTextEventArgs(new List<TextHistoryRecord>(lst)));
            return lst;
        }

        public List<TextHistoryRecord> TrimStartTab(int nLineIndex, int nLineCount, int nTabSize) {
            List<TextHistoryRecord> lst = new List<TextHistoryRecord>();
            if (nLineIndex < 0) {
                nLineCount += nLineCount;
                nLineIndex = 0;
            }
            if (nLineIndex >= m_nLineCount) {
                return lst;
            }
            if (nLineIndex + nLineCount > m_nLineCount) {
                nLineCount = m_nLineCount - nLineIndex;
            }
            bool bStarted = false;
            int nRemoveCounter = 0, nRemoveLen = 0;
            for (int i = 0; i < nLineCount; i++) {
                nRemoveLen = 0;
                var thr = new TextHistoryRecord();
                var line = m_lines[nLineIndex + i];
                if (line.RawString[0] == '\t') {
                    nRemoveLen = 1;
                } else {
                    while (nRemoveLen < nTabSize) {
                        switch (line.RawString[nRemoveLen]) {
                            case ' ':
                                nRemoveLen++;
                                continue;
                            case '\t':
                                nRemoveLen++;
                                continue;
                            default:
                                break;
                        }
                        break;
                    }
                }
                line.IndexOfFirstChar -= nRemoveCounter;
                if (nRemoveLen == 0) continue;
                if (!bStarted) {
                    this.OnTextStartChange(new TextManagerTextEventArgs(lst));
                    bStarted = true;
                }
                thr.OldText = line.RawString.Substring(0, nRemoveLen);
                thr.Index = line.IndexOfFirstChar;
                thr.NewText = "";
                line.RawString = line.RawString.Substring(nRemoveLen);
                nRemoveCounter += nRemoveLen;
                this.OnLineChanged(new TextManagerLineEventArgs(nLineIndex + i, line, thr));
                lst.Add(thr);
            }
            if (lst.Count == 0) return lst;
            for (int i = nLineIndex + nLineCount; i < m_nLineCount; i++) {
                m_lines[i].IndexOfFirstChar -= nRemoveCounter;
            }
            m_nTextLength -= nRemoveCounter;
            this.OnTextChanged(new TextManagerTextEventArgs(lst));
            return lst;
        }

        #region Private

        private TextHistoryRecord SetText(int nIndex, int nLen, string strText, bool isHistory) {
            if (strText == null) {
                strText = "";
            }
            string strOld = string.Empty;
            if (nLen != 0) {
                strOld = this.GetText(nIndex, nLen);
            }
            if (strOld == strText) {
                return TextHistoryRecord.Empty;
            }
            var historyRecord = new TextHistoryRecord() {
                Index = nIndex,
                OldText = strOld,
                NewText = strText
            };
            if (!isHistory) {
                var lstHistory = new List<TextHistoryRecord>() { historyRecord };
                this.OnTextStartChange(new TextManagerTextEventArgs(lstHistory));
                historyRecord = lstHistory[0];
            }
            List<TextLine> lst = this.GetTexLines(historyRecord.NewText);
            int nIndexStartLine = this.GetLineIndexFromCharIndex(nIndex);
            int nIndexEndLine = this.GetLineIndexFromCharIndex(nIndex + nLen);
            var line_start = m_lines[nIndexStartLine];
            var line_end = m_lines[nIndexEndLine];
            string strLeft = line_start.RawString.Substring(0, nIndex - line_start.IndexOfFirstChar);
            string strRight = line_end.RawString.Substring(nIndex + nLen - line_end.IndexOfFirstChar);

            lst[0].RawString = strLeft + lst[0].RawString;
            lst[0].IndexOfFirstChar = line_start.IndexOfFirstChar;
            lst[lst.Count - 1].RawString += strRight;
            int nIncrement = historyRecord.NewText.Length - nLen;
            m_nTextLength += nIncrement;

            line_start.RawString = lst[0].RawString;
            this.OnLineChanged(new TextManagerLineEventArgs(strLeft.Length, line_start, historyRecord));

            int nShouldRemoveLines = nIndexEndLine - nIndexStartLine;
            int nShouldAddLines = lst.Count - 1 - nShouldRemoveLines;
            for (int i = nIndexStartLine + 1; i <= nIndexEndLine; i++) {
                this.OnRemoveLine(new TextManagerLineEventArgs(i, m_lines[i], historyRecord));
            }
            if (nShouldAddLines > 0) {
                this.InsertEmptyLines(nIndexStartLine, nShouldAddLines, nIncrement);
            } else {
                this.RemoveLines(nIndexStartLine + 1, -nShouldAddLines, nIncrement, historyRecord);
            }
            for (int i = 1, j = nIndexStartLine + 1; i < lst.Count; i++, j++) {
                var prev_line = m_lines[j - 1];
                m_lines[j] = lst[i];
                m_lines[j].IndexOfFirstChar = prev_line.IndexOfFirstChar + prev_line.RawString.Length;
                this.OnLineAdded(new TextManagerLineEventArgs(j, m_lines[j], historyRecord));
            }
            if (nShouldAddLines == 0) {
                for (int i = nIndexStartLine + 1; i < m_nLineCount; i++) {
                    TextLine prve_line = m_lines[i - 1];
                    m_lines[i].IndexOfFirstChar = prve_line.IndexOfFirstChar + prve_line.RawString.Length;
                }
            } else {
                m_nLineCount += nShouldAddLines;
                if (!isHistory) {
                    this.OnLineCountChanged(EventArgs.Empty);
                }
            }
            if (!isHistory) {
                this.OnTextChanged(new TextManagerTextEventArgs(new List<TextHistoryRecord>() { historyRecord }));
            }
            return historyRecord;
        }

        private List<TextLine> GetTexLines(string strText) {
            int nIndexStart = 0, nLen = 0;
            TextLine line = new TextLine();
            List<TextLine> lst = new List<TextLine>();
            lst.Add(line);
            for (int i = 0; i < strText.Length; i++) {
                char c = strText[i];
                nLen++;
                if (c == '\n' || c == '\r') {
                    if (c == '\r' && i + 1 < strText.Length && strText[i + 1] == '\n') {
                        nLen++;
                        i++;
                    }
                    line.RawString = strText.Substring(nIndexStart, nLen);
                    line = new TextLine();
                    lst.Add(line);
                    nIndexStart = i + 1;
                    nLen = 0;
                }
            }
            if (nLen != 0) {
                line.RawString = strText.Substring(nIndexStart, nLen);
            }
            return lst;
        }

        private void InsertEmptyLines(int nIndex, int nCount, int nOffsetIncrement) {
            if (nCount == 0) return;
            this.EnsureSpace(nCount);
            for (int i = m_nLineCount + nCount - 1; i > nIndex + nCount; i--) {
                m_lines[i] = m_lines[i - nCount];
                if (m_lines[i] == null) {
                    m_lines[i] = new TextLine();
                }
                m_lines[i].IndexOfFirstChar += nOffsetIncrement;
            }
        }

        private void RemoveLines(int nIndex, int nCount, int nOffsetIncrement, TextHistoryRecord history) {
            if (nCount == 0) return;
            int nCounter = nCount;
            for (int i = nIndex; i < m_nLineCount - nCount; i++) {
                if (nCounter > 0) {
                    nCounter--;
                }
                m_lines[i] = m_lines[i + nCount];
                m_lines[i].IndexOfFirstChar += nOffsetIncrement;
            }
        }

        private void EnsureSpace(int nCount) {
            if (m_nLineCount + nCount <= m_lines.Length) {
                return;
            }
            int nLen = m_nLineCount + nCount;
            TextLine[] new_arr = new TextLine[Math.Max(m_nLineCount + nCount, m_lines.Length << 1)];
            Array.Copy(m_lines, new_arr, m_lines.Length);
            m_lines = new_arr;
        }

        #endregion

        #region Interface

        public IEnumerator<TextLine> GetEnumerator() {
            for (int i = 0; i < m_nLineCount; i++) {
                yield return m_lines[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }
}
