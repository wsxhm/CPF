using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public class WordSplitter : TextBoundary
    {
        // The following variable values do not have a fixed value. Just a FLAG. Just like enumeration.
        public const int Other = 0;
        public const int Double_Quote = 1;
        public const int Single_Quote = 2;
        public const int Hebrew_Letter = 3;
        public const int CR = 4;
        public const int LF = 5;
        public const int Newline = 6;
        public const int Extend = 7;
        public const int Regional_Indicator = 8;
        public const int Format = 9;
        public const int Katakana = 10;
        public const int ALetter = 11;
        public const int MidLetter = 12;
        public const int MidNum = 13;
        public const int MidNumLet = 14;
        public const int Numeric = 15;
        public const int ExtendNumLet = 16;
        public const int ZWJ = 17;
        public const int WSegSpace = 18;
        public const int Extended_Pictographic = 19;
        public const int Custom_Property_Ascii = 20;

        private static int[] m_arr_cache_break_type;
        private static Dictionary<int, int> m_dic_cache_break_type;
        private static List<RangeInfo> m_lst_code_range = new List<RangeInfo>();

        static WordSplitter() {
            // Cc   [2] <control-000B>..<control-000C>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0000B, End = 0x0000C, Type = Newline });
            // Nd  [10] DIGIT ZERO..DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00030, End = 0x00039, Type = Numeric });
            // L&  [26] LATIN CAPITAL LETTER A..LATIN CAPITAL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00041, End = 0x0005A, Type = ALetter });
            // L&  [26] LATIN SMALL LETTER A..LATIN SMALL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00061, End = 0x0007A, Type = ALetter });
            // L&  [23] LATIN CAPITAL LETTER A WITH GRAVE..LATIN CAPITAL LETTER O WITH DIAERESIS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x000C0, End = 0x000D6, Type = ALetter });
            // L&  [31] LATIN CAPITAL LETTER O WITH STROKE..LATIN SMALL LETTER O WITH DIAERESIS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x000D8, End = 0x000F6, Type = ALetter });
            // L& [195] LATIN SMALL LETTER O WITH STROKE..LATIN SMALL LETTER EZH WITH TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x000F8, End = 0x001BA, Type = ALetter });
            // L&   [4] LATIN CAPITAL LETTER TONE FIVE..LATIN LETTER WYNN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x001BC, End = 0x001BF, Type = ALetter });
            // Lo   [4] LATIN LETTER DENTAL CLICK..LATIN LETTER RETROFLEX CLICK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x001C0, End = 0x001C3, Type = ALetter });
            // L& [208] LATIN CAPITAL LETTER DZ WITH CARON..LATIN SMALL LETTER EZH WITH CURL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x001C4, End = 0x00293, Type = ALetter });
            // L&  [27] LATIN LETTER PHARYNGEAL VOICED FRICATIVE..LATIN SMALL LETTER TURNED H WITH FISHHOOK AND TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00295, End = 0x002AF, Type = ALetter });
            // Lm  [18] MODIFIER LETTER SMALL H..MODIFIER LETTER REVERSED GLOTTAL STOP
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002B0, End = 0x002C1, Type = ALetter });
            // Sk   [4] MODIFIER LETTER LEFT ARROWHEAD..MODIFIER LETTER DOWN ARROWHEAD
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002C2, End = 0x002C5, Type = ALetter });
            // Lm  [12] MODIFIER LETTER CIRCUMFLEX ACCENT..MODIFIER LETTER HALF TRIANGULAR COLON
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002C6, End = 0x002D1, Type = ALetter });
            // Sk   [6] MODIFIER LETTER CENTRED RIGHT HALF RING..MODIFIER LETTER MINUS SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002D2, End = 0x002D7, Type = ALetter });
            // Sk   [2] MODIFIER LETTER RHOTIC HOOK..MODIFIER LETTER CROSS ACCENT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002DE, End = 0x002DF, Type = ALetter });
            // Lm   [5] MODIFIER LETTER SMALL GAMMA..MODIFIER LETTER SMALL REVERSED GLOTTAL STOP
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002E0, End = 0x002E4, Type = ALetter });
            // Sk   [7] MODIFIER LETTER EXTRA-HIGH TONE BAR..MODIFIER LETTER YANG DEPARTING TONE MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002E5, End = 0x002EB, Type = ALetter });
            // Sk  [17] MODIFIER LETTER LOW DOWN ARROWHEAD..MODIFIER LETTER LOW LEFT ARROW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x002EF, End = 0x002FF, Type = ALetter });
            // Mn [112] COMBINING GRAVE ACCENT..COMBINING LATIN SMALL LETTER X
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00300, End = 0x0036F, Type = Extend });
            // L&   [4] GREEK CAPITAL LETTER HETA..GREEK SMALL LETTER ARCHAIC SAMPI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00370, End = 0x00373, Type = ALetter });
            // L&   [2] GREEK CAPITAL LETTER PAMPHYLIAN DIGAMMA..GREEK SMALL LETTER PAMPHYLIAN DIGAMMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00376, End = 0x00377, Type = ALetter });
            // L&   [3] GREEK SMALL REVERSED LUNATE SIGMA SYMBOL..GREEK SMALL REVERSED DOTTED LUNATE SIGMA SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0037B, End = 0x0037D, Type = ALetter });
            // L&   [3] GREEK CAPITAL LETTER EPSILON WITH TONOS..GREEK CAPITAL LETTER IOTA WITH TONOS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00388, End = 0x0038A, Type = ALetter });
            // L&  [20] GREEK CAPITAL LETTER UPSILON WITH TONOS..GREEK CAPITAL LETTER RHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0038E, End = 0x003A1, Type = ALetter });
            // L&  [83] GREEK CAPITAL LETTER SIGMA..GREEK LUNATE EPSILON SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x003A3, End = 0x003F5, Type = ALetter });
            // L& [139] GREEK CAPITAL LETTER SHO..CYRILLIC SMALL LETTER KOPPA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x003F7, End = 0x00481, Type = ALetter });
            // Mn   [5] COMBINING CYRILLIC TITLO..COMBINING CYRILLIC POKRYTIE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00483, End = 0x00487, Type = Extend });
            // Me   [2] COMBINING CYRILLIC HUNDRED THOUSANDS SIGN..COMBINING CYRILLIC MILLIONS SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00488, End = 0x00489, Type = Extend });
            // L& [166] CYRILLIC CAPITAL LETTER SHORT I WITH TAIL..CYRILLIC SMALL LETTER EL WITH DESCENDER
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0048A, End = 0x0052F, Type = ALetter });
            // L&  [38] ARMENIAN CAPITAL LETTER AYB..ARMENIAN CAPITAL LETTER FEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00531, End = 0x00556, Type = ALetter });
            // Po   [3] ARMENIAN APOSTROPHE..ARMENIAN EXCLAMATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0055A, End = 0x0055C, Type = ALetter });
            // L&  [41] ARMENIAN SMALL LETTER TURNED AYB..ARMENIAN SMALL LETTER YI WITH STROKE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00560, End = 0x00588, Type = ALetter });
            // Mn  [45] HEBREW ACCENT ETNAHTA..HEBREW POINT METEG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00591, End = 0x005BD, Type = Extend });
            // Mn   [2] HEBREW POINT SHIN DOT..HEBREW POINT SIN DOT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x005C1, End = 0x005C2, Type = Extend });
            // Mn   [2] HEBREW MARK UPPER DOT..HEBREW MARK LOWER DOT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x005C4, End = 0x005C5, Type = Extend });
            // Lo  [27] HEBREW LETTER ALEF..HEBREW LETTER TAV
            m_lst_code_range.Add(new RangeInfo() { Start = 0x005D0, End = 0x005EA, Type = Hebrew_Letter });
            // Lo   [4] HEBREW YOD TRIANGLE..HEBREW LIGATURE YIDDISH DOUBLE YOD
            m_lst_code_range.Add(new RangeInfo() { Start = 0x005EF, End = 0x005F2, Type = Hebrew_Letter });
            // Cf   [6] ARABIC NUMBER SIGN..ARABIC NUMBER MARK ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00600, End = 0x00605, Type = Format });
            // Po   [2] ARABIC COMMA..ARABIC DATE SEPARATOR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0060C, End = 0x0060D, Type = MidNum });
            // Mn  [11] ARABIC SIGN SALLALLAHOU ALAYHE WASSALLAM..ARABIC SMALL KASRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00610, End = 0x0061A, Type = Extend });
            // Lo  [32] ARABIC LETTER KASHMIRI YEH..ARABIC LETTER FARSI YEH WITH THREE DOTS ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00620, End = 0x0063F, Type = ALetter });
            // Lo  [10] ARABIC LETTER FEH..ARABIC LETTER YEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00641, End = 0x0064A, Type = ALetter });
            // Mn  [21] ARABIC FATHATAN..ARABIC WAVY HAMZA BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0064B, End = 0x0065F, Type = Extend });
            // Nd  [10] ARABIC-INDIC DIGIT ZERO..ARABIC-INDIC DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00660, End = 0x00669, Type = Numeric });
            // Lo   [2] ARABIC LETTER DOTLESS BEH..ARABIC LETTER DOTLESS QAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0066E, End = 0x0066F, Type = ALetter });
            // Lo  [99] ARABIC LETTER ALEF WASLA..ARABIC LETTER YEH BARREE WITH HAMZA ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00671, End = 0x006D3, Type = ALetter });
            // Mn   [7] ARABIC SMALL HIGH LIGATURE SAD WITH LAM WITH ALEF MAKSURA..ARABIC SMALL HIGH SEEN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006D6, End = 0x006DC, Type = Extend });
            // Mn   [6] ARABIC SMALL HIGH ROUNDED ZERO..ARABIC SMALL HIGH MADDA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006DF, End = 0x006E4, Type = Extend });
            // Lm   [2] ARABIC SMALL WAW..ARABIC SMALL YEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006E5, End = 0x006E6, Type = ALetter });
            // Mn   [2] ARABIC SMALL HIGH YEH..ARABIC SMALL HIGH NOON
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006E7, End = 0x006E8, Type = Extend });
            // Mn   [4] ARABIC EMPTY CENTRE LOW STOP..ARABIC SMALL LOW MEEM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006EA, End = 0x006ED, Type = Extend });
            // Lo   [2] ARABIC LETTER DAL WITH INVERTED V..ARABIC LETTER REH WITH INVERTED V
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006EE, End = 0x006EF, Type = ALetter });
            // Nd  [10] EXTENDED ARABIC-INDIC DIGIT ZERO..EXTENDED ARABIC-INDIC DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006F0, End = 0x006F9, Type = Numeric });
            // Lo   [3] ARABIC LETTER SHEEN WITH DOT BELOW..ARABIC LETTER GHAIN WITH DOT BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x006FA, End = 0x006FC, Type = ALetter });
            // Lo  [30] SYRIAC LETTER BETH..SYRIAC LETTER PERSIAN DHALATH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00712, End = 0x0072F, Type = ALetter });
            // Mn  [27] SYRIAC PTHAHA ABOVE..SYRIAC BARREKH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00730, End = 0x0074A, Type = Extend });
            // Lo  [89] SYRIAC LETTER SOGDIAN ZHAIN..THAANA LETTER WAAVU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0074D, End = 0x007A5, Type = ALetter });
            // Mn  [11] THAANA ABAFILI..THAANA SUKUN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x007A6, End = 0x007B0, Type = Extend });
            // Nd  [10] NKO DIGIT ZERO..NKO DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x007C0, End = 0x007C9, Type = Numeric });
            // Lo  [33] NKO LETTER A..NKO LETTER JONA RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x007CA, End = 0x007EA, Type = ALetter });
            // Mn   [9] NKO COMBINING SHORT HIGH TONE..NKO COMBINING DOUBLE DOT ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x007EB, End = 0x007F3, Type = Extend });
            // Lm   [2] NKO HIGH TONE APOSTROPHE..NKO LOW TONE APOSTROPHE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x007F4, End = 0x007F5, Type = ALetter });
            // Lo  [22] SAMARITAN LETTER ALAF..SAMARITAN LETTER TAAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00800, End = 0x00815, Type = ALetter });
            // Mn   [4] SAMARITAN MARK IN..SAMARITAN MARK DAGESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00816, End = 0x00819, Type = Extend });
            // Mn   [9] SAMARITAN MARK EPENTHETIC YUT..SAMARITAN VOWEL SIGN A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0081B, End = 0x00823, Type = Extend });
            // Mn   [3] SAMARITAN VOWEL SIGN SHORT A..SAMARITAN VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00825, End = 0x00827, Type = Extend });
            // Mn   [5] SAMARITAN VOWEL SIGN LONG I..SAMARITAN MARK NEQUDAA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00829, End = 0x0082D, Type = Extend });
            // Lo  [25] MANDAIC LETTER HALQA..MANDAIC LETTER AIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00840, End = 0x00858, Type = ALetter });
            // Mn   [3] MANDAIC AFFRICATION MARK..MANDAIC GEMINATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00859, End = 0x0085B, Type = Extend });
            // Lo  [11] SYRIAC LETTER MALAYALAM NGA..SYRIAC LETTER MALAYALAM SSA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00860, End = 0x0086A, Type = ALetter });
            // Lo  [24] ARABIC LETTER ALEF WITH ATTACHED FATHA..ARABIC BASELINE ROUND DOT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00870, End = 0x00887, Type = ALetter });
            // Lo   [6] ARABIC LETTER NOON WITH INVERTED SMALL V..ARABIC VERTICAL TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00889, End = 0x0088E, Type = ALetter });
            // Cf   [2] ARABIC POUND MARK ABOVE..ARABIC PIASTRE MARK ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00890, End = 0x00891, Type = Format });
            // Mn   [8] ARABIC SMALL HIGH WORD AL-JUZ..ARABIC HALF MADDA OVER MADDA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00898, End = 0x0089F, Type = Extend });
            // Lo  [41] ARABIC LETTER BEH WITH SMALL V BELOW..ARABIC LETTER GRAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x008A0, End = 0x008C8, Type = ALetter });
            // Mn  [24] ARABIC SMALL HIGH FARSI YEH..ARABIC SMALL HIGH SIGN SAFHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x008CA, End = 0x008E1, Type = Extend });
            // Mn  [32] ARABIC TURNED DAMMA BELOW..DEVANAGARI SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x008E3, End = 0x00902, Type = Extend });
            // Lo  [54] DEVANAGARI LETTER SHORT A..DEVANAGARI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00904, End = 0x00939, Type = ALetter });
            // Mc   [3] DEVANAGARI VOWEL SIGN AA..DEVANAGARI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0093E, End = 0x00940, Type = Extend });
            // Mn   [8] DEVANAGARI VOWEL SIGN U..DEVANAGARI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00941, End = 0x00948, Type = Extend });
            // Mc   [4] DEVANAGARI VOWEL SIGN CANDRA O..DEVANAGARI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00949, End = 0x0094C, Type = Extend });
            // Mc   [2] DEVANAGARI VOWEL SIGN PRISHTHAMATRA E..DEVANAGARI VOWEL SIGN AW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0094E, End = 0x0094F, Type = Extend });
            // Mn   [7] DEVANAGARI STRESS SIGN UDATTA..DEVANAGARI VOWEL SIGN UUE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00951, End = 0x00957, Type = Extend });
            // Lo  [10] DEVANAGARI LETTER QA..DEVANAGARI LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00958, End = 0x00961, Type = ALetter });
            // Mn   [2] DEVANAGARI VOWEL SIGN VOCALIC L..DEVANAGARI VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00962, End = 0x00963, Type = Extend });
            // Nd  [10] DEVANAGARI DIGIT ZERO..DEVANAGARI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00966, End = 0x0096F, Type = Numeric });
            // Lo  [15] DEVANAGARI LETTER CANDRA A..BENGALI ANJI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00972, End = 0x00980, Type = ALetter });
            // Mc   [2] BENGALI SIGN ANUSVARA..BENGALI SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00982, End = 0x00983, Type = Extend });
            // Lo   [8] BENGALI LETTER A..BENGALI LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00985, End = 0x0098C, Type = ALetter });
            // Lo   [2] BENGALI LETTER E..BENGALI LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0098F, End = 0x00990, Type = ALetter });
            // Lo  [22] BENGALI LETTER O..BENGALI LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00993, End = 0x009A8, Type = ALetter });
            // Lo   [7] BENGALI LETTER PA..BENGALI LETTER RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009AA, End = 0x009B0, Type = ALetter });
            // Lo   [4] BENGALI LETTER SHA..BENGALI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009B6, End = 0x009B9, Type = ALetter });
            // Mc   [3] BENGALI VOWEL SIGN AA..BENGALI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009BE, End = 0x009C0, Type = Extend });
            // Mn   [4] BENGALI VOWEL SIGN U..BENGALI VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009C1, End = 0x009C4, Type = Extend });
            // Mc   [2] BENGALI VOWEL SIGN E..BENGALI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009C7, End = 0x009C8, Type = Extend });
            // Mc   [2] BENGALI VOWEL SIGN O..BENGALI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009CB, End = 0x009CC, Type = Extend });
            // Lo   [2] BENGALI LETTER RRA..BENGALI LETTER RHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009DC, End = 0x009DD, Type = ALetter });
            // Lo   [3] BENGALI LETTER YYA..BENGALI LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009DF, End = 0x009E1, Type = ALetter });
            // Mn   [2] BENGALI VOWEL SIGN VOCALIC L..BENGALI VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009E2, End = 0x009E3, Type = Extend });
            // Nd  [10] BENGALI DIGIT ZERO..BENGALI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009E6, End = 0x009EF, Type = Numeric });
            // Lo   [2] BENGALI LETTER RA WITH MIDDLE DIAGONAL..BENGALI LETTER RA WITH LOWER DIAGONAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x009F0, End = 0x009F1, Type = ALetter });
            // Mn   [2] GURMUKHI SIGN ADAK BINDI..GURMUKHI SIGN BINDI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A01, End = 0x00A02, Type = Extend });
            // Lo   [6] GURMUKHI LETTER A..GURMUKHI LETTER UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A05, End = 0x00A0A, Type = ALetter });
            // Lo   [2] GURMUKHI LETTER EE..GURMUKHI LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A0F, End = 0x00A10, Type = ALetter });
            // Lo  [22] GURMUKHI LETTER OO..GURMUKHI LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A13, End = 0x00A28, Type = ALetter });
            // Lo   [7] GURMUKHI LETTER PA..GURMUKHI LETTER RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A2A, End = 0x00A30, Type = ALetter });
            // Lo   [2] GURMUKHI LETTER LA..GURMUKHI LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A32, End = 0x00A33, Type = ALetter });
            // Lo   [2] GURMUKHI LETTER VA..GURMUKHI LETTER SHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A35, End = 0x00A36, Type = ALetter });
            // Lo   [2] GURMUKHI LETTER SA..GURMUKHI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A38, End = 0x00A39, Type = ALetter });
            // Mc   [3] GURMUKHI VOWEL SIGN AA..GURMUKHI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A3E, End = 0x00A40, Type = Extend });
            // Mn   [2] GURMUKHI VOWEL SIGN U..GURMUKHI VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A41, End = 0x00A42, Type = Extend });
            // Mn   [2] GURMUKHI VOWEL SIGN EE..GURMUKHI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A47, End = 0x00A48, Type = Extend });
            // Mn   [3] GURMUKHI VOWEL SIGN OO..GURMUKHI SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A4B, End = 0x00A4D, Type = Extend });
            // Lo   [4] GURMUKHI LETTER KHHA..GURMUKHI LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A59, End = 0x00A5C, Type = ALetter });
            // Nd  [10] GURMUKHI DIGIT ZERO..GURMUKHI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A66, End = 0x00A6F, Type = Numeric });
            // Mn   [2] GURMUKHI TIPPI..GURMUKHI ADDAK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A70, End = 0x00A71, Type = Extend });
            // Lo   [3] GURMUKHI IRI..GURMUKHI EK ONKAR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A72, End = 0x00A74, Type = ALetter });
            // Mn   [2] GUJARATI SIGN CANDRABINDU..GUJARATI SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A81, End = 0x00A82, Type = Extend });
            // Lo   [9] GUJARATI LETTER A..GUJARATI VOWEL CANDRA E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A85, End = 0x00A8D, Type = ALetter });
            // Lo   [3] GUJARATI LETTER E..GUJARATI VOWEL CANDRA O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A8F, End = 0x00A91, Type = ALetter });
            // Lo  [22] GUJARATI LETTER O..GUJARATI LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00A93, End = 0x00AA8, Type = ALetter });
            // Lo   [7] GUJARATI LETTER PA..GUJARATI LETTER RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AAA, End = 0x00AB0, Type = ALetter });
            // Lo   [2] GUJARATI LETTER LA..GUJARATI LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AB2, End = 0x00AB3, Type = ALetter });
            // Lo   [5] GUJARATI LETTER VA..GUJARATI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AB5, End = 0x00AB9, Type = ALetter });
            // Mc   [3] GUJARATI VOWEL SIGN AA..GUJARATI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00ABE, End = 0x00AC0, Type = Extend });
            // Mn   [5] GUJARATI VOWEL SIGN U..GUJARATI VOWEL SIGN CANDRA E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AC1, End = 0x00AC5, Type = Extend });
            // Mn   [2] GUJARATI VOWEL SIGN E..GUJARATI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AC7, End = 0x00AC8, Type = Extend });
            // Mc   [2] GUJARATI VOWEL SIGN O..GUJARATI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00ACB, End = 0x00ACC, Type = Extend });
            // Lo   [2] GUJARATI LETTER VOCALIC RR..GUJARATI LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AE0, End = 0x00AE1, Type = ALetter });
            // Mn   [2] GUJARATI VOWEL SIGN VOCALIC L..GUJARATI VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AE2, End = 0x00AE3, Type = Extend });
            // Nd  [10] GUJARATI DIGIT ZERO..GUJARATI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AE6, End = 0x00AEF, Type = Numeric });
            // Mn   [6] GUJARATI SIGN SUKUN..GUJARATI SIGN TWO-CIRCLE NUKTA ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00AFA, End = 0x00AFF, Type = Extend });
            // Mc   [2] ORIYA SIGN ANUSVARA..ORIYA SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B02, End = 0x00B03, Type = Extend });
            // Lo   [8] ORIYA LETTER A..ORIYA LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B05, End = 0x00B0C, Type = ALetter });
            // Lo   [2] ORIYA LETTER E..ORIYA LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B0F, End = 0x00B10, Type = ALetter });
            // Lo  [22] ORIYA LETTER O..ORIYA LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B13, End = 0x00B28, Type = ALetter });
            // Lo   [7] ORIYA LETTER PA..ORIYA LETTER RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B2A, End = 0x00B30, Type = ALetter });
            // Lo   [2] ORIYA LETTER LA..ORIYA LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B32, End = 0x00B33, Type = ALetter });
            // Lo   [5] ORIYA LETTER VA..ORIYA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B35, End = 0x00B39, Type = ALetter });
            // Mn   [4] ORIYA VOWEL SIGN U..ORIYA VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B41, End = 0x00B44, Type = Extend });
            // Mc   [2] ORIYA VOWEL SIGN E..ORIYA VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B47, End = 0x00B48, Type = Extend });
            // Mc   [2] ORIYA VOWEL SIGN O..ORIYA VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B4B, End = 0x00B4C, Type = Extend });
            // Mn   [2] ORIYA SIGN OVERLINE..ORIYA AI LENGTH MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B55, End = 0x00B56, Type = Extend });
            // Lo   [2] ORIYA LETTER RRA..ORIYA LETTER RHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B5C, End = 0x00B5D, Type = ALetter });
            // Lo   [3] ORIYA LETTER YYA..ORIYA LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B5F, End = 0x00B61, Type = ALetter });
            // Mn   [2] ORIYA VOWEL SIGN VOCALIC L..ORIYA VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B62, End = 0x00B63, Type = Extend });
            // Nd  [10] ORIYA DIGIT ZERO..ORIYA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B66, End = 0x00B6F, Type = Numeric });
            // Lo   [6] TAMIL LETTER A..TAMIL LETTER UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B85, End = 0x00B8A, Type = ALetter });
            // Lo   [3] TAMIL LETTER E..TAMIL LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B8E, End = 0x00B90, Type = ALetter });
            // Lo   [4] TAMIL LETTER O..TAMIL LETTER KA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B92, End = 0x00B95, Type = ALetter });
            // Lo   [2] TAMIL LETTER NGA..TAMIL LETTER CA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B99, End = 0x00B9A, Type = ALetter });
            // Lo   [2] TAMIL LETTER NYA..TAMIL LETTER TTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00B9E, End = 0x00B9F, Type = ALetter });
            // Lo   [2] TAMIL LETTER NNA..TAMIL LETTER TA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BA3, End = 0x00BA4, Type = ALetter });
            // Lo   [3] TAMIL LETTER NA..TAMIL LETTER PA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BA8, End = 0x00BAA, Type = ALetter });
            // Lo  [12] TAMIL LETTER MA..TAMIL LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BAE, End = 0x00BB9, Type = ALetter });
            // Mc   [2] TAMIL VOWEL SIGN AA..TAMIL VOWEL SIGN I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BBE, End = 0x00BBF, Type = Extend });
            // Mc   [2] TAMIL VOWEL SIGN U..TAMIL VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BC1, End = 0x00BC2, Type = Extend });
            // Mc   [3] TAMIL VOWEL SIGN E..TAMIL VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BC6, End = 0x00BC8, Type = Extend });
            // Mc   [3] TAMIL VOWEL SIGN O..TAMIL VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BCA, End = 0x00BCC, Type = Extend });
            // Nd  [10] TAMIL DIGIT ZERO..TAMIL DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00BE6, End = 0x00BEF, Type = Numeric });
            // Mc   [3] TELUGU SIGN CANDRABINDU..TELUGU SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C01, End = 0x00C03, Type = Extend });
            // Lo   [8] TELUGU LETTER A..TELUGU LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C05, End = 0x00C0C, Type = ALetter });
            // Lo   [3] TELUGU LETTER E..TELUGU LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C0E, End = 0x00C10, Type = ALetter });
            // Lo  [23] TELUGU LETTER O..TELUGU LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C12, End = 0x00C28, Type = ALetter });
            // Lo  [16] TELUGU LETTER PA..TELUGU LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C2A, End = 0x00C39, Type = ALetter });
            // Mn   [3] TELUGU VOWEL SIGN AA..TELUGU VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C3E, End = 0x00C40, Type = Extend });
            // Mc   [4] TELUGU VOWEL SIGN U..TELUGU VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C41, End = 0x00C44, Type = Extend });
            // Mn   [3] TELUGU VOWEL SIGN E..TELUGU VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C46, End = 0x00C48, Type = Extend });
            // Mn   [4] TELUGU VOWEL SIGN O..TELUGU SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C4A, End = 0x00C4D, Type = Extend });
            // Mn   [2] TELUGU LENGTH MARK..TELUGU AI LENGTH MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C55, End = 0x00C56, Type = Extend });
            // Lo   [3] TELUGU LETTER TSA..TELUGU LETTER RRRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C58, End = 0x00C5A, Type = ALetter });
            // Lo   [2] TELUGU LETTER VOCALIC RR..TELUGU LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C60, End = 0x00C61, Type = ALetter });
            // Mn   [2] TELUGU VOWEL SIGN VOCALIC L..TELUGU VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C62, End = 0x00C63, Type = Extend });
            // Nd  [10] TELUGU DIGIT ZERO..TELUGU DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C66, End = 0x00C6F, Type = Numeric });
            // Mc   [2] KANNADA SIGN ANUSVARA..KANNADA SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C82, End = 0x00C83, Type = Extend });
            // Lo   [8] KANNADA LETTER A..KANNADA LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C85, End = 0x00C8C, Type = ALetter });
            // Lo   [3] KANNADA LETTER E..KANNADA LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C8E, End = 0x00C90, Type = ALetter });
            // Lo  [23] KANNADA LETTER O..KANNADA LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00C92, End = 0x00CA8, Type = ALetter });
            // Lo  [10] KANNADA LETTER PA..KANNADA LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CAA, End = 0x00CB3, Type = ALetter });
            // Lo   [5] KANNADA LETTER VA..KANNADA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CB5, End = 0x00CB9, Type = ALetter });
            // Mc   [5] KANNADA VOWEL SIGN II..KANNADA VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CC0, End = 0x00CC4, Type = Extend });
            // Mc   [2] KANNADA VOWEL SIGN EE..KANNADA VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CC7, End = 0x00CC8, Type = Extend });
            // Mc   [2] KANNADA VOWEL SIGN O..KANNADA VOWEL SIGN OO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CCA, End = 0x00CCB, Type = Extend });
            // Mn   [2] KANNADA VOWEL SIGN AU..KANNADA SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CCC, End = 0x00CCD, Type = Extend });
            // Mc   [2] KANNADA LENGTH MARK..KANNADA AI LENGTH MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CD5, End = 0x00CD6, Type = Extend });
            // Lo   [2] KANNADA LETTER NAKAARA POLLU..KANNADA LETTER FA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CDD, End = 0x00CDE, Type = ALetter });
            // Lo   [2] KANNADA LETTER VOCALIC RR..KANNADA LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CE0, End = 0x00CE1, Type = ALetter });
            // Mn   [2] KANNADA VOWEL SIGN VOCALIC L..KANNADA VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CE2, End = 0x00CE3, Type = Extend });
            // Nd  [10] KANNADA DIGIT ZERO..KANNADA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CE6, End = 0x00CEF, Type = Numeric });
            // Lo   [2] KANNADA SIGN JIHVAMULIYA..KANNADA SIGN UPADHMANIYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00CF1, End = 0x00CF2, Type = ALetter });
            // Mn   [2] MALAYALAM SIGN COMBINING ANUSVARA ABOVE..MALAYALAM SIGN CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D00, End = 0x00D01, Type = Extend });
            // Mc   [2] MALAYALAM SIGN ANUSVARA..MALAYALAM SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D02, End = 0x00D03, Type = Extend });
            // Lo   [9] MALAYALAM LETTER VEDIC ANUSVARA..MALAYALAM LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D04, End = 0x00D0C, Type = ALetter });
            // Lo   [3] MALAYALAM LETTER E..MALAYALAM LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D0E, End = 0x00D10, Type = ALetter });
            // Lo  [41] MALAYALAM LETTER O..MALAYALAM LETTER TTTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D12, End = 0x00D3A, Type = ALetter });
            // Mn   [2] MALAYALAM SIGN VERTICAL BAR VIRAMA..MALAYALAM SIGN CIRCULAR VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D3B, End = 0x00D3C, Type = Extend });
            // Mc   [3] MALAYALAM VOWEL SIGN AA..MALAYALAM VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D3E, End = 0x00D40, Type = Extend });
            // Mn   [4] MALAYALAM VOWEL SIGN U..MALAYALAM VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D41, End = 0x00D44, Type = Extend });
            // Mc   [3] MALAYALAM VOWEL SIGN E..MALAYALAM VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D46, End = 0x00D48, Type = Extend });
            // Mc   [3] MALAYALAM VOWEL SIGN O..MALAYALAM VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D4A, End = 0x00D4C, Type = Extend });
            // Lo   [3] MALAYALAM LETTER CHILLU M..MALAYALAM LETTER CHILLU LLL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D54, End = 0x00D56, Type = ALetter });
            // Lo   [3] MALAYALAM LETTER ARCHAIC II..MALAYALAM LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D5F, End = 0x00D61, Type = ALetter });
            // Mn   [2] MALAYALAM VOWEL SIGN VOCALIC L..MALAYALAM VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D62, End = 0x00D63, Type = Extend });
            // Nd  [10] MALAYALAM DIGIT ZERO..MALAYALAM DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D66, End = 0x00D6F, Type = Numeric });
            // Lo   [6] MALAYALAM LETTER CHILLU NN..MALAYALAM LETTER CHILLU K
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D7A, End = 0x00D7F, Type = ALetter });
            // Mc   [2] SINHALA SIGN ANUSVARAYA..SINHALA SIGN VISARGAYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D82, End = 0x00D83, Type = Extend });
            // Lo  [18] SINHALA LETTER AYANNA..SINHALA LETTER AUYANNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D85, End = 0x00D96, Type = ALetter });
            // Lo  [24] SINHALA LETTER ALPAPRAANA KAYANNA..SINHALA LETTER DANTAJA NAYANNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00D9A, End = 0x00DB1, Type = ALetter });
            // Lo   [9] SINHALA LETTER SANYAKA DAYANNA..SINHALA LETTER RAYANNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DB3, End = 0x00DBB, Type = ALetter });
            // Lo   [7] SINHALA LETTER VAYANNA..SINHALA LETTER FAYANNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DC0, End = 0x00DC6, Type = ALetter });
            // Mc   [3] SINHALA VOWEL SIGN AELA-PILLA..SINHALA VOWEL SIGN DIGA AEDA-PILLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DCF, End = 0x00DD1, Type = Extend });
            // Mn   [3] SINHALA VOWEL SIGN KETTI IS-PILLA..SINHALA VOWEL SIGN KETTI PAA-PILLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DD2, End = 0x00DD4, Type = Extend });
            // Mc   [8] SINHALA VOWEL SIGN GAETTA-PILLA..SINHALA VOWEL SIGN GAYANUKITTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DD8, End = 0x00DDF, Type = Extend });
            // Nd  [10] SINHALA LITH DIGIT ZERO..SINHALA LITH DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DE6, End = 0x00DEF, Type = Numeric });
            // Mc   [2] SINHALA VOWEL SIGN DIGA GAETTA-PILLA..SINHALA VOWEL SIGN DIGA GAYANUKITTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00DF2, End = 0x00DF3, Type = Extend });
            // Mn   [7] THAI CHARACTER SARA I..THAI CHARACTER PHINTHU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00E34, End = 0x00E3A, Type = Extend });
            // Mn   [8] THAI CHARACTER MAITAIKHU..THAI CHARACTER YAMAKKAN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00E47, End = 0x00E4E, Type = Extend });
            // Nd  [10] THAI DIGIT ZERO..THAI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00E50, End = 0x00E59, Type = Numeric });
            // Mn   [9] LAO VOWEL SIGN I..LAO SEMIVOWEL SIGN LO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00EB4, End = 0x00EBC, Type = Extend });
            // Mn   [6] LAO TONE MAI EK..LAO NIGGAHITA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00EC8, End = 0x00ECD, Type = Extend });
            // Nd  [10] LAO DIGIT ZERO..LAO DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00ED0, End = 0x00ED9, Type = Numeric });
            // Mn   [2] TIBETAN ASTROLOGICAL SIGN -KHYUD PA..TIBETAN ASTROLOGICAL SIGN SDONG TSHUGS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F18, End = 0x00F19, Type = Extend });
            // Nd  [10] TIBETAN DIGIT ZERO..TIBETAN DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F20, End = 0x00F29, Type = Numeric });
            // Mc   [2] TIBETAN SIGN YAR TSHES..TIBETAN SIGN MAR TSHES
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F3E, End = 0x00F3F, Type = Extend });
            // Lo   [8] TIBETAN LETTER KA..TIBETAN LETTER JA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F40, End = 0x00F47, Type = ALetter });
            // Lo  [36] TIBETAN LETTER NYA..TIBETAN LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F49, End = 0x00F6C, Type = ALetter });
            // Mn  [14] TIBETAN VOWEL SIGN AA..TIBETAN SIGN RJES SU NGA RO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F71, End = 0x00F7E, Type = Extend });
            // Mn   [5] TIBETAN VOWEL SIGN REVERSED I..TIBETAN MARK HALANTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F80, End = 0x00F84, Type = Extend });
            // Mn   [2] TIBETAN SIGN LCI RTAGS..TIBETAN SIGN YANG RTAGS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F86, End = 0x00F87, Type = Extend });
            // Lo   [5] TIBETAN SIGN LCE TSA CAN..TIBETAN SIGN INVERTED MCHU CAN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F88, End = 0x00F8C, Type = ALetter });
            // Mn  [11] TIBETAN SUBJOINED SIGN LCE TSA CAN..TIBETAN SUBJOINED LETTER JA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F8D, End = 0x00F97, Type = Extend });
            // Mn  [36] TIBETAN SUBJOINED LETTER NYA..TIBETAN SUBJOINED LETTER FIXED-FORM RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x00F99, End = 0x00FBC, Type = Extend });
            // Mc   [2] MYANMAR VOWEL SIGN TALL AA..MYANMAR VOWEL SIGN AA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0102B, End = 0x0102C, Type = Extend });
            // Mn   [4] MYANMAR VOWEL SIGN I..MYANMAR VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0102D, End = 0x01030, Type = Extend });
            // Mn   [6] MYANMAR VOWEL SIGN AI..MYANMAR SIGN DOT BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01032, End = 0x01037, Type = Extend });
            // Mn   [2] MYANMAR SIGN VIRAMA..MYANMAR SIGN ASAT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01039, End = 0x0103A, Type = Extend });
            // Mc   [2] MYANMAR CONSONANT SIGN MEDIAL YA..MYANMAR CONSONANT SIGN MEDIAL RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0103B, End = 0x0103C, Type = Extend });
            // Mn   [2] MYANMAR CONSONANT SIGN MEDIAL WA..MYANMAR CONSONANT SIGN MEDIAL HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0103D, End = 0x0103E, Type = Extend });
            // Nd  [10] MYANMAR DIGIT ZERO..MYANMAR DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01040, End = 0x01049, Type = Numeric });
            // Mc   [2] MYANMAR VOWEL SIGN VOCALIC R..MYANMAR VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01056, End = 0x01057, Type = Extend });
            // Mn   [2] MYANMAR VOWEL SIGN VOCALIC L..MYANMAR VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01058, End = 0x01059, Type = Extend });
            // Mn   [3] MYANMAR CONSONANT SIGN MON MEDIAL NA..MYANMAR CONSONANT SIGN MON MEDIAL LA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0105E, End = 0x01060, Type = Extend });
            // Mc   [3] MYANMAR VOWEL SIGN SGAW KAREN EU..MYANMAR TONE MARK SGAW KAREN KE PHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01062, End = 0x01064, Type = Extend });
            // Mc   [7] MYANMAR VOWEL SIGN WESTERN PWO KAREN EU..MYANMAR SIGN WESTERN PWO KAREN TONE-5
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01067, End = 0x0106D, Type = Extend });
            // Mn   [4] MYANMAR VOWEL SIGN GEBA KAREN I..MYANMAR VOWEL SIGN KAYAH EE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01071, End = 0x01074, Type = Extend });
            // Mc   [2] MYANMAR VOWEL SIGN SHAN AA..MYANMAR VOWEL SIGN SHAN E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01083, End = 0x01084, Type = Extend });
            // Mn   [2] MYANMAR VOWEL SIGN SHAN E ABOVE..MYANMAR VOWEL SIGN SHAN FINAL Y
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01085, End = 0x01086, Type = Extend });
            // Mc   [6] MYANMAR SIGN SHAN TONE-2..MYANMAR SIGN SHAN COUNCIL TONE-3
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01087, End = 0x0108C, Type = Extend });
            // Nd  [10] MYANMAR SHAN DIGIT ZERO..MYANMAR SHAN DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01090, End = 0x01099, Type = Numeric });
            // Mc   [3] MYANMAR SIGN KHAMTI TONE-1..MYANMAR VOWEL SIGN AITON A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0109A, End = 0x0109C, Type = Extend });
            // L&  [38] GEORGIAN CAPITAL LETTER AN..GEORGIAN CAPITAL LETTER HOE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x010A0, End = 0x010C5, Type = ALetter });
            // L&  [43] GEORGIAN LETTER AN..GEORGIAN LETTER AIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x010D0, End = 0x010FA, Type = ALetter });
            // L&   [3] GEORGIAN LETTER AEN..GEORGIAN LETTER LABIAL SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x010FD, End = 0x010FF, Type = ALetter });
            // Lo [329] HANGUL CHOSEONG KIYEOK..ETHIOPIC SYLLABLE QWA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01100, End = 0x01248, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE QWI..ETHIOPIC SYLLABLE QWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0124A, End = 0x0124D, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE QHA..ETHIOPIC SYLLABLE QHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01250, End = 0x01256, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE QHWI..ETHIOPIC SYLLABLE QHWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0125A, End = 0x0125D, Type = ALetter });
            // Lo  [41] ETHIOPIC SYLLABLE BA..ETHIOPIC SYLLABLE XWA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01260, End = 0x01288, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE XWI..ETHIOPIC SYLLABLE XWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0128A, End = 0x0128D, Type = ALetter });
            // Lo  [33] ETHIOPIC SYLLABLE NA..ETHIOPIC SYLLABLE KWA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01290, End = 0x012B0, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE KWI..ETHIOPIC SYLLABLE KWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x012B2, End = 0x012B5, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE KXA..ETHIOPIC SYLLABLE KXO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x012B8, End = 0x012BE, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE KXWI..ETHIOPIC SYLLABLE KXWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x012C2, End = 0x012C5, Type = ALetter });
            // Lo  [15] ETHIOPIC SYLLABLE WA..ETHIOPIC SYLLABLE PHARYNGEAL O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x012C8, End = 0x012D6, Type = ALetter });
            // Lo  [57] ETHIOPIC SYLLABLE ZA..ETHIOPIC SYLLABLE GWA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x012D8, End = 0x01310, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE GWI..ETHIOPIC SYLLABLE GWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01312, End = 0x01315, Type = ALetter });
            // Lo  [67] ETHIOPIC SYLLABLE GGA..ETHIOPIC SYLLABLE FYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01318, End = 0x0135A, Type = ALetter });
            // Mn   [3] ETHIOPIC COMBINING GEMINATION AND VOWEL LENGTH MARK..ETHIOPIC COMBINING GEMINATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0135D, End = 0x0135F, Type = Extend });
            // Lo  [16] ETHIOPIC SYLLABLE SEBATBEIT MWA..ETHIOPIC SYLLABLE PWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01380, End = 0x0138F, Type = ALetter });
            // L&  [86] CHEROKEE LETTER A..CHEROKEE LETTER MV
            m_lst_code_range.Add(new RangeInfo() { Start = 0x013A0, End = 0x013F5, Type = ALetter });
            // L&   [6] CHEROKEE SMALL LETTER YE..CHEROKEE SMALL LETTER MV
            m_lst_code_range.Add(new RangeInfo() { Start = 0x013F8, End = 0x013FD, Type = ALetter });
            // Lo [620] CANADIAN SYLLABICS E..CANADIAN SYLLABICS CARRIER TTSA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01401, End = 0x0166C, Type = ALetter });
            // Lo  [17] CANADIAN SYLLABICS QAI..CANADIAN SYLLABICS BLACKFOOT W
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0166F, End = 0x0167F, Type = ALetter });
            // Lo  [26] OGHAM LETTER BEITH..OGHAM LETTER PEITH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01681, End = 0x0169A, Type = ALetter });
            // Lo  [75] RUNIC LETTER FEHU FEOH FE F..RUNIC LETTER X
            m_lst_code_range.Add(new RangeInfo() { Start = 0x016A0, End = 0x016EA, Type = ALetter });
            // Nl   [3] RUNIC ARLAUG SYMBOL..RUNIC BELGTHOR SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x016EE, End = 0x016F0, Type = ALetter });
            // Lo   [8] RUNIC LETTER K..RUNIC LETTER FRANKS CASKET AESC
            m_lst_code_range.Add(new RangeInfo() { Start = 0x016F1, End = 0x016F8, Type = ALetter });
            // Lo  [18] TAGALOG LETTER A..TAGALOG LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01700, End = 0x01711, Type = ALetter });
            // Mn   [3] TAGALOG VOWEL SIGN I..TAGALOG SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01712, End = 0x01714, Type = Extend });
            // Lo  [19] TAGALOG LETTER ARCHAIC RA..HANUNOO LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0171F, End = 0x01731, Type = ALetter });
            // Mn   [2] HANUNOO VOWEL SIGN I..HANUNOO VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01732, End = 0x01733, Type = Extend });
            // Lo  [18] BUHID LETTER A..BUHID LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01740, End = 0x01751, Type = ALetter });
            // Mn   [2] BUHID VOWEL SIGN I..BUHID VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01752, End = 0x01753, Type = Extend });
            // Lo  [13] TAGBANWA LETTER A..TAGBANWA LETTER YA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01760, End = 0x0176C, Type = ALetter });
            // Lo   [3] TAGBANWA LETTER LA..TAGBANWA LETTER SA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0176E, End = 0x01770, Type = ALetter });
            // Mn   [2] TAGBANWA VOWEL SIGN I..TAGBANWA VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01772, End = 0x01773, Type = Extend });
            // Mn   [2] KHMER VOWEL INHERENT AQ..KHMER VOWEL INHERENT AA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017B4, End = 0x017B5, Type = Extend });
            // Mn   [7] KHMER VOWEL SIGN I..KHMER VOWEL SIGN UA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017B7, End = 0x017BD, Type = Extend });
            // Mc   [8] KHMER VOWEL SIGN OE..KHMER VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017BE, End = 0x017C5, Type = Extend });
            // Mc   [2] KHMER SIGN REAHMUK..KHMER SIGN YUUKALEAPINTU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017C7, End = 0x017C8, Type = Extend });
            // Mn  [11] KHMER SIGN MUUSIKATOAN..KHMER SIGN BATHAMASAT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017C9, End = 0x017D3, Type = Extend });
            // Nd  [10] KHMER DIGIT ZERO..KHMER DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x017E0, End = 0x017E9, Type = Numeric });
            // Mn   [3] MONGOLIAN FREE VARIATION SELECTOR ONE..MONGOLIAN FREE VARIATION SELECTOR THREE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0180B, End = 0x0180D, Type = Extend });
            // Nd  [10] MONGOLIAN DIGIT ZERO..MONGOLIAN DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01810, End = 0x01819, Type = Numeric });
            // Lo  [35] MONGOLIAN LETTER A..MONGOLIAN LETTER CHI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01820, End = 0x01842, Type = ALetter });
            // Lo  [53] MONGOLIAN LETTER TODO E..MONGOLIAN LETTER CHA WITH TWO DOTS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01844, End = 0x01878, Type = ALetter });
            // Lo   [5] MONGOLIAN LETTER ALI GALI ANUSVARA ONE..MONGOLIAN LETTER ALI GALI INVERTED UBADAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01880, End = 0x01884, Type = ALetter });
            // Mn   [2] MONGOLIAN LETTER ALI GALI BALUDA..MONGOLIAN LETTER ALI GALI THREE BALUDA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01885, End = 0x01886, Type = Extend });
            // Lo  [34] MONGOLIAN LETTER ALI GALI A..MONGOLIAN LETTER MANCHU ALI GALI BHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01887, End = 0x018A8, Type = ALetter });
            // Lo  [70] CANADIAN SYLLABICS OY..CANADIAN SYLLABICS CARRIER DENTAL S
            m_lst_code_range.Add(new RangeInfo() { Start = 0x018B0, End = 0x018F5, Type = ALetter });
            // Lo  [31] LIMBU VOWEL-CARRIER LETTER..LIMBU LETTER TRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01900, End = 0x0191E, Type = ALetter });
            // Mn   [3] LIMBU VOWEL SIGN A..LIMBU VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01920, End = 0x01922, Type = Extend });
            // Mc   [4] LIMBU VOWEL SIGN EE..LIMBU VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01923, End = 0x01926, Type = Extend });
            // Mn   [2] LIMBU VOWEL SIGN E..LIMBU VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01927, End = 0x01928, Type = Extend });
            // Mc   [3] LIMBU SUBJOINED LETTER YA..LIMBU SUBJOINED LETTER WA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01929, End = 0x0192B, Type = Extend });
            // Mc   [2] LIMBU SMALL LETTER KA..LIMBU SMALL LETTER NGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01930, End = 0x01931, Type = Extend });
            // Mc   [6] LIMBU SMALL LETTER TA..LIMBU SMALL LETTER LA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01933, End = 0x01938, Type = Extend });
            // Mn   [3] LIMBU SIGN MUKPHRENG..LIMBU SIGN SA-I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01939, End = 0x0193B, Type = Extend });
            // Nd  [10] LIMBU DIGIT ZERO..LIMBU DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01946, End = 0x0194F, Type = Numeric });
            // Nd  [10] NEW TAI LUE DIGIT ZERO..NEW TAI LUE DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x019D0, End = 0x019D9, Type = Numeric });
            // Lo  [23] BUGINESE LETTER KA..BUGINESE LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A00, End = 0x01A16, Type = ALetter });
            // Mn   [2] BUGINESE VOWEL SIGN I..BUGINESE VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A17, End = 0x01A18, Type = Extend });
            // Mc   [2] BUGINESE VOWEL SIGN E..BUGINESE VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A19, End = 0x01A1A, Type = Extend });
            // Mn   [7] TAI THAM SIGN MAI KANG LAI..TAI THAM CONSONANT SIGN SA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A58, End = 0x01A5E, Type = Extend });
            // Mc   [2] TAI THAM VOWEL SIGN AA..TAI THAM VOWEL SIGN TALL AA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A63, End = 0x01A64, Type = Extend });
            // Mn   [8] TAI THAM VOWEL SIGN I..TAI THAM VOWEL SIGN OA BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A65, End = 0x01A6C, Type = Extend });
            // Mc   [6] TAI THAM VOWEL SIGN OY..TAI THAM VOWEL SIGN THAM AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A6D, End = 0x01A72, Type = Extend });
            // Mn  [10] TAI THAM VOWEL SIGN OA ABOVE..TAI THAM SIGN KHUEN-LUE KARAN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A73, End = 0x01A7C, Type = Extend });
            // Nd  [10] TAI THAM HORA DIGIT ZERO..TAI THAM HORA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A80, End = 0x01A89, Type = Numeric });
            // Nd  [10] TAI THAM THAM DIGIT ZERO..TAI THAM THAM DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01A90, End = 0x01A99, Type = Numeric });
            // Mn  [14] COMBINING DOUBLED CIRCUMFLEX ACCENT..COMBINING PARENTHESES BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01AB0, End = 0x01ABD, Type = Extend });
            // Mn  [16] COMBINING LATIN SMALL LETTER W BELOW..COMBINING LATIN SMALL LETTER INSULAR T
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01ABF, End = 0x01ACE, Type = Extend });
            // Mn   [4] BALINESE SIGN ULU RICEM..BALINESE SIGN SURANG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B00, End = 0x01B03, Type = Extend });
            // Lo  [47] BALINESE LETTER AKARA..BALINESE LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B05, End = 0x01B33, Type = ALetter });
            // Mn   [5] BALINESE VOWEL SIGN ULU..BALINESE VOWEL SIGN RA REPA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B36, End = 0x01B3A, Type = Extend });
            // Mc   [5] BALINESE VOWEL SIGN LA LENGA TEDUNG..BALINESE VOWEL SIGN TALING REPA TEDUNG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B3D, End = 0x01B41, Type = Extend });
            // Mc   [2] BALINESE VOWEL SIGN PEPET TEDUNG..BALINESE ADEG ADEG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B43, End = 0x01B44, Type = Extend });
            // Lo   [8] BALINESE LETTER KAF SASAK..BALINESE LETTER ARCHAIC JNYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B45, End = 0x01B4C, Type = ALetter });
            // Nd  [10] BALINESE DIGIT ZERO..BALINESE DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B50, End = 0x01B59, Type = Numeric });
            // Mn   [9] BALINESE MUSICAL SYMBOL COMBINING TEGEH..BALINESE MUSICAL SYMBOL COMBINING GONG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B6B, End = 0x01B73, Type = Extend });
            // Mn   [2] SUNDANESE SIGN PANYECEK..SUNDANESE SIGN PANGLAYAR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B80, End = 0x01B81, Type = Extend });
            // Lo  [30] SUNDANESE LETTER A..SUNDANESE LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01B83, End = 0x01BA0, Type = ALetter });
            // Mn   [4] SUNDANESE CONSONANT SIGN PANYAKRA..SUNDANESE VOWEL SIGN PANYUKU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BA2, End = 0x01BA5, Type = Extend });
            // Mc   [2] SUNDANESE VOWEL SIGN PANAELAENG..SUNDANESE VOWEL SIGN PANOLONG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BA6, End = 0x01BA7, Type = Extend });
            // Mn   [2] SUNDANESE VOWEL SIGN PAMEPET..SUNDANESE VOWEL SIGN PANEULEUNG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BA8, End = 0x01BA9, Type = Extend });
            // Mn   [3] SUNDANESE SIGN VIRAMA..SUNDANESE CONSONANT SIGN PASANGAN WA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BAB, End = 0x01BAD, Type = Extend });
            // Lo   [2] SUNDANESE LETTER KHA..SUNDANESE LETTER SYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BAE, End = 0x01BAF, Type = ALetter });
            // Nd  [10] SUNDANESE DIGIT ZERO..SUNDANESE DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BB0, End = 0x01BB9, Type = Numeric });
            // Lo  [44] SUNDANESE AVAGRAHA..BATAK LETTER U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BBA, End = 0x01BE5, Type = ALetter });
            // Mn   [2] BATAK VOWEL SIGN PAKPAK E..BATAK VOWEL SIGN EE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BE8, End = 0x01BE9, Type = Extend });
            // Mc   [3] BATAK VOWEL SIGN I..BATAK VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BEA, End = 0x01BEC, Type = Extend });
            // Mn   [3] BATAK VOWEL SIGN U FOR SIMALUNGUN SA..BATAK CONSONANT SIGN H
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BEF, End = 0x01BF1, Type = Extend });
            // Mc   [2] BATAK PANGOLAT..BATAK PANONGONAN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01BF2, End = 0x01BF3, Type = Extend });
            // Lo  [36] LEPCHA LETTER KA..LEPCHA LETTER A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C00, End = 0x01C23, Type = ALetter });
            // Mc   [8] LEPCHA SUBJOINED LETTER YA..LEPCHA VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C24, End = 0x01C2B, Type = Extend });
            // Mn   [8] LEPCHA VOWEL SIGN E..LEPCHA CONSONANT SIGN T
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C2C, End = 0x01C33, Type = Extend });
            // Mc   [2] LEPCHA CONSONANT SIGN NYIN-DO..LEPCHA CONSONANT SIGN KANG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C34, End = 0x01C35, Type = Extend });
            // Mn   [2] LEPCHA SIGN RAN..LEPCHA SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C36, End = 0x01C37, Type = Extend });
            // Nd  [10] LEPCHA DIGIT ZERO..LEPCHA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C40, End = 0x01C49, Type = Numeric });
            // Lo   [3] LEPCHA LETTER TTA..LEPCHA LETTER DDA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C4D, End = 0x01C4F, Type = ALetter });
            // Nd  [10] OL CHIKI DIGIT ZERO..OL CHIKI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C50, End = 0x01C59, Type = Numeric });
            // Lo  [30] OL CHIKI LETTER LA..OL CHIKI LETTER OH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C5A, End = 0x01C77, Type = ALetter });
            // Lm   [6] OL CHIKI MU TTUDDAG..OL CHIKI AHAD
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C78, End = 0x01C7D, Type = ALetter });
            // L&   [9] CYRILLIC SMALL LETTER ROUNDED VE..CYRILLIC SMALL LETTER UNBLENDED UK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C80, End = 0x01C88, Type = ALetter });
            // L&  [43] GEORGIAN MTAVRULI CAPITAL LETTER AN..GEORGIAN MTAVRULI CAPITAL LETTER AIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01C90, End = 0x01CBA, Type = ALetter });
            // L&   [3] GEORGIAN MTAVRULI CAPITAL LETTER AEN..GEORGIAN MTAVRULI CAPITAL LETTER LABIAL SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CBD, End = 0x01CBF, Type = ALetter });
            // Mn   [3] VEDIC TONE KARSHANA..VEDIC TONE PRENKHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CD0, End = 0x01CD2, Type = Extend });
            // Mn  [13] VEDIC SIGN YAJURVEDIC MIDLINE SVARITA..VEDIC TONE RIGVEDIC KASHMIRI INDEPENDENT SVARITA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CD4, End = 0x01CE0, Type = Extend });
            // Mn   [7] VEDIC SIGN VISARGA SVARITA..VEDIC SIGN VISARGA ANUDATTA WITH TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CE2, End = 0x01CE8, Type = Extend });
            // Lo   [4] VEDIC SIGN ANUSVARA ANTARGOMUKHA..VEDIC SIGN ANUSVARA VAMAGOMUKHA WITH TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CE9, End = 0x01CEC, Type = ALetter });
            // Lo   [6] VEDIC SIGN HEXIFORM LONG ANUSVARA..VEDIC SIGN ROTATED ARDHAVISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CEE, End = 0x01CF3, Type = ALetter });
            // Lo   [2] VEDIC SIGN JIHVAMULIYA..VEDIC SIGN UPADHMANIYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CF5, End = 0x01CF6, Type = ALetter });
            // Mn   [2] VEDIC TONE RING ABOVE..VEDIC TONE DOUBLE RING ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01CF8, End = 0x01CF9, Type = Extend });
            // L&  [44] LATIN LETTER SMALL CAPITAL A..CYRILLIC LETTER SMALL CAPITAL EL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01D00, End = 0x01D2B, Type = ALetter });
            // Lm  [63] MODIFIER LETTER CAPITAL A..GREEK SUBSCRIPT SMALL LETTER CHI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01D2C, End = 0x01D6A, Type = ALetter });
            // L&  [13] LATIN SMALL LETTER UE..LATIN SMALL LETTER TURNED G
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01D6B, End = 0x01D77, Type = ALetter });
            // L&  [34] LATIN SMALL LETTER INSULAR G..LATIN SMALL LETTER EZH WITH RETROFLEX HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01D79, End = 0x01D9A, Type = ALetter });
            // Lm  [37] MODIFIER LETTER SMALL TURNED ALPHA..MODIFIER LETTER SMALL THETA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01D9B, End = 0x01DBF, Type = ALetter });
            // Mn  [64] COMBINING DOTTED GRAVE ACCENT..COMBINING RIGHT ARROWHEAD AND DOWN ARROWHEAD BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01DC0, End = 0x01DFF, Type = Extend });
            // L& [278] LATIN CAPITAL LETTER A WITH RING BELOW..GREEK SMALL LETTER EPSILON WITH DASIA AND OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01E00, End = 0x01F15, Type = ALetter });
            // L&   [6] GREEK CAPITAL LETTER EPSILON WITH PSILI..GREEK CAPITAL LETTER EPSILON WITH DASIA AND OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F18, End = 0x01F1D, Type = ALetter });
            // L&  [38] GREEK SMALL LETTER ETA WITH PSILI..GREEK SMALL LETTER OMICRON WITH DASIA AND OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F20, End = 0x01F45, Type = ALetter });
            // L&   [6] GREEK CAPITAL LETTER OMICRON WITH PSILI..GREEK CAPITAL LETTER OMICRON WITH DASIA AND OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F48, End = 0x01F4D, Type = ALetter });
            // L&   [8] GREEK SMALL LETTER UPSILON WITH PSILI..GREEK SMALL LETTER UPSILON WITH DASIA AND PERISPOMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F50, End = 0x01F57, Type = ALetter });
            // L&  [31] GREEK CAPITAL LETTER UPSILON WITH DASIA AND PERISPOMENI..GREEK SMALL LETTER OMEGA WITH OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F5F, End = 0x01F7D, Type = ALetter });
            // L&  [53] GREEK SMALL LETTER ALPHA WITH PSILI AND YPOGEGRAMMENI..GREEK SMALL LETTER ALPHA WITH OXIA AND YPOGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01F80, End = 0x01FB4, Type = ALetter });
            // L&   [7] GREEK SMALL LETTER ALPHA WITH PERISPOMENI..GREEK CAPITAL LETTER ALPHA WITH PROSGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FB6, End = 0x01FBC, Type = ALetter });
            // L&   [3] GREEK SMALL LETTER ETA WITH VARIA AND YPOGEGRAMMENI..GREEK SMALL LETTER ETA WITH OXIA AND YPOGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FC2, End = 0x01FC4, Type = ALetter });
            // L&   [7] GREEK SMALL LETTER ETA WITH PERISPOMENI..GREEK CAPITAL LETTER ETA WITH PROSGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FC6, End = 0x01FCC, Type = ALetter });
            // L&   [4] GREEK SMALL LETTER IOTA WITH VRACHY..GREEK SMALL LETTER IOTA WITH DIALYTIKA AND OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FD0, End = 0x01FD3, Type = ALetter });
            // L&   [6] GREEK SMALL LETTER IOTA WITH PERISPOMENI..GREEK CAPITAL LETTER IOTA WITH OXIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FD6, End = 0x01FDB, Type = ALetter });
            // L&  [13] GREEK SMALL LETTER UPSILON WITH VRACHY..GREEK CAPITAL LETTER RHO WITH DASIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FE0, End = 0x01FEC, Type = ALetter });
            // L&   [3] GREEK SMALL LETTER OMEGA WITH VARIA AND YPOGEGRAMMENI..GREEK SMALL LETTER OMEGA WITH OXIA AND YPOGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FF2, End = 0x01FF4, Type = ALetter });
            // L&   [7] GREEK SMALL LETTER OMEGA WITH PERISPOMENI..GREEK CAPITAL LETTER OMEGA WITH PROSGEGRAMMENI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x01FF6, End = 0x01FFC, Type = ALetter });
            // Zs   [7] EN QUAD..SIX-PER-EM SPACE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02000, End = 0x02006, Type = WSegSpace });
            // Zs   [3] PUNCTUATION SPACE..HAIR SPACE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02008, End = 0x0200A, Type = WSegSpace });
            // Cf   [2] LEFT-TO-RIGHT MARK..RIGHT-TO-LEFT MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0200E, End = 0x0200F, Type = Format });
            // Cf   [5] LEFT-TO-RIGHT EMBEDDING..RIGHT-TO-LEFT OVERRIDE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0202A, End = 0x0202E, Type = Format });
            // Pc   [2] UNDERTIE..CHARACTER TIE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0203F, End = 0x02040, Type = ExtendNumLet });
            // Cf   [5] WORD JOINER..INVISIBLE PLUS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02060, End = 0x02064, Type = Format });
            // Cf  [10] LEFT-TO-RIGHT ISOLATE..NOMINAL DIGIT SHAPES
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02066, End = 0x0206F, Type = Format });
            // Lm  [13] LATIN SUBSCRIPT SMALL LETTER A..LATIN SUBSCRIPT SMALL LETTER T
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02090, End = 0x0209C, Type = ALetter });
            // Mn  [13] COMBINING LEFT HARPOON ABOVE..COMBINING FOUR DOTS ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x020D0, End = 0x020DC, Type = Extend });
            // Me   [4] COMBINING ENCLOSING CIRCLE..COMBINING ENCLOSING CIRCLE BACKSLASH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x020DD, End = 0x020E0, Type = Extend });
            // Me   [3] COMBINING ENCLOSING SCREEN..COMBINING ENCLOSING UPWARD POINTING TRIANGLE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x020E2, End = 0x020E4, Type = Extend });
            // Mn  [12] COMBINING REVERSE SOLIDUS OVERLAY..COMBINING ASTERISK ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x020E5, End = 0x020F0, Type = Extend });
            // L&  [10] SCRIPT SMALL G..SCRIPT SMALL L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0210A, End = 0x02113, Type = ALetter });
            // L&   [5] DOUBLE-STRUCK CAPITAL P..DOUBLE-STRUCK CAPITAL R
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02119, End = 0x0211D, Type = ALetter });
            // L&   [4] KELVIN SIGN..BLACK-LETTER CAPITAL C
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0212A, End = 0x0212D, Type = ALetter });
            // L&   [6] SCRIPT SMALL E..SCRIPT SMALL O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0212F, End = 0x02134, Type = ALetter });
            // Lo   [4] ALEF SYMBOL..DALET SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02135, End = 0x02138, Type = ALetter });
            // L&   [4] DOUBLE-STRUCK SMALL PI..DOUBLE-STRUCK CAPITAL PI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0213C, End = 0x0213F, Type = ALetter });
            // L&   [5] DOUBLE-STRUCK ITALIC CAPITAL D..DOUBLE-STRUCK ITALIC SMALL J
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02145, End = 0x02149, Type = ALetter });
            // Nl  [35] ROMAN NUMERAL ONE..ROMAN NUMERAL TEN THOUSAND
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02160, End = 0x02182, Type = ALetter });
            // L&   [2] ROMAN NUMERAL REVERSED ONE HUNDRED..LATIN SMALL LETTER REVERSED C
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02183, End = 0x02184, Type = ALetter });
            // Nl   [4] ROMAN NUMERAL SIX LATE FORM..ROMAN NUMERAL ONE HUNDRED THOUSAND
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02185, End = 0x02188, Type = ALetter });
            // E0.6   [6] (↔️..↙️)    left-right arrow..down-left arrow
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02194, End = 0x02199, Type = Extended_Pictographic });
            // E0.6   [2] (↩️..↪️)    right arrow curving left..left arrow curving right
            m_lst_code_range.Add(new RangeInfo() { Start = 0x021A9, End = 0x021AA, Type = Extended_Pictographic });
            // E0.6   [2] (⌚..⌛)    watch..hourglass done
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0231A, End = 0x0231B, Type = Extended_Pictographic });
            // E0.6   [4] (⏩..⏬)    fast-forward button..fast down button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x023E9, End = 0x023EC, Type = Extended_Pictographic });
            // E0.7   [2] (⏭️..⏮️)    next track button..last track button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x023ED, End = 0x023EE, Type = Extended_Pictographic });
            // E1.0   [2] (⏱️..⏲️)    stopwatch..timer clock
            m_lst_code_range.Add(new RangeInfo() { Start = 0x023F1, End = 0x023F2, Type = Extended_Pictographic });
            // E0.7   [3] (⏸️..⏺️)    pause button..record button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x023F8, End = 0x023FA, Type = Extended_Pictographic });
            // So  [52] CIRCLED LATIN CAPITAL LETTER A..CIRCLED LATIN SMALL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x024B6, End = 0x024E9, Type = ALetter });
            // E0.6   [2] (▪️..▫️)    black small square..white small square
            m_lst_code_range.Add(new RangeInfo() { Start = 0x025AA, End = 0x025AB, Type = Extended_Pictographic });
            // E0.6   [4] (◻️..◾)    white medium square..black medium-small square
            m_lst_code_range.Add(new RangeInfo() { Start = 0x025FB, End = 0x025FE, Type = Extended_Pictographic });
            // E0.6   [2] (☀️..☁️)    sun..cloud
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02600, End = 0x02601, Type = Extended_Pictographic });
            // E0.7   [2] (☂️..☃️)    umbrella..snowman
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02602, End = 0x02603, Type = Extended_Pictographic });
            // E0.0   [7] (☇..☍)    LIGHTNING..OPPOSITION
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02607, End = 0x0260D, Type = Extended_Pictographic });
            // E0.0   [2] (☏..☐)    WHITE TELEPHONE..BALLOT BOX
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0260F, End = 0x02610, Type = Extended_Pictographic });
            // E0.6   [2] (☔..☕)    umbrella with rain drops..hot beverage
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02614, End = 0x02615, Type = Extended_Pictographic });
            // E0.0   [2] (☖..☗)    WHITE SHOGI PIECE..BLACK SHOGI PIECE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02616, End = 0x02617, Type = Extended_Pictographic });
            // E0.0   [4] (☙..☜)    REVERSED ROTATED FLORAL HEART BULLET..WHITE LEFT POINTING INDEX
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02619, End = 0x0261C, Type = Extended_Pictographic });
            // E0.0   [2] (☞..☟)    WHITE RIGHT POINTING INDEX..WHITE DOWN POINTING INDEX
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0261E, End = 0x0261F, Type = Extended_Pictographic });
            // E1.0   [2] (☢️..☣️)    radioactive..biohazard
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02622, End = 0x02623, Type = Extended_Pictographic });
            // E0.0   [2] (☤..☥)    CADUCEUS..ANKH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02624, End = 0x02625, Type = Extended_Pictographic });
            // E0.0   [3] (☧..☩)    CHI RHO..CROSS OF JERUSALEM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02627, End = 0x02629, Type = Extended_Pictographic });
            // E0.0   [3] (☫..☭)    FARSI SYMBOL..HAMMER AND SICKLE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0262B, End = 0x0262D, Type = Extended_Pictographic });
            // E0.0   [8] (☰..☷)    TRIGRAM FOR HEAVEN..TRIGRAM FOR EARTH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02630, End = 0x02637, Type = Extended_Pictographic });
            // E0.7   [2] (☸️..☹️)    wheel of dharma..frowning face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02638, End = 0x02639, Type = Extended_Pictographic });
            // E0.0   [5] (☻..☿)    BLACK SMILING FACE..MERCURY
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0263B, End = 0x0263F, Type = Extended_Pictographic });
            // E0.0   [5] (♃..♇)    JUPITER..PLUTO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02643, End = 0x02647, Type = Extended_Pictographic });
            // E0.6  [12] (♈..♓)    Aries..Pisces
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02648, End = 0x02653, Type = Extended_Pictographic });
            // E0.0  [11] (♔..♞)    WHITE CHESS KING..BLACK CHESS KNIGHT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02654, End = 0x0265E, Type = Extended_Pictographic });
            // E0.0   [2] (♡..♢)    WHITE HEART SUIT..WHITE DIAMOND SUIT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02661, End = 0x02662, Type = Extended_Pictographic });
            // E0.6   [2] (♥️..♦️)    heart suit..diamond suit
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02665, End = 0x02666, Type = Extended_Pictographic });
            // E0.0  [18] (♩..♺)    QUARTER NOTE..RECYCLING SYMBOL FOR GENERIC MATERIALS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02669, End = 0x0267A, Type = Extended_Pictographic });
            // E0.0   [2] (♼..♽)    RECYCLED PAPER SYMBOL..PARTIALLY-RECYCLED PAPER SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0267C, End = 0x0267D, Type = Extended_Pictographic });
            // E0.0   [6] (⚀..⚅)    DIE FACE-1..DIE FACE-6
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02680, End = 0x02685, Type = Extended_Pictographic });
            // E0.0   [2] (⚐..⚑)    WHITE FLAG..BLACK FLAG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02690, End = 0x02691, Type = Extended_Pictographic });
            // E1.0   [2] (⚖️..⚗️)    balance scale..alembic
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02696, End = 0x02697, Type = Extended_Pictographic });
            // E1.0   [2] (⚛️..⚜️)    atom symbol..fleur-de-lis
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0269B, End = 0x0269C, Type = Extended_Pictographic });
            // E0.0   [3] (⚝..⚟)    OUTLINED WHITE STAR..THREE LINES CONVERGING LEFT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0269D, End = 0x0269F, Type = Extended_Pictographic });
            // E0.6   [2] (⚠️..⚡)    warning..high voltage
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026A0, End = 0x026A1, Type = Extended_Pictographic });
            // E0.0   [5] (⚢..⚦)    DOUBLED FEMALE SIGN..MALE WITH STROKE SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026A2, End = 0x026A6, Type = Extended_Pictographic });
            // E0.0   [2] (⚨..⚩)    VERTICAL MALE WITH STROKE SIGN..HORIZONTAL MALE WITH STROKE SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026A8, End = 0x026A9, Type = Extended_Pictographic });
            // E0.6   [2] (⚪..⚫)    white circle..black circle
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026AA, End = 0x026AB, Type = Extended_Pictographic });
            // E0.0   [4] (⚬..⚯)    MEDIUM SMALL WHITE CIRCLE..UNMARRIED PARTNERSHIP SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026AC, End = 0x026AF, Type = Extended_Pictographic });
            // E1.0   [2] (⚰️..⚱️)    coffin..funeral urn
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026B0, End = 0x026B1, Type = Extended_Pictographic });
            // E0.0  [11] (⚲..⚼)    NEUTER..SESQUIQUADRATE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026B2, End = 0x026BC, Type = Extended_Pictographic });
            // E0.6   [2] (⚽..⚾)    soccer ball..baseball
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026BD, End = 0x026BE, Type = Extended_Pictographic });
            // E0.0   [5] (⚿..⛃)    SQUARED KEY..BLACK DRAUGHTS KING
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026BF, End = 0x026C3, Type = Extended_Pictographic });
            // E0.6   [2] (⛄..⛅)    snowman without snow..sun behind cloud
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026C4, End = 0x026C5, Type = Extended_Pictographic });
            // E0.0   [2] (⛆..⛇)    RAIN..BLACK SNOWMAN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026C6, End = 0x026C7, Type = Extended_Pictographic });
            // E0.0   [5] (⛉..⛍)    TURNED WHITE SHOGI PIECE..DISABLED CAR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026C9, End = 0x026CD, Type = Extended_Pictographic });
            // E0.0  [20] (⛕..⛨)    ALTERNATE ONE-WAY LEFT WAY TRAFFIC..BLACK CROSS ON SHIELD
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026D5, End = 0x026E8, Type = Extended_Pictographic });
            // E0.0   [5] (⛫..⛯)    CASTLE..MAP SYMBOL FOR LIGHTHOUSE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026EB, End = 0x026EF, Type = Extended_Pictographic });
            // E0.7   [2] (⛰️..⛱️)    mountain..umbrella on ground
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026F0, End = 0x026F1, Type = Extended_Pictographic });
            // E0.6   [2] (⛲..⛳)    fountain..flag in hole
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026F2, End = 0x026F3, Type = Extended_Pictographic });
            // E0.7   [3] (⛷️..⛹️)    skier..person bouncing ball
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026F7, End = 0x026F9, Type = Extended_Pictographic });
            // E0.0   [2] (⛻..⛼)    JAPANESE BANK SYMBOL..HEADSTONE GRAVEYARD SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026FB, End = 0x026FC, Type = Extended_Pictographic });
            // E0.0   [4] (⛾..✁)    CUP ON BLACK SQUARE..UPPER BLADE SCISSORS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x026FE, End = 0x02701, Type = Extended_Pictographic });
            // E0.0   [2] (✃..✄)    LOWER BLADE SCISSORS..WHITE SCISSORS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02703, End = 0x02704, Type = Extended_Pictographic });
            // E0.6   [5] (✈️..✌️)    airplane..victory hand
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02708, End = 0x0270C, Type = Extended_Pictographic });
            // E0.0   [2] (✐..✑)    UPPER RIGHT PENCIL..WHITE NIB
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02710, End = 0x02711, Type = Extended_Pictographic });
            // E0.6   [2] (✳️..✴️)    eight-spoked asterisk..eight-pointed star
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02733, End = 0x02734, Type = Extended_Pictographic });
            // E0.6   [3] (❓..❕)    red question mark..white exclamation mark
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02753, End = 0x02755, Type = Extended_Pictographic });
            // E0.0   [3] (❥..❧)    ROTATED HEAVY BLACK HEART BULLET..ROTATED FLORAL HEART BULLET
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02765, End = 0x02767, Type = Extended_Pictographic });
            // E0.6   [3] (➕..➗)    plus..divide
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02795, End = 0x02797, Type = Extended_Pictographic });
            // E0.6   [2] (⤴️..⤵️)    right arrow curving up..right arrow curving down
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02934, End = 0x02935, Type = Extended_Pictographic });
            // E0.6   [3] (⬅️..⬇️)    left arrow..down arrow
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02B05, End = 0x02B07, Type = Extended_Pictographic });
            // E0.6   [2] (⬛..⬜)    black large square..white large square
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02B1B, End = 0x02B1C, Type = Extended_Pictographic });
            // L& [124] GLAGOLITIC CAPITAL LETTER AZU..LATIN LETTER SMALL CAPITAL TURNED E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02C00, End = 0x02C7B, Type = ALetter });
            // Lm   [2] LATIN SUBSCRIPT SMALL LETTER J..MODIFIER LETTER CAPITAL V
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02C7C, End = 0x02C7D, Type = ALetter });
            // L& [103] LATIN CAPITAL LETTER S WITH SWASH TAIL..COPTIC SYMBOL KAI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02C7E, End = 0x02CE4, Type = ALetter });
            // L&   [4] COPTIC CAPITAL LETTER CRYPTOGRAMMIC SHEI..COPTIC SMALL LETTER CRYPTOGRAMMIC GANGIA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02CEB, End = 0x02CEE, Type = ALetter });
            // Mn   [3] COPTIC COMBINING NI ABOVE..COPTIC COMBINING SPIRITUS LENIS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02CEF, End = 0x02CF1, Type = Extend });
            // L&   [2] COPTIC CAPITAL LETTER BOHAIRIC KHEI..COPTIC SMALL LETTER BOHAIRIC KHEI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02CF2, End = 0x02CF3, Type = ALetter });
            // L&  [38] GEORGIAN SMALL LETTER AN..GEORGIAN SMALL LETTER HOE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02D00, End = 0x02D25, Type = ALetter });
            // Lo  [56] TIFINAGH LETTER YA..TIFINAGH LETTER YO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02D30, End = 0x02D67, Type = ALetter });
            // Lo  [23] ETHIOPIC SYLLABLE LOA..ETHIOPIC SYLLABLE GGWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02D80, End = 0x02D96, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE SSA..ETHIOPIC SYLLABLE SSO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DA0, End = 0x02DA6, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE CCA..ETHIOPIC SYLLABLE CCO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DA8, End = 0x02DAE, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE ZZA..ETHIOPIC SYLLABLE ZZO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DB0, End = 0x02DB6, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE CCHA..ETHIOPIC SYLLABLE CCHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DB8, End = 0x02DBE, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE QYA..ETHIOPIC SYLLABLE QYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DC0, End = 0x02DC6, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE KYA..ETHIOPIC SYLLABLE KYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DC8, End = 0x02DCE, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE XYA..ETHIOPIC SYLLABLE XYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DD0, End = 0x02DD6, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE GYA..ETHIOPIC SYLLABLE GYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DD8, End = 0x02DDE, Type = ALetter });
            // Mn  [32] COMBINING CYRILLIC LETTER BE..COMBINING CYRILLIC LETTER IOTIFIED BIG YUS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x02DE0, End = 0x02DFF, Type = Extend });
            // Mn   [4] IDEOGRAPHIC LEVEL TONE MARK..IDEOGRAPHIC ENTERING TONE MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0302A, End = 0x0302D, Type = Extend });
            // Mc   [2] HANGUL SINGLE DOT TONE MARK..HANGUL DOUBLE DOT TONE MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0302E, End = 0x0302F, Type = Extend });
            // Lm   [5] VERTICAL KANA REPEAT MARK..VERTICAL KANA REPEAT MARK LOWER HALF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x03031, End = 0x03035, Type = Katakana });
            // Mn   [2] COMBINING KATAKANA-HIRAGANA VOICED SOUND MARK..COMBINING KATAKANA-HIRAGANA SEMI-VOICED SOUND MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x03099, End = 0x0309A, Type = Extend });
            // Sk   [2] KATAKANA-HIRAGANA VOICED SOUND MARK..KATAKANA-HIRAGANA SEMI-VOICED SOUND MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0309B, End = 0x0309C, Type = Katakana });
            // Lo  [90] KATAKANA LETTER SMALL A..KATAKANA LETTER VO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x030A1, End = 0x030FA, Type = Katakana });
            // Lm   [3] KATAKANA-HIRAGANA PROLONGED SOUND MARK..KATAKANA VOICED ITERATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x030FC, End = 0x030FE, Type = Katakana });
            // Lo  [43] BOPOMOFO LETTER B..BOPOMOFO LETTER NN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x03105, End = 0x0312F, Type = ALetter });
            // Lo  [94] HANGUL LETTER KIYEOK..HANGUL LETTER ARAEAE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x03131, End = 0x0318E, Type = ALetter });
            // Lo  [32] BOPOMOFO LETTER BU..BOPOMOFO LETTER AH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x031A0, End = 0x031BF, Type = ALetter });
            // Lo  [16] KATAKANA LETTER SMALL KU..KATAKANA LETTER SMALL RO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x031F0, End = 0x031FF, Type = Katakana });
            // So  [47] CIRCLED KATAKANA A..CIRCLED KATAKANA WO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x032D0, End = 0x032FE, Type = Katakana });
            // So  [88] SQUARE APAATO..SQUARE WATTO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x03300, End = 0x03357, Type = Katakana });
            // Lo  [21] YI SYLLABLE IT..YI SYLLABLE E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A000, End = 0x0A014, Type = ALetter });
            // Lo [1143] YI SYLLABLE BIT..YI SYLLABLE YYR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A016, End = 0x0A48C, Type = ALetter });
            // Lo  [40] LISU LETTER BA..LISU LETTER OE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A4D0, End = 0x0A4F7, Type = ALetter });
            // Lm   [6] LISU LETTER TONE MYA TI..LISU LETTER TONE MYA JEU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A4F8, End = 0x0A4FD, Type = ALetter });
            // Lo [268] VAI SYLLABLE EE..VAI SYLLABLE NG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A500, End = 0x0A60B, Type = ALetter });
            // Lo  [16] VAI SYLLABLE NDOLE FA..VAI SYMBOL JONG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A610, End = 0x0A61F, Type = ALetter });
            // Nd  [10] VAI DIGIT ZERO..VAI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A620, End = 0x0A629, Type = Numeric });
            // Lo   [2] VAI SYLLABLE NDOLE MA..VAI SYLLABLE NDOLE DO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A62A, End = 0x0A62B, Type = ALetter });
            // L&  [46] CYRILLIC CAPITAL LETTER ZEMLYA..CYRILLIC SMALL LETTER DOUBLE MONOCULAR O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A640, End = 0x0A66D, Type = ALetter });
            // Me   [3] COMBINING CYRILLIC TEN MILLIONS SIGN..COMBINING CYRILLIC THOUSAND MILLIONS SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A670, End = 0x0A672, Type = Extend });
            // Mn  [10] COMBINING CYRILLIC LETTER UKRAINIAN IE..COMBINING CYRILLIC PAYEROK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A674, End = 0x0A67D, Type = Extend });
            // L&  [28] CYRILLIC CAPITAL LETTER DWE..CYRILLIC SMALL LETTER CROSSED O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A680, End = 0x0A69B, Type = ALetter });
            // Lm   [2] MODIFIER LETTER CYRILLIC HARD SIGN..MODIFIER LETTER CYRILLIC SOFT SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A69C, End = 0x0A69D, Type = ALetter });
            // Mn   [2] COMBINING CYRILLIC LETTER EF..COMBINING CYRILLIC LETTER IOTIFIED E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A69E, End = 0x0A69F, Type = Extend });
            // Lo  [70] BAMUM LETTER A..BAMUM LETTER KI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A6A0, End = 0x0A6E5, Type = ALetter });
            // Nl  [10] BAMUM LETTER MO..BAMUM LETTER KOGHOM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A6E6, End = 0x0A6EF, Type = ALetter });
            // Mn   [2] BAMUM COMBINING MARK KOQNDON..BAMUM COMBINING MARK TUKWENTIS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A6F0, End = 0x0A6F1, Type = Extend });
            // Sk  [15] MODIFIER LETTER EXTRA-HIGH DOTTED TONE BAR..MODIFIER LETTER EXTRA-LOW LEFT-STEM TONE BAR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A708, End = 0x0A716, Type = ALetter });
            // Lm   [9] MODIFIER LETTER DOT VERTICAL BAR..MODIFIER LETTER LOW INVERTED EXCLAMATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A717, End = 0x0A71F, Type = ALetter });
            // Sk   [2] MODIFIER LETTER STRESS AND HIGH TONE..MODIFIER LETTER STRESS AND LOW TONE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A720, End = 0x0A721, Type = ALetter });
            // L&  [78] LATIN CAPITAL LETTER EGYPTOLOGICAL ALEF..LATIN SMALL LETTER CON
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A722, End = 0x0A76F, Type = ALetter });
            // L&  [23] LATIN SMALL LETTER DUM..LATIN SMALL LETTER INSULAR T
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A771, End = 0x0A787, Type = ALetter });
            // Sk   [2] MODIFIER LETTER COLON..MODIFIER LETTER SHORT EQUALS SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A789, End = 0x0A78A, Type = ALetter });
            // L&   [4] LATIN CAPITAL LETTER SALTILLO..LATIN SMALL LETTER L WITH RETROFLEX HOOK AND BELT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A78B, End = 0x0A78E, Type = ALetter });
            // L&  [59] LATIN CAPITAL LETTER N WITH DESCENDER..LATIN SMALL LETTER S WITH SHORT STROKE OVERLAY
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A790, End = 0x0A7CA, Type = ALetter });
            // L&   [2] LATIN CAPITAL LETTER CLOSED INSULAR G..LATIN SMALL LETTER CLOSED INSULAR G
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7D0, End = 0x0A7D1, Type = ALetter });
            // L&   [5] LATIN SMALL LETTER DOUBLE WYNN..LATIN SMALL LETTER SIGMOID S
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7D5, End = 0x0A7D9, Type = ALetter });
            // Lm   [3] MODIFIER LETTER CAPITAL C..MODIFIER LETTER CAPITAL Q
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7F2, End = 0x0A7F4, Type = ALetter });
            // L&   [2] LATIN CAPITAL LETTER REVERSED HALF H..LATIN SMALL LETTER REVERSED HALF H
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7F5, End = 0x0A7F6, Type = ALetter });
            // Lm   [2] MODIFIER LETTER CAPITAL H WITH STROKE..MODIFIER LETTER SMALL LIGATURE OE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7F8, End = 0x0A7F9, Type = ALetter });
            // Lo   [7] LATIN EPIGRAPHIC LETTER REVERSED F..SYLOTI NAGRI LETTER I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A7FB, End = 0x0A801, Type = ALetter });
            // Lo   [3] SYLOTI NAGRI LETTER U..SYLOTI NAGRI LETTER O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A803, End = 0x0A805, Type = ALetter });
            // Lo   [4] SYLOTI NAGRI LETTER KO..SYLOTI NAGRI LETTER GHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A807, End = 0x0A80A, Type = ALetter });
            // Lo  [23] SYLOTI NAGRI LETTER CO..SYLOTI NAGRI LETTER HO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A80C, End = 0x0A822, Type = ALetter });
            // Mc   [2] SYLOTI NAGRI VOWEL SIGN A..SYLOTI NAGRI VOWEL SIGN I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A823, End = 0x0A824, Type = Extend });
            // Mn   [2] SYLOTI NAGRI VOWEL SIGN U..SYLOTI NAGRI VOWEL SIGN E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A825, End = 0x0A826, Type = Extend });
            // Lo  [52] PHAGS-PA LETTER KA..PHAGS-PA LETTER CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A840, End = 0x0A873, Type = ALetter });
            // Mc   [2] SAURASHTRA SIGN ANUSVARA..SAURASHTRA SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A880, End = 0x0A881, Type = Extend });
            // Lo  [50] SAURASHTRA LETTER A..SAURASHTRA LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A882, End = 0x0A8B3, Type = ALetter });
            // Mc  [16] SAURASHTRA CONSONANT SIGN HAARU..SAURASHTRA VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8B4, End = 0x0A8C3, Type = Extend });
            // Mn   [2] SAURASHTRA SIGN VIRAMA..SAURASHTRA SIGN CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8C4, End = 0x0A8C5, Type = Extend });
            // Nd  [10] SAURASHTRA DIGIT ZERO..SAURASHTRA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8D0, End = 0x0A8D9, Type = Numeric });
            // Mn  [18] COMBINING DEVANAGARI DIGIT ZERO..COMBINING DEVANAGARI SIGN AVAGRAHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8E0, End = 0x0A8F1, Type = Extend });
            // Lo   [6] DEVANAGARI SIGN SPACING CANDRABINDU..DEVANAGARI SIGN CANDRABINDU AVAGRAHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8F2, End = 0x0A8F7, Type = ALetter });
            // Lo   [2] DEVANAGARI JAIN OM..DEVANAGARI LETTER AY
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A8FD, End = 0x0A8FE, Type = ALetter });
            // Nd  [10] KAYAH LI DIGIT ZERO..KAYAH LI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A900, End = 0x0A909, Type = Numeric });
            // Lo  [28] KAYAH LI LETTER KA..KAYAH LI LETTER OO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A90A, End = 0x0A925, Type = ALetter });
            // Mn   [8] KAYAH LI VOWEL UE..KAYAH LI TONE CALYA PLOPHU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A926, End = 0x0A92D, Type = Extend });
            // Lo  [23] REJANG LETTER KA..REJANG LETTER A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A930, End = 0x0A946, Type = ALetter });
            // Mn  [11] REJANG VOWEL SIGN I..REJANG CONSONANT SIGN R
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A947, End = 0x0A951, Type = Extend });
            // Mc   [2] REJANG CONSONANT SIGN H..REJANG VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A952, End = 0x0A953, Type = Extend });
            // Lo  [29] HANGUL CHOSEONG TIKEUT-MIEUM..HANGUL CHOSEONG SSANGYEORINHIEUH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A960, End = 0x0A97C, Type = ALetter });
            // Mn   [3] JAVANESE SIGN PANYANGGA..JAVANESE SIGN LAYAR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A980, End = 0x0A982, Type = Extend });
            // Lo  [47] JAVANESE LETTER A..JAVANESE LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A984, End = 0x0A9B2, Type = ALetter });
            // Mc   [2] JAVANESE VOWEL SIGN TARUNG..JAVANESE VOWEL SIGN TOLONG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9B4, End = 0x0A9B5, Type = Extend });
            // Mn   [4] JAVANESE VOWEL SIGN WULU..JAVANESE VOWEL SIGN SUKU MENDUT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9B6, End = 0x0A9B9, Type = Extend });
            // Mc   [2] JAVANESE VOWEL SIGN TALING..JAVANESE VOWEL SIGN DIRGA MURE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9BA, End = 0x0A9BB, Type = Extend });
            // Mn   [2] JAVANESE VOWEL SIGN PEPET..JAVANESE CONSONANT SIGN KERET
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9BC, End = 0x0A9BD, Type = Extend });
            // Mc   [3] JAVANESE CONSONANT SIGN PENGKAL..JAVANESE PANGKON
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9BE, End = 0x0A9C0, Type = Extend });
            // Nd  [10] JAVANESE DIGIT ZERO..JAVANESE DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9D0, End = 0x0A9D9, Type = Numeric });
            // Nd  [10] MYANMAR TAI LAING DIGIT ZERO..MYANMAR TAI LAING DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0A9F0, End = 0x0A9F9, Type = Numeric });
            // Lo  [41] CHAM LETTER A..CHAM LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA00, End = 0x0AA28, Type = ALetter });
            // Mn   [6] CHAM VOWEL SIGN AA..CHAM VOWEL SIGN OE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA29, End = 0x0AA2E, Type = Extend });
            // Mc   [2] CHAM VOWEL SIGN O..CHAM VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA2F, End = 0x0AA30, Type = Extend });
            // Mn   [2] CHAM VOWEL SIGN AU..CHAM VOWEL SIGN UE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA31, End = 0x0AA32, Type = Extend });
            // Mc   [2] CHAM CONSONANT SIGN YA..CHAM CONSONANT SIGN RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA33, End = 0x0AA34, Type = Extend });
            // Mn   [2] CHAM CONSONANT SIGN LA..CHAM CONSONANT SIGN WA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA35, End = 0x0AA36, Type = Extend });
            // Lo   [3] CHAM LETTER FINAL K..CHAM LETTER FINAL NG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA40, End = 0x0AA42, Type = ALetter });
            // Lo   [8] CHAM LETTER FINAL CH..CHAM LETTER FINAL SS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA44, End = 0x0AA4B, Type = ALetter });
            // Nd  [10] CHAM DIGIT ZERO..CHAM DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AA50, End = 0x0AA59, Type = Numeric });
            // Mn   [3] TAI VIET VOWEL I..TAI VIET VOWEL U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAB2, End = 0x0AAB4, Type = Extend });
            // Mn   [2] TAI VIET MAI KHIT..TAI VIET VOWEL IA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAB7, End = 0x0AAB8, Type = Extend });
            // Mn   [2] TAI VIET VOWEL AM..TAI VIET TONE MAI EK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AABE, End = 0x0AABF, Type = Extend });
            // Lo  [11] MEETEI MAYEK LETTER E..MEETEI MAYEK LETTER SSA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAE0, End = 0x0AAEA, Type = ALetter });
            // Mn   [2] MEETEI MAYEK VOWEL SIGN UU..MEETEI MAYEK VOWEL SIGN AAI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAEC, End = 0x0AAED, Type = Extend });
            // Mc   [2] MEETEI MAYEK VOWEL SIGN AU..MEETEI MAYEK VOWEL SIGN AAU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAEE, End = 0x0AAEF, Type = Extend });
            // Lm   [2] MEETEI MAYEK SYLLABLE REPETITION MARK..MEETEI MAYEK WORD REPETITION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AAF3, End = 0x0AAF4, Type = ALetter });
            // Lo   [6] ETHIOPIC SYLLABLE TTHU..ETHIOPIC SYLLABLE TTHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB01, End = 0x0AB06, Type = ALetter });
            // Lo   [6] ETHIOPIC SYLLABLE DDHU..ETHIOPIC SYLLABLE DDHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB09, End = 0x0AB0E, Type = ALetter });
            // Lo   [6] ETHIOPIC SYLLABLE DZU..ETHIOPIC SYLLABLE DZO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB11, End = 0x0AB16, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE CCHHA..ETHIOPIC SYLLABLE CCHHO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB20, End = 0x0AB26, Type = ALetter });
            // Lo   [7] ETHIOPIC SYLLABLE BBA..ETHIOPIC SYLLABLE BBO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB28, End = 0x0AB2E, Type = ALetter });
            // L&  [43] LATIN SMALL LETTER BARRED ALPHA..LATIN SMALL LETTER Y WITH SHORT RIGHT LEG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB30, End = 0x0AB5A, Type = ALetter });
            // Lm   [4] MODIFIER LETTER SMALL HENG..MODIFIER LETTER SMALL U WITH LEFT HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB5C, End = 0x0AB5F, Type = ALetter });
            // L&   [9] LATIN SMALL LETTER SAKHA YAT..LATIN SMALL LETTER TURNED R WITH MIDDLE TILDE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB60, End = 0x0AB68, Type = ALetter });
            // L&  [80] CHEROKEE SMALL LETTER A..CHEROKEE SMALL LETTER YA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AB70, End = 0x0ABBF, Type = ALetter });
            // Lo  [35] MEETEI MAYEK LETTER KOK..MEETEI MAYEK LETTER I LONSUM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0ABC0, End = 0x0ABE2, Type = ALetter });
            // Mc   [2] MEETEI MAYEK VOWEL SIGN ONAP..MEETEI MAYEK VOWEL SIGN INAP
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0ABE3, End = 0x0ABE4, Type = Extend });
            // Mc   [2] MEETEI MAYEK VOWEL SIGN YENAP..MEETEI MAYEK VOWEL SIGN SOUNAP
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0ABE6, End = 0x0ABE7, Type = Extend });
            // Mc   [2] MEETEI MAYEK VOWEL SIGN CHEINAP..MEETEI MAYEK VOWEL SIGN NUNG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0ABE9, End = 0x0ABEA, Type = Extend });
            // Nd  [10] MEETEI MAYEK DIGIT ZERO..MEETEI MAYEK DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0ABF0, End = 0x0ABF9, Type = Numeric });
            // Lo [11172] HANGUL SYLLABLE GA..HANGUL SYLLABLE HIH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0AC00, End = 0x0D7A3, Type = ALetter });
            // Lo  [23] HANGUL JUNGSEONG O-YEO..HANGUL JUNGSEONG ARAEA-E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0D7B0, End = 0x0D7C6, Type = ALetter });
            // Lo  [49] HANGUL JONGSEONG NIEUN-RIEUL..HANGUL JONGSEONG PHIEUPH-THIEUTH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0D7CB, End = 0x0D7FB, Type = ALetter });
            // L&   [7] LATIN SMALL LIGATURE FF..LATIN SMALL LIGATURE ST
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB00, End = 0x0FB06, Type = ALetter });
            // L&   [5] ARMENIAN SMALL LIGATURE MEN NOW..ARMENIAN SMALL LIGATURE MEN XEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB13, End = 0x0FB17, Type = ALetter });
            // Lo  [10] HEBREW LIGATURE YIDDISH YOD YOD PATAH..HEBREW LETTER WIDE TAV
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB1F, End = 0x0FB28, Type = Hebrew_Letter });
            // Lo  [13] HEBREW LETTER SHIN WITH SHIN DOT..HEBREW LETTER ZAYIN WITH DAGESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB2A, End = 0x0FB36, Type = Hebrew_Letter });
            // Lo   [5] HEBREW LETTER TET WITH DAGESH..HEBREW LETTER LAMED WITH DAGESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB38, End = 0x0FB3C, Type = Hebrew_Letter });
            // Lo   [2] HEBREW LETTER NUN WITH DAGESH..HEBREW LETTER SAMEKH WITH DAGESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB40, End = 0x0FB41, Type = Hebrew_Letter });
            // Lo   [2] HEBREW LETTER FINAL PE WITH DAGESH..HEBREW LETTER PE WITH DAGESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB43, End = 0x0FB44, Type = Hebrew_Letter });
            // Lo  [10] HEBREW LETTER TSADI WITH DAGESH..HEBREW LIGATURE ALEF LAMED
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB46, End = 0x0FB4F, Type = Hebrew_Letter });
            // Lo  [98] ARABIC LETTER ALEF WASLA ISOLATED FORM..ARABIC LETTER YEH BARREE WITH HAMZA ABOVE FINAL FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FB50, End = 0x0FBB1, Type = ALetter });
            // Lo [363] ARABIC LETTER NG ISOLATED FORM..ARABIC LIGATURE ALEF WITH FATHATAN ISOLATED FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FBD3, End = 0x0FD3D, Type = ALetter });
            // Lo  [64] ARABIC LIGATURE TEH WITH JEEM WITH MEEM INITIAL FORM..ARABIC LIGATURE MEEM WITH KHAH WITH MEEM INITIAL FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FD50, End = 0x0FD8F, Type = ALetter });
            // Lo  [54] ARABIC LIGATURE MEEM WITH JEEM WITH KHAH INITIAL FORM..ARABIC LIGATURE NOON WITH JEEM WITH YEH FINAL FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FD92, End = 0x0FDC7, Type = ALetter });
            // Lo  [12] ARABIC LIGATURE SALLA USED AS KORANIC STOP SIGN ISOLATED FORM..ARABIC LIGATURE JALLAJALALOUHOU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FDF0, End = 0x0FDFB, Type = ALetter });
            // Mn  [16] VARIATION SELECTOR-1..VARIATION SELECTOR-16
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE00, End = 0x0FE0F, Type = Extend });
            // Mn  [16] COMBINING LIGATURE LEFT HALF..COMBINING CYRILLIC TITLO RIGHT HALF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE20, End = 0x0FE2F, Type = Extend });
            // Pc   [2] PRESENTATION FORM FOR VERTICAL LOW LINE..PRESENTATION FORM FOR VERTICAL WAVY LOW LINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE33, End = 0x0FE34, Type = ExtendNumLet });
            // Pc   [3] DASHED LOW LINE..WAVY LOW LINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE4D, End = 0x0FE4F, Type = ExtendNumLet });
            // Lo   [5] ARABIC FATHATAN ISOLATED FORM..ARABIC KASRATAN ISOLATED FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE70, End = 0x0FE74, Type = ALetter });
            // Lo [135] ARABIC FATHA ISOLATED FORM..ARABIC LIGATURE LAM WITH ALEF FINAL FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FE76, End = 0x0FEFC, Type = ALetter });
            // Nd  [10] FULLWIDTH DIGIT ZERO..FULLWIDTH DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF10, End = 0x0FF19, Type = Numeric });
            // L&  [26] FULLWIDTH LATIN CAPITAL LETTER A..FULLWIDTH LATIN CAPITAL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF21, End = 0x0FF3A, Type = ALetter });
            // L&  [26] FULLWIDTH LATIN SMALL LETTER A..FULLWIDTH LATIN SMALL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF41, End = 0x0FF5A, Type = ALetter });
            // Lo  [10] HALFWIDTH KATAKANA LETTER WO..HALFWIDTH KATAKANA LETTER SMALL TU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF66, End = 0x0FF6F, Type = Katakana });
            // Lo  [45] HALFWIDTH KATAKANA LETTER A..HALFWIDTH KATAKANA LETTER N
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF71, End = 0x0FF9D, Type = Katakana });
            // Lm   [2] HALFWIDTH KATAKANA VOICED SOUND MARK..HALFWIDTH KATAKANA SEMI-VOICED SOUND MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FF9E, End = 0x0FF9F, Type = Extend });
            // Lo  [31] HALFWIDTH HANGUL FILLER..HALFWIDTH HANGUL LETTER HIEUH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFA0, End = 0x0FFBE, Type = ALetter });
            // Lo   [6] HALFWIDTH HANGUL LETTER A..HALFWIDTH HANGUL LETTER E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFC2, End = 0x0FFC7, Type = ALetter });
            // Lo   [6] HALFWIDTH HANGUL LETTER YEO..HALFWIDTH HANGUL LETTER OE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFCA, End = 0x0FFCF, Type = ALetter });
            // Lo   [6] HALFWIDTH HANGUL LETTER YO..HALFWIDTH HANGUL LETTER YU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFD2, End = 0x0FFD7, Type = ALetter });
            // Lo   [3] HALFWIDTH HANGUL LETTER EU..HALFWIDTH HANGUL LETTER I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFDA, End = 0x0FFDC, Type = ALetter });
            // Cf   [3] INTERLINEAR ANNOTATION ANCHOR..INTERLINEAR ANNOTATION TERMINATOR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x0FFF9, End = 0x0FFFB, Type = Format });
            // Lo  [12] LINEAR B SYLLABLE B008 A..LINEAR B SYLLABLE B046 JE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10000, End = 0x1000B, Type = ALetter });
            // Lo  [26] LINEAR B SYLLABLE B036 JO..LINEAR B SYLLABLE B032 QO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1000D, End = 0x10026, Type = ALetter });
            // Lo  [19] LINEAR B SYLLABLE B060 RA..LINEAR B SYLLABLE B042 WO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10028, End = 0x1003A, Type = ALetter });
            // Lo   [2] LINEAR B SYLLABLE B017 ZA..LINEAR B SYLLABLE B074 ZE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1003C, End = 0x1003D, Type = ALetter });
            // Lo  [15] LINEAR B SYLLABLE B020 ZO..LINEAR B SYLLABLE B091 TWO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1003F, End = 0x1004D, Type = ALetter });
            // Lo  [14] LINEAR B SYMBOL B018..LINEAR B SYMBOL B089
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10050, End = 0x1005D, Type = ALetter });
            // Lo [123] LINEAR B IDEOGRAM B100 MAN..LINEAR B IDEOGRAM VESSEL B305
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10080, End = 0x100FA, Type = ALetter });
            // Nl  [53] GREEK ACROPHONIC ATTIC ONE QUARTER..GREEK ACROPHONIC STRATIAN FIFTY MNAS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10140, End = 0x10174, Type = ALetter });
            // Lo  [29] LYCIAN LETTER A..LYCIAN LETTER X
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10280, End = 0x1029C, Type = ALetter });
            // Lo  [49] CARIAN LETTER A..CARIAN LETTER UUU3
            m_lst_code_range.Add(new RangeInfo() { Start = 0x102A0, End = 0x102D0, Type = ALetter });
            // Lo  [32] OLD ITALIC LETTER A..OLD ITALIC LETTER ESS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10300, End = 0x1031F, Type = ALetter });
            // Lo  [20] OLD ITALIC LETTER YE..GOTHIC LETTER PAIRTHRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1032D, End = 0x10340, Type = ALetter });
            // Lo   [8] GOTHIC LETTER RAIDA..GOTHIC LETTER OTHAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10342, End = 0x10349, Type = ALetter });
            // Lo  [38] OLD PERMIC LETTER AN..OLD PERMIC LETTER IA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10350, End = 0x10375, Type = ALetter });
            // Mn   [5] COMBINING OLD PERMIC LETTER AN..COMBINING OLD PERMIC LETTER SII
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10376, End = 0x1037A, Type = Extend });
            // Lo  [30] UGARITIC LETTER ALPA..UGARITIC LETTER SSU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10380, End = 0x1039D, Type = ALetter });
            // Lo  [36] OLD PERSIAN SIGN A..OLD PERSIAN SIGN HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x103A0, End = 0x103C3, Type = ALetter });
            // Lo   [8] OLD PERSIAN SIGN AURAMAZDAA..OLD PERSIAN SIGN BUUMISH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x103C8, End = 0x103CF, Type = ALetter });
            // Nl   [5] OLD PERSIAN NUMBER ONE..OLD PERSIAN NUMBER HUNDRED
            m_lst_code_range.Add(new RangeInfo() { Start = 0x103D1, End = 0x103D5, Type = ALetter });
            // L&  [80] DESERET CAPITAL LETTER LONG I..DESERET SMALL LETTER EW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10400, End = 0x1044F, Type = ALetter });
            // Lo  [78] SHAVIAN LETTER PEEP..OSMANYA LETTER OO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10450, End = 0x1049D, Type = ALetter });
            // Nd  [10] OSMANYA DIGIT ZERO..OSMANYA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x104A0, End = 0x104A9, Type = Numeric });
            // L&  [36] OSAGE CAPITAL LETTER A..OSAGE CAPITAL LETTER ZHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x104B0, End = 0x104D3, Type = ALetter });
            // L&  [36] OSAGE SMALL LETTER A..OSAGE SMALL LETTER ZHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x104D8, End = 0x104FB, Type = ALetter });
            // Lo  [40] ELBASAN LETTER A..ELBASAN LETTER KHE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10500, End = 0x10527, Type = ALetter });
            // Lo  [52] CAUCASIAN ALBANIAN LETTER ALT..CAUCASIAN ALBANIAN LETTER KIW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10530, End = 0x10563, Type = ALetter });
            // L&  [11] VITHKUQI CAPITAL LETTER A..VITHKUQI CAPITAL LETTER GA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10570, End = 0x1057A, Type = ALetter });
            // L&  [15] VITHKUQI CAPITAL LETTER HA..VITHKUQI CAPITAL LETTER RE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1057C, End = 0x1058A, Type = ALetter });
            // L&   [7] VITHKUQI CAPITAL LETTER SE..VITHKUQI CAPITAL LETTER XE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1058C, End = 0x10592, Type = ALetter });
            // L&   [2] VITHKUQI CAPITAL LETTER Y..VITHKUQI CAPITAL LETTER ZE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10594, End = 0x10595, Type = ALetter });
            // L&  [11] VITHKUQI SMALL LETTER A..VITHKUQI SMALL LETTER GA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10597, End = 0x105A1, Type = ALetter });
            // L&  [15] VITHKUQI SMALL LETTER HA..VITHKUQI SMALL LETTER RE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x105A3, End = 0x105B1, Type = ALetter });
            // L&   [7] VITHKUQI SMALL LETTER SE..VITHKUQI SMALL LETTER XE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x105B3, End = 0x105B9, Type = ALetter });
            // L&   [2] VITHKUQI SMALL LETTER Y..VITHKUQI SMALL LETTER ZE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x105BB, End = 0x105BC, Type = ALetter });
            // Lo [311] LINEAR A SIGN AB001..LINEAR A SIGN A664
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10600, End = 0x10736, Type = ALetter });
            // Lo  [22] LINEAR A SIGN A701 A..LINEAR A SIGN A732 JE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10740, End = 0x10755, Type = ALetter });
            // Lo   [8] LINEAR A SIGN A800..LINEAR A SIGN A807
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10760, End = 0x10767, Type = ALetter });
            // Lm   [6] MODIFIER LETTER SMALL CAPITAL AA..MODIFIER LETTER SMALL B WITH HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10780, End = 0x10785, Type = ALetter });
            // Lm  [42] MODIFIER LETTER SMALL DZ DIGRAPH..MODIFIER LETTER SMALL V WITH RIGHT HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10787, End = 0x107B0, Type = ALetter });
            // Lm   [9] MODIFIER LETTER SMALL CAPITAL Y..MODIFIER LETTER SMALL S WITH CURL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x107B2, End = 0x107BA, Type = ALetter });
            // Lo   [6] CYPRIOT SYLLABLE A..CYPRIOT SYLLABLE JA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10800, End = 0x10805, Type = ALetter });
            // Lo  [44] CYPRIOT SYLLABLE KA..CYPRIOT SYLLABLE WO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1080A, End = 0x10835, Type = ALetter });
            // Lo   [2] CYPRIOT SYLLABLE XA..CYPRIOT SYLLABLE XE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10837, End = 0x10838, Type = ALetter });
            // Lo  [23] CYPRIOT SYLLABLE ZO..IMPERIAL ARAMAIC LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1083F, End = 0x10855, Type = ALetter });
            // Lo  [23] PALMYRENE LETTER ALEPH..PALMYRENE LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10860, End = 0x10876, Type = ALetter });
            // Lo  [31] NABATAEAN LETTER FINAL ALEPH..NABATAEAN LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10880, End = 0x1089E, Type = ALetter });
            // Lo  [19] HATRAN LETTER ALEPH..HATRAN LETTER QOPH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x108E0, End = 0x108F2, Type = ALetter });
            // Lo   [2] HATRAN LETTER SHIN..HATRAN LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x108F4, End = 0x108F5, Type = ALetter });
            // Lo  [22] PHOENICIAN LETTER ALF..PHOENICIAN LETTER TAU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10900, End = 0x10915, Type = ALetter });
            // Lo  [26] LYDIAN LETTER A..LYDIAN LETTER C
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10920, End = 0x10939, Type = ALetter });
            // Lo  [56] MEROITIC HIEROGLYPHIC LETTER A..MEROITIC CURSIVE LETTER DA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10980, End = 0x109B7, Type = ALetter });
            // Lo   [2] MEROITIC CURSIVE LOGOGRAM RMT..MEROITIC CURSIVE LOGOGRAM IMN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x109BE, End = 0x109BF, Type = ALetter });
            // Mn   [3] KHAROSHTHI VOWEL SIGN I..KHAROSHTHI VOWEL SIGN VOCALIC R
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A01, End = 0x10A03, Type = Extend });
            // Mn   [2] KHAROSHTHI VOWEL SIGN E..KHAROSHTHI VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A05, End = 0x10A06, Type = Extend });
            // Mn   [4] KHAROSHTHI VOWEL LENGTH MARK..KHAROSHTHI SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A0C, End = 0x10A0F, Type = Extend });
            // Lo   [4] KHAROSHTHI LETTER KA..KHAROSHTHI LETTER GHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A10, End = 0x10A13, Type = ALetter });
            // Lo   [3] KHAROSHTHI LETTER CA..KHAROSHTHI LETTER JA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A15, End = 0x10A17, Type = ALetter });
            // Lo  [29] KHAROSHTHI LETTER NYA..KHAROSHTHI LETTER VHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A19, End = 0x10A35, Type = ALetter });
            // Mn   [3] KHAROSHTHI SIGN BAR ABOVE..KHAROSHTHI SIGN DOT BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A38, End = 0x10A3A, Type = Extend });
            // Lo  [29] OLD SOUTH ARABIAN LETTER HE..OLD SOUTH ARABIAN LETTER THETH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A60, End = 0x10A7C, Type = ALetter });
            // Lo  [29] OLD NORTH ARABIAN LETTER HEH..OLD NORTH ARABIAN LETTER ZAH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10A80, End = 0x10A9C, Type = ALetter });
            // Lo   [8] MANICHAEAN LETTER ALEPH..MANICHAEAN LETTER WAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10AC0, End = 0x10AC7, Type = ALetter });
            // Lo  [28] MANICHAEAN LETTER ZAYIN..MANICHAEAN LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10AC9, End = 0x10AE4, Type = ALetter });
            // Mn   [2] MANICHAEAN ABBREVIATION MARK ABOVE..MANICHAEAN ABBREVIATION MARK BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10AE5, End = 0x10AE6, Type = Extend });
            // Lo  [54] AVESTAN LETTER A..AVESTAN LETTER HE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10B00, End = 0x10B35, Type = ALetter });
            // Lo  [22] INSCRIPTIONAL PARTHIAN LETTER ALEPH..INSCRIPTIONAL PARTHIAN LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10B40, End = 0x10B55, Type = ALetter });
            // Lo  [19] INSCRIPTIONAL PAHLAVI LETTER ALEPH..INSCRIPTIONAL PAHLAVI LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10B60, End = 0x10B72, Type = ALetter });
            // Lo  [18] PSALTER PAHLAVI LETTER ALEPH..PSALTER PAHLAVI LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10B80, End = 0x10B91, Type = ALetter });
            // Lo  [73] OLD TURKIC LETTER ORKHON A..OLD TURKIC LETTER ORKHON BASH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10C00, End = 0x10C48, Type = ALetter });
            // L&  [51] OLD HUNGARIAN CAPITAL LETTER A..OLD HUNGARIAN CAPITAL LETTER US
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10C80, End = 0x10CB2, Type = ALetter });
            // L&  [51] OLD HUNGARIAN SMALL LETTER A..OLD HUNGARIAN SMALL LETTER US
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10CC0, End = 0x10CF2, Type = ALetter });
            // Lo  [36] HANIFI ROHINGYA LETTER A..HANIFI ROHINGYA MARK NA KHONNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10D00, End = 0x10D23, Type = ALetter });
            // Mn   [4] HANIFI ROHINGYA SIGN HARBAHAY..HANIFI ROHINGYA SIGN TASSI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10D24, End = 0x10D27, Type = Extend });
            // Nd  [10] HANIFI ROHINGYA DIGIT ZERO..HANIFI ROHINGYA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10D30, End = 0x10D39, Type = Numeric });
            // Lo  [42] YEZIDI LETTER ELIF..YEZIDI LETTER ET
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10E80, End = 0x10EA9, Type = ALetter });
            // Mn   [2] YEZIDI COMBINING HAMZA MARK..YEZIDI COMBINING MADDA MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10EAB, End = 0x10EAC, Type = Extend });
            // Lo   [2] YEZIDI LETTER LAM WITH DOT ABOVE..YEZIDI LETTER YOT WITH CIRCUMFLEX ABOVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10EB0, End = 0x10EB1, Type = ALetter });
            // Lo  [29] OLD SOGDIAN LETTER ALEPH..OLD SOGDIAN LETTER FINAL TAW WITH VERTICAL TAIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10F00, End = 0x10F1C, Type = ALetter });
            // Lo  [22] SOGDIAN LETTER ALEPH..SOGDIAN INDEPENDENT SHIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10F30, End = 0x10F45, Type = ALetter });
            // Mn  [11] SOGDIAN COMBINING DOT BELOW..SOGDIAN COMBINING STROKE BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10F46, End = 0x10F50, Type = Extend });
            // Lo  [18] OLD UYGHUR LETTER ALEPH..OLD UYGHUR LETTER LESH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10F70, End = 0x10F81, Type = ALetter });
            // Mn   [4] OLD UYGHUR COMBINING DOT ABOVE..OLD UYGHUR COMBINING TWO DOTS BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10F82, End = 0x10F85, Type = Extend });
            // Lo  [21] CHORASMIAN LETTER ALEPH..CHORASMIAN LETTER TAW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10FB0, End = 0x10FC4, Type = ALetter });
            // Lo  [23] ELYMAIC LETTER ALEPH..ELYMAIC LIGATURE ZAYIN-YODH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x10FE0, End = 0x10FF6, Type = ALetter });
            // Lo  [53] BRAHMI SIGN JIHVAMULIYA..BRAHMI LETTER OLD TAMIL NNNA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11003, End = 0x11037, Type = ALetter });
            // Mn  [15] BRAHMI VOWEL SIGN AA..BRAHMI VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11038, End = 0x11046, Type = Extend });
            // Nd  [10] BRAHMI DIGIT ZERO..BRAHMI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11066, End = 0x1106F, Type = Numeric });
            // Lo   [2] BRAHMI LETTER OLD TAMIL SHORT E..BRAHMI LETTER OLD TAMIL SHORT O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11071, End = 0x11072, Type = ALetter });
            // Mn   [2] BRAHMI VOWEL SIGN OLD TAMIL SHORT E..BRAHMI VOWEL SIGN OLD TAMIL SHORT O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11073, End = 0x11074, Type = Extend });
            // Mn   [3] BRAHMI NUMBER JOINER..KAITHI SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1107F, End = 0x11081, Type = Extend });
            // Lo  [45] KAITHI LETTER A..KAITHI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11083, End = 0x110AF, Type = ALetter });
            // Mc   [3] KAITHI VOWEL SIGN AA..KAITHI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110B0, End = 0x110B2, Type = Extend });
            // Mn   [4] KAITHI VOWEL SIGN U..KAITHI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110B3, End = 0x110B6, Type = Extend });
            // Mc   [2] KAITHI VOWEL SIGN O..KAITHI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110B7, End = 0x110B8, Type = Extend });
            // Mn   [2] KAITHI SIGN VIRAMA..KAITHI SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110B9, End = 0x110BA, Type = Extend });
            // Lo  [25] SORA SOMPENG LETTER SAH..SORA SOMPENG LETTER MAE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110D0, End = 0x110E8, Type = ALetter });
            // Nd  [10] SORA SOMPENG DIGIT ZERO..SORA SOMPENG DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x110F0, End = 0x110F9, Type = Numeric });
            // Mn   [3] CHAKMA SIGN CANDRABINDU..CHAKMA SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11100, End = 0x11102, Type = Extend });
            // Lo  [36] CHAKMA LETTER AA..CHAKMA LETTER HAA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11103, End = 0x11126, Type = ALetter });
            // Mn   [5] CHAKMA VOWEL SIGN A..CHAKMA VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11127, End = 0x1112B, Type = Extend });
            // Mn   [8] CHAKMA VOWEL SIGN AI..CHAKMA MAAYYAA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1112D, End = 0x11134, Type = Extend });
            // Nd  [10] CHAKMA DIGIT ZERO..CHAKMA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11136, End = 0x1113F, Type = Numeric });
            // Mc   [2] CHAKMA VOWEL SIGN AA..CHAKMA VOWEL SIGN EI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11145, End = 0x11146, Type = Extend });
            // Lo  [35] MAHAJANI LETTER A..MAHAJANI LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11150, End = 0x11172, Type = ALetter });
            // Mn   [2] SHARADA SIGN CANDRABINDU..SHARADA SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11180, End = 0x11181, Type = Extend });
            // Lo  [48] SHARADA LETTER A..SHARADA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11183, End = 0x111B2, Type = ALetter });
            // Mc   [3] SHARADA VOWEL SIGN AA..SHARADA VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111B3, End = 0x111B5, Type = Extend });
            // Mn   [9] SHARADA VOWEL SIGN U..SHARADA VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111B6, End = 0x111BE, Type = Extend });
            // Mc   [2] SHARADA VOWEL SIGN AU..SHARADA SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111BF, End = 0x111C0, Type = Extend });
            // Lo   [4] SHARADA SIGN AVAGRAHA..SHARADA OM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111C1, End = 0x111C4, Type = ALetter });
            // Mn   [4] SHARADA SANDHI MARK..SHARADA EXTRA SHORT VOWEL MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111C9, End = 0x111CC, Type = Extend });
            // Nd  [10] SHARADA DIGIT ZERO..SHARADA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x111D0, End = 0x111D9, Type = Numeric });
            // Lo  [18] KHOJKI LETTER A..KHOJKI LETTER JJA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11200, End = 0x11211, Type = ALetter });
            // Lo  [25] KHOJKI LETTER NYA..KHOJKI LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11213, End = 0x1122B, Type = ALetter });
            // Mc   [3] KHOJKI VOWEL SIGN AA..KHOJKI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1122C, End = 0x1122E, Type = Extend });
            // Mn   [3] KHOJKI VOWEL SIGN U..KHOJKI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1122F, End = 0x11231, Type = Extend });
            // Mc   [2] KHOJKI VOWEL SIGN O..KHOJKI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11232, End = 0x11233, Type = Extend });
            // Mn   [2] KHOJKI SIGN NUKTA..KHOJKI SIGN SHADDA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11236, End = 0x11237, Type = Extend });
            // Lo   [7] MULTANI LETTER A..MULTANI LETTER GA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11280, End = 0x11286, Type = ALetter });
            // Lo   [4] MULTANI LETTER CA..MULTANI LETTER JJA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1128A, End = 0x1128D, Type = ALetter });
            // Lo  [15] MULTANI LETTER NYA..MULTANI LETTER BA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1128F, End = 0x1129D, Type = ALetter });
            // Lo  [10] MULTANI LETTER BHA..MULTANI LETTER RHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1129F, End = 0x112A8, Type = ALetter });
            // Lo  [47] KHUDAWADI LETTER A..KHUDAWADI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x112B0, End = 0x112DE, Type = ALetter });
            // Mc   [3] KHUDAWADI VOWEL SIGN AA..KHUDAWADI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x112E0, End = 0x112E2, Type = Extend });
            // Mn   [8] KHUDAWADI VOWEL SIGN U..KHUDAWADI SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x112E3, End = 0x112EA, Type = Extend });
            // Nd  [10] KHUDAWADI DIGIT ZERO..KHUDAWADI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x112F0, End = 0x112F9, Type = Numeric });
            // Mn   [2] GRANTHA SIGN COMBINING ANUSVARA ABOVE..GRANTHA SIGN CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11300, End = 0x11301, Type = Extend });
            // Mc   [2] GRANTHA SIGN ANUSVARA..GRANTHA SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11302, End = 0x11303, Type = Extend });
            // Lo   [8] GRANTHA LETTER A..GRANTHA LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11305, End = 0x1130C, Type = ALetter });
            // Lo   [2] GRANTHA LETTER EE..GRANTHA LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1130F, End = 0x11310, Type = ALetter });
            // Lo  [22] GRANTHA LETTER OO..GRANTHA LETTER NA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11313, End = 0x11328, Type = ALetter });
            // Lo   [7] GRANTHA LETTER PA..GRANTHA LETTER RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1132A, End = 0x11330, Type = ALetter });
            // Lo   [2] GRANTHA LETTER LA..GRANTHA LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11332, End = 0x11333, Type = ALetter });
            // Lo   [5] GRANTHA LETTER VA..GRANTHA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11335, End = 0x11339, Type = ALetter });
            // Mn   [2] COMBINING BINDU BELOW..GRANTHA SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1133B, End = 0x1133C, Type = Extend });
            // Mc   [2] GRANTHA VOWEL SIGN AA..GRANTHA VOWEL SIGN I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1133E, End = 0x1133F, Type = Extend });
            // Mc   [4] GRANTHA VOWEL SIGN U..GRANTHA VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11341, End = 0x11344, Type = Extend });
            // Mc   [2] GRANTHA VOWEL SIGN EE..GRANTHA VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11347, End = 0x11348, Type = Extend });
            // Mc   [3] GRANTHA VOWEL SIGN OO..GRANTHA SIGN VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1134B, End = 0x1134D, Type = Extend });
            // Lo   [5] GRANTHA SIGN PLUTA..GRANTHA LETTER VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1135D, End = 0x11361, Type = ALetter });
            // Mc   [2] GRANTHA VOWEL SIGN VOCALIC L..GRANTHA VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11362, End = 0x11363, Type = Extend });
            // Mn   [7] COMBINING GRANTHA DIGIT ZERO..COMBINING GRANTHA DIGIT SIX
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11366, End = 0x1136C, Type = Extend });
            // Mn   [5] COMBINING GRANTHA LETTER A..COMBINING GRANTHA LETTER PA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11370, End = 0x11374, Type = Extend });
            // Lo  [53] NEWA LETTER A..NEWA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11400, End = 0x11434, Type = ALetter });
            // Mc   [3] NEWA VOWEL SIGN AA..NEWA VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11435, End = 0x11437, Type = Extend });
            // Mn   [8] NEWA VOWEL SIGN U..NEWA VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11438, End = 0x1143F, Type = Extend });
            // Mc   [2] NEWA VOWEL SIGN O..NEWA VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11440, End = 0x11441, Type = Extend });
            // Mn   [3] NEWA SIGN VIRAMA..NEWA SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11442, End = 0x11444, Type = Extend });
            // Lo   [4] NEWA SIGN AVAGRAHA..NEWA SIDDHI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11447, End = 0x1144A, Type = ALetter });
            // Nd  [10] NEWA DIGIT ZERO..NEWA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11450, End = 0x11459, Type = Numeric });
            // Lo   [3] NEWA LETTER VEDIC ANUSVARA..NEWA SIGN UPADHMANIYA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1145F, End = 0x11461, Type = ALetter });
            // Lo  [48] TIRHUTA ANJI..TIRHUTA LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11480, End = 0x114AF, Type = ALetter });
            // Mc   [3] TIRHUTA VOWEL SIGN AA..TIRHUTA VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114B0, End = 0x114B2, Type = Extend });
            // Mn   [6] TIRHUTA VOWEL SIGN U..TIRHUTA VOWEL SIGN VOCALIC LL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114B3, End = 0x114B8, Type = Extend });
            // Mc   [4] TIRHUTA VOWEL SIGN AI..TIRHUTA VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114BB, End = 0x114BE, Type = Extend });
            // Mn   [2] TIRHUTA SIGN CANDRABINDU..TIRHUTA SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114BF, End = 0x114C0, Type = Extend });
            // Mn   [2] TIRHUTA SIGN VIRAMA..TIRHUTA SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114C2, End = 0x114C3, Type = Extend });
            // Lo   [2] TIRHUTA SIGN AVAGRAHA..TIRHUTA GVANG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114C4, End = 0x114C5, Type = ALetter });
            // Nd  [10] TIRHUTA DIGIT ZERO..TIRHUTA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x114D0, End = 0x114D9, Type = Numeric });
            // Lo  [47] SIDDHAM LETTER A..SIDDHAM LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11580, End = 0x115AE, Type = ALetter });
            // Mc   [3] SIDDHAM VOWEL SIGN AA..SIDDHAM VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115AF, End = 0x115B1, Type = Extend });
            // Mn   [4] SIDDHAM VOWEL SIGN U..SIDDHAM VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115B2, End = 0x115B5, Type = Extend });
            // Mc   [4] SIDDHAM VOWEL SIGN E..SIDDHAM VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115B8, End = 0x115BB, Type = Extend });
            // Mn   [2] SIDDHAM SIGN CANDRABINDU..SIDDHAM SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115BC, End = 0x115BD, Type = Extend });
            // Mn   [2] SIDDHAM SIGN VIRAMA..SIDDHAM SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115BF, End = 0x115C0, Type = Extend });
            // Lo   [4] SIDDHAM LETTER THREE-CIRCLE ALTERNATE I..SIDDHAM LETTER ALTERNATE U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115D8, End = 0x115DB, Type = ALetter });
            // Mn   [2] SIDDHAM VOWEL SIGN ALTERNATE U..SIDDHAM VOWEL SIGN ALTERNATE UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x115DC, End = 0x115DD, Type = Extend });
            // Lo  [48] MODI LETTER A..MODI LETTER LLA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11600, End = 0x1162F, Type = ALetter });
            // Mc   [3] MODI VOWEL SIGN AA..MODI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11630, End = 0x11632, Type = Extend });
            // Mn   [8] MODI VOWEL SIGN U..MODI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11633, End = 0x1163A, Type = Extend });
            // Mc   [2] MODI VOWEL SIGN O..MODI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1163B, End = 0x1163C, Type = Extend });
            // Mn   [2] MODI SIGN VIRAMA..MODI SIGN ARDHACANDRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1163F, End = 0x11640, Type = Extend });
            // Nd  [10] MODI DIGIT ZERO..MODI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11650, End = 0x11659, Type = Numeric });
            // Lo  [43] TAKRI LETTER A..TAKRI LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11680, End = 0x116AA, Type = ALetter });
            // Mc   [2] TAKRI VOWEL SIGN I..TAKRI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x116AE, End = 0x116AF, Type = Extend });
            // Mn   [6] TAKRI VOWEL SIGN U..TAKRI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x116B0, End = 0x116B5, Type = Extend });
            // Nd  [10] TAKRI DIGIT ZERO..TAKRI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x116C0, End = 0x116C9, Type = Numeric });
            // Mn   [3] AHOM CONSONANT SIGN MEDIAL LA..AHOM CONSONANT SIGN MEDIAL LIGATING RA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1171D, End = 0x1171F, Type = Extend });
            // Mc   [2] AHOM VOWEL SIGN A..AHOM VOWEL SIGN AA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11720, End = 0x11721, Type = Extend });
            // Mn   [4] AHOM VOWEL SIGN I..AHOM VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11722, End = 0x11725, Type = Extend });
            // Mn   [5] AHOM VOWEL SIGN AW..AHOM SIGN KILLER
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11727, End = 0x1172B, Type = Extend });
            // Nd  [10] AHOM DIGIT ZERO..AHOM DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11730, End = 0x11739, Type = Numeric });
            // Lo  [44] DOGRA LETTER A..DOGRA LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11800, End = 0x1182B, Type = ALetter });
            // Mc   [3] DOGRA VOWEL SIGN AA..DOGRA VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1182C, End = 0x1182E, Type = Extend });
            // Mn   [9] DOGRA VOWEL SIGN U..DOGRA SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1182F, End = 0x11837, Type = Extend });
            // Mn   [2] DOGRA SIGN VIRAMA..DOGRA SIGN NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11839, End = 0x1183A, Type = Extend });
            // L&  [64] WARANG CITI CAPITAL LETTER NGAA..WARANG CITI SMALL LETTER VIYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x118A0, End = 0x118DF, Type = ALetter });
            // Nd  [10] WARANG CITI DIGIT ZERO..WARANG CITI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x118E0, End = 0x118E9, Type = Numeric });
            // Lo   [8] WARANG CITI OM..DIVES AKURU LETTER E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x118FF, End = 0x11906, Type = ALetter });
            // Lo   [8] DIVES AKURU LETTER KA..DIVES AKURU LETTER JA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1190C, End = 0x11913, Type = ALetter });
            // Lo   [2] DIVES AKURU LETTER NYA..DIVES AKURU LETTER TTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11915, End = 0x11916, Type = ALetter });
            // Lo  [24] DIVES AKURU LETTER DDA..DIVES AKURU LETTER ZA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11918, End = 0x1192F, Type = ALetter });
            // Mc   [6] DIVES AKURU VOWEL SIGN AA..DIVES AKURU VOWEL SIGN E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11930, End = 0x11935, Type = Extend });
            // Mc   [2] DIVES AKURU VOWEL SIGN AI..DIVES AKURU VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11937, End = 0x11938, Type = Extend });
            // Mn   [2] DIVES AKURU SIGN ANUSVARA..DIVES AKURU SIGN CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1193B, End = 0x1193C, Type = Extend });
            // Nd  [10] DIVES AKURU DIGIT ZERO..DIVES AKURU DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11950, End = 0x11959, Type = Numeric });
            // Lo   [8] NANDINAGARI LETTER A..NANDINAGARI LETTER VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119A0, End = 0x119A7, Type = ALetter });
            // Lo  [39] NANDINAGARI LETTER E..NANDINAGARI LETTER RRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119AA, End = 0x119D0, Type = ALetter });
            // Mc   [3] NANDINAGARI VOWEL SIGN AA..NANDINAGARI VOWEL SIGN II
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119D1, End = 0x119D3, Type = Extend });
            // Mn   [4] NANDINAGARI VOWEL SIGN U..NANDINAGARI VOWEL SIGN VOCALIC RR
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119D4, End = 0x119D7, Type = Extend });
            // Mn   [2] NANDINAGARI VOWEL SIGN E..NANDINAGARI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119DA, End = 0x119DB, Type = Extend });
            // Mc   [4] NANDINAGARI VOWEL SIGN O..NANDINAGARI SIGN VISARGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x119DC, End = 0x119DF, Type = Extend });
            // Mn  [10] ZANABAZAR SQUARE VOWEL SIGN I..ZANABAZAR SQUARE VOWEL LENGTH MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A01, End = 0x11A0A, Type = Extend });
            // Lo  [40] ZANABAZAR SQUARE LETTER KA..ZANABAZAR SQUARE LETTER KSSA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A0B, End = 0x11A32, Type = ALetter });
            // Mn   [6] ZANABAZAR SQUARE FINAL CONSONANT MARK..ZANABAZAR SQUARE SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A33, End = 0x11A38, Type = Extend });
            // Mn   [4] ZANABAZAR SQUARE CLUSTER-FINAL LETTER YA..ZANABAZAR SQUARE CLUSTER-FINAL LETTER VA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A3B, End = 0x11A3E, Type = Extend });
            // Mn   [6] SOYOMBO VOWEL SIGN I..SOYOMBO VOWEL SIGN OE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A51, End = 0x11A56, Type = Extend });
            // Mc   [2] SOYOMBO VOWEL SIGN AI..SOYOMBO VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A57, End = 0x11A58, Type = Extend });
            // Mn   [3] SOYOMBO VOWEL SIGN VOCALIC R..SOYOMBO VOWEL LENGTH MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A59, End = 0x11A5B, Type = Extend });
            // Lo  [46] SOYOMBO LETTER KA..SOYOMBO CLUSTER-INITIAL LETTER SA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A5C, End = 0x11A89, Type = ALetter });
            // Mn  [13] SOYOMBO FINAL CONSONANT SIGN G..SOYOMBO SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A8A, End = 0x11A96, Type = Extend });
            // Mn   [2] SOYOMBO GEMINATION MARK..SOYOMBO SUBJOINER
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11A98, End = 0x11A99, Type = Extend });
            // Lo  [73] CANADIAN SYLLABICS NATTILIK HI..PAU CIN HAU GLOTTAL STOP FINAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11AB0, End = 0x11AF8, Type = ALetter });
            // Lo   [9] BHAIKSUKI LETTER A..BHAIKSUKI LETTER VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C00, End = 0x11C08, Type = ALetter });
            // Lo  [37] BHAIKSUKI LETTER E..BHAIKSUKI LETTER HA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C0A, End = 0x11C2E, Type = ALetter });
            // Mn   [7] BHAIKSUKI VOWEL SIGN I..BHAIKSUKI VOWEL SIGN VOCALIC L
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C30, End = 0x11C36, Type = Extend });
            // Mn   [6] BHAIKSUKI VOWEL SIGN E..BHAIKSUKI SIGN ANUSVARA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C38, End = 0x11C3D, Type = Extend });
            // Nd  [10] BHAIKSUKI DIGIT ZERO..BHAIKSUKI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C50, End = 0x11C59, Type = Numeric });
            // Lo  [30] MARCHEN LETTER KA..MARCHEN LETTER A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C72, End = 0x11C8F, Type = ALetter });
            // Mn  [22] MARCHEN SUBJOINED LETTER KA..MARCHEN SUBJOINED LETTER ZA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11C92, End = 0x11CA7, Type = Extend });
            // Mn   [7] MARCHEN SUBJOINED LETTER RA..MARCHEN VOWEL SIGN AA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11CAA, End = 0x11CB0, Type = Extend });
            // Mn   [2] MARCHEN VOWEL SIGN U..MARCHEN VOWEL SIGN E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11CB2, End = 0x11CB3, Type = Extend });
            // Mn   [2] MARCHEN SIGN ANUSVARA..MARCHEN SIGN CANDRABINDU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11CB5, End = 0x11CB6, Type = Extend });
            // Lo   [7] MASARAM GONDI LETTER A..MASARAM GONDI LETTER E
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D00, End = 0x11D06, Type = ALetter });
            // Lo   [2] MASARAM GONDI LETTER AI..MASARAM GONDI LETTER O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D08, End = 0x11D09, Type = ALetter });
            // Lo  [38] MASARAM GONDI LETTER AU..MASARAM GONDI LETTER TRA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D0B, End = 0x11D30, Type = ALetter });
            // Mn   [6] MASARAM GONDI VOWEL SIGN AA..MASARAM GONDI VOWEL SIGN VOCALIC R
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D31, End = 0x11D36, Type = Extend });
            // Mn   [2] MASARAM GONDI VOWEL SIGN AI..MASARAM GONDI VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D3C, End = 0x11D3D, Type = Extend });
            // Mn   [7] MASARAM GONDI VOWEL SIGN AU..MASARAM GONDI VIRAMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D3F, End = 0x11D45, Type = Extend });
            // Nd  [10] MASARAM GONDI DIGIT ZERO..MASARAM GONDI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D50, End = 0x11D59, Type = Numeric });
            // Lo   [6] GUNJALA GONDI LETTER A..GUNJALA GONDI LETTER UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D60, End = 0x11D65, Type = ALetter });
            // Lo   [2] GUNJALA GONDI LETTER EE..GUNJALA GONDI LETTER AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D67, End = 0x11D68, Type = ALetter });
            // Lo  [32] GUNJALA GONDI LETTER OO..GUNJALA GONDI LETTER SA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D6A, End = 0x11D89, Type = ALetter });
            // Mc   [5] GUNJALA GONDI VOWEL SIGN AA..GUNJALA GONDI VOWEL SIGN UU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D8A, End = 0x11D8E, Type = Extend });
            // Mn   [2] GUNJALA GONDI VOWEL SIGN EE..GUNJALA GONDI VOWEL SIGN AI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D90, End = 0x11D91, Type = Extend });
            // Mc   [2] GUNJALA GONDI VOWEL SIGN OO..GUNJALA GONDI VOWEL SIGN AU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11D93, End = 0x11D94, Type = Extend });
            // Nd  [10] GUNJALA GONDI DIGIT ZERO..GUNJALA GONDI DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11DA0, End = 0x11DA9, Type = Numeric });
            // Lo  [19] MAKASAR LETTER KA..MAKASAR ANGKA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11EE0, End = 0x11EF2, Type = ALetter });
            // Mn   [2] MAKASAR VOWEL SIGN I..MAKASAR VOWEL SIGN U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11EF3, End = 0x11EF4, Type = Extend });
            // Mc   [2] MAKASAR VOWEL SIGN E..MAKASAR VOWEL SIGN O
            m_lst_code_range.Add(new RangeInfo() { Start = 0x11EF5, End = 0x11EF6, Type = Extend });
            // Lo [922] CUNEIFORM SIGN A..CUNEIFORM SIGN U U
            m_lst_code_range.Add(new RangeInfo() { Start = 0x12000, End = 0x12399, Type = ALetter });
            // Nl [111] CUNEIFORM NUMERIC SIGN TWO ASH..CUNEIFORM NUMERIC SIGN NINE U VARIANT FORM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x12400, End = 0x1246E, Type = ALetter });
            // Lo [196] CUNEIFORM SIGN AB TIMES NUN TENU..CUNEIFORM SIGN ZU5 TIMES THREE DISH TENU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x12480, End = 0x12543, Type = ALetter });
            // Lo  [97] CYPRO-MINOAN SIGN CM001..CYPRO-MINOAN SIGN CM114
            m_lst_code_range.Add(new RangeInfo() { Start = 0x12F90, End = 0x12FF0, Type = ALetter });
            // Lo [1071] EGYPTIAN HIEROGLYPH A001..EGYPTIAN HIEROGLYPH AA032
            m_lst_code_range.Add(new RangeInfo() { Start = 0x13000, End = 0x1342E, Type = ALetter });
            // Cf   [9] EGYPTIAN HIEROGLYPH VERTICAL JOINER..EGYPTIAN HIEROGLYPH END SEGMENT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x13430, End = 0x13438, Type = Format });
            // Lo [583] ANATOLIAN HIEROGLYPH A001..ANATOLIAN HIEROGLYPH A530
            m_lst_code_range.Add(new RangeInfo() { Start = 0x14400, End = 0x14646, Type = ALetter });
            // Lo [569] BAMUM LETTER PHASE-A NGKUE MFON..BAMUM LETTER PHASE-F VUEQ
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16800, End = 0x16A38, Type = ALetter });
            // Lo  [31] MRO LETTER TA..MRO LETTER TEK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16A40, End = 0x16A5E, Type = ALetter });
            // Nd  [10] MRO DIGIT ZERO..MRO DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16A60, End = 0x16A69, Type = Numeric });
            // Lo  [79] TANGSA LETTER OZ..TANGSA LETTER ZA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16A70, End = 0x16ABE, Type = ALetter });
            // Nd  [10] TANGSA DIGIT ZERO..TANGSA DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16AC0, End = 0x16AC9, Type = Numeric });
            // Lo  [30] BASSA VAH LETTER ENNI..BASSA VAH LETTER I
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16AD0, End = 0x16AED, Type = ALetter });
            // Mn   [5] BASSA VAH COMBINING HIGH TONE..BASSA VAH COMBINING HIGH-LOW TONE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16AF0, End = 0x16AF4, Type = Extend });
            // Lo  [48] PAHAWH HMONG VOWEL KEEB..PAHAWH HMONG CONSONANT CAU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B00, End = 0x16B2F, Type = ALetter });
            // Mn   [7] PAHAWH HMONG MARK CIM TUB..PAHAWH HMONG MARK CIM TAUM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B30, End = 0x16B36, Type = Extend });
            // Lm   [4] PAHAWH HMONG SIGN VOS SEEV..PAHAWH HMONG SIGN IB YAM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B40, End = 0x16B43, Type = ALetter });
            // Nd  [10] PAHAWH HMONG DIGIT ZERO..PAHAWH HMONG DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B50, End = 0x16B59, Type = Numeric });
            // Lo  [21] PAHAWH HMONG SIGN VOS LUB..PAHAWH HMONG SIGN CIM NRES TOS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B63, End = 0x16B77, Type = ALetter });
            // Lo  [19] PAHAWH HMONG CLAN SIGN TSHEEJ..PAHAWH HMONG CLAN SIGN VWJ
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16B7D, End = 0x16B8F, Type = ALetter });
            // L&  [64] MEDEFAIDRIN CAPITAL LETTER M..MEDEFAIDRIN SMALL LETTER Y
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16E40, End = 0x16E7F, Type = ALetter });
            // Lo  [75] MIAO LETTER PA..MIAO LETTER RTE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16F00, End = 0x16F4A, Type = ALetter });
            // Mc  [55] MIAO SIGN ASPIRATION..MIAO VOWEL SIGN UI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16F51, End = 0x16F87, Type = Extend });
            // Mn   [4] MIAO TONE RIGHT..MIAO TONE BELOW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16F8F, End = 0x16F92, Type = Extend });
            // Lm  [13] MIAO LETTER TONE-2..MIAO LETTER REFORMED TONE-8
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16F93, End = 0x16F9F, Type = ALetter });
            // Lm   [2] TANGUT ITERATION MARK..NUSHU ITERATION MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16FE0, End = 0x16FE1, Type = ALetter });
            // Mc   [2] VIETNAMESE ALTERNATE READING MARK CA..VIETNAMESE ALTERNATE READING MARK NHAY
            m_lst_code_range.Add(new RangeInfo() { Start = 0x16FF0, End = 0x16FF1, Type = Extend });
            // Lm   [4] KATAKANA LETTER MINNAN TONE-2..KATAKANA LETTER MINNAN TONE-5
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1AFF0, End = 0x1AFF3, Type = Katakana });
            // Lm   [7] KATAKANA LETTER MINNAN TONE-7..KATAKANA LETTER MINNAN NASALIZED TONE-5
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1AFF5, End = 0x1AFFB, Type = Katakana });
            // Lm   [2] KATAKANA LETTER MINNAN NASALIZED TONE-7..KATAKANA LETTER MINNAN NASALIZED TONE-8
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1AFFD, End = 0x1AFFE, Type = Katakana });
            // Lo   [3] KATAKANA LETTER ARCHAIC YI..KATAKANA LETTER ARCHAIC WU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1B120, End = 0x1B122, Type = Katakana });
            // Lo   [4] KATAKANA LETTER SMALL WI..KATAKANA LETTER SMALL N
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1B164, End = 0x1B167, Type = Katakana });
            // Lo [107] DUPLOYAN LETTER H..DUPLOYAN LETTER VOCALIC M
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BC00, End = 0x1BC6A, Type = ALetter });
            // Lo  [13] DUPLOYAN AFFIX LEFT HORIZONTAL SECANT..DUPLOYAN AFFIX ATTACHED TANGENT HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BC70, End = 0x1BC7C, Type = ALetter });
            // Lo   [9] DUPLOYAN AFFIX HIGH ACUTE..DUPLOYAN AFFIX HIGH VERTICAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BC80, End = 0x1BC88, Type = ALetter });
            // Lo  [10] DUPLOYAN AFFIX LOW ACUTE..DUPLOYAN AFFIX LOW ARROW
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BC90, End = 0x1BC99, Type = ALetter });
            // Mn   [2] DUPLOYAN THICK LETTER SELECTOR..DUPLOYAN DOUBLE MARK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BC9D, End = 0x1BC9E, Type = Extend });
            // Cf   [4] SHORTHAND FORMAT LETTER OVERLAP..SHORTHAND FORMAT UP STEP
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1BCA0, End = 0x1BCA3, Type = Format });
            // Mn  [46] ZNAMENNY COMBINING MARK GORAZDO NIZKO S KRYZHEM ON LEFT..ZNAMENNY COMBINING MARK KRYZH ON LEFT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1CF00, End = 0x1CF2D, Type = Extend });
            // Mn  [23] ZNAMENNY COMBINING TONAL RANGE MARK MRACHNO..ZNAMENNY PRIZNAK MODIFIER ROG
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1CF30, End = 0x1CF46, Type = Extend });
            // Mc   [2] MUSICAL SYMBOL COMBINING STEM..MUSICAL SYMBOL COMBINING SPRECHGESANG STEM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D165, End = 0x1D166, Type = Extend });
            // Mn   [3] MUSICAL SYMBOL COMBINING TREMOLO-1..MUSICAL SYMBOL COMBINING TREMOLO-3
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D167, End = 0x1D169, Type = Extend });
            // Mc   [6] MUSICAL SYMBOL COMBINING AUGMENTATION DOT..MUSICAL SYMBOL COMBINING FLAG-5
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D16D, End = 0x1D172, Type = Extend });
            // Cf   [8] MUSICAL SYMBOL BEGIN BEAM..MUSICAL SYMBOL END PHRASE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D173, End = 0x1D17A, Type = Format });
            // Mn   [8] MUSICAL SYMBOL COMBINING ACCENT..MUSICAL SYMBOL COMBINING LOURE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D17B, End = 0x1D182, Type = Extend });
            // Mn   [7] MUSICAL SYMBOL COMBINING DOIT..MUSICAL SYMBOL COMBINING TRIPLE TONGUE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D185, End = 0x1D18B, Type = Extend });
            // Mn   [4] MUSICAL SYMBOL COMBINING DOWN BOW..MUSICAL SYMBOL COMBINING SNAP PIZZICATO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D1AA, End = 0x1D1AD, Type = Extend });
            // Mn   [3] COMBINING GREEK MUSICAL TRISEME..COMBINING GREEK MUSICAL PENTASEME
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D242, End = 0x1D244, Type = Extend });
            // L&  [85] MATHEMATICAL BOLD CAPITAL A..MATHEMATICAL ITALIC SMALL G
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D400, End = 0x1D454, Type = ALetter });
            // L&  [71] MATHEMATICAL ITALIC SMALL I..MATHEMATICAL SCRIPT CAPITAL A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D456, End = 0x1D49C, Type = ALetter });
            // L&   [2] MATHEMATICAL SCRIPT CAPITAL C..MATHEMATICAL SCRIPT CAPITAL D
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D49E, End = 0x1D49F, Type = ALetter });
            // L&   [2] MATHEMATICAL SCRIPT CAPITAL J..MATHEMATICAL SCRIPT CAPITAL K
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D4A5, End = 0x1D4A6, Type = ALetter });
            // L&   [4] MATHEMATICAL SCRIPT CAPITAL N..MATHEMATICAL SCRIPT CAPITAL Q
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D4A9, End = 0x1D4AC, Type = ALetter });
            // L&  [12] MATHEMATICAL SCRIPT CAPITAL S..MATHEMATICAL SCRIPT SMALL D
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D4AE, End = 0x1D4B9, Type = ALetter });
            // L&   [7] MATHEMATICAL SCRIPT SMALL H..MATHEMATICAL SCRIPT SMALL N
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D4BD, End = 0x1D4C3, Type = ALetter });
            // L&  [65] MATHEMATICAL SCRIPT SMALL P..MATHEMATICAL FRAKTUR CAPITAL B
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D4C5, End = 0x1D505, Type = ALetter });
            // L&   [4] MATHEMATICAL FRAKTUR CAPITAL D..MATHEMATICAL FRAKTUR CAPITAL G
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D507, End = 0x1D50A, Type = ALetter });
            // L&   [8] MATHEMATICAL FRAKTUR CAPITAL J..MATHEMATICAL FRAKTUR CAPITAL Q
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D50D, End = 0x1D514, Type = ALetter });
            // L&   [7] MATHEMATICAL FRAKTUR CAPITAL S..MATHEMATICAL FRAKTUR CAPITAL Y
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D516, End = 0x1D51C, Type = ALetter });
            // L&  [28] MATHEMATICAL FRAKTUR SMALL A..MATHEMATICAL DOUBLE-STRUCK CAPITAL B
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D51E, End = 0x1D539, Type = ALetter });
            // L&   [4] MATHEMATICAL DOUBLE-STRUCK CAPITAL D..MATHEMATICAL DOUBLE-STRUCK CAPITAL G
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D53B, End = 0x1D53E, Type = ALetter });
            // L&   [5] MATHEMATICAL DOUBLE-STRUCK CAPITAL I..MATHEMATICAL DOUBLE-STRUCK CAPITAL M
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D540, End = 0x1D544, Type = ALetter });
            // L&   [7] MATHEMATICAL DOUBLE-STRUCK CAPITAL S..MATHEMATICAL DOUBLE-STRUCK CAPITAL Y
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D54A, End = 0x1D550, Type = ALetter });
            // L& [340] MATHEMATICAL DOUBLE-STRUCK SMALL A..MATHEMATICAL ITALIC SMALL DOTLESS J
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D552, End = 0x1D6A5, Type = ALetter });
            // L&  [25] MATHEMATICAL BOLD CAPITAL ALPHA..MATHEMATICAL BOLD CAPITAL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D6A8, End = 0x1D6C0, Type = ALetter });
            // L&  [25] MATHEMATICAL BOLD SMALL ALPHA..MATHEMATICAL BOLD SMALL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D6C2, End = 0x1D6DA, Type = ALetter });
            // L&  [31] MATHEMATICAL BOLD EPSILON SYMBOL..MATHEMATICAL ITALIC CAPITAL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D6DC, End = 0x1D6FA, Type = ALetter });
            // L&  [25] MATHEMATICAL ITALIC SMALL ALPHA..MATHEMATICAL ITALIC SMALL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D6FC, End = 0x1D714, Type = ALetter });
            // L&  [31] MATHEMATICAL ITALIC EPSILON SYMBOL..MATHEMATICAL BOLD ITALIC CAPITAL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D716, End = 0x1D734, Type = ALetter });
            // L&  [25] MATHEMATICAL BOLD ITALIC SMALL ALPHA..MATHEMATICAL BOLD ITALIC SMALL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D736, End = 0x1D74E, Type = ALetter });
            // L&  [31] MATHEMATICAL BOLD ITALIC EPSILON SYMBOL..MATHEMATICAL SANS-SERIF BOLD CAPITAL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D750, End = 0x1D76E, Type = ALetter });
            // L&  [25] MATHEMATICAL SANS-SERIF BOLD SMALL ALPHA..MATHEMATICAL SANS-SERIF BOLD SMALL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D770, End = 0x1D788, Type = ALetter });
            // L&  [31] MATHEMATICAL SANS-SERIF BOLD EPSILON SYMBOL..MATHEMATICAL SANS-SERIF BOLD ITALIC CAPITAL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D78A, End = 0x1D7A8, Type = ALetter });
            // L&  [25] MATHEMATICAL SANS-SERIF BOLD ITALIC SMALL ALPHA..MATHEMATICAL SANS-SERIF BOLD ITALIC SMALL OMEGA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D7AA, End = 0x1D7C2, Type = ALetter });
            // L&   [8] MATHEMATICAL SANS-SERIF BOLD ITALIC EPSILON SYMBOL..MATHEMATICAL BOLD SMALL DIGAMMA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D7C4, End = 0x1D7CB, Type = ALetter });
            // Nd  [50] MATHEMATICAL BOLD DIGIT ZERO..MATHEMATICAL MONOSPACE DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1D7CE, End = 0x1D7FF, Type = Numeric });
            // Mn  [55] SIGNWRITING HEAD RIM..SIGNWRITING AIR SUCKING IN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DA00, End = 0x1DA36, Type = Extend });
            // Mn  [50] SIGNWRITING MOUTH CLOSED NEUTRAL..SIGNWRITING EXCITEMENT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DA3B, End = 0x1DA6C, Type = Extend });
            // Mn   [5] SIGNWRITING FILL MODIFIER-2..SIGNWRITING FILL MODIFIER-6
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DA9B, End = 0x1DA9F, Type = Extend });
            // Mn  [15] SIGNWRITING ROTATION MODIFIER-2..SIGNWRITING ROTATION MODIFIER-16
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DAA1, End = 0x1DAAF, Type = Extend });
            // L&  [10] LATIN SMALL LETTER FENG DIGRAPH WITH TRILL..LATIN SMALL LETTER T WITH HOOK AND RETROFLEX HOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DF00, End = 0x1DF09, Type = ALetter });
            // L&  [20] LATIN SMALL LETTER ESH WITH DOUBLE BAR..LATIN SMALL LETTER S WITH CURL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1DF0B, End = 0x1DF1E, Type = ALetter });
            // Mn   [7] COMBINING GLAGOLITIC LETTER AZU..COMBINING GLAGOLITIC LETTER ZHIVETE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E000, End = 0x1E006, Type = Extend });
            // Mn  [17] COMBINING GLAGOLITIC LETTER ZEMLJA..COMBINING GLAGOLITIC LETTER HERU
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E008, End = 0x1E018, Type = Extend });
            // Mn   [7] COMBINING GLAGOLITIC LETTER SHTA..COMBINING GLAGOLITIC LETTER YATI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E01B, End = 0x1E021, Type = Extend });
            // Mn   [2] COMBINING GLAGOLITIC LETTER YU..COMBINING GLAGOLITIC LETTER SMALL YUS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E023, End = 0x1E024, Type = Extend });
            // Mn   [5] COMBINING GLAGOLITIC LETTER YO..COMBINING GLAGOLITIC LETTER FITA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E026, End = 0x1E02A, Type = Extend });
            // Lo  [45] NYIAKENG PUACHUE HMONG LETTER MA..NYIAKENG PUACHUE HMONG LETTER W
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E100, End = 0x1E12C, Type = ALetter });
            // Mn   [7] NYIAKENG PUACHUE HMONG TONE-B..NYIAKENG PUACHUE HMONG TONE-D
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E130, End = 0x1E136, Type = Extend });
            // Lm   [7] NYIAKENG PUACHUE HMONG SIGN FOR PERSON..NYIAKENG PUACHUE HMONG SYLLABLE LENGTHENER
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E137, End = 0x1E13D, Type = ALetter });
            // Nd  [10] NYIAKENG PUACHUE HMONG DIGIT ZERO..NYIAKENG PUACHUE HMONG DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E140, End = 0x1E149, Type = Numeric });
            // Lo  [30] TOTO LETTER PA..TOTO LETTER A
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E290, End = 0x1E2AD, Type = ALetter });
            // Lo  [44] WANCHO LETTER AA..WANCHO LETTER YIH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E2C0, End = 0x1E2EB, Type = ALetter });
            // Mn   [4] WANCHO TONE TUP..WANCHO TONE KOINI
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E2EC, End = 0x1E2EF, Type = Extend });
            // Nd  [10] WANCHO DIGIT ZERO..WANCHO DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E2F0, End = 0x1E2F9, Type = Numeric });
            // Lo   [7] ETHIOPIC SYLLABLE HHYA..ETHIOPIC SYLLABLE HHYO
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E7E0, End = 0x1E7E6, Type = ALetter });
            // Lo   [4] ETHIOPIC SYLLABLE GURAGE HHWA..ETHIOPIC SYLLABLE HHWE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E7E8, End = 0x1E7EB, Type = ALetter });
            // Lo   [2] ETHIOPIC SYLLABLE GURAGE MWI..ETHIOPIC SYLLABLE GURAGE MWEE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E7ED, End = 0x1E7EE, Type = ALetter });
            // Lo  [15] ETHIOPIC SYLLABLE GURAGE QWI..ETHIOPIC SYLLABLE GURAGE PWEE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E7F0, End = 0x1E7FE, Type = ALetter });
            // Lo [197] MENDE KIKAKUI SYLLABLE M001 KI..MENDE KIKAKUI SYLLABLE M060 NYON
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E800, End = 0x1E8C4, Type = ALetter });
            // Mn   [7] MENDE KIKAKUI COMBINING NUMBER TEENS..MENDE KIKAKUI COMBINING NUMBER MILLIONS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E8D0, End = 0x1E8D6, Type = Extend });
            // L&  [68] ADLAM CAPITAL LETTER ALIF..ADLAM SMALL LETTER SHA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E900, End = 0x1E943, Type = ALetter });
            // Mn   [7] ADLAM ALIF LENGTHENER..ADLAM NUKTA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E944, End = 0x1E94A, Type = Extend });
            // Nd  [10] ADLAM DIGIT ZERO..ADLAM DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1E950, End = 0x1E959, Type = Numeric });
            // Lo   [4] ARABIC MATHEMATICAL ALEF..ARABIC MATHEMATICAL DAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE00, End = 0x1EE03, Type = ALetter });
            // Lo  [27] ARABIC MATHEMATICAL WAW..ARABIC MATHEMATICAL DOTLESS QAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE05, End = 0x1EE1F, Type = ALetter });
            // Lo   [2] ARABIC MATHEMATICAL INITIAL BEH..ARABIC MATHEMATICAL INITIAL JEEM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE21, End = 0x1EE22, Type = ALetter });
            // Lo  [10] ARABIC MATHEMATICAL INITIAL YEH..ARABIC MATHEMATICAL INITIAL QAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE29, End = 0x1EE32, Type = ALetter });
            // Lo   [4] ARABIC MATHEMATICAL INITIAL SHEEN..ARABIC MATHEMATICAL INITIAL KHAH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE34, End = 0x1EE37, Type = ALetter });
            // Lo   [3] ARABIC MATHEMATICAL TAILED NOON..ARABIC MATHEMATICAL TAILED AIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE4D, End = 0x1EE4F, Type = ALetter });
            // Lo   [2] ARABIC MATHEMATICAL TAILED SAD..ARABIC MATHEMATICAL TAILED QAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE51, End = 0x1EE52, Type = ALetter });
            // Lo   [2] ARABIC MATHEMATICAL STRETCHED BEH..ARABIC MATHEMATICAL STRETCHED JEEM
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE61, End = 0x1EE62, Type = ALetter });
            // Lo   [4] ARABIC MATHEMATICAL STRETCHED HAH..ARABIC MATHEMATICAL STRETCHED KAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE67, End = 0x1EE6A, Type = ALetter });
            // Lo   [7] ARABIC MATHEMATICAL STRETCHED MEEM..ARABIC MATHEMATICAL STRETCHED QAF
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE6C, End = 0x1EE72, Type = ALetter });
            // Lo   [4] ARABIC MATHEMATICAL STRETCHED SHEEN..ARABIC MATHEMATICAL STRETCHED KHAH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE74, End = 0x1EE77, Type = ALetter });
            // Lo   [4] ARABIC MATHEMATICAL STRETCHED DAD..ARABIC MATHEMATICAL STRETCHED DOTLESS BEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE79, End = 0x1EE7C, Type = ALetter });
            // Lo  [10] ARABIC MATHEMATICAL LOOPED ALEF..ARABIC MATHEMATICAL LOOPED YEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE80, End = 0x1EE89, Type = ALetter });
            // Lo  [17] ARABIC MATHEMATICAL LOOPED LAM..ARABIC MATHEMATICAL LOOPED GHAIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EE8B, End = 0x1EE9B, Type = ALetter });
            // Lo   [3] ARABIC MATHEMATICAL DOUBLE-STRUCK BEH..ARABIC MATHEMATICAL DOUBLE-STRUCK DAL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EEA1, End = 0x1EEA3, Type = ALetter });
            // Lo   [5] ARABIC MATHEMATICAL DOUBLE-STRUCK WAW..ARABIC MATHEMATICAL DOUBLE-STRUCK YEH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EEA5, End = 0x1EEA9, Type = ALetter });
            // Lo  [17] ARABIC MATHEMATICAL DOUBLE-STRUCK LAM..ARABIC MATHEMATICAL DOUBLE-STRUCK GHAIN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1EEAB, End = 0x1EEBB, Type = ALetter });
            // E0.0   [4] (🀀..🀃)    MAHJONG TILE EAST WIND..MAHJONG TILE NORTH WIND
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F000, End = 0x1F003, Type = Extended_Pictographic });
            // E0.0 [202] (🀅..🃎)    MAHJONG TILE GREEN DRAGON..PLAYING CARD KING OF DIAMONDS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F005, End = 0x1F0CE, Type = Extended_Pictographic });
            // E0.0  [48] (🃐..🃿)    <reserved-1F0D0>..<reserved-1F0FF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F0D0, End = 0x1F0FF, Type = Extended_Pictographic });
            // E0.0   [3] (🄍..🄏)    CIRCLED ZERO WITH SLASH..CIRCLED DOLLAR SIGN WITH OVERLAID BACKSLASH
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F10D, End = 0x1F10F, Type = Extended_Pictographic });
            // So  [26] SQUARED LATIN CAPITAL LETTER A..SQUARED LATIN CAPITAL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F130, End = 0x1F149, Type = ALetter });
            // So  [26] NEGATIVE CIRCLED LATIN CAPITAL LETTER A..NEGATIVE CIRCLED LATIN CAPITAL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F150, End = 0x1F169, Type = ALetter });
            // E0.0   [4] (🅬..🅯)    RAISED MR SIGN..CIRCLED HUMAN FIGURE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F16C, End = 0x1F16F, Type = Extended_Pictographic });
            // So  [26] NEGATIVE SQUARED LATIN CAPITAL LETTER A..NEGATIVE SQUARED LATIN CAPITAL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F170, End = 0x1F189, Type = ALetter });
            // E0.6   [2] (🅰️..🅱️)    A button (blood type)..B button (blood type)
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F170, End = 0x1F171, Type = Extended_Pictographic });
            // E0.6   [2] (🅾️..🅿️)    O button (blood type)..P button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F17E, End = 0x1F17F, Type = Extended_Pictographic });
            // E0.6  [10] (🆑..🆚)    CL button..VS button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F191, End = 0x1F19A, Type = Extended_Pictographic });
            // E0.0  [57] (🆭..🇥)    MASK WORK SYMBOL..<reserved-1F1E5>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F1AD, End = 0x1F1E5, Type = Extended_Pictographic });
            // So  [26] REGIONAL INDICATOR SYMBOL LETTER A..REGIONAL INDICATOR SYMBOL LETTER Z
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F1E6, End = 0x1F1FF, Type = Regional_Indicator });
            // E0.6   [2] (🈁..🈂️)    Japanese “here” button..Japanese “service charge” button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F201, End = 0x1F202, Type = Extended_Pictographic });
            // E0.0  [13] (🈃..🈏)    <reserved-1F203>..<reserved-1F20F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F203, End = 0x1F20F, Type = Extended_Pictographic });
            // E0.6   [9] (🈲..🈺)    Japanese “prohibited” button..Japanese “open for business” button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F232, End = 0x1F23A, Type = Extended_Pictographic });
            // E0.0   [4] (🈼..🈿)    <reserved-1F23C>..<reserved-1F23F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F23C, End = 0x1F23F, Type = Extended_Pictographic });
            // E0.0   [7] (🉉..🉏)    <reserved-1F249>..<reserved-1F24F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F249, End = 0x1F24F, Type = Extended_Pictographic });
            // E0.6   [2] (🉐..🉑)    Japanese “bargain” button..Japanese “acceptable” button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F250, End = 0x1F251, Type = Extended_Pictographic });
            // E0.0 [174] (🉒..🋿)    <reserved-1F252>..<reserved-1F2FF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F252, End = 0x1F2FF, Type = Extended_Pictographic });
            // E0.6  [13] (🌀..🌌)    cyclone..milky way
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F300, End = 0x1F30C, Type = Extended_Pictographic });
            // E0.7   [2] (🌍..🌎)    globe showing Europe-Africa..globe showing Americas
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F30D, End = 0x1F30E, Type = Extended_Pictographic });
            // E0.6   [3] (🌓..🌕)    first quarter moon..full moon
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F313, End = 0x1F315, Type = Extended_Pictographic });
            // E1.0   [3] (🌖..🌘)    waning gibbous moon..waning crescent moon
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F316, End = 0x1F318, Type = Extended_Pictographic });
            // E1.0   [2] (🌝..🌞)    full moon face..sun with face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F31D, End = 0x1F31E, Type = Extended_Pictographic });
            // E0.6   [2] (🌟..🌠)    glowing star..shooting star
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F31F, End = 0x1F320, Type = Extended_Pictographic });
            // E0.0   [2] (🌢..🌣)    BLACK DROPLET..WHITE SUN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F322, End = 0x1F323, Type = Extended_Pictographic });
            // E0.7   [9] (🌤️..🌬️)    sun behind small cloud..wind face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F324, End = 0x1F32C, Type = Extended_Pictographic });
            // E1.0   [3] (🌭..🌯)    hot dog..burrito
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F32D, End = 0x1F32F, Type = Extended_Pictographic });
            // E0.6   [2] (🌰..🌱)    chestnut..seedling
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F330, End = 0x1F331, Type = Extended_Pictographic });
            // E1.0   [2] (🌲..🌳)    evergreen tree..deciduous tree
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F332, End = 0x1F333, Type = Extended_Pictographic });
            // E0.6   [2] (🌴..🌵)    palm tree..cactus
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F334, End = 0x1F335, Type = Extended_Pictographic });
            // E0.6  [20] (🌷..🍊)    tulip..tangerine
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F337, End = 0x1F34A, Type = Extended_Pictographic });
            // E0.6   [4] (🍌..🍏)    banana..green apple
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F34C, End = 0x1F34F, Type = Extended_Pictographic });
            // E0.6  [43] (🍑..🍻)    peach..clinking beer mugs
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F351, End = 0x1F37B, Type = Extended_Pictographic });
            // E1.0   [2] (🍾..🍿)    bottle with popping cork..popcorn
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F37E, End = 0x1F37F, Type = Extended_Pictographic });
            // E0.6  [20] (🎀..🎓)    ribbon..graduation cap
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F380, End = 0x1F393, Type = Extended_Pictographic });
            // E0.0   [2] (🎔..🎕)    HEART WITH TIP ON THE LEFT..BOUQUET OF FLOWERS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F394, End = 0x1F395, Type = Extended_Pictographic });
            // E0.7   [2] (🎖️..🎗️)    military medal..reminder ribbon
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F396, End = 0x1F397, Type = Extended_Pictographic });
            // E0.7   [3] (🎙️..🎛️)    studio microphone..control knobs
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F399, End = 0x1F39B, Type = Extended_Pictographic });
            // E0.0   [2] (🎜..🎝)    BEAMED ASCENDING MUSICAL NOTES..BEAMED DESCENDING MUSICAL NOTES
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F39C, End = 0x1F39D, Type = Extended_Pictographic });
            // E0.7   [2] (🎞️..🎟️)    film frames..admission tickets
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F39E, End = 0x1F39F, Type = Extended_Pictographic });
            // E0.6  [37] (🎠..🏄)    carousel horse..person surfing
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3A0, End = 0x1F3C4, Type = Extended_Pictographic });
            // E0.7   [4] (🏋️..🏎️)    person lifting weights..racing car
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3CB, End = 0x1F3CE, Type = Extended_Pictographic });
            // E1.0   [5] (🏏..🏓)    cricket game..ping pong
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3CF, End = 0x1F3D3, Type = Extended_Pictographic });
            // E0.7  [12] (🏔️..🏟️)    snow-capped mountain..stadium
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3D4, End = 0x1F3DF, Type = Extended_Pictographic });
            // E0.6   [4] (🏠..🏣)    house..Japanese post office
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3E0, End = 0x1F3E3, Type = Extended_Pictographic });
            // E0.6  [12] (🏥..🏰)    hospital..castle
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3E5, End = 0x1F3F0, Type = Extended_Pictographic });
            // E0.0   [2] (🏱..🏲)    WHITE PENNANT..BLACK PENNANT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3F1, End = 0x1F3F2, Type = Extended_Pictographic });
            // E1.0   [3] (🏸..🏺)    badminton..amphora
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3F8, End = 0x1F3FA, Type = Extended_Pictographic });
            // Sk   [5] EMOJI MODIFIER FITZPATRICK TYPE-1-2..EMOJI MODIFIER FITZPATRICK TYPE-6
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F3FB, End = 0x1F3FF, Type = Extend });
            // E1.0   [8] (🐀..🐇)    rat..rabbit
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F400, End = 0x1F407, Type = Extended_Pictographic });
            // E1.0   [3] (🐉..🐋)    dragon..whale
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F409, End = 0x1F40B, Type = Extended_Pictographic });
            // E0.6   [3] (🐌..🐎)    snail..horse
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F40C, End = 0x1F40E, Type = Extended_Pictographic });
            // E1.0   [2] (🐏..🐐)    ram..goat
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F40F, End = 0x1F410, Type = Extended_Pictographic });
            // E0.6   [2] (🐑..🐒)    ewe..monkey
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F411, End = 0x1F412, Type = Extended_Pictographic });
            // E0.6  [19] (🐗..🐩)    boar..poodle
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F417, End = 0x1F429, Type = Extended_Pictographic });
            // E0.6  [20] (🐫..🐾)    two-hump camel..paw prints
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F42B, End = 0x1F43E, Type = Extended_Pictographic });
            // E0.6  [35] (👂..👤)    ear..bust in silhouette
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F442, End = 0x1F464, Type = Extended_Pictographic });
            // E0.6   [6] (👦..👫)    boy..woman and man holding hands
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F466, End = 0x1F46B, Type = Extended_Pictographic });
            // E1.0   [2] (👬..👭)    men holding hands..women holding hands
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F46C, End = 0x1F46D, Type = Extended_Pictographic });
            // E0.6  [63] (👮..💬)    police officer..speech balloon
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F46E, End = 0x1F4AC, Type = Extended_Pictographic });
            // E0.6   [8] (💮..💵)    white flower..dollar banknote
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4AE, End = 0x1F4B5, Type = Extended_Pictographic });
            // E1.0   [2] (💶..💷)    euro banknote..pound banknote
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4B6, End = 0x1F4B7, Type = Extended_Pictographic });
            // E0.6  [52] (💸..📫)    money with wings..closed mailbox with raised flag
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4B8, End = 0x1F4EB, Type = Extended_Pictographic });
            // E0.7   [2] (📬..📭)    open mailbox with raised flag..open mailbox with lowered flag
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4EC, End = 0x1F4ED, Type = Extended_Pictographic });
            // E0.6   [5] (📰..📴)    newspaper..mobile phone off
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4F0, End = 0x1F4F4, Type = Extended_Pictographic });
            // E0.6   [2] (📶..📷)    antenna bars..camera
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4F6, End = 0x1F4F7, Type = Extended_Pictographic });
            // E0.6   [4] (📹..📼)    video camera..videocassette
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4F9, End = 0x1F4FC, Type = Extended_Pictographic });
            // E1.0   [4] (📿..🔂)    prayer beads..repeat single button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F4FF, End = 0x1F502, Type = Extended_Pictographic });
            // E1.0   [4] (🔄..🔇)    counterclockwise arrows button..muted speaker
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F504, End = 0x1F507, Type = Extended_Pictographic });
            // E0.6  [11] (🔊..🔔)    speaker high volume..bell
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F50A, End = 0x1F514, Type = Extended_Pictographic });
            // E0.6  [22] (🔖..🔫)    bookmark..water pistol
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F516, End = 0x1F52B, Type = Extended_Pictographic });
            // E1.0   [2] (🔬..🔭)    microscope..telescope
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F52C, End = 0x1F52D, Type = Extended_Pictographic });
            // E0.6  [16] (🔮..🔽)    crystal ball..downwards button
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F52E, End = 0x1F53D, Type = Extended_Pictographic });
            // E0.0   [3] (🕆..🕈)    WHITE LATIN CROSS..CELTIC CROSS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F546, End = 0x1F548, Type = Extended_Pictographic });
            // E0.7   [2] (🕉️..🕊️)    om..dove
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F549, End = 0x1F54A, Type = Extended_Pictographic });
            // E1.0   [4] (🕋..🕎)    kaaba..menorah
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F54B, End = 0x1F54E, Type = Extended_Pictographic });
            // E0.6  [12] (🕐..🕛)    one o’clock..twelve o’clock
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F550, End = 0x1F55B, Type = Extended_Pictographic });
            // E0.7  [12] (🕜..🕧)    one-thirty..twelve-thirty
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F55C, End = 0x1F567, Type = Extended_Pictographic });
            // E0.0   [7] (🕨..🕮)    RIGHT SPEAKER..BOOK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F568, End = 0x1F56E, Type = Extended_Pictographic });
            // E0.7   [2] (🕯️..🕰️)    candle..mantelpiece clock
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F56F, End = 0x1F570, Type = Extended_Pictographic });
            // E0.0   [2] (🕱..🕲)    BLACK SKULL AND CROSSBONES..NO PIRACY
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F571, End = 0x1F572, Type = Extended_Pictographic });
            // E0.7   [7] (🕳️..🕹️)    hole..joystick
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F573, End = 0x1F579, Type = Extended_Pictographic });
            // E0.0  [12] (🕻..🖆)    LEFT HAND TELEPHONE RECEIVER..PEN OVER STAMPED ENVELOPE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F57B, End = 0x1F586, Type = Extended_Pictographic });
            // E0.0   [2] (🖈..🖉)    BLACK PUSHPIN..LOWER LEFT PENCIL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F588, End = 0x1F589, Type = Extended_Pictographic });
            // E0.7   [4] (🖊️..🖍️)    pen..crayon
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F58A, End = 0x1F58D, Type = Extended_Pictographic });
            // E0.0   [2] (🖎..🖏)    LEFT WRITING HAND..TURNED OK HAND SIGN
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F58E, End = 0x1F58F, Type = Extended_Pictographic });
            // E0.0   [4] (🖑..🖔)    REVERSED RAISED HAND WITH FINGERS SPLAYED..REVERSED VICTORY HAND
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F591, End = 0x1F594, Type = Extended_Pictographic });
            // E1.0   [2] (🖕..🖖)    middle finger..vulcan salute
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F595, End = 0x1F596, Type = Extended_Pictographic });
            // E0.0  [13] (🖗..🖣)    WHITE DOWN POINTING LEFT HAND INDEX..BLACK DOWN POINTING BACKHAND INDEX
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F597, End = 0x1F5A3, Type = Extended_Pictographic });
            // E0.0   [2] (🖦..🖧)    KEYBOARD AND MOUSE..THREE NETWORKED COMPUTERS
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5A6, End = 0x1F5A7, Type = Extended_Pictographic });
            // E0.0   [8] (🖩..🖰)    POCKET CALCULATOR..TWO BUTTON MOUSE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5A9, End = 0x1F5B0, Type = Extended_Pictographic });
            // E0.7   [2] (🖱️..🖲️)    computer mouse..trackball
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5B1, End = 0x1F5B2, Type = Extended_Pictographic });
            // E0.0   [9] (🖳..🖻)    OLD PERSONAL COMPUTER..DOCUMENT WITH PICTURE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5B3, End = 0x1F5BB, Type = Extended_Pictographic });
            // E0.0   [5] (🖽..🗁)    FRAME WITH TILES..OPEN FOLDER
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5BD, End = 0x1F5C1, Type = Extended_Pictographic });
            // E0.7   [3] (🗂️..🗄️)    card index dividers..file cabinet
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5C2, End = 0x1F5C4, Type = Extended_Pictographic });
            // E0.0  [12] (🗅..🗐)    EMPTY NOTE..PAGES
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5C5, End = 0x1F5D0, Type = Extended_Pictographic });
            // E0.7   [3] (🗑️..🗓️)    wastebasket..spiral calendar
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5D1, End = 0x1F5D3, Type = Extended_Pictographic });
            // E0.0   [8] (🗔..🗛)    DESKTOP WINDOW..DECREASE FONT SIZE SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5D4, End = 0x1F5DB, Type = Extended_Pictographic });
            // E0.7   [3] (🗜️..🗞️)    clamp..rolled-up newspaper
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5DC, End = 0x1F5DE, Type = Extended_Pictographic });
            // E0.0   [2] (🗟..🗠)    PAGE WITH CIRCLED TEXT..STOCK CHART
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5DF, End = 0x1F5E0, Type = Extended_Pictographic });
            // E0.0   [4] (🗤..🗧)    THREE RAYS ABOVE..THREE RAYS RIGHT
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5E4, End = 0x1F5E7, Type = Extended_Pictographic });
            // E0.0   [6] (🗩..🗮)    RIGHT SPEECH BUBBLE..LEFT ANGER BUBBLE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5E9, End = 0x1F5EE, Type = Extended_Pictographic });
            // E0.0   [3] (🗰..🗲)    MOOD BUBBLE..LIGHTNING MOOD
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5F0, End = 0x1F5F2, Type = Extended_Pictographic });
            // E0.0   [6] (🗴..🗹)    BALLOT SCRIPT X..BALLOT BOX WITH BOLD CHECK
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5F4, End = 0x1F5F9, Type = Extended_Pictographic });
            // E0.6   [5] (🗻..🗿)    mount fuji..moai
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F5FB, End = 0x1F5FF, Type = Extended_Pictographic });
            // E0.6   [6] (😁..😆)    beaming face with smiling eyes..grinning squinting face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F601, End = 0x1F606, Type = Extended_Pictographic });
            // E1.0   [2] (😇..😈)    smiling face with halo..smiling face with horns
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F607, End = 0x1F608, Type = Extended_Pictographic });
            // E0.6   [5] (😉..😍)    winking face..smiling face with heart-eyes
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F609, End = 0x1F60D, Type = Extended_Pictographic });
            // E0.6   [3] (😒..😔)    unamused face..pensive face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F612, End = 0x1F614, Type = Extended_Pictographic });
            // E0.6   [3] (😜..😞)    winking face with tongue..disappointed face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F61C, End = 0x1F61E, Type = Extended_Pictographic });
            // E0.6   [6] (😠..😥)    angry face..sad but relieved face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F620, End = 0x1F625, Type = Extended_Pictographic });
            // E1.0   [2] (😦..😧)    frowning face with open mouth..anguished face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F626, End = 0x1F627, Type = Extended_Pictographic });
            // E0.6   [4] (😨..😫)    fearful face..tired face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F628, End = 0x1F62B, Type = Extended_Pictographic });
            // E1.0   [2] (😮..😯)    face with open mouth..hushed face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F62E, End = 0x1F62F, Type = Extended_Pictographic });
            // E0.6   [4] (😰..😳)    anxious face with sweat..flushed face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F630, End = 0x1F633, Type = Extended_Pictographic });
            // E0.6  [10] (😷..🙀)    face with medical mask..weary cat
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F637, End = 0x1F640, Type = Extended_Pictographic });
            // E1.0   [4] (🙁..🙄)    slightly frowning face..face with rolling eyes
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F641, End = 0x1F644, Type = Extended_Pictographic });
            // E0.6  [11] (🙅..🙏)    person gesturing NO..folded hands
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F645, End = 0x1F64F, Type = Extended_Pictographic });
            // E1.0   [2] (🚁..🚂)    helicopter..locomotive
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F681, End = 0x1F682, Type = Extended_Pictographic });
            // E0.6   [3] (🚃..🚅)    railway car..bullet train
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F683, End = 0x1F685, Type = Extended_Pictographic });
            // E1.0   [2] (🚊..🚋)    tram..tram car
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F68A, End = 0x1F68B, Type = Extended_Pictographic });
            // E0.6   [3] (🚑..🚓)    ambulance..police car
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F691, End = 0x1F693, Type = Extended_Pictographic });
            // E0.6   [2] (🚙..🚚)    sport utility vehicle..delivery truck
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F699, End = 0x1F69A, Type = Extended_Pictographic });
            // E1.0   [7] (🚛..🚡)    articulated lorry..aerial tramway
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F69B, End = 0x1F6A1, Type = Extended_Pictographic });
            // E0.6   [2] (🚤..🚥)    speedboat..horizontal traffic light
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6A4, End = 0x1F6A5, Type = Extended_Pictographic });
            // E0.6   [7] (🚧..🚭)    construction..no smoking
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6A7, End = 0x1F6AD, Type = Extended_Pictographic });
            // E1.0   [4] (🚮..🚱)    litter in bin sign..non-potable water
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6AE, End = 0x1F6B1, Type = Extended_Pictographic });
            // E1.0   [3] (🚳..🚵)    no bicycles..person mountain biking
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6B3, End = 0x1F6B5, Type = Extended_Pictographic });
            // E1.0   [2] (🚷..🚸)    no pedestrians..children crossing
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6B7, End = 0x1F6B8, Type = Extended_Pictographic });
            // E0.6   [6] (🚹..🚾)    men’s room..water closet
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6B9, End = 0x1F6BE, Type = Extended_Pictographic });
            // E1.0   [5] (🛁..🛅)    bathtub..left luggage
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6C1, End = 0x1F6C5, Type = Extended_Pictographic });
            // E0.0   [5] (🛆..🛊)    TRIANGLE WITH ROUNDED CORNERS..GIRLS SYMBOL
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6C6, End = 0x1F6CA, Type = Extended_Pictographic });
            // E0.7   [3] (🛍️..🛏️)    shopping bags..bed
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6CD, End = 0x1F6CF, Type = Extended_Pictographic });
            // E3.0   [2] (🛑..🛒)    stop sign..shopping cart
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6D1, End = 0x1F6D2, Type = Extended_Pictographic });
            // E0.0   [2] (🛓..🛔)    STUPA..PAGODA
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6D3, End = 0x1F6D4, Type = Extended_Pictographic });
            // E13.0  [2] (🛖..🛗)    hut..elevator
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6D6, End = 0x1F6D7, Type = Extended_Pictographic });
            // E0.0   [5] (🛘..🛜)    <reserved-1F6D8>..<reserved-1F6DC>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6D8, End = 0x1F6DC, Type = Extended_Pictographic });
            // E14.0  [3] (🛝..🛟)    playground slide..ring buoy
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6DD, End = 0x1F6DF, Type = Extended_Pictographic });
            // E0.7   [6] (🛠️..🛥️)    hammer and wrench..motor boat
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6E0, End = 0x1F6E5, Type = Extended_Pictographic });
            // E0.0   [3] (🛦..🛨)    UP-POINTING MILITARY AIRPLANE..UP-POINTING SMALL AIRPLANE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6E6, End = 0x1F6E8, Type = Extended_Pictographic });
            // E1.0   [2] (🛫..🛬)    airplane departure..airplane arrival
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6EB, End = 0x1F6EC, Type = Extended_Pictographic });
            // E0.0   [3] (🛭..🛯)    <reserved-1F6ED>..<reserved-1F6EF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6ED, End = 0x1F6EF, Type = Extended_Pictographic });
            // E0.0   [2] (🛱..🛲)    ONCOMING FIRE ENGINE..DIESEL LOCOMOTIVE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6F1, End = 0x1F6F2, Type = Extended_Pictographic });
            // E3.0   [3] (🛴..🛶)    kick scooter..canoe
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6F4, End = 0x1F6F6, Type = Extended_Pictographic });
            // E5.0   [2] (🛷..🛸)    sled..flying saucer
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6F7, End = 0x1F6F8, Type = Extended_Pictographic });
            // E13.0  [2] (🛻..🛼)    pickup truck..roller skate
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6FB, End = 0x1F6FC, Type = Extended_Pictographic });
            // E0.0   [3] (🛽..🛿)    <reserved-1F6FD>..<reserved-1F6FF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F6FD, End = 0x1F6FF, Type = Extended_Pictographic });
            // E0.0  [12] (🝴..🝿)    <reserved-1F774>..<reserved-1F77F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F774, End = 0x1F77F, Type = Extended_Pictographic });
            // E0.0  [11] (🟕..🟟)    CIRCLED TRIANGLE..<reserved-1F7DF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F7D5, End = 0x1F7DF, Type = Extended_Pictographic });
            // E12.0 [12] (🟠..🟫)    orange circle..brown square
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F7E0, End = 0x1F7EB, Type = Extended_Pictographic });
            // E0.0   [4] (🟬..🟯)    <reserved-1F7EC>..<reserved-1F7EF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F7EC, End = 0x1F7EF, Type = Extended_Pictographic });
            // E0.0  [15] (🟱..🟿)    <reserved-1F7F1>..<reserved-1F7FF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F7F1, End = 0x1F7FF, Type = Extended_Pictographic });
            // E0.0   [4] (🠌..🠏)    <reserved-1F80C>..<reserved-1F80F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F80C, End = 0x1F80F, Type = Extended_Pictographic });
            // E0.0   [8] (🡈..🡏)    <reserved-1F848>..<reserved-1F84F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F848, End = 0x1F84F, Type = Extended_Pictographic });
            // E0.0   [6] (🡚..🡟)    <reserved-1F85A>..<reserved-1F85F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F85A, End = 0x1F85F, Type = Extended_Pictographic });
            // E0.0   [8] (🢈..🢏)    <reserved-1F888>..<reserved-1F88F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F888, End = 0x1F88F, Type = Extended_Pictographic });
            // E0.0  [82] (🢮..🣿)    <reserved-1F8AE>..<reserved-1F8FF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F8AE, End = 0x1F8FF, Type = Extended_Pictographic });
            // E12.0  [3] (🤍..🤏)    white heart..pinching hand
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F90D, End = 0x1F90F, Type = Extended_Pictographic });
            // E1.0   [9] (🤐..🤘)    zipper-mouth face..sign of the horns
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F910, End = 0x1F918, Type = Extended_Pictographic });
            // E3.0   [6] (🤙..🤞)    call me hand..crossed fingers
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F919, End = 0x1F91E, Type = Extended_Pictographic });
            // E3.0   [8] (🤠..🤧)    cowboy hat face..sneezing face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F920, End = 0x1F927, Type = Extended_Pictographic });
            // E5.0   [8] (🤨..🤯)    face with raised eyebrow..exploding head
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F928, End = 0x1F92F, Type = Extended_Pictographic });
            // E5.0   [2] (🤱..🤲)    breast-feeding..palms up together
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F931, End = 0x1F932, Type = Extended_Pictographic });
            // E3.0   [8] (🤳..🤺)    selfie..person fencing
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F933, End = 0x1F93A, Type = Extended_Pictographic });
            // E3.0   [3] (🤼..🤾)    people wrestling..person playing handball
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F93C, End = 0x1F93E, Type = Extended_Pictographic });
            // E3.0   [6] (🥀..🥅)    wilted flower..goal net
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F940, End = 0x1F945, Type = Extended_Pictographic });
            // E3.0   [5] (🥇..🥋)    1st place medal..martial arts uniform
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F947, End = 0x1F94B, Type = Extended_Pictographic });
            // E11.0  [3] (🥍..🥏)    lacrosse..flying disc
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F94D, End = 0x1F94F, Type = Extended_Pictographic });
            // E3.0  [15] (🥐..🥞)    croissant..pancakes
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F950, End = 0x1F95E, Type = Extended_Pictographic });
            // E5.0  [13] (🥟..🥫)    dumpling..canned food
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F95F, End = 0x1F96B, Type = Extended_Pictographic });
            // E11.0  [5] (🥬..🥰)    leafy green..smiling face with hearts
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F96C, End = 0x1F970, Type = Extended_Pictographic });
            // E11.0  [4] (🥳..🥶)    partying face..cold face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F973, End = 0x1F976, Type = Extended_Pictographic });
            // E13.0  [2] (🥷..🥸)    ninja..disguised face
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F977, End = 0x1F978, Type = Extended_Pictographic });
            // E11.0  [4] (🥼..🥿)    lab coat..flat shoe
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F97C, End = 0x1F97F, Type = Extended_Pictographic });
            // E1.0   [5] (🦀..🦄)    crab..unicorn
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F980, End = 0x1F984, Type = Extended_Pictographic });
            // E3.0  [13] (🦅..🦑)    eagle..squid
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F985, End = 0x1F991, Type = Extended_Pictographic });
            // E5.0   [6] (🦒..🦗)    giraffe..cricket
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F992, End = 0x1F997, Type = Extended_Pictographic });
            // E11.0 [11] (🦘..🦢)    kangaroo..swan
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F998, End = 0x1F9A2, Type = Extended_Pictographic });
            // E13.0  [2] (🦣..🦤)    mammoth..dodo
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9A3, End = 0x1F9A4, Type = Extended_Pictographic });
            // E12.0  [6] (🦥..🦪)    sloth..oyster
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9A5, End = 0x1F9AA, Type = Extended_Pictographic });
            // E13.0  [3] (🦫..🦭)    beaver..seal
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9AB, End = 0x1F9AD, Type = Extended_Pictographic });
            // E12.0  [2] (🦮..🦯)    guide dog..white cane
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9AE, End = 0x1F9AF, Type = Extended_Pictographic });
            // E11.0 [10] (🦰..🦹)    red hair..supervillain
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9B0, End = 0x1F9B9, Type = Extended_Pictographic });
            // E12.0  [6] (🦺..🦿)    safety vest..mechanical leg
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9BA, End = 0x1F9BF, Type = Extended_Pictographic });
            // E11.0  [2] (🧁..🧂)    cupcake..salt
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9C1, End = 0x1F9C2, Type = Extended_Pictographic });
            // E12.0  [8] (🧃..🧊)    beverage box..ice
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9C3, End = 0x1F9CA, Type = Extended_Pictographic });
            // E12.0  [3] (🧍..🧏)    person standing..deaf person
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9CD, End = 0x1F9CF, Type = Extended_Pictographic });
            // E5.0  [23] (🧐..🧦)    face with monocle..socks
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9D0, End = 0x1F9E6, Type = Extended_Pictographic });
            // E11.0 [25] (🧧..🧿)    red envelope..nazar amulet
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1F9E7, End = 0x1F9FF, Type = Extended_Pictographic });
            // E0.0 [112] (🨀..🩯)    NEUTRAL CHESS KING..<reserved-1FA6F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA00, End = 0x1FA6F, Type = Extended_Pictographic });
            // E12.0  [4] (🩰..🩳)    ballet shoes..shorts
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA70, End = 0x1FA73, Type = Extended_Pictographic });
            // E0.0   [3] (🩵..🩷)    <reserved-1FA75>..<reserved-1FA77>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA75, End = 0x1FA77, Type = Extended_Pictographic });
            // E12.0  [3] (🩸..🩺)    drop of blood..stethoscope
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA78, End = 0x1FA7A, Type = Extended_Pictographic });
            // E14.0  [2] (🩻..🩼)    x-ray..crutch
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA7B, End = 0x1FA7C, Type = Extended_Pictographic });
            // E0.0   [3] (🩽..🩿)    <reserved-1FA7D>..<reserved-1FA7F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA7D, End = 0x1FA7F, Type = Extended_Pictographic });
            // E12.0  [3] (🪀..🪂)    yo-yo..parachute
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA80, End = 0x1FA82, Type = Extended_Pictographic });
            // E13.0  [4] (🪃..🪆)    boomerang..nesting dolls
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA83, End = 0x1FA86, Type = Extended_Pictographic });
            // E0.0   [9] (🪇..🪏)    <reserved-1FA87>..<reserved-1FA8F>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA87, End = 0x1FA8F, Type = Extended_Pictographic });
            // E12.0  [6] (🪐..🪕)    ringed planet..banjo
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA90, End = 0x1FA95, Type = Extended_Pictographic });
            // E13.0 [19] (🪖..🪨)    military helmet..rock
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FA96, End = 0x1FAA8, Type = Extended_Pictographic });
            // E14.0  [4] (🪩..🪬)    mirror ball..hamsa
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAA9, End = 0x1FAAC, Type = Extended_Pictographic });
            // E0.0   [3] (🪭..🪯)    <reserved-1FAAD>..<reserved-1FAAF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAAD, End = 0x1FAAF, Type = Extended_Pictographic });
            // E13.0  [7] (🪰..🪶)    fly..feather
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAB0, End = 0x1FAB6, Type = Extended_Pictographic });
            // E14.0  [4] (🪷..🪺)    lotus..nest with eggs
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAB7, End = 0x1FABA, Type = Extended_Pictographic });
            // E0.0   [5] (🪻..🪿)    <reserved-1FABB>..<reserved-1FABF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FABB, End = 0x1FABF, Type = Extended_Pictographic });
            // E13.0  [3] (🫀..🫂)    anatomical heart..people hugging
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAC0, End = 0x1FAC2, Type = Extended_Pictographic });
            // E14.0  [3] (🫃..🫅)    pregnant man..person with crown
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAC3, End = 0x1FAC5, Type = Extended_Pictographic });
            // E0.0  [10] (🫆..🫏)    <reserved-1FAC6>..<reserved-1FACF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAC6, End = 0x1FACF, Type = Extended_Pictographic });
            // E13.0  [7] (🫐..🫖)    blueberries..teapot
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAD0, End = 0x1FAD6, Type = Extended_Pictographic });
            // E14.0  [3] (🫗..🫙)    pouring liquid..jar
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAD7, End = 0x1FAD9, Type = Extended_Pictographic });
            // E0.0   [6] (🫚..🫟)    <reserved-1FADA>..<reserved-1FADF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FADA, End = 0x1FADF, Type = Extended_Pictographic });
            // E14.0  [8] (🫠..🫧)    melting face..bubbles
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAE0, End = 0x1FAE7, Type = Extended_Pictographic });
            // E0.0   [8] (🫨..🫯)    <reserved-1FAE8>..<reserved-1FAEF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAE8, End = 0x1FAEF, Type = Extended_Pictographic });
            // E14.0  [7] (🫰..🫶)    hand with index finger and thumb crossed..heart hands
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAF0, End = 0x1FAF6, Type = Extended_Pictographic });
            // E0.0   [9] (🫷..🫿)    <reserved-1FAF7>..<reserved-1FAFF>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FAF7, End = 0x1FAFF, Type = Extended_Pictographic });
            // Nd  [10] SEGMENTED DIGIT ZERO..SEGMENTED DIGIT NINE
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FBF0, End = 0x1FBF9, Type = Numeric });
            // E0.0[1022] (🰀..🿽)    <reserved-1FC00>..<reserved-1FFFD>
            m_lst_code_range.Add(new RangeInfo() { Start = 0x1FC00, End = 0x1FFFD, Type = Extended_Pictographic });
            // Cf  [96] TAG SPACE..CANCEL TAG
            m_lst_code_range.Add(new RangeInfo() { Start = 0xE0020, End = 0xE007F, Type = Extend });
            // Mn [240] VARIATION SELECTOR-17..VARIATION SELECTOR-256
            m_lst_code_range.Add(new RangeInfo() { Start = 0xE0100, End = 0xE01EF, Type = Extend });
        }

        /// <summary>
        /// Create cache from [m_lst_code_range] to int[];
        /// </summary>
        /// <returns>The array length</returns>
        public static int CreateArrayCache() {
            if (m_arr_cache_break_type != null) {
                return m_arr_cache_break_type.Length;
            }
            int[] arr = new int[m_lst_code_range[m_lst_code_range.Count - 1].End + 1];
            foreach (var v in m_lst_code_range) {
                for (int i = v.Start; i <= v.End; i++)
                    arr[i] = v.Type;
            }
            m_arr_cache_break_type = arr;
            return arr.Length;
        }
        /// <summary>
        /// Create cache from [m_lst_code_range] to Dictionary(int,int)
        /// </summary>
        /// <returns>Dictionary.Count</returns>
        public static int CreateDictionaryCache() {
            if (m_dic_cache_break_type != null) {
                return m_dic_cache_break_type.Count;
            }
            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var v in m_lst_code_range) {
                for (int i = v.Start; i <= v.End; i++)
                    dic.Add(i, v.Type);
            }
            m_dic_cache_break_type = dic;
            return dic.Count;
        }
        /// <summary>
        /// Clear all cache
        /// </summary>
        public static void ClearCache() {
            if (m_dic_cache_break_type != null) {
                m_dic_cache_break_type.Clear();
            }
            m_dic_cache_break_type = null;
            m_arr_cache_break_type = null;
        }

        protected override bool ShouldBreak(int nRightBreakType, List<int> lstHistoryBreakType) {
            int nLeftBreakType = lstHistoryBreakType[lstHistoryBreakType.Count - 1];
            // The urles from: https://unicode.org/reports/tr29/#Word_Boundary_Rules
            // AHLetter     (ALetter    |   Hebrew_Letter)
            // MidNumLetQ   (MidNumLet  |   Single_Quote)
            // ÷    Boundary (allow break here)
            // ×    No boundary(do not allow break here)
            // →    Treat whatever on the left side as if it were what is on the right side
            // WB*  LeftChar_Property (÷ | × | →) RightChar_Property

            // Break at the start and end of text, unless the text is empty.
            // WB1 	sot     ÷   Any
            // WB2 	Any     ÷   eot
            // for WB1 and WB2 not need code

            // Do not break within CRLF.
            // WB3  CR      ×   LF
            if (nLeftBreakType == CR && nRightBreakType == LF) {
                return false;
            }

            // Otherwise break before and after Newlines (including CR and LF)
            // WB3a     (Newline | CR | LF) ÷	 
            // WB3b                         ÷   (Newline | CR | LF)
            switch (nLeftBreakType) {
                case Newline:
                case CR:
                case LF:
                    return true;
            }
            switch (nRightBreakType) {
                case Newline:
                case CR:
                case LF:
                    return true;
            }

            // Do not break within emoji zwj sequences.
            // WB3c     ZWJ     ×   \p{Extended_Pictographic}
            if (nLeftBreakType == ZWJ && nRightBreakType == Extended_Pictographic) {
                return false;
            }

            // Keep horizontal whitespace together.
            // WB3d     WSegSpace   ×   WSegSpace
            if (nLeftBreakType == WSegSpace && nRightBreakType == WSegSpace) {
                return false;
            }

            // Ignore Format and Extend characters, except after sot, CR, LF, and Newline. (See Section 6.2, Replacing Ignore Rules.) This also has the effect of: Any × (Format | Extend | ZWJ)
            // WB4      X (Extend | Format | ZWJ)*  →   X
            // Note: sorry.. I don't what's mean .. so ...
            if (nRightBreakType == Format || nRightBreakType == Extend || nRightBreakType == ZWJ) {
                return false;
            }

            // Do not break between most letters.
            // WB5  AHLetter    ×   AHLetter
            if ((nLeftBreakType == ALetter || nLeftBreakType == Hebrew_Letter) && (nRightBreakType == ALetter || nRightBreakType == Hebrew_Letter)) {
                return false;
            }

            // Do not break letters across certain punctuation.
            // WB6                          AHLetter        ×   (MidLetter | MidNumLetQ) AHLetter
            // WB7      AHLetter (MidLetter | MidNumLetQ)   ×   AHLetter
            // WB7a                         Hebrew_Letter   ×   Single_Quote
            // WB7b                         Hebrew_Letter   ×   Double_Quote Hebrew_Letter
            // WB7c             Hebrew_Letter Double_Quote  ×   Hebrew_Letter
            if (nRightBreakType == ALetter || nRightBreakType == Hebrew_Letter) { // WB6 WB7
                if (lstHistoryBreakType.Count > 1) {
                    int a = lstHistoryBreakType[lstHistoryBreakType.Count - 2];
                    int b = lstHistoryBreakType[lstHistoryBreakType.Count - 1];
                    if ((a == ALetter || a == Hebrew_Letter) && (b == MidLetter || b == MidNumLet || b == Single_Quote)) {
                        return false;
                    }
                }
            }
            if (nLeftBreakType == Hebrew_Letter && nRightBreakType == Single_Quote) {
                return false;
            }
            if (nRightBreakType == Hebrew_Letter) { //WB7b WB7c
                if (lstHistoryBreakType.Count > 1) {
                    int a = lstHistoryBreakType[lstHistoryBreakType.Count - 2];
                    int b = lstHistoryBreakType[lstHistoryBreakType.Count - 1];
                    if (a == Hebrew_Letter && b == Double_Quote) {
                        return false;
                    }
                }
            }

            // Do not break within sequences of digits, or digits adjacent to letters (“3a”, or “A3”).
            // WB8      Numeric     ×   Numeric
            // WB9      AHLetter    ×   Numeric
            // WB10     Numeric     ×   AHLetter
            if (nLeftBreakType == Numeric && nRightBreakType == Numeric) {
                return false;
            }
            if ((nLeftBreakType == ALetter || nLeftBreakType == Hebrew_Letter) && nRightBreakType == Numeric) {
                return false;
            }
            if (nLeftBreakType == Numeric && (nRightBreakType == ALetter || nRightBreakType == Hebrew_Letter)) {
                return false;
            }

            // Do not break within sequences, such as “3.2” or “3,456.789”.
            // WB11     Numeric (MidNum | MidNumLetQ)   ×   Numeric
            // WB12     Numeric                         ×   (MidNum | MidNumLetQ) Numeric
            if (nRightBreakType == Numeric) {
                if (lstHistoryBreakType.Count > 1) {
                    int a = lstHistoryBreakType[lstHistoryBreakType.Count - 2];
                    int b = lstHistoryBreakType[lstHistoryBreakType.Count - 1];
                    if (a == Numeric && (b == MidNum || b == MidNumLet || b == Single_Quote)) {
                        return false;
                    }
                }
            }

            // Do not break between Katakana.
            // WB13     Katakana    ×   Katakana
            if (nLeftBreakType == Katakana && nRightBreakType == Katakana) {
                return false;
            }

            // Do not break from extenders.
            // WB13a    (AHLetter | Numeric | Katakana | ExtendNumLet)  ×   ExtendNumLet
            // WB13b                                    ExtendNumLet    ×	(AHLetter | Numeric | Katakana)
            if (nRightBreakType == ExtendNumLet) {
                switch (nLeftBreakType) {
                    case ALetter:
                    case Hebrew_Letter:
                    case Numeric:
                    case Katakana:
                    case ExtendNumLet:
                        return false;
                }
            }
            if (nLeftBreakType == ExtendNumLet) {
                switch (nRightBreakType) {
                    case ALetter:
                    case Hebrew_Letter:
                    case Numeric:
                    case Katakana:
                        return false;
                }
            }

            // Do not break within emoji flag sequences. That is,
            // do not break between regional indicator (RI) symbols if there is an odd number of RI characters before the break point.
            // GB12  sot (RI RI)* RI     ×   RI
            // GB13  [^RI] (RI RI)* RI   ×   RI
            if (nRightBreakType == Regional_Indicator) {
                int nRICount = 0;
                for (int i = lstHistoryBreakType.Count - 1; i >= 0; i--) {
                    if (lstHistoryBreakType[i] != Regional_Indicator) {
                        break;
                    }
                    nRICount++;
                }
                if (nRICount % 2 == 1) return false;
            }

            // The rule not in unicode document. just by myself.
            if (nLeftBreakType == nRightBreakType) return false;
            return true;
        }

        protected override int GetBreakProperty(int nCodePoint) {
            switch (nCodePoint) {
                case 0x00022: return Double_Quote;         // Po       QUOTATION MARK
                case 0x00027: return Single_Quote;         // Po       APOSTROPHE
                case 0x0FB1D: return Hebrew_Letter;        // Lo       HEBREW LETTER YOD WITH HIRIQ
                case 0x0FB3E: return Hebrew_Letter;        // Lo       HEBREW LETTER MEM WITH DAGESH
                case 0x0000D: return CR;                   // Cc       <control-000D>
                case 0x0000A: return LF;                   // Cc       <control-000A>
                case 0x00085: return Newline;              // Cc       <control-0085>
                case 0x02028: return Newline;              // Zl       LINE SEPARATOR
                case 0x02029: return Newline;              // Zp       PARAGRAPH SEPARATOR
                case 0x005BF: return Extend;               // Mn       HEBREW POINT RAFE
                case 0x005C7: return Extend;               // Mn       HEBREW POINT QAMATS QATAN
                case 0x00670: return Extend;               // Mn       ARABIC LETTER SUPERSCRIPT ALEF
                case 0x00711: return Extend;               // Mn       SYRIAC LETTER SUPERSCRIPT ALAPH
                case 0x007FD: return Extend;               // Mn       NKO DANTAYALAN
                case 0x00903: return Extend;               // Mc       DEVANAGARI SIGN VISARGA
                case 0x0093A: return Extend;               // Mn       DEVANAGARI VOWEL SIGN OE
                case 0x0093B: return Extend;               // Mc       DEVANAGARI VOWEL SIGN OOE
                case 0x0093C: return Extend;               // Mn       DEVANAGARI SIGN NUKTA
                case 0x0094D: return Extend;               // Mn       DEVANAGARI SIGN VIRAMA
                case 0x00981: return Extend;               // Mn       BENGALI SIGN CANDRABINDU
                case 0x009BC: return Extend;               // Mn       BENGALI SIGN NUKTA
                case 0x009CD: return Extend;               // Mn       BENGALI SIGN VIRAMA
                case 0x009D7: return Extend;               // Mc       BENGALI AU LENGTH MARK
                case 0x009FE: return Extend;               // Mn       BENGALI SANDHI MARK
                case 0x00A03: return Extend;               // Mc       GURMUKHI SIGN VISARGA
                case 0x00A3C: return Extend;               // Mn       GURMUKHI SIGN NUKTA
                case 0x00A51: return Extend;               // Mn       GURMUKHI SIGN UDAAT
                case 0x00A75: return Extend;               // Mn       GURMUKHI SIGN YAKASH
                case 0x00A83: return Extend;               // Mc       GUJARATI SIGN VISARGA
                case 0x00ABC: return Extend;               // Mn       GUJARATI SIGN NUKTA
                case 0x00AC9: return Extend;               // Mc       GUJARATI VOWEL SIGN CANDRA O
                case 0x00ACD: return Extend;               // Mn       GUJARATI SIGN VIRAMA
                case 0x00B01: return Extend;               // Mn       ORIYA SIGN CANDRABINDU
                case 0x00B3C: return Extend;               // Mn       ORIYA SIGN NUKTA
                case 0x00B3E: return Extend;               // Mc       ORIYA VOWEL SIGN AA
                case 0x00B3F: return Extend;               // Mn       ORIYA VOWEL SIGN I
                case 0x00B40: return Extend;               // Mc       ORIYA VOWEL SIGN II
                case 0x00B4D: return Extend;               // Mn       ORIYA SIGN VIRAMA
                case 0x00B57: return Extend;               // Mc       ORIYA AU LENGTH MARK
                case 0x00B82: return Extend;               // Mn       TAMIL SIGN ANUSVARA
                case 0x00BC0: return Extend;               // Mn       TAMIL VOWEL SIGN II
                case 0x00BCD: return Extend;               // Mn       TAMIL SIGN VIRAMA
                case 0x00BD7: return Extend;               // Mc       TAMIL AU LENGTH MARK
                case 0x00C00: return Extend;               // Mn       TELUGU SIGN COMBINING CANDRABINDU ABOVE
                case 0x00C04: return Extend;               // Mn       TELUGU SIGN COMBINING ANUSVARA ABOVE
                case 0x00C3C: return Extend;               // Mn       TELUGU SIGN NUKTA
                case 0x00C81: return Extend;               // Mn       KANNADA SIGN CANDRABINDU
                case 0x00CBC: return Extend;               // Mn       KANNADA SIGN NUKTA
                case 0x00CBE: return Extend;               // Mc       KANNADA VOWEL SIGN AA
                case 0x00CBF: return Extend;               // Mn       KANNADA VOWEL SIGN I
                case 0x00CC6: return Extend;               // Mn       KANNADA VOWEL SIGN E
                case 0x00D4D: return Extend;               // Mn       MALAYALAM SIGN VIRAMA
                case 0x00D57: return Extend;               // Mc       MALAYALAM AU LENGTH MARK
                case 0x00D81: return Extend;               // Mn       SINHALA SIGN CANDRABINDU
                case 0x00DCA: return Extend;               // Mn       SINHALA SIGN AL-LAKUNA
                case 0x00DD6: return Extend;               // Mn       SINHALA VOWEL SIGN DIGA PAA-PILLA
                case 0x00E31: return Extend;               // Mn       THAI CHARACTER MAI HAN-AKAT
                case 0x00EB1: return Extend;               // Mn       LAO VOWEL SIGN MAI KAN
                case 0x00F35: return Extend;               // Mn       TIBETAN MARK NGAS BZUNG NYI ZLA
                case 0x00F37: return Extend;               // Mn       TIBETAN MARK NGAS BZUNG SGOR RTAGS
                case 0x00F39: return Extend;               // Mn       TIBETAN MARK TSA -PHRU
                case 0x00F7F: return Extend;               // Mc       TIBETAN SIGN RNAM BCAD
                case 0x00FC6: return Extend;               // Mn       TIBETAN SYMBOL PADMA GDAN
                case 0x01031: return Extend;               // Mc       MYANMAR VOWEL SIGN E
                case 0x01038: return Extend;               // Mc       MYANMAR SIGN VISARGA
                case 0x01082: return Extend;               // Mn       MYANMAR CONSONANT SIGN SHAN MEDIAL WA
                case 0x0108D: return Extend;               // Mn       MYANMAR SIGN SHAN COUNCIL EMPHATIC TONE
                case 0x0108F: return Extend;               // Mc       MYANMAR SIGN RUMAI PALAUNG TONE-5
                case 0x0109D: return Extend;               // Mn       MYANMAR VOWEL SIGN AITON AI
                case 0x01715: return Extend;               // Mc       TAGALOG SIGN PAMUDPOD
                case 0x01734: return Extend;               // Mc       HANUNOO SIGN PAMUDPOD
                case 0x017B6: return Extend;               // Mc       KHMER VOWEL SIGN AA
                case 0x017C6: return Extend;               // Mn       KHMER SIGN NIKAHIT
                case 0x017DD: return Extend;               // Mn       KHMER SIGN ATTHACAN
                case 0x0180F: return Extend;               // Mn       MONGOLIAN FREE VARIATION SELECTOR FOUR
                case 0x018A9: return Extend;               // Mn       MONGOLIAN LETTER ALI GALI DAGALGA
                case 0x01932: return Extend;               // Mn       LIMBU SMALL LETTER ANUSVARA
                case 0x01A1B: return Extend;               // Mn       BUGINESE VOWEL SIGN AE
                case 0x01A55: return Extend;               // Mc       TAI THAM CONSONANT SIGN MEDIAL RA
                case 0x01A56: return Extend;               // Mn       TAI THAM CONSONANT SIGN MEDIAL LA
                case 0x01A57: return Extend;               // Mc       TAI THAM CONSONANT SIGN LA TANG LAI
                case 0x01A60: return Extend;               // Mn       TAI THAM SIGN SAKOT
                case 0x01A61: return Extend;               // Mc       TAI THAM VOWEL SIGN A
                case 0x01A62: return Extend;               // Mn       TAI THAM VOWEL SIGN MAI SAT
                case 0x01A7F: return Extend;               // Mn       TAI THAM COMBINING CRYPTOGRAMMIC DOT
                case 0x01ABE: return Extend;               // Me       COMBINING PARENTHESES OVERLAY
                case 0x01B04: return Extend;               // Mc       BALINESE SIGN BISAH
                case 0x01B34: return Extend;               // Mn       BALINESE SIGN REREKAN
                case 0x01B35: return Extend;               // Mc       BALINESE VOWEL SIGN TEDUNG
                case 0x01B3B: return Extend;               // Mc       BALINESE VOWEL SIGN RA REPA TEDUNG
                case 0x01B3C: return Extend;               // Mn       BALINESE VOWEL SIGN LA LENGA
                case 0x01B42: return Extend;               // Mn       BALINESE VOWEL SIGN PEPET
                case 0x01B82: return Extend;               // Mc       SUNDANESE SIGN PANGWISAD
                case 0x01BA1: return Extend;               // Mc       SUNDANESE CONSONANT SIGN PAMINGKAL
                case 0x01BAA: return Extend;               // Mc       SUNDANESE SIGN PAMAAEH
                case 0x01BE6: return Extend;               // Mn       BATAK SIGN TOMPI
                case 0x01BE7: return Extend;               // Mc       BATAK VOWEL SIGN E
                case 0x01BED: return Extend;               // Mn       BATAK VOWEL SIGN KARO O
                case 0x01BEE: return Extend;               // Mc       BATAK VOWEL SIGN U
                case 0x01CE1: return Extend;               // Mc       VEDIC TONE ATHARVAVEDIC INDEPENDENT SVARITA
                case 0x01CED: return Extend;               // Mn       VEDIC SIGN TIRYAK
                case 0x01CF4: return Extend;               // Mn       VEDIC TONE CANDRA ABOVE
                case 0x01CF7: return Extend;               // Mc       VEDIC SIGN ATIKRAMA
                case 0x0200C: return Extend;               // Cf       ZERO WIDTH NON-JOINER
                case 0x020E1: return Extend;               // Mn       COMBINING LEFT RIGHT ARROW ABOVE
                case 0x02D7F: return Extend;               // Mn       TIFINAGH CONSONANT JOINER
                case 0x0A66F: return Extend;               // Mn       COMBINING CYRILLIC VZMET
                case 0x0A802: return Extend;               // Mn       SYLOTI NAGRI SIGN DVISVARA
                case 0x0A806: return Extend;               // Mn       SYLOTI NAGRI SIGN HASANTA
                case 0x0A80B: return Extend;               // Mn       SYLOTI NAGRI SIGN ANUSVARA
                case 0x0A827: return Extend;               // Mc       SYLOTI NAGRI VOWEL SIGN OO
                case 0x0A82C: return Extend;               // Mn       SYLOTI NAGRI SIGN ALTERNATE HASANTA
                case 0x0A8FF: return Extend;               // Mn       DEVANAGARI VOWEL SIGN AY
                case 0x0A983: return Extend;               // Mc       JAVANESE SIGN WIGNYAN
                case 0x0A9B3: return Extend;               // Mn       JAVANESE SIGN CECAK TELU
                case 0x0A9E5: return Extend;               // Mn       MYANMAR SIGN SHAN SAW
                case 0x0AA43: return Extend;               // Mn       CHAM CONSONANT SIGN FINAL NG
                case 0x0AA4C: return Extend;               // Mn       CHAM CONSONANT SIGN FINAL M
                case 0x0AA4D: return Extend;               // Mc       CHAM CONSONANT SIGN FINAL H
                case 0x0AA7B: return Extend;               // Mc       MYANMAR SIGN PAO KAREN TONE
                case 0x0AA7C: return Extend;               // Mn       MYANMAR SIGN TAI LAING TONE-2
                case 0x0AA7D: return Extend;               // Mc       MYANMAR SIGN TAI LAING TONE-5
                case 0x0AAB0: return Extend;               // Mn       TAI VIET MAI KANG
                case 0x0AAC1: return Extend;               // Mn       TAI VIET TONE MAI THO
                case 0x0AAEB: return Extend;               // Mc       MEETEI MAYEK VOWEL SIGN II
                case 0x0AAF5: return Extend;               // Mc       MEETEI MAYEK VOWEL SIGN VISARGA
                case 0x0AAF6: return Extend;               // Mn       MEETEI MAYEK VIRAMA
                case 0x0ABE5: return Extend;               // Mn       MEETEI MAYEK VOWEL SIGN ANAP
                case 0x0ABE8: return Extend;               // Mn       MEETEI MAYEK VOWEL SIGN UNAP
                case 0x0ABEC: return Extend;               // Mc       MEETEI MAYEK LUM IYEK
                case 0x0ABED: return Extend;               // Mn       MEETEI MAYEK APUN IYEK
                case 0x0FB1E: return Extend;               // Mn       HEBREW POINT JUDEO-SPANISH VARIKA
                case 0x101FD: return Extend;               // Mn       PHAISTOS DISC SIGN COMBINING OBLIQUE STROKE
                case 0x102E0: return Extend;               // Mn       COPTIC EPACT THOUSANDS MARK
                case 0x10A3F: return Extend;               // Mn       KHAROSHTHI VIRAMA
                case 0x11000: return Extend;               // Mc       BRAHMI SIGN CANDRABINDU
                case 0x11001: return Extend;               // Mn       BRAHMI SIGN ANUSVARA
                case 0x11002: return Extend;               // Mc       BRAHMI SIGN VISARGA
                case 0x11070: return Extend;               // Mn       BRAHMI SIGN OLD TAMIL VIRAMA
                case 0x11082: return Extend;               // Mc       KAITHI SIGN VISARGA
                case 0x110C2: return Extend;               // Mn       KAITHI VOWEL SIGN VOCALIC R
                case 0x1112C: return Extend;               // Mc       CHAKMA VOWEL SIGN E
                case 0x11173: return Extend;               // Mn       MAHAJANI SIGN NUKTA
                case 0x11182: return Extend;               // Mc       SHARADA SIGN VISARGA
                case 0x111CE: return Extend;               // Mc       SHARADA VOWEL SIGN PRISHTHAMATRA E
                case 0x111CF: return Extend;               // Mn       SHARADA SIGN INVERTED CANDRABINDU
                case 0x11234: return Extend;               // Mn       KHOJKI SIGN ANUSVARA
                case 0x11235: return Extend;               // Mc       KHOJKI SIGN VIRAMA
                case 0x1123E: return Extend;               // Mn       KHOJKI SIGN SUKUN
                case 0x112DF: return Extend;               // Mn       KHUDAWADI SIGN ANUSVARA
                case 0x11340: return Extend;               // Mn       GRANTHA VOWEL SIGN II
                case 0x11357: return Extend;               // Mc       GRANTHA AU LENGTH MARK
                case 0x11445: return Extend;               // Mc       NEWA SIGN VISARGA
                case 0x11446: return Extend;               // Mn       NEWA SIGN NUKTA
                case 0x1145E: return Extend;               // Mn       NEWA SANDHI MARK
                case 0x114B9: return Extend;               // Mc       TIRHUTA VOWEL SIGN E
                case 0x114BA: return Extend;               // Mn       TIRHUTA VOWEL SIGN SHORT E
                case 0x114C1: return Extend;               // Mc       TIRHUTA SIGN VISARGA
                case 0x115BE: return Extend;               // Mc       SIDDHAM SIGN VISARGA
                case 0x1163D: return Extend;               // Mn       MODI SIGN ANUSVARA
                case 0x1163E: return Extend;               // Mc       MODI SIGN VISARGA
                case 0x116AB: return Extend;               // Mn       TAKRI SIGN ANUSVARA
                case 0x116AC: return Extend;               // Mc       TAKRI SIGN VISARGA
                case 0x116AD: return Extend;               // Mn       TAKRI VOWEL SIGN AA
                case 0x116B6: return Extend;               // Mc       TAKRI SIGN VIRAMA
                case 0x116B7: return Extend;               // Mn       TAKRI SIGN NUKTA
                case 0x11726: return Extend;               // Mc       AHOM VOWEL SIGN E
                case 0x11838: return Extend;               // Mc       DOGRA SIGN VISARGA
                case 0x1193D: return Extend;               // Mc       DIVES AKURU SIGN HALANTA
                case 0x1193E: return Extend;               // Mn       DIVES AKURU VIRAMA
                case 0x11940: return Extend;               // Mc       DIVES AKURU MEDIAL YA
                case 0x11942: return Extend;               // Mc       DIVES AKURU MEDIAL RA
                case 0x11943: return Extend;               // Mn       DIVES AKURU SIGN NUKTA
                case 0x119E0: return Extend;               // Mn       NANDINAGARI SIGN VIRAMA
                case 0x119E4: return Extend;               // Mc       NANDINAGARI VOWEL SIGN PRISHTHAMATRA E
                case 0x11A39: return Extend;               // Mc       ZANABAZAR SQUARE SIGN VISARGA
                case 0x11A47: return Extend;               // Mn       ZANABAZAR SQUARE SUBJOINER
                case 0x11A97: return Extend;               // Mc       SOYOMBO SIGN VISARGA
                case 0x11C2F: return Extend;               // Mc       BHAIKSUKI VOWEL SIGN AA
                case 0x11C3E: return Extend;               // Mc       BHAIKSUKI SIGN VISARGA
                case 0x11C3F: return Extend;               // Mn       BHAIKSUKI SIGN VIRAMA
                case 0x11CA9: return Extend;               // Mc       MARCHEN SUBJOINED LETTER YA
                case 0x11CB1: return Extend;               // Mc       MARCHEN VOWEL SIGN I
                case 0x11CB4: return Extend;               // Mc       MARCHEN VOWEL SIGN O
                case 0x11D3A: return Extend;               // Mn       MASARAM GONDI VOWEL SIGN E
                case 0x11D47: return Extend;               // Mn       MASARAM GONDI RA-KARA
                case 0x11D95: return Extend;               // Mn       GUNJALA GONDI SIGN ANUSVARA
                case 0x11D96: return Extend;               // Mc       GUNJALA GONDI SIGN VISARGA
                case 0x11D97: return Extend;               // Mn       GUNJALA GONDI VIRAMA
                case 0x16F4F: return Extend;               // Mn       MIAO SIGN CONSONANT MODIFIER BAR
                case 0x16FE4: return Extend;               // Mn       KHITAN SMALL SCRIPT FILLER
                case 0x1DA75: return Extend;               // Mn       SIGNWRITING UPPER BODY TILTING FROM HIP JOINTS
                case 0x1DA84: return Extend;               // Mn       SIGNWRITING LOCATION HEAD NECK
                case 0x1E2AE: return Extend;               // Mn       TOTO SIGN RISING TONE
                case 0x000AD: return Format;               // Cf       SOFT HYPHEN
                case 0x0061C: return Format;               // Cf       ARABIC LETTER MARK
                case 0x006DD: return Format;               // Cf       ARABIC END OF AYAH
                case 0x0070F: return Format;               // Cf       SYRIAC ABBREVIATION MARK
                case 0x008E2: return Format;               // Cf       ARABIC DISPUTED END OF AYAH
                case 0x0180E: return Format;               // Cf       MONGOLIAN VOWEL SEPARATOR
                case 0x0FEFF: return Format;               // Cf       ZERO WIDTH NO-BREAK SPACE
                case 0x110BD: return Format;               // Cf       KAITHI NUMBER SIGN
                case 0x110CD: return Format;               // Cf       KAITHI NUMBER SIGN ABOVE
                case 0xE0001: return Format;               // Cf       LANGUAGE TAG
                case 0x030A0: return Katakana;             // Pd       KATAKANA-HIRAGANA DOUBLE HYPHEN
                case 0x030FF: return Katakana;             // Lo       KATAKANA DIGRAPH KOTO
                case 0x0FF70: return Katakana;             // Lm       HALFWIDTH KATAKANA-HIRAGANA PROLONGED SOUND MARK
                case 0x1B000: return Katakana;             // Lo       KATAKANA LETTER ARCHAIC E
                case 0x000AA: return ALetter;              // Lo       FEMININE ORDINAL INDICATOR
                case 0x000B5: return ALetter;              // L&       MICRO SIGN
                case 0x000BA: return ALetter;              // Lo       MASCULINE ORDINAL INDICATOR
                case 0x001BB: return ALetter;              // Lo       LATIN LETTER TWO WITH STROKE
                case 0x00294: return ALetter;              // Lo       LATIN LETTER GLOTTAL STOP
                case 0x002EC: return ALetter;              // Lm       MODIFIER LETTER VOICING
                case 0x002ED: return ALetter;              // Sk       MODIFIER LETTER UNASPIRATED
                case 0x002EE: return ALetter;              // Lm       MODIFIER LETTER DOUBLE APOSTROPHE
                case 0x00374: return ALetter;              // Lm       GREEK NUMERAL SIGN
                case 0x0037A: return ALetter;              // Lm       GREEK YPOGEGRAMMENI
                case 0x0037F: return ALetter;              // L&       GREEK CAPITAL LETTER YOT
                case 0x00386: return ALetter;              // L&       GREEK CAPITAL LETTER ALPHA WITH TONOS
                case 0x0038C: return ALetter;              // L&       GREEK CAPITAL LETTER OMICRON WITH TONOS
                case 0x00559: return ALetter;              // Lm       ARMENIAN MODIFIER LETTER LEFT HALF RING
                case 0x0055E: return ALetter;              // Po       ARMENIAN QUESTION MARK
                case 0x0058A: return ALetter;              // Pd       ARMENIAN HYPHEN
                case 0x005F3: return ALetter;              // Po       HEBREW PUNCTUATION GERESH
                case 0x00640: return ALetter;              // Lm       ARABIC TATWEEL
                case 0x006D5: return ALetter;              // Lo       ARABIC LETTER AE
                case 0x006FF: return ALetter;              // Lo       ARABIC LETTER HEH WITH INVERTED V
                case 0x00710: return ALetter;              // Lo       SYRIAC LETTER ALAPH
                case 0x007B1: return ALetter;              // Lo       THAANA LETTER NAA
                case 0x007FA: return ALetter;              // Lm       NKO LAJANYALAN
                case 0x0081A: return ALetter;              // Lm       SAMARITAN MODIFIER LETTER EPENTHETIC YUT
                case 0x00824: return ALetter;              // Lm       SAMARITAN MODIFIER LETTER SHORT A
                case 0x00828: return ALetter;              // Lm       SAMARITAN MODIFIER LETTER I
                case 0x008C9: return ALetter;              // Lm       ARABIC SMALL FARSI YEH
                case 0x0093D: return ALetter;              // Lo       DEVANAGARI SIGN AVAGRAHA
                case 0x00950: return ALetter;              // Lo       DEVANAGARI OM
                case 0x00971: return ALetter;              // Lm       DEVANAGARI SIGN HIGH SPACING DOT
                case 0x009B2: return ALetter;              // Lo       BENGALI LETTER LA
                case 0x009BD: return ALetter;              // Lo       BENGALI SIGN AVAGRAHA
                case 0x009CE: return ALetter;              // Lo       BENGALI LETTER KHANDA TA
                case 0x009FC: return ALetter;              // Lo       BENGALI LETTER VEDIC ANUSVARA
                case 0x00A5E: return ALetter;              // Lo       GURMUKHI LETTER FA
                case 0x00ABD: return ALetter;              // Lo       GUJARATI SIGN AVAGRAHA
                case 0x00AD0: return ALetter;              // Lo       GUJARATI OM
                case 0x00AF9: return ALetter;              // Lo       GUJARATI LETTER ZHA
                case 0x00B3D: return ALetter;              // Lo       ORIYA SIGN AVAGRAHA
                case 0x00B71: return ALetter;              // Lo       ORIYA LETTER WA
                case 0x00B83: return ALetter;              // Lo       TAMIL SIGN VISARGA
                case 0x00B9C: return ALetter;              // Lo       TAMIL LETTER JA
                case 0x00BD0: return ALetter;              // Lo       TAMIL OM
                case 0x00C3D: return ALetter;              // Lo       TELUGU SIGN AVAGRAHA
                case 0x00C5D: return ALetter;              // Lo       TELUGU LETTER NAKAARA POLLU
                case 0x00C80: return ALetter;              // Lo       KANNADA SIGN SPACING CANDRABINDU
                case 0x00CBD: return ALetter;              // Lo       KANNADA SIGN AVAGRAHA
                case 0x00D3D: return ALetter;              // Lo       MALAYALAM SIGN AVAGRAHA
                case 0x00D4E: return ALetter;              // Lo       MALAYALAM LETTER DOT REPH
                case 0x00DBD: return ALetter;              // Lo       SINHALA LETTER DANTAJA LAYANNA
                case 0x00F00: return ALetter;              // Lo       TIBETAN SYLLABLE OM
                case 0x010C7: return ALetter;              // L&       GEORGIAN CAPITAL LETTER YN
                case 0x010CD: return ALetter;              // L&       GEORGIAN CAPITAL LETTER AEN
                case 0x010FC: return ALetter;              // Lm       MODIFIER LETTER GEORGIAN NAR
                case 0x01258: return ALetter;              // Lo       ETHIOPIC SYLLABLE QHWA
                case 0x012C0: return ALetter;              // Lo       ETHIOPIC SYLLABLE KXWA
                case 0x01843: return ALetter;              // Lm       MONGOLIAN LETTER TODO LONG VOWEL SIGN
                case 0x018AA: return ALetter;              // Lo       MONGOLIAN LETTER MANCHU ALI GALI LHA
                case 0x01CFA: return ALetter;              // Lo       VEDIC SIGN DOUBLE ANUSVARA ANTARGOMUKHA
                case 0x01D78: return ALetter;              // Lm       MODIFIER LETTER CYRILLIC EN
                case 0x01F59: return ALetter;              // L&       GREEK CAPITAL LETTER UPSILON WITH DASIA
                case 0x01F5B: return ALetter;              // L&       GREEK CAPITAL LETTER UPSILON WITH DASIA AND VARIA
                case 0x01F5D: return ALetter;              // L&       GREEK CAPITAL LETTER UPSILON WITH DASIA AND OXIA
                case 0x01FBE: return ALetter;              // L&       GREEK PROSGEGRAMMENI
                case 0x02071: return ALetter;              // Lm       SUPERSCRIPT LATIN SMALL LETTER I
                case 0x0207F: return ALetter;              // Lm       SUPERSCRIPT LATIN SMALL LETTER N
                case 0x02102: return ALetter;              // L&       DOUBLE-STRUCK CAPITAL C
                case 0x02107: return ALetter;              // L&       EULER CONSTANT
                case 0x02115: return ALetter;              // L&       DOUBLE-STRUCK CAPITAL N
                case 0x02124: return ALetter;              // L&       DOUBLE-STRUCK CAPITAL Z
                case 0x02126: return ALetter;              // L&       OHM SIGN
                case 0x02128: return ALetter;              // L&       BLACK-LETTER CAPITAL Z
                case 0x02139: return ALetter;              // L&       INFORMATION SOURCE
                case 0x0214E: return ALetter;              // L&       TURNED SMALL F
                case 0x02D27: return ALetter;              // L&       GEORGIAN SMALL LETTER YN
                case 0x02D2D: return ALetter;              // L&       GEORGIAN SMALL LETTER AEN
                case 0x02D6F: return ALetter;              // Lm       TIFINAGH MODIFIER LETTER LABIALIZATION MARK
                case 0x02E2F: return ALetter;              // Lm       VERTICAL TILDE
                case 0x03005: return ALetter;              // Lm       IDEOGRAPHIC ITERATION MARK
                case 0x0303B: return ALetter;              // Lm       VERTICAL IDEOGRAPHIC ITERATION MARK
                case 0x0303C: return ALetter;              // Lo       MASU MARK
                case 0x0A015: return ALetter;              // Lm       YI SYLLABLE WU
                case 0x0A60C: return ALetter;              // Lm       VAI SYLLABLE LENGTHENER
                case 0x0A66E: return ALetter;              // Lo       CYRILLIC LETTER MULTIOCULAR O
                case 0x0A67F: return ALetter;              // Lm       CYRILLIC PAYEROK
                case 0x0A770: return ALetter;              // Lm       MODIFIER LETTER US
                case 0x0A788: return ALetter;              // Lm       MODIFIER LETTER LOW CIRCUMFLEX ACCENT
                case 0x0A78F: return ALetter;              // Lo       LATIN LETTER SINOLOGICAL DOT
                case 0x0A7D3: return ALetter;              // L&       LATIN SMALL LETTER DOUBLE THORN
                case 0x0A7F7: return ALetter;              // Lo       LATIN EPIGRAPHIC LETTER SIDEWAYS I
                case 0x0A7FA: return ALetter;              // L&       LATIN LETTER SMALL CAPITAL TURNED M
                case 0x0A8FB: return ALetter;              // Lo       DEVANAGARI HEADSTROKE
                case 0x0A9CF: return ALetter;              // Lm       JAVANESE PANGRANGKEP
                case 0x0AAF2: return ALetter;              // Lo       MEETEI MAYEK ANJI
                case 0x0AB5B: return ALetter;              // Sk       MODIFIER BREVE WITH INVERTED BREVE
                case 0x0AB69: return ALetter;              // Lm       MODIFIER LETTER SMALL TURNED W
                case 0x10341: return ALetter;              // Nl       GOTHIC LETTER NINETY
                case 0x1034A: return ALetter;              // Nl       GOTHIC LETTER NINE HUNDRED
                case 0x10808: return ALetter;              // Lo       CYPRIOT SYLLABLE JO
                case 0x1083C: return ALetter;              // Lo       CYPRIOT SYLLABLE ZA
                case 0x10A00: return ALetter;              // Lo       KHAROSHTHI LETTER A
                case 0x10F27: return ALetter;              // Lo       OLD SOGDIAN LIGATURE AYIN-DALETH
                case 0x11075: return ALetter;              // Lo       BRAHMI LETTER OLD TAMIL LLA
                case 0x11144: return ALetter;              // Lo       CHAKMA LETTER LHAA
                case 0x11147: return ALetter;              // Lo       CHAKMA LETTER VAA
                case 0x11176: return ALetter;              // Lo       MAHAJANI LIGATURE SHRI
                case 0x111DA: return ALetter;              // Lo       SHARADA EKAM
                case 0x111DC: return ALetter;              // Lo       SHARADA HEADSTROKE
                case 0x11288: return ALetter;              // Lo       MULTANI LETTER GHA
                case 0x1133D: return ALetter;              // Lo       GRANTHA SIGN AVAGRAHA
                case 0x11350: return ALetter;              // Lo       GRANTHA OM
                case 0x114C7: return ALetter;              // Lo       TIRHUTA OM
                case 0x11644: return ALetter;              // Lo       MODI SIGN HUVA
                case 0x116B8: return ALetter;              // Lo       TAKRI LETTER ARCHAIC KHA
                case 0x11909: return ALetter;              // Lo       DIVES AKURU LETTER O
                case 0x1193F: return ALetter;              // Lo       DIVES AKURU PREFIXED NASAL SIGN
                case 0x11941: return ALetter;              // Lo       DIVES AKURU INITIAL RA
                case 0x119E1: return ALetter;              // Lo       NANDINAGARI SIGN AVAGRAHA
                case 0x119E3: return ALetter;              // Lo       NANDINAGARI HEADSTROKE
                case 0x11A00: return ALetter;              // Lo       ZANABAZAR SQUARE LETTER A
                case 0x11A3A: return ALetter;              // Lo       ZANABAZAR SQUARE CLUSTER-INITIAL LETTER RA
                case 0x11A50: return ALetter;              // Lo       SOYOMBO LETTER A
                case 0x11A9D: return ALetter;              // Lo       SOYOMBO MARK PLUTA
                case 0x11C40: return ALetter;              // Lo       BHAIKSUKI SIGN AVAGRAHA
                case 0x11D46: return ALetter;              // Lo       MASARAM GONDI REPHA
                case 0x11D98: return ALetter;              // Lo       GUNJALA GONDI OM
                case 0x11FB0: return ALetter;              // Lo       LISU LETTER YHA
                case 0x16F50: return ALetter;              // Lo       MIAO LETTER NASALIZATION
                case 0x16FE3: return ALetter;              // Lm       OLD CHINESE ITERATION MARK
                case 0x1D4A2: return ALetter;              // L&       MATHEMATICAL SCRIPT CAPITAL G
                case 0x1D4BB: return ALetter;              // L&       MATHEMATICAL SCRIPT SMALL F
                case 0x1D546: return ALetter;              // L&       MATHEMATICAL DOUBLE-STRUCK CAPITAL O
                case 0x1DF0A: return ALetter;              // Lo       LATIN LETTER RETROFLEX CLICK WITH RETROFLEX HOOK
                case 0x1E14E: return ALetter;              // Lo       NYIAKENG PUACHUE HMONG LOGOGRAM NYAJ
                case 0x1E94B: return ALetter;              // Lm       ADLAM NASALIZATION MARK
                case 0x1EE24: return ALetter;              // Lo       ARABIC MATHEMATICAL INITIAL HEH
                case 0x1EE27: return ALetter;              // Lo       ARABIC MATHEMATICAL INITIAL HAH
                case 0x1EE39: return ALetter;              // Lo       ARABIC MATHEMATICAL INITIAL DAD
                case 0x1EE3B: return ALetter;              // Lo       ARABIC MATHEMATICAL INITIAL GHAIN
                case 0x1EE42: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED JEEM
                case 0x1EE47: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED HAH
                case 0x1EE49: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED YEH
                case 0x1EE4B: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED LAM
                case 0x1EE54: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED SHEEN
                case 0x1EE57: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED KHAH
                case 0x1EE59: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED DAD
                case 0x1EE5B: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED GHAIN
                case 0x1EE5D: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED DOTLESS NOON
                case 0x1EE5F: return ALetter;              // Lo       ARABIC MATHEMATICAL TAILED DOTLESS QAF
                case 0x1EE64: return ALetter;              // Lo       ARABIC MATHEMATICAL STRETCHED HEH
                case 0x1EE7E: return ALetter;              // Lo       ARABIC MATHEMATICAL STRETCHED DOTLESS FEH
                case 0x0003A: return MidLetter;            // Po       COLON
                case 0x000B7: return MidLetter;            // Po       MIDDLE DOT
                case 0x00387: return MidLetter;            // Po       GREEK ANO TELEIA
                case 0x0055F: return MidLetter;            // Po       ARMENIAN ABBREVIATION MARK
                case 0x005F4: return MidLetter;            // Po       HEBREW PUNCTUATION GERSHAYIM
                case 0x02027: return MidLetter;            // Po       HYPHENATION POINT
                case 0x0FE13: return MidLetter;            // Po       PRESENTATION FORM FOR VERTICAL COLON
                case 0x0FE55: return MidLetter;            // Po       SMALL COLON
                case 0x0FF1A: return MidLetter;            // Po       FULLWIDTH COLON
                case 0x0002C: return MidNum;               // Po       COMMA
                case 0x0003B: return MidNum;               // Po       SEMICOLON
                case 0x0037E: return MidNum;               // Po       GREEK QUESTION MARK
                case 0x00589: return MidNum;               // Po       ARMENIAN FULL STOP
                case 0x0066C: return MidNum;               // Po       ARABIC THOUSANDS SEPARATOR
                case 0x007F8: return MidNum;               // Po       NKO COMMA
                case 0x02044: return MidNum;               // Sm       FRACTION SLASH
                case 0x0FE10: return MidNum;               // Po       PRESENTATION FORM FOR VERTICAL COMMA
                case 0x0FE14: return MidNum;               // Po       PRESENTATION FORM FOR VERTICAL SEMICOLON
                case 0x0FE50: return MidNum;               // Po       SMALL COMMA
                case 0x0FE54: return MidNum;               // Po       SMALL SEMICOLON
                case 0x0FF0C: return MidNum;               // Po       FULLWIDTH COMMA
                case 0x0FF1B: return MidNum;               // Po       FULLWIDTH SEMICOLON
                case 0x0002E: return MidNumLet;            // Po       FULL STOP
                case 0x02018: return MidNumLet;            // Pi       LEFT SINGLE QUOTATION MARK
                case 0x02019: return MidNumLet;            // Pf       RIGHT SINGLE QUOTATION MARK
                case 0x02024: return MidNumLet;            // Po       ONE DOT LEADER
                case 0x0FE52: return MidNumLet;            // Po       SMALL FULL STOP
                case 0x0FF07: return MidNumLet;            // Po       FULLWIDTH APOSTROPHE
                case 0x0FF0E: return MidNumLet;            // Po       FULLWIDTH FULL STOP
                case 0x0066B: return Numeric;              // Po       ARABIC DECIMAL SEPARATOR
                case 0x0005F: return ExtendNumLet;         // Pc       LOW LINE
                case 0x0202F: return ExtendNumLet;         // Zs       NARROW NO-BREAK SPACE
                case 0x02054: return ExtendNumLet;         // Pc       INVERTED UNDERTIE
                case 0x0FF3F: return ExtendNumLet;         // Pc       FULLWIDTH LOW LINE
                case 0x0200D: return ZWJ;                  // Cf       ZERO WIDTH JOINER
                case 0x00020: return WSegSpace;            // Zs       SPACE
                case 0x01680: return WSegSpace;            // Zs       OGHAM SPACE MARK
                case 0x0205F: return WSegSpace;            // Zs       MEDIUM MATHEMATICAL SPACE
                case 0x03000: return WSegSpace;            // Zs       IDEOGRAPHIC SPACE
                case 0x000A9: return Extended_Pictographic; // E0.6   [1] (©️)       copyright
                case 0x000AE: return Extended_Pictographic; // E0.6   [1] (®️)       registered
                case 0x0203C: return Extended_Pictographic; // E0.6   [1] (‼️)       double exclamation mark
                case 0x02049: return Extended_Pictographic; // E0.6   [1] (⁉️)       exclamation question mark
                case 0x02122: return Extended_Pictographic; // E0.6   [1] (™️)       trade mark
                //case 0x02139: return Extended_Pictographic; // E0.6   [1] (ℹ️)       information
                case 0x02328: return Extended_Pictographic; // E1.0   [1] (⌨️)       keyboard
                case 0x02388: return Extended_Pictographic; // E0.0   [1] (⎈)       HELM SYMBOL
                case 0x023CF: return Extended_Pictographic; // E1.0   [1] (⏏️)       eject button
                case 0x023EF: return Extended_Pictographic; // E1.0   [1] (⏯️)       play or pause button
                case 0x023F0: return Extended_Pictographic; // E0.6   [1] (⏰)       alarm clock
                case 0x023F3: return Extended_Pictographic; // E0.6   [1] (⏳)       hourglass not done
                case 0x024C2: return Extended_Pictographic; // E0.6   [1] (Ⓜ️)       circled M
                case 0x025B6: return Extended_Pictographic; // E0.6   [1] (▶️)       play button
                case 0x025C0: return Extended_Pictographic; // E0.6   [1] (◀️)       reverse button
                case 0x02604: return Extended_Pictographic; // E1.0   [1] (☄️)       comet
                case 0x02605: return Extended_Pictographic; // E0.0   [1] (★)       BLACK STAR
                case 0x0260E: return Extended_Pictographic; // E0.6   [1] (☎️)       telephone
                case 0x02611: return Extended_Pictographic; // E0.6   [1] (☑️)       check box with check
                case 0x02612: return Extended_Pictographic; // E0.0   [1] (☒)       BALLOT BOX WITH X
                case 0x02618: return Extended_Pictographic; // E1.0   [1] (☘️)       shamrock
                case 0x0261D: return Extended_Pictographic; // E0.6   [1] (☝️)       index pointing up
                case 0x02620: return Extended_Pictographic; // E1.0   [1] (☠️)       skull and crossbones
                case 0x02621: return Extended_Pictographic; // E0.0   [1] (☡)       CAUTION SIGN
                case 0x02626: return Extended_Pictographic; // E1.0   [1] (☦️)       orthodox cross
                case 0x0262A: return Extended_Pictographic; // E0.7   [1] (☪️)       star and crescent
                case 0x0262E: return Extended_Pictographic; // E1.0   [1] (☮️)       peace symbol
                case 0x0262F: return Extended_Pictographic; // E0.7   [1] (☯️)       yin yang
                case 0x0263A: return Extended_Pictographic; // E0.6   [1] (☺️)       smiling face
                case 0x02640: return Extended_Pictographic; // E4.0   [1] (♀️)       female sign
                case 0x02641: return Extended_Pictographic; // E0.0   [1] (♁)       EARTH
                case 0x02642: return Extended_Pictographic; // E4.0   [1] (♂️)       male sign
                case 0x0265F: return Extended_Pictographic; // E11.0  [1] (♟️)       chess pawn
                case 0x02660: return Extended_Pictographic; // E0.6   [1] (♠️)       spade suit
                case 0x02663: return Extended_Pictographic; // E0.6   [1] (♣️)       club suit
                case 0x02664: return Extended_Pictographic; // E0.0   [1] (♤)       WHITE SPADE SUIT
                case 0x02667: return Extended_Pictographic; // E0.0   [1] (♧)       WHITE CLUB SUIT
                case 0x02668: return Extended_Pictographic; // E0.6   [1] (♨️)       hot springs
                case 0x0267B: return Extended_Pictographic; // E0.6   [1] (♻️)       recycling symbol
                case 0x0267E: return Extended_Pictographic; // E11.0  [1] (♾️)       infinity
                case 0x0267F: return Extended_Pictographic; // E0.6   [1] (♿)       wheelchair symbol
                case 0x02692: return Extended_Pictographic; // E1.0   [1] (⚒️)       hammer and pick
                case 0x02693: return Extended_Pictographic; // E0.6   [1] (⚓)       anchor
                case 0x02694: return Extended_Pictographic; // E1.0   [1] (⚔️)       crossed swords
                case 0x02695: return Extended_Pictographic; // E4.0   [1] (⚕️)       medical symbol
                case 0x02698: return Extended_Pictographic; // E0.0   [1] (⚘)       FLOWER
                case 0x02699: return Extended_Pictographic; // E1.0   [1] (⚙️)       gear
                case 0x0269A: return Extended_Pictographic; // E0.0   [1] (⚚)       STAFF OF HERMES
                case 0x026A7: return Extended_Pictographic; // E13.0  [1] (⚧️)       transgender symbol
                case 0x026C8: return Extended_Pictographic; // E0.7   [1] (⛈️)       cloud with lightning and rain
                case 0x026CE: return Extended_Pictographic; // E0.6   [1] (⛎)       Ophiuchus
                case 0x026CF: return Extended_Pictographic; // E0.7   [1] (⛏️)       pick
                case 0x026D0: return Extended_Pictographic; // E0.0   [1] (⛐)       CAR SLIDING
                case 0x026D1: return Extended_Pictographic; // E0.7   [1] (⛑️)       rescue worker’s helmet
                case 0x026D2: return Extended_Pictographic; // E0.0   [1] (⛒)       CIRCLED CROSSING LANES
                case 0x026D3: return Extended_Pictographic; // E0.7   [1] (⛓️)       chains
                case 0x026D4: return Extended_Pictographic; // E0.6   [1] (⛔)       no entry
                case 0x026E9: return Extended_Pictographic; // E0.7   [1] (⛩️)       shinto shrine
                case 0x026EA: return Extended_Pictographic; // E0.6   [1] (⛪)       church
                case 0x026F4: return Extended_Pictographic; // E0.7   [1] (⛴️)       ferry
                case 0x026F5: return Extended_Pictographic; // E0.6   [1] (⛵)       sailboat
                case 0x026F6: return Extended_Pictographic; // E0.0   [1] (⛶)       SQUARE FOUR CORNERS
                case 0x026FA: return Extended_Pictographic; // E0.6   [1] (⛺)       tent
                case 0x026FD: return Extended_Pictographic; // E0.6   [1] (⛽)       fuel pump
                case 0x02702: return Extended_Pictographic; // E0.6   [1] (✂️)       scissors
                case 0x02705: return Extended_Pictographic; // E0.6   [1] (✅)       check mark button
                case 0x0270D: return Extended_Pictographic; // E0.7   [1] (✍️)       writing hand
                case 0x0270E: return Extended_Pictographic; // E0.0   [1] (✎)       LOWER RIGHT PENCIL
                case 0x0270F: return Extended_Pictographic; // E0.6   [1] (✏️)       pencil
                case 0x02712: return Extended_Pictographic; // E0.6   [1] (✒️)       black nib
                case 0x02714: return Extended_Pictographic; // E0.6   [1] (✔️)       check mark
                case 0x02716: return Extended_Pictographic; // E0.6   [1] (✖️)       multiply
                case 0x0271D: return Extended_Pictographic; // E0.7   [1] (✝️)       latin cross
                case 0x02721: return Extended_Pictographic; // E0.7   [1] (✡️)       star of David
                case 0x02728: return Extended_Pictographic; // E0.6   [1] (✨)       sparkles
                case 0x02744: return Extended_Pictographic; // E0.6   [1] (❄️)       snowflake
                case 0x02747: return Extended_Pictographic; // E0.6   [1] (❇️)       sparkle
                case 0x0274C: return Extended_Pictographic; // E0.6   [1] (❌)       cross mark
                case 0x0274E: return Extended_Pictographic; // E0.6   [1] (❎)       cross mark button
                case 0x02757: return Extended_Pictographic; // E0.6   [1] (❗)       red exclamation mark
                case 0x02763: return Extended_Pictographic; // E1.0   [1] (❣️)       heart exclamation
                case 0x02764: return Extended_Pictographic; // E0.6   [1] (❤️)       red heart
                case 0x027A1: return Extended_Pictographic; // E0.6   [1] (➡️)       right arrow
                case 0x027B0: return Extended_Pictographic; // E0.6   [1] (➰)       curly loop
                case 0x027BF: return Extended_Pictographic; // E1.0   [1] (➿)       double curly loop
                case 0x02B50: return Extended_Pictographic; // E0.6   [1] (⭐)       star
                case 0x02B55: return Extended_Pictographic; // E0.6   [1] (⭕)       hollow red circle
                case 0x03030: return Extended_Pictographic; // E0.6   [1] (〰️)       wavy dash
                case 0x0303D: return Extended_Pictographic; // E0.6   [1] (〽️)       part alternation mark
                case 0x03297: return Extended_Pictographic; // E0.6   [1] (㊗️)       Japanese “congratulations” button
                case 0x03299: return Extended_Pictographic; // E0.6   [1] (㊙️)       Japanese “secret” button
                case 0x1F004: return Extended_Pictographic; // E0.6   [1] (🀄)       mahjong red dragon
                case 0x1F0CF: return Extended_Pictographic; // E0.6   [1] (🃏)       joker
                case 0x1F12F: return Extended_Pictographic; // E0.0   [1] (🄯)       COPYLEFT SYMBOL
                case 0x1F18E: return Extended_Pictographic; // E0.6   [1] (🆎)       AB button (blood type)
                case 0x1F21A: return Extended_Pictographic; // E0.6   [1] (🈚)       Japanese “free of charge” button
                case 0x1F22F: return Extended_Pictographic; // E0.6   [1] (🈯)       Japanese “reserved” button
                case 0x1F30F: return Extended_Pictographic; // E0.6   [1] (🌏)       globe showing Asia-Australia
                case 0x1F310: return Extended_Pictographic; // E1.0   [1] (🌐)       globe with meridians
                case 0x1F311: return Extended_Pictographic; // E0.6   [1] (🌑)       new moon
                case 0x1F312: return Extended_Pictographic; // E1.0   [1] (🌒)       waxing crescent moon
                case 0x1F319: return Extended_Pictographic; // E0.6   [1] (🌙)       crescent moon
                case 0x1F31A: return Extended_Pictographic; // E1.0   [1] (🌚)       new moon face
                case 0x1F31B: return Extended_Pictographic; // E0.6   [1] (🌛)       first quarter moon face
                case 0x1F31C: return Extended_Pictographic; // E0.7   [1] (🌜)       last quarter moon face
                case 0x1F321: return Extended_Pictographic; // E0.7   [1] (🌡️)       thermometer
                case 0x1F336: return Extended_Pictographic; // E0.7   [1] (🌶️)       hot pepper
                case 0x1F34B: return Extended_Pictographic; // E1.0   [1] (🍋)       lemon
                case 0x1F350: return Extended_Pictographic; // E1.0   [1] (🍐)       pear
                case 0x1F37C: return Extended_Pictographic; // E1.0   [1] (🍼)       baby bottle
                case 0x1F37D: return Extended_Pictographic; // E0.7   [1] (🍽️)       fork and knife with plate
                case 0x1F398: return Extended_Pictographic; // E0.0   [1] (🎘)       MUSICAL KEYBOARD WITH JACKS
                case 0x1F3C5: return Extended_Pictographic; // E1.0   [1] (🏅)       sports medal
                case 0x1F3C6: return Extended_Pictographic; // E0.6   [1] (🏆)       trophy
                case 0x1F3C7: return Extended_Pictographic; // E1.0   [1] (🏇)       horse racing
                case 0x1F3C8: return Extended_Pictographic; // E0.6   [1] (🏈)       american football
                case 0x1F3C9: return Extended_Pictographic; // E1.0   [1] (🏉)       rugby football
                case 0x1F3CA: return Extended_Pictographic; // E0.6   [1] (🏊)       person swimming
                case 0x1F3E4: return Extended_Pictographic; // E1.0   [1] (🏤)       post office
                case 0x1F3F3: return Extended_Pictographic; // E0.7   [1] (🏳️)       white flag
                case 0x1F3F4: return Extended_Pictographic; // E1.0   [1] (🏴)       black flag
                case 0x1F3F5: return Extended_Pictographic; // E0.7   [1] (🏵️)       rosette
                case 0x1F3F6: return Extended_Pictographic; // E0.0   [1] (🏶)       BLACK ROSETTE
                case 0x1F3F7: return Extended_Pictographic; // E0.7   [1] (🏷️)       label
                case 0x1F408: return Extended_Pictographic; // E0.7   [1] (🐈)       cat
                case 0x1F413: return Extended_Pictographic; // E1.0   [1] (🐓)       rooster
                case 0x1F414: return Extended_Pictographic; // E0.6   [1] (🐔)       chicken
                case 0x1F415: return Extended_Pictographic; // E0.7   [1] (🐕)       dog
                case 0x1F416: return Extended_Pictographic; // E1.0   [1] (🐖)       pig
                case 0x1F42A: return Extended_Pictographic; // E1.0   [1] (🐪)       camel
                case 0x1F43F: return Extended_Pictographic; // E0.7   [1] (🐿️)       chipmunk
                case 0x1F440: return Extended_Pictographic; // E0.6   [1] (👀)       eyes
                case 0x1F441: return Extended_Pictographic; // E0.7   [1] (👁️)       eye
                case 0x1F465: return Extended_Pictographic; // E1.0   [1] (👥)       busts in silhouette
                case 0x1F4AD: return Extended_Pictographic; // E1.0   [1] (💭)       thought balloon
                case 0x1F4EE: return Extended_Pictographic; // E0.6   [1] (📮)       postbox
                case 0x1F4EF: return Extended_Pictographic; // E1.0   [1] (📯)       postal horn
                case 0x1F4F5: return Extended_Pictographic; // E1.0   [1] (📵)       no mobile phones
                case 0x1F4F8: return Extended_Pictographic; // E1.0   [1] (📸)       camera with flash
                case 0x1F4FD: return Extended_Pictographic; // E0.7   [1] (📽️)       film projector
                case 0x1F4FE: return Extended_Pictographic; // E0.0   [1] (📾)       PORTABLE STEREO
                case 0x1F503: return Extended_Pictographic; // E0.6   [1] (🔃)       clockwise vertical arrows
                case 0x1F508: return Extended_Pictographic; // E0.7   [1] (🔈)       speaker low volume
                case 0x1F509: return Extended_Pictographic; // E1.0   [1] (🔉)       speaker medium volume
                case 0x1F515: return Extended_Pictographic; // E1.0   [1] (🔕)       bell with slash
                case 0x1F54F: return Extended_Pictographic; // E0.0   [1] (🕏)       BOWL OF HYGIEIA
                case 0x1F57A: return Extended_Pictographic; // E3.0   [1] (🕺)       man dancing
                case 0x1F587: return Extended_Pictographic; // E0.7   [1] (🖇️)       linked paperclips
                case 0x1F590: return Extended_Pictographic; // E0.7   [1] (🖐️)       hand with fingers splayed
                case 0x1F5A4: return Extended_Pictographic; // E3.0   [1] (🖤)       black heart
                case 0x1F5A5: return Extended_Pictographic; // E0.7   [1] (🖥️)       desktop computer
                case 0x1F5A8: return Extended_Pictographic; // E0.7   [1] (🖨️)       printer
                case 0x1F5BC: return Extended_Pictographic; // E0.7   [1] (🖼️)       framed picture
                case 0x1F5E1: return Extended_Pictographic; // E0.7   [1] (🗡️)       dagger
                case 0x1F5E2: return Extended_Pictographic; // E0.0   [1] (🗢)       LIPS
                case 0x1F5E3: return Extended_Pictographic; // E0.7   [1] (🗣️)       speaking head
                case 0x1F5E8: return Extended_Pictographic; // E2.0   [1] (🗨️)       left speech bubble
                case 0x1F5EF: return Extended_Pictographic; // E0.7   [1] (🗯️)       right anger bubble
                case 0x1F5F3: return Extended_Pictographic; // E0.7   [1] (🗳️)       ballot box with ballot
                case 0x1F5FA: return Extended_Pictographic; // E0.7   [1] (🗺️)       world map
                case 0x1F600: return Extended_Pictographic; // E1.0   [1] (😀)       grinning face
                case 0x1F60E: return Extended_Pictographic; // E1.0   [1] (😎)       smiling face with sunglasses
                case 0x1F60F: return Extended_Pictographic; // E0.6   [1] (😏)       smirking face
                case 0x1F610: return Extended_Pictographic; // E0.7   [1] (😐)       neutral face
                case 0x1F611: return Extended_Pictographic; // E1.0   [1] (😑)       expressionless face
                case 0x1F615: return Extended_Pictographic; // E1.0   [1] (😕)       confused face
                case 0x1F616: return Extended_Pictographic; // E0.6   [1] (😖)       confounded face
                case 0x1F617: return Extended_Pictographic; // E1.0   [1] (😗)       kissing face
                case 0x1F618: return Extended_Pictographic; // E0.6   [1] (😘)       face blowing a kiss
                case 0x1F619: return Extended_Pictographic; // E1.0   [1] (😙)       kissing face with smiling eyes
                case 0x1F61A: return Extended_Pictographic; // E0.6   [1] (😚)       kissing face with closed eyes
                case 0x1F61B: return Extended_Pictographic; // E1.0   [1] (😛)       face with tongue
                case 0x1F61F: return Extended_Pictographic; // E1.0   [1] (😟)       worried face
                case 0x1F62C: return Extended_Pictographic; // E1.0   [1] (😬)       grimacing face
                case 0x1F62D: return Extended_Pictographic; // E0.6   [1] (😭)       loudly crying face
                case 0x1F634: return Extended_Pictographic; // E1.0   [1] (😴)       sleeping face
                case 0x1F635: return Extended_Pictographic; // E0.6   [1] (😵)       face with crossed-out eyes
                case 0x1F636: return Extended_Pictographic; // E1.0   [1] (😶)       face without mouth
                case 0x1F680: return Extended_Pictographic; // E0.6   [1] (🚀)       rocket
                case 0x1F686: return Extended_Pictographic; // E1.0   [1] (🚆)       train
                case 0x1F687: return Extended_Pictographic; // E0.6   [1] (🚇)       metro
                case 0x1F688: return Extended_Pictographic; // E1.0   [1] (🚈)       light rail
                case 0x1F689: return Extended_Pictographic; // E0.6   [1] (🚉)       station
                case 0x1F68C: return Extended_Pictographic; // E0.6   [1] (🚌)       bus
                case 0x1F68D: return Extended_Pictographic; // E0.7   [1] (🚍)       oncoming bus
                case 0x1F68E: return Extended_Pictographic; // E1.0   [1] (🚎)       trolleybus
                case 0x1F68F: return Extended_Pictographic; // E0.6   [1] (🚏)       bus stop
                case 0x1F690: return Extended_Pictographic; // E1.0   [1] (🚐)       minibus
                case 0x1F694: return Extended_Pictographic; // E0.7   [1] (🚔)       oncoming police car
                case 0x1F695: return Extended_Pictographic; // E0.6   [1] (🚕)       taxi
                case 0x1F696: return Extended_Pictographic; // E1.0   [1] (🚖)       oncoming taxi
                case 0x1F697: return Extended_Pictographic; // E0.6   [1] (🚗)       automobile
                case 0x1F698: return Extended_Pictographic; // E0.7   [1] (🚘)       oncoming automobile
                case 0x1F6A2: return Extended_Pictographic; // E0.6   [1] (🚢)       ship
                case 0x1F6A3: return Extended_Pictographic; // E1.0   [1] (🚣)       person rowing boat
                case 0x1F6A6: return Extended_Pictographic; // E1.0   [1] (🚦)       vertical traffic light
                case 0x1F6B2: return Extended_Pictographic; // E0.6   [1] (🚲)       bicycle
                case 0x1F6B6: return Extended_Pictographic; // E0.6   [1] (🚶)       person walking
                case 0x1F6BF: return Extended_Pictographic; // E1.0   [1] (🚿)       shower
                case 0x1F6C0: return Extended_Pictographic; // E0.6   [1] (🛀)       person taking bath
                case 0x1F6CB: return Extended_Pictographic; // E0.7   [1] (🛋️)       couch and lamp
                case 0x1F6CC: return Extended_Pictographic; // E1.0   [1] (🛌)       person in bed
                case 0x1F6D0: return Extended_Pictographic; // E1.0   [1] (🛐)       place of worship
                case 0x1F6D5: return Extended_Pictographic; // E12.0  [1] (🛕)       hindu temple
                case 0x1F6E9: return Extended_Pictographic; // E0.7   [1] (🛩️)       small airplane
                case 0x1F6EA: return Extended_Pictographic; // E0.0   [1] (🛪)       NORTHEAST-POINTING AIRPLANE
                case 0x1F6F0: return Extended_Pictographic; // E0.7   [1] (🛰️)       satellite
                case 0x1F6F3: return Extended_Pictographic; // E0.7   [1] (🛳️)       passenger ship
                case 0x1F6F9: return Extended_Pictographic; // E11.0  [1] (🛹)       skateboard
                case 0x1F6FA: return Extended_Pictographic; // E12.0  [1] (🛺)       auto rickshaw
                case 0x1F7F0: return Extended_Pictographic; // E14.0  [1] (🟰)       heavy equals sign
                case 0x1F90C: return Extended_Pictographic; // E13.0  [1] (🤌)       pinched fingers
                case 0x1F91F: return Extended_Pictographic; // E5.0   [1] (🤟)       love-you gesture
                case 0x1F930: return Extended_Pictographic; // E3.0   [1] (🤰)       pregnant woman
                case 0x1F93F: return Extended_Pictographic; // E12.0  [1] (🤿)       diving mask
                case 0x1F94C: return Extended_Pictographic; // E5.0   [1] (🥌)       curling stone
                case 0x1F971: return Extended_Pictographic; // E12.0  [1] (🥱)       yawning face
                case 0x1F972: return Extended_Pictographic; // E13.0  [1] (🥲)       smiling face with tear
                case 0x1F979: return Extended_Pictographic; // E14.0  [1] (🥹)       face holding back tears
                case 0x1F97A: return Extended_Pictographic; // E11.0  [1] (🥺)       pleading face
                case 0x1F97B: return Extended_Pictographic; // E12.0  [1] (🥻)       sari
                case 0x1F9C0: return Extended_Pictographic; // E1.0   [1] (🧀)       cheese wedge
                case 0x1F9CB: return Extended_Pictographic; // E13.0  [1] (🧋)       bubble tea
                case 0x1F9CC: return Extended_Pictographic; // E14.0  [1] (🧌)       troll
                case 0x1FA74: return Extended_Pictographic; // E13.0  [1] (🩴)       thong sandal
            }
            if (m_dic_cache_break_type != null) {
                if (m_dic_cache_break_type.ContainsKey(nCodePoint)) {
                    return m_dic_cache_break_type[nCodePoint];
                } else {
                    return Other;
                }
            }
            if (m_arr_cache_break_type != null) {
                if (nCodePoint >= m_arr_cache_break_type.Length) {
                    return Other;
                } else {
                    return m_arr_cache_break_type[nCodePoint];
                }
            }
            return TextBoundary.BinarySearchRangeFromList(0, m_lst_code_range.Count, nCodePoint, m_lst_code_range);
        }

        private static int GetCustomWordBreakProperty(int nCodePoint) {
            if (nCodePoint >= 32 && nCodePoint <= 126) {
                return Custom_Property_Ascii;
            }
            return Other;
        }
    }
}
