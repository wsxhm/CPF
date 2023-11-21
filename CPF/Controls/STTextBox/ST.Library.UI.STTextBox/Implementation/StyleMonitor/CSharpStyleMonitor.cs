using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ST.Library.UI.STTextBox
{
    public class CSharpStyleMonitor : TextStyleMonitor
    {
        public TextStyle DefaultStyle { get; set; }
        public TextStyle CommentStyle { get; set; }
        public TextStyle DocCommentStyle { get; set; }
        public TextStyle KeyWordStyle { get; set; }
        public TextStyle StringStyle { get; set; }
        public TextStyle OperatorStyle { get; set; }
        public TextStyle DelimiterStyle { get; set; }
        public TextStyle NumberStyle { get; set; }
        public TextStyle BuiltInTypeStyle { get; set; }
        public TextStyle PropertyStyle { get; set; }
        public TextStyle CustomTypeStyle { get; set; }
        public TextStyle FunctionDeclStyle { get; set; }
        public TextStyle FunctionCallStyle { get; set; }
        public TextStyle FunctionBuiltInStyle { get; set; }

        private enum SymbolType
        {
            Comment,
            DocComment,
            KeyWord,
            String,
            Operator,
            Delimiter,
            Number,
            BuiltInType,
            CustomType,
            Property,
            FunctionDecl,
            FunctionCall,
            FunctionBuiltIn,
            None
        }

        private class Symbol : IComparable
        {
            public SymbolType Type;
            public string Value;
            public int Index;
            public int Length;

            public Symbol(SymbolType type, string value, int nIndex, int nLen) {
                this.Type = type;
                this.Value = value;
                this.Index = nIndex;
                this.Length = nLen;
            }
            public override string ToString() {
                return "[" + this.Type + "] - (" + this.Index + "," + this.Length + ") - " + this.Value;
            }

            public int CompareTo(object obj) {
                return this.Index - ((Symbol)obj).Index;
            }
        }

        private static string m_str_type = "void,var,object,dynamic,long,ulong,int,uint,short,ushort,byte,sbyte,float,double,decimal,bool,char,string,params";
        private static string m_str_modifier = "private,public,internal,protected,partial,sealed,abstract,static,extern,virtual,override,fixed,unsafe,readonly,const";
        private static string m_str_object = "event,enum,struct,class,new";
        private static string m_str_flow = "for,foreach,if,else,do,while,switch,case,default,break,continue,goto,return";
        private static string m_str_value = "null,true,false";
        private static string m_str_other = "try,catch,finally,throw,using,namespace,this,base,in,is,as,get,set,ref,out,value,stackalloc,unchecked,lock";
        private static string m_str_func = "sizeof,typeof,nameof";
        private static string m_str_linq = "let,select,from,group,into,where,join,orderby,ascending,descending";
        private static string m_str_operator = "+-*/<>!=&|~^?:";
        private static string m_str_delimiter = "[](){},;";
        private static Regex m_reg_w = new Regex(@"\w+");
        private static Regex m_reg_W = new Regex(@"\W");

        private static HashSet<string> m_hs_all_key;
        private static HashSet<string> m_hs_type;
        private static HashSet<string> m_hs_object;
        private static HashSet<string> m_hs_modifier;
        private static HashSet<string> m_hs_function;
        private static HashSet<string> m_hs_operator;
        private static HashSet<string> m_hs_delimiter;

        private List<Symbol> m_lst_smb;
        private List<TextStyleRange> m_lst_range;

        static CSharpStyleMonitor() {
            m_hs_all_key = new HashSet<string>();
            string[] strs = { m_str_type, m_str_modifier, m_str_object, m_str_flow, m_str_value, m_str_other, m_str_func, m_str_linq };
            foreach (var v in strs) {
                foreach (var s in v.Split(',')) {
                    m_hs_all_key.Add(s);
                }
            }
            m_hs_type = new HashSet<string>();
            foreach (var v in m_str_type.Split(',')) {
                m_hs_type.Add(v);
            }
            m_hs_modifier = new HashSet<string>();
            foreach (var v in m_str_modifier.Split(',')) {
                m_hs_modifier.Add(v);
            }
            m_hs_object = new HashSet<string>();
            foreach (var v in m_str_object.Split(',')) {
                m_hs_object.Add(v);
            }
            m_hs_function = new HashSet<string>();
            foreach (var v in m_str_func.Split(',')) {
                m_hs_function.Add(v);
            }
            m_hs_operator = new HashSet<string>();
            foreach (var v in m_str_operator) {
                m_hs_operator.Add(v.ToString());
            }
            m_hs_delimiter = new HashSet<string>();
            foreach (var v in m_str_delimiter) {
                m_hs_delimiter.Add(v.ToString());
            }
        }

        public CSharpStyleMonitor() {
            m_lst_smb = new List<Symbol>();
            m_lst_range = new List<TextStyleRange>();
            this.DefaultStyle = new TextStyle() { ForeColor = Color.Black };
            this.CommentStyle = new TextStyle() { ForeColor = Color.Green };
            this.DocCommentStyle = new TextStyle() { ForeColor = Color.Gray };
            this.KeyWordStyle = new TextStyle() { ForeColor = Color.Blue };
            this.StringStyle = new TextStyle() { ForeColor = Color.Brown };
            this.OperatorStyle = new TextStyle() { ForeColor = Color.DarkMagenta };
            this.DelimiterStyle = new TextStyle() { ForeColor = Color.Gray };
            this.NumberStyle = new TextStyle() { ForeColor = Color.Orange };
            this.BuiltInTypeStyle = new TextStyle() { ForeColor = Color.CornflowerBlue };
            this.PropertyStyle = new TextStyle() { ForeColor = Color.SlateGray };
            this.CustomTypeStyle = new TextStyle() { ForeColor = Color.DarkCyan };
            this.FunctionDeclStyle = new TextStyle() { ForeColor = Color.DarkViolet };
            this.FunctionCallStyle = new TextStyle() { ForeColor = Color.PaleVioletRed };
            this.FunctionBuiltInStyle = new TextStyle() { ForeColor = Color.CornflowerBlue };
        }

        public override void Init(string strText) {
            m_lst_smb.Clear();
            int nIndex = 0;
            //bool bFlag = false;
            string strTemp = string.Empty;
            StringBuilder sb = new StringBuilder();
            List<Symbol> lst = new List<Symbol>();
            for (int i = 0; i < strText.Length; i++) {
                char ch = strText[i];
                //====== symbol ======
                //bFlag = (ch >= '0' && ch <= '9');
                //bFlag |= (ch >= 'a' && ch <= 'z');
                //bFlag |= (ch >= 'A' && ch <= 'Z');
                //bFlag |= (ch == '_');
                if (this.CheckIsIdentifierChar(ch)) {
                    sb.Append(ch);
                    continue;
                }
                //====================
                if (sb.Length != 0) {
                    lst.Add(new Symbol(SymbolType.None, sb.ToString(), i - sb.Length, sb.Length));
                    sb.Remove(0, sb.Length);
                }
                switch (ch) {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        continue;
                }
                if (ch == '/' && i + 1 < strText.Length) {
                    switch (strText[i + 1]) {
                        case '*':
                            strTemp = this.GetComment1(strText, i);
                            m_lst_smb.Add(new Symbol(SymbolType.Comment, strTemp, i, strTemp.Length));
                            i += strTemp.Length;
                            nIndex = i;
                            continue;
                        case '/':
                            strTemp = this.GetComment2(strText, i);
                            nIndex = i;
                            if (strTemp.StartsWith("///")) {
                                Regex reg = new Regex(@"<.*?>");
                                foreach (Match m in reg.Matches(strTemp)) {
                                    if (nIndex < i + m.Index) {
                                        m_lst_smb.Add(new Symbol(SymbolType.Comment, strText.Substring(nIndex, i + m.Index - nIndex), nIndex, i + m.Index - nIndex));
                                        nIndex = i + m.Index;
                                    }
                                    m_lst_smb.Add(new Symbol(SymbolType.DocComment, m.Value, i + m.Index, m.Length));
                                    nIndex = i + m.Index + m.Length;
                                }
                                if (nIndex < i + strTemp.Length) {
                                    m_lst_smb.Add(new Symbol(SymbolType.Comment, strText.Substring(nIndex - i, strTemp.Length - nIndex + i), nIndex, strTemp.Length - nIndex + i));
                                }
                            } else {
                                m_lst_smb.Add(new Symbol(SymbolType.Comment, strTemp, i, strTemp.Length));
                            }
                            i += strTemp.Length;
                            nIndex = i;
                            continue;
                    }
                }
                if (ch == '@' && i + 1 < strText.Length && strText[i + 1] == '"') {
                    string strString = this.GetString1(strText, i);
                    lst.Add(new Symbol(SymbolType.String, strString, i, strString.Length));
                    i += strString.Length - 1;
                    nIndex = i;
                    continue;
                }
                if (ch == '"' || ch == '\'') {
                    string strString = ch == '\'' ? this.GetChar(strText, i) : this.GetString2(strText, i);
                    lst.Add(new Symbol(SymbolType.String, strString, i, strString.Length));
                    i += strString.Length - 1;
                    nIndex = i;
                    continue;
                }
                lst.Add(new Symbol(SymbolType.None, ch.ToString(), i, 1));
            }
            if (sb.Length != 0) {
                lst.Add(new Symbol(SymbolType.None, sb.ToString(), strText.Length - sb.Length, sb.Length));
            }
            this.ProcessSymbol(lst);
            m_lst_smb.AddRange(lst);
            m_lst_smb.Sort();
            var aa = this.Clear(m_lst_smb);
            //m_lst_smb.AddRange(aa);
            //m_lst_smb.Sort();
            m_lst_range = this.SymbolToRange(aa);
        }

        private List<Symbol> Clear(List<Symbol> lst) {
            List<Symbol> ret = new List<Symbol>();
            if (lst == null || lst.Count == 0) {
                return ret;
            }
            Symbol last = lst[0];
            if (last.Type != SymbolType.None) {
                ret.Add(last);
            }
            for (int i = 1; i < lst.Count; i++) {
                if (m_hs_operator.Contains(lst[i].Value)) {
                    lst[i].Type = SymbolType.Operator;
                }
                if (char.IsNumber(lst[i].Value[0])) {
                    lst[i].Type = SymbolType.Number;
                }
                if (m_hs_delimiter.Contains(lst[i].Value)) {
                    lst[i].Type = SymbolType.Delimiter;
                }
                switch (lst[i].Value) {
                    case "<":   // like -> List<int>
                    case ">":
                        if (lst[i - 1].Type == SymbolType.BuiltInType || lst[i - 1].Type == SymbolType.CustomType) {
                            lst[i].Type = SymbolType.None;
                        }
                        break;
                }
                if (lst[i].Value == "." && i + 1 < lst.Count) {
                    lst[i].Type = lst[i + 1].Type;
                }
                if (lst[i].Type == SymbolType.None) {
                    last = lst[i];
                    continue;
                }
                if (last.Type == lst[i].Type) {
                    last.Length = lst[i].Index + lst[i].Length - last.Index;
                    continue;
                }
                last = lst[i];
                ret.Add(lst[i]);
            }
            return ret;
        }

        private List<TextStyleRange> SymbolToRange(List<Symbol> lst) {
            List<TextStyleRange> lst_ret = new List<TextStyleRange>();
            foreach (var v in lst) {
                lst_ret.Add(new TextStyleRange() {
                    Index = v.Index,
                    Length = v.Length,
                    Style = this.GetColorFromSymbolType(v.Type)
                });
            }
            return lst_ret;
        }

        private void ProcessSymbol(List<Symbol> lst) {
            for (int i = 0; i < lst.Count; i++) {
                //if (m_hs_operators.Contains(lst[i].Value)) {
                //    lst[i].Type = SymbolType.Operator;
                //}
                //if (char.IsNumber(lst[i].Value[0])) {
                //    lst[i].Type = SymbolType.Number;
                //}
                if (lst[i].Value == "." && i + 1 < lst.Count && this.CheckIsIdentifier(lst[i + 1].Value)) {
                    lst[i + 1].Type = SymbolType.Property;
                }
                if (m_hs_modifier.Contains(lst[i].Value)) {
                    lst[i].Type = SymbolType.KeyWord;
                    if (!this.CheckIsDeclare(lst, i)) {
                        continue;
                    }
                    i += this.CheckDeclare(lst, i);
                    continue;
                }
                if (m_hs_object.Contains(lst[i].Value)) {
                    lst[i].Type = SymbolType.KeyWord;
                    i += this.CheckObject(lst, i);
                    continue;
                }
                if (m_hs_all_key.Contains(lst[i].Value)) {
                    lst[i].Type = m_hs_type.Contains(lst[i].Value) ? SymbolType.BuiltInType : SymbolType.KeyWord;
                    if (m_hs_function.Contains(lst[i].Value)) lst[i].Type = SymbolType.FunctionBuiltIn;
                    switch (lst[i].Value) {
                        case "sizeof":
                        case "typeof":
                        case "foreach":
                        case "catch":
                            if (i + 2 < lst.Count && this.CheckIsIdentifier(lst[i + 2].Value)) {
                                i += this.CheckDataType(lst, i + 2);
                                continue;
                            }
                            i += 2;
                            break;
                    }
                    continue;
                }
                switch (lst[i].Value) { // A new sentence after these.
                    case ";":
                    case "{":
                    case "}":
                    case "]":
                        if (i + 1 >= lst.Count) break;
                        if (lst[i + 1].Value == "[") {
                            i += this.CheckAttribute(lst, i + 1);
                            break;
                        }
                        if (!this.CheckIsIdentifier(lst[i + 1].Value)) break;
                        // If can not match a key word, maybe is a custom type (such as: {CustomType a = null;})
                        if (m_hs_type.Contains(lst[i + 1].Value) && this.CheckIsDeclare(lst, i + 1)) {
                            i += this.CheckDataType(lst, i + 1) + 1;
                        }
                        break;
                    case "=":
                        this.ProcessEqual(lst, i);
                        break;
                    case "[":
                        if (i - 1 < 0) break;
                        switch (lst[i - 1].Value) {
                            case ";":
                            case "{":
                            case "}":
                            case "]":
                                i += this.CheckAttribute(lst, i) - 1;
                                break;
                        }
                        break;
                    case "(":
                        i += this.CheckFunction(lst, i);
                        break;
                }
            }
        }

        private int CheckObject(List<Symbol> lst, int nIndex) {
            // after [class enum struct new]
            for (int i = nIndex + 1; i < lst.Count; i++) {
                i = this.GetNextSymbolIndex(lst, i);
                switch (lst[i].Value) {
                    case ":":   // Extened
                    case "<":   // Generic
                    case ",":
                    case ">":
                        continue;
                    default:
                        if (!this.CheckIsIdentifier(lst[i].Value)) {
                            return i - nIndex;
                        }
                        break;
                }
                lst[i].Type = m_hs_type.Contains(lst[i].Value) ? SymbolType.BuiltInType : SymbolType.CustomType;
            }
            return lst.Count - nIndex;
        }

        private int CheckDeclare(List<Symbol> lst, int nIndex) {
            // Rule: MODIFIER? DATA_TYPE NAME [( or <]
            int nTemp = nIndex;
            while (nTemp < lst.Count && m_hs_modifier.Contains(lst[nTemp].Value)) {
                lst[nTemp++].Type = SymbolType.KeyWord;    //Igonre the modifier
            }
            if (nTemp >= lst.Count) return nTemp - nIndex;
            if (!this.CheckIsIdentifier(lst[nTemp].Value)) {
                return nTemp - nIndex;
            }
            if (m_hs_object.Contains(lst[nTemp].Value)) {
                // if is [class struce enum .. ], so it's a declaration of an object
                lst[nTemp].Type = SymbolType.KeyWord;
                nTemp += this.CheckObject(lst, nTemp);
                return nTemp - nIndex;
            }
            // now lst[nTemp] is a DATA_TYPE
            if (nTemp + 1 >= lst.Count || lst[nTemp + 1].Value != "(") { // if is end or next char is not '('
                // if lst[nTemp + 1] is "(". maybe like this "MODIFIER? DATA_TYPE("
                // maybe is a Constructor like this "public ClassName("
                nTemp += this.CheckDataType(lst, nTemp);
            }
            if (nTemp >= lst.Count) return nTemp - nIndex;
            // now lst[nTemp] is NAME
            if (lst[nTemp].Value == "operator") { // Overloaded operator functions are special. So deal with it independently.
                lst[nTemp].Type = SymbolType.KeyWord;
                if (nTemp + 2 < lst.Count && lst[++nTemp + 1].Value != "(") {
                    nTemp++;
                }
                if (lst[nTemp + 1].Value != "(") {
                    return nTemp - nIndex;
                }
            }
            if (++nTemp >= lst.Count) return nTemp - nIndex;
            // now -> "MODIFIER? DATA_TYPE NAME<". maybe is a generic function
            if (lst[nTemp].Value == "<") {
                int n = nTemp;
                nTemp += this.CheckDataType(lst, nTemp - 1) - 1;
                lst[n - 1].Type = SymbolType.FunctionDecl;
                if (nTemp >= lst.Count) return nTemp - nIndex;
            }
            // now -> "MODIFIER? DATA_TYPE NAME(". so it's a function
            if (lst[nTemp].Value == "(") {
                if (this.CheckIsIdentifier(lst[nTemp - 1].Value)) {
                    lst[nTemp - 1].Type = SymbolType.FunctionDecl;
                }
                nTemp += this.CheckFunctionParam(lst, nTemp) - 1;
            }
            // The other case is the declaration of a variable.
            return nTemp - nIndex;
        }

        private int CheckFunction(List<Symbol> lst, int nIndex) {
            // lst[nIndex] is "("
            if (nIndex - 1 >= 0 && lst[nIndex - 1].Type != SymbolType.None && lst[nIndex - 1].Type != SymbolType.Property) {
                return 0;
            }
            // first check if is convert like -> (xxx)xx or (xxx<xxx.XXX,xxx>[])xxx
            for (int i = nIndex + 1; i < lst.Count; i++) {
                switch (lst[i].Value) {
                    case ".":   // xxx.xxx
                    case "<":   // generic
                    case ",":
                    case ">":
                    case "[":   // array
                    case "]":
                        continue;
                    case ")":
                        if (i + 1 < lst.Count && this.CheckIsIdentifier(lst[i + 1].Value)) {
                            return this.CheckDataType(lst, nIndex + 1);
                        }
                        break;
                    default:
                        if (!this.CheckIsIdentifier(lst[i].Value)) {
                            break;
                        }
                        continue;
                }
                break;
            }
            // Note: On the left of "(" can be a function or a constructor of an object
            //       But if is "new xxx(" will not run this function. will run CheckObject()
            int nTemp = nIndex - 1;
            if (nTemp < 0) return 0;
            // if last char is '>', maybe is a generic function
            if (lst[nTemp].Value == ">") {  // maybe is a generic, find the '<'
                nTemp -= this.GetGenericLength(lst, nTemp);
                this.CheckDataType(lst, nTemp);
            }
            if (nTemp < 0) return 0;
            // new  -> list[nTemp] is NAME
            if (!this.CheckIsIdentifier(lst[nTemp].Value)) return 0;
            if (nTemp - 1 >= 0 && this.CheckIsBoundary(lst[nTemp - 1].Value[0])) {
                lst[nTemp].Type = SymbolType.FunctionCall;
            } else {
                lst[nTemp].Type = SymbolType.FunctionDecl;
            }
            //this.CheckFunctionName(lst, nTemp);
            //if (lst[nTemp].Value == "catch") {
            //    return this.CheckFunctionParam(lst, nTemp + 1);
            //}
            if (--nTemp < 0) return 0;
            // now -> lst[nTemp] maybe is DATA_TYPE
            if (lst[nTemp].Value == "]") {  // maybe is a array, find the '['
                nTemp -= this.GetArrayLength(lst, nTemp);
            }
            if (nTemp < 0) return 0;
            if (lst[nTemp].Value == ">") {  // maybe is a generic, find the '<'
                nTemp -= this.GetGenericLength(lst, nTemp);
                this.CheckDataType(lst, nTemp);
            }
            if (nTemp < 0) return 0;
            // now -> lst[nTemp] maybe is DATA_TYPE;
            if (this.CheckIsBoundary(lst[nTemp].Value[0])) {   // It's a function call like ".XXX(" or ";XXX("
                return 0;
            }
            // other cases, it's a function declaration
            if (lst[nTemp].Type == SymbolType.None) {          // if none ,so it's not generic
                lst[nTemp].Type = m_hs_type.Contains(lst[nTemp].Value) ? SymbolType.BuiltInType : SymbolType.CustomType;
            }
            return this.CheckFunctionParam(lst, nIndex) - 1;
        }

        private int CheckFunctionParam(List<Symbol> lst, int nIndex) {
            // lst[nIndex] is '(' or '(' right char
            int nTemp = nIndex;
            for (int i = nIndex; i < lst.Count; i++) {
                nTemp = nIndex;
                switch (lst[i].Value) {
                    case "(":
                    case ",":
                        continue;
                    case "[":
                        i += this.CheckAttribute(lst, i) - 1;
                        continue;
                    case ")":
                        return i - nIndex + 1;
                    case "params":
                        lst[i].Type = SymbolType.KeyWord;
                        continue;
                    default:
                        if (!this.CheckIsIdentifier(lst[i].Value)) {
                            nTemp = i;
                            break;
                        }
                        i += this.CheckDataType(lst, i);
                        if (i < lst.Count && !this.CheckIsIdentifier(lst[i].Value)) {
                            nTemp = i;
                            break;
                        }
                        continue;
                }
                break;
            }
            return nTemp - nIndex + 1;
        }

        private int CheckAttribute(List<Symbol> lst, int nIndex) {
            // Attribute -> [attr(value)] or [attr(value),attr(vule)]
            // lst[nIndex] is '[' and left char is [';', '{', '}', ']' ]
            if (nIndex + 2 >= lst.Count) {
                return 1;
            }
            if (lst[nIndex + 1].Value == "]") { // maybe just a array like -> new int[][]();
                return 2;
            }
            if (lst[nIndex + 2].Value == "]") { // maybe just a array like -> new int[2][2]();
                return 3;                       // Note: new int[][GetLength(x)](); will run this
            }
            int nTemp = this.GetNextSymbolIndex(lst, nIndex + 1);
            lst[nTemp].Type = SymbolType.CustomType;
            for (int i = nIndex + 2; i < lst.Count; i++) {
                if (lst[i].Value == "." && i + 1 < lst.Count && this.CheckIsIdentifier(lst[i + 1].Value)) {
                    lst[i + 1].Type = SymbolType.Property;
                }
                if ((lst[i].Value == ")" || lst[i].Value == ",") && i - 3 >= 0) {
                    if (lst[i - 2].Value == ".") {                  // like this [attr(XXX.xx, xx.XXX.xx)]
                        lst[i - 3].Type = SymbolType.CustomType;    // XXX will be set as custom type
                    }
                }
                if (lst[i].Value == "(" && lst[i - 1].Type == SymbolType.None) {
                    lst[i - 1].Type = SymbolType.CustomType;
                }
                if (m_hs_all_key.Contains(lst[i].Value) && lst[i - 1].Value != ".") {
                    lst[i].Type = SymbolType.KeyWord;
                    switch (lst[i].Value) {
                        case "sizeof":
                        case "typeof":
                            if (i + 2 < lst.Count && lst[i + 1].Value == "(" && this.CheckIsIdentifier(lst[i + 2].Value)) {
                                lst[i + 2].Type = m_hs_type.Contains(lst[i + 2].Value) ? SymbolType.BuiltInType : SymbolType.CustomType;
                            }
                            i += 2;
                            break;
                    }
                }
                if (lst[i].Value == "]") {
                    return i - nIndex + 1;
                }
            }
            return 1;
        }

        private int CheckDataType(List<Symbol> lst, int nIndex) {
            // Note: This function will not check if the lst[nIndex] is valid data type
            //       The current function treats lst[nIndex] directly as a data type.
            int nTemp = this.GetNextSymbolIndex(lst, nIndex);
            // Rule: DATA_TYPE NAME
            //if (nTemp + 1 < lst.Count) {
            if (m_hs_object.Contains(lst[nTemp].Value)) {
                lst[nTemp].Type = SymbolType.KeyWord;
                nTemp += this.CheckObject(lst, nTemp);
                return nTemp - nIndex;
            } else if (m_hs_type.Contains(lst[nTemp].Value)) {
                lst[nTemp].Type = SymbolType.BuiltInType;
            } else {    // -> like "public int XXX{get; private set;}"
                lst[nTemp].Type = m_hs_all_key.Contains(lst[nTemp].Value) ? SymbolType.KeyWord : SymbolType.CustomType;
            }
            if (nTemp + 1 < lst.Count && lst[nTemp + 1].Value == "<") {  // so it's generic type like -> "xxx<xx,xx>"
                for (int j = nTemp + 2; j < lst.Count; j++) {
                    switch (lst[j].Value) {
                        case ".":
                        case ",":
                            continue;
                        case ">":
                            nTemp = j;
                            break;
                        default:
                            j += this.CheckDataType(lst, j) - 1;
                            continue;
                    }
                    break;
                }
            }
            // so it's array like -> "xxx[]"
            if (nTemp + 1 < lst.Count && lst[nTemp + 1].Value == "[") {
                nTemp += this.GetArrayLength(lst, nTemp + 1);
            }
            //}
            return nTemp - nIndex + 1;
        }

        private void ProcessEqual(List<Symbol> lst, int nIndex) {
            if (lst[nIndex].Value != "=") {
                return;
            }
            if (nIndex + 1 < lst.Count && lst[nIndex + 1].Value == "=") {   // == return
                return;
            }
            if (--nIndex < 0) return;
            // now lst[nIndex] is variable
            if (this.CheckIsIdentifier(lst[nIndex].Value)) {        // it's a variable -> "xxx ="
                //lst[nIndex].Type = SymbolType.None;
            } else {
                return;
            }
            if (--nIndex < 0) return;
            // now lst[nIndex] is data type
            if (this.CheckIsIdentifier(lst[nIndex].Value)) {
                lst[nIndex].Type = m_hs_type.Contains(lst[nIndex].Value) ? SymbolType.BuiltInType : SymbolType.CustomType;
                return;
            }
            if (lst[nIndex].Value == "]") { // maybe is array -> "] xxx =" -> "xxx[] xxx ="
                nIndex -= this.GetArrayLength(lst, nIndex);
            }
            if (lst[nIndex].Value == ">") {
                nIndex -= this.GetGenericLength(lst, nIndex);
                this.CheckDataType(lst, nIndex);
            }
        }

        private bool CheckIsIdentifier(string strText) {
            return m_reg_w.Match(strText).Value.Length == strText.Length;
            //char ch = strText[0];
            //bool bFlag = ch == '_';
            //bFlag |= ch >= 'a' && ch <= 'z';
            //bFlag |= ch >= 'A' && ch <= 'Z';
            //return bFlag;
        }

        private bool CheckIsIdentifierChar(char ch) {
            return !m_reg_W.IsMatch(ch.ToString());
            //if (this.CheckIsBoundary(ch)) return false;
            //if (ch < '0') return false;
            //if (ch > 'z' && ch < 256) return false;
            //return true;
            //switch (ch) { 
            //    case ' ':
            //    case  '\t':
            //    case '\r':
            //    case '\n':
            //        case 
            //}
        }

        private bool CheckIsBoundary(char ch) {
            switch (ch) {
                case '.':
                case ';':
                case ',':
                case '!':
                case '=':
                case '(':
                case ')':
                case '<':
                case '>':
                case '{':
                case '}':
                case '[':
                case ']':
                case '+':
                case '-':
                case '*':
                case '/':
                case '\\':
                case '&':
                case '|':
                case '^':
                case '~':
                case '%':
                case '?':
                case ':':
                    return true;
            }
            return false;
        }

        private bool CheckIsDeclare(List<Symbol> lst, int nIndex) {
            // Rule: MODIFIER? DATA_TYPE NAME
            int nTemp = nIndex;
            while (nTemp < lst.Count && m_hs_modifier.Contains(lst[nTemp].Value)) {
                nTemp++;    //Igonre the modifier
            }
            nTemp = this.GetNextSymbolIndex(lst, nIndex);
            // now lst[nTemp] is DATA_TYPE
            if (!this.CheckIsIdentifier(lst[nTemp].Value)) {
                return false;
            }
            if (nTemp + 1 < lst.Count && lst[nTemp + 1].Value == "<") { // generic
                int nLen = this.GetGenericLength(lst, nTemp + 1);
                if (nLen == 0) {
                    return false;
                }
                nTemp += nLen;
            }
            if (nTemp + 1 < lst.Count && lst[nTemp + 1].Value == "[") { // array
                int nLen = this.GetArrayLength(lst, nTemp + 1);
                if (nLen == 0) {
                    return false;
                }
                nTemp += nLen;
            }
            if (nTemp + 1 < lst.Count) {
                return this.CheckIsIdentifier(lst[nTemp + 1].Value);    // check the NAME
            }
            return true;
        }

        private int GetNextSymbolIndex(List<Symbol> lst, int nIndex) {
            // "xxx.xxx.xxx.xxx.XXX"
            // will return XXX's index
            int nTemp = nIndex;
            while (nIndex + 1 < lst.Count && lst[nIndex + 1].Value == ".") {
                if (nIndex + 2 < lst.Count && this.CheckIsIdentifier(lst[nIndex + 2].Value)) {
                    lst[nIndex + 2].Type = SymbolType.Property;
                }
                nIndex += 2;
            }
            if (nIndex >= lst.Count) nIndex -= 2;
            if (nIndex < 0) nIndex = 0;
            return nIndex;
        }

        private int GetGenericLength(List<Symbol> lst, int nIndex) {
            // such as: XXX<xx,xx> or XXX<xxx,XXX<xxx.xxx.XXX>>
            int nCounter = 0;
            if (lst[nIndex].Value == "<") {
                for (int i = nIndex; i < lst.Count; i++) {
                    switch (lst[i].Value) {
                        case ".":
                        case ",":
                            continue;
                        case "<":
                            nCounter++;
                            continue;
                        case ">":
                            if (--nCounter == 0) {
                                return i - nIndex + 1;
                            }
                            continue;
                        default:
                            if (!this.CheckIsIdentifier(lst[i].Value)) {
                                return 0;
                            }
                            continue;
                    }
                }
            } else if (lst[nIndex].Value == ">") {
                for (int i = nIndex; i >= 0; i--) {
                    switch (lst[i].Value) {
                        case ".":
                        case ",":
                            continue;
                        case ">":
                            nCounter++;
                            continue;
                        case "<":
                            if (--nCounter == 0) {
                                return nIndex - i + 1;
                            }
                            continue;
                        default:
                            if (!this.CheckIsIdentifier(lst[i].Value)) {
                                return 0;
                            }
                            continue;
                    }
                }
            }
            return 0;
        }

        private int GetArrayLength(List<Symbol> lst, int nIndex) {
            // such as: [] or [,] or [,,,] or [][]
            if (lst[nIndex].Value == "[") {
                for (int i = nIndex; i < lst.Count; i++) {
                    switch (lst[i].Value) {
                        case ",":
                            continue;
                        case "]":
                            if (i + 1 < lst.Count && lst[i + 1].Value == "[") {
                                continue;
                            }
                            return i - nIndex + 1;
                    }
                }
            } else if (lst[nIndex].Value == "]") {
                for (int i = nIndex; i >= 0; i--) {
                    switch (lst[i].Value) {
                        case ",":
                            continue;
                        case "[":
                            if (i - 1 >= 0 && lst[i - 1].Value == "]") {
                                continue;
                            }
                            return nIndex - i + 1;
                    }
                }
            }
            return 0;
        }

        private string GetComment1(string strText, int nIndex) {
            for (int i = nIndex + 3; i < strText.Length; i++) {
                if (strText[i] == '/' && strText[i - 1] == '*') {
                    return strText.Substring(nIndex, i - nIndex + 1);
                }
            }
            return strText.Substring(nIndex);
        }

        private string GetComment2(string strText, int nIndex) {
            for (int i = nIndex + 2; i < strText.Length; i++) {
                switch (strText[i]) {
                    case '\r':
                    case '\n':
                        return strText.Substring(nIndex, i - nIndex);
                }
            }
            return strText.Substring(nIndex);
        }

        private string GetString1(string strText, int nIndex) {
            for (int i = nIndex + 2; i < strText.Length; i++) {
                if (strText[i] == '"') {
                    if (i + 1 < strText.Length && strText[i + 1] == '"') {
                        i++;
                        continue;
                    }
                    return strText.Substring(nIndex, i - nIndex + 1);
                }
            }
            return strText.Substring(nIndex);
        }

        public string GetString2(string strText, int nIndex) {
            for (int i = nIndex + 1; i < strText.Length; i++) {
                if (strText[i] == '\\') {
                    i++;
                    continue;
                }
                if (strText[i] == '"') {
                    return strText.Substring(nIndex, i - nIndex + 1);
                }
                switch (strText[i]) {
                    case '\r':
                    case '\n':
                        return strText.Substring(nIndex, i - nIndex);
                }
            }
            return strText.Substring(nIndex);
        }

        public string GetChar(string strText, int nIndex) {
            for (int i = nIndex + 1; i < strText.Length; i++) {
                if (strText[i] == '\\') {
                    i++;
                    continue;
                }
                if (strText[i] == '\'') {
                    return strText.Substring(nIndex, i - nIndex + 1);
                }
                switch (strText[i]) {
                    case '\r':
                    case '\n':
                        return strText.Substring(nIndex, i - nIndex);
                }
            }
            return strText.Substring(nIndex);
        }

        public override void OnSelectionChanged(TextManager textManager, int nStart, int nLen) { }

        public override void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs) {
            this.Init(textManager.GetText());
        }

        public override TextStyleRange GetStyleFromCharIndex(int nIndex) {
            return TextStyleMonitor.GetStyleFromCharIndex(nIndex, m_lst_range);
            //int nLeft = 0, nRight = m_lst_smb.Count - 1, nMid = 0;
            //bool bFind = false;
            //while (nLeft <= nRight) {
            //    nMid = (nLeft + nRight) >> 1;
            //    if (m_lst_smb[nMid].Index > nIndex) {
            //        nRight = nMid - 1;
            //    } else if (m_lst_smb[nMid].Index + m_lst_smb[nMid].Length <= nIndex) {
            //        nLeft = nMid + 1;
            //    } else {
            //        bFind = true;
            //        break;
            //    }
            //}
            //if (!bFind) {
            //    //return new TextStyleRange() { Index = nIndex, Length = 1, Style = new TextStyle() { ForeColor = this.DefaultColor } };
            //}
            //var ret = new TextStyleRange() {
            //    Index = m_lst_smb[nMid].Index,
            //    Length = m_lst_smb[nMid].Length
            //};
            //ret.Style.ForeColor = this.GetColorFromSymbolType(m_lst_smb[nMid].Type);
            //return ret;
        }

        private TextStyle GetColorFromSymbolType(SymbolType smbType) {
            switch (smbType) {
                case SymbolType.Comment:
                    return this.CommentStyle;
                case SymbolType.DocComment:
                    return this.DocCommentStyle;
                case SymbolType.KeyWord:
                    return this.KeyWordStyle;
                case SymbolType.String:
                    return this.StringStyle;
                case SymbolType.Operator:
                    return this.OperatorStyle;
                case SymbolType.Delimiter:
                    return this.DelimiterStyle;
                case SymbolType.Number:
                    return this.NumberStyle;
                case SymbolType.BuiltInType:
                    return this.BuiltInTypeStyle;
                case SymbolType.Property:
                    return this.PropertyStyle;
                case SymbolType.CustomType:
                    return this.CustomTypeStyle;
                case SymbolType.FunctionDecl:
                    return this.FunctionDeclStyle;
                case SymbolType.FunctionCall:
                    return this.FunctionCallStyle;
                case SymbolType.FunctionBuiltIn:
                    return this.FunctionBuiltInStyle;
            }
            return this.DefaultStyle;
        }
    }
}
