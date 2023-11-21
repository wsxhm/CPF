using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public abstract class TextStyleMonitor : ITextStyleMonitor
    {
        public static List<TextStyleRange> FillDefaultStyle(List<TextStyleRange> lst, int nTextLength, TextStyle style) {
            List<TextStyleRange> lstRet = new List<TextStyleRange>();
            int nIndex = 0;
            foreach (var tsr in lst) {
                if (nIndex != tsr.Index) {
                    lstRet.Add(new TextStyleRange() {
                        Index = nIndex,
                        Length = tsr.Index - nIndex,
                        Style = style
                    });
                }
                lstRet.Add(new TextStyleRange() {
                    Index = tsr.Index,
                    Length = tsr.Length,
                    Style = tsr.Style
                });
                nIndex = tsr.Index + tsr.Length;
            }
            if (nIndex < nTextLength) {
                lstRet.Add(new TextStyleRange() {
                    Index = nIndex,
                    Length = nTextLength - nIndex
                });
            }
            return lstRet;
        }

        public static TextStyleRange GetStyleFromCharIndex(int nIndex, List<TextStyleRange> lst) {
            if (lst == null || lst.Count == 0) {
                return TextStyleRange.Empty;
            }
            int nLeft = 0, nRight = lst.Count - 1, nMid = 0;
            while (nLeft <= nRight) {
                nMid = (nLeft + nRight) >> 1;
                if (lst[nMid].Index > nIndex) {
                    nRight = nMid - 1;
                } else if (lst[nMid].Index + lst[nMid].Length <= nIndex) {
                    nLeft = nMid + 1;
                } else {
                    return lst[nMid];
                }
            }
            if (nIndex < lst[nMid].Index) {
                if (nMid == 0) return new TextStyleRange() { Index = 0, Length = lst[0].Index };
                return new TextStyleRange() {
                    Index = lst[nMid - 1].Index + lst[nMid - 1].Length,
                    Length = lst[nMid].Index - lst[nMid - 1].Index - lst[nMid - 1].Length
                };
            }
            if (nMid == lst.Count - 1) return new TextStyleRange() {
                Index = lst[nMid].Index + lst[nMid].Length, Length = int.MaxValue
            };
            return new TextStyleRange() {
                Index = lst[nMid].Index + lst[nMid].Length,
                Length = lst[nMid + 1].Index - lst[nMid].Index - lst[nMid].Length
            };
        }

        public abstract void Init(string strText);
        public abstract void OnSelectionChanged(TextManager textManager, int nStart, int nLen);
        public abstract void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs);
        public abstract TextStyleRange GetStyleFromCharIndex(int nIndex);
    }
}
