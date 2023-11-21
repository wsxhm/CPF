using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public abstract class TextBoundary : ITextBoundary
    {
        public enum Type
        {
            Other,
            Double_Quote,
            Single_Quote,
            Hebrew_Letter,
            CR,
            LF,
            Newline,
            Extend,
            Regional_Indicator,
            Format,
            Katakana,
            ALetter,
            MidLetter,
            MidNum,
            MidNumLet,
            Numeric,
            ExtendNumLet,
            ZWJ,
            WSegSpace,
            Extended_Pictographic,
            Control,
            SpacingMark,
            L,
            V,
            T,
            LV,
            LVT,
            Prepend,
            E_Base,
            E_Modifier,
            Glue_After_Zwj,
            E_Base_GAZ,
            Custom_Property_Ascii
        }

        protected struct RangeInfo
        {
            public int Start;
            public int End;
            public int Type;
        }

        public List<string> Split(string strText) {
            List<string> lst_result = new List<string>(strText.Length);
            this.SplitPrivate(strText, lst_result, 0, null, null);
            return lst_result;
        }

        public int GetCount(string strText) {
            int nLen = this.SplitPrivate(strText, null, 0, null, null);
            return nLen;
        }

        public void Each(string strText, EachVoidCallBack cb) {
            this.SplitPrivate(strText, null, 0, cb, null);
        }

        public void Each(string strText, int nIndex, EachVoidCallBack cb) {
            this.SplitPrivate(strText, null, nIndex, cb, null);
        }

        public void Each(string strText, EachBoolCallBack cb) {
            this.SplitPrivate(strText, null, 0, null, cb);
        }

        public void Each(string strText, int nIndex, EachBoolCallBack cb) {
            this.SplitPrivate(strText, null, nIndex, null, cb);
        }

        public static int GetCodePoint(string strText, int nIndex) {
            if (strText[nIndex] < '\uD800' || strText[nIndex] > '\uDFFF') {
                return strText[nIndex];
            }
            if (char.IsHighSurrogate(strText, nIndex)) {
                if (nIndex + 1 >= strText.Length) {
                    return 0;
                }
            } else {
                if (--nIndex < 0) {
                    return 0;
                }
            }
            return ((strText[nIndex] & 0x03FF) << 10) + (strText[nIndex + 1] & 0x03FF) + 0x10000;
        }

        private int SplitPrivate(string strText, List<string> lst_result, int nIndexCurrent, EachVoidCallBack cb_void, EachBoolCallBack cb_bool) {
            int nCounter = 0;
            List<int> lst_history_break_type = new List<int>();
            if (string.IsNullOrEmpty(strText) || nIndexCurrent >= strText.Length) {
                return 0;
            }
            int nIndexCharStart = 0, nCharLen = 0, nLastCharLen = 0;
            int nCodePoint = 0;
            int nLeftBreakType = 0, nRightBreakType = 0;

            while (nIndexCurrent < strText.Length && char.IsLowSurrogate(strText, nIndexCurrent)) {
                nIndexCurrent++;
                nCharLen++;
            }
            if (nCharLen != 0) {
                nCounter++;
                if (!this.CharCompleted(strText, nIndexCurrent - nCharLen, nCharLen, lst_result, cb_void, cb_bool)) {
                    return nCounter;
                }
            }
            nIndexCharStart = nIndexCurrent;
            nCodePoint = TextBoundary.GetCodePoint(strText, nIndexCurrent);
            nLastCharLen = nCodePoint >= 0x10000 ? 2 : 1;       // >= 0x10000 is double char
            nLeftBreakType = this.GetBreakProperty(nCodePoint);
            nIndexCurrent += nLastCharLen;
            nCharLen = nLastCharLen;
            lst_history_break_type.Add(nLeftBreakType);
            while (nIndexCurrent < strText.Length) {
                nCodePoint = TextBoundary.GetCodePoint(strText, nIndexCurrent);
                nLastCharLen = nCodePoint >= 0x10000 ? 2 : 1;   // >= 0x10000 is double char
                nRightBreakType = this.GetBreakProperty(nCodePoint);
                if (this.ShouldBreak(nRightBreakType, lst_history_break_type)) {
                    nCounter++;
                    if (!this.CharCompleted(strText, nIndexCharStart, nCharLen, lst_result, cb_void, cb_bool)) {
                        return nCounter;
                    }
                    nIndexCharStart = nIndexCurrent;
                    nCharLen = nLastCharLen;
                    lst_history_break_type.Clear();
                } else {
                    nCharLen += nLastCharLen;
                }
                lst_history_break_type.Add(nRightBreakType);
                nIndexCurrent += nLastCharLen;
                nLeftBreakType = nRightBreakType;
            }
            if (nCharLen != 0) {
                nCounter++;
                this.CharCompleted(strText, nIndexCharStart, nCharLen, lst_result, cb_void, cb_bool);
            }
            return nCounter;
        }

        private bool CharCompleted(string strText, int nIndex, int nLen, List<string> lst_result, EachVoidCallBack cb_void, EachBoolCallBack cb_bool) {
            //return value...[true:continue]...[false:break]
            if (lst_result != null) {
                lst_result.Add(strText.Substring(nIndex, nLen));
            }
            if (cb_void != null) {
                cb_void(strText, nIndex, nLen);
            } else if (cb_bool != null && !cb_bool(strText, nIndex, nLen)) {
                return false;
            }
            return true;
        }

        protected abstract bool ShouldBreak(int nRightBreakType, List<int> lstHistoryBreakType);

        protected abstract int GetBreakProperty(int nCodePoint);

        protected static int BinarySearchRangeFromList(int nStart, int nEnd, int nValue, List<RangeInfo> lst) {
            if (nEnd < nStart) {
                return 0;
            }
            int nMid = nStart + (nEnd - nStart) / 2;
            if (lst[nMid].Start > nValue) {
                return TextBoundary.BinarySearchRangeFromList(nStart, nMid - 1, nValue, lst);
            } else if (lst[nMid].End < nValue) {
                return TextBoundary.BinarySearchRangeFromList(nMid + 1, nEnd, nValue, lst);
            } else {
                return lst[nMid].Type;
            }
        }
    }
}
