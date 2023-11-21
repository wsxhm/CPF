using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

namespace ST.Library.Drawing.SvgRender
{
    public partial class SvgAttributes : IEnumerable
    {
        private Dictionary<string, string> m_dic_attr_has_set = new Dictionary<string, string>();
        private Dictionary<string, string> m_dic_attr_default = new Dictionary<string, string>();

        public string this[string strKey] {
            get {
                if (m_dic_attr_has_set.ContainsKey(strKey)) {
                    return m_dic_attr_has_set[strKey];
                }
                return null;
            }
        }

        public bool Set(string strKey, string strValue) {
            if (m_dic_attr_has_set.ContainsKey(strKey)) {
                if (string.IsNullOrEmpty(strValue)) {
                    m_dic_attr_has_set.Remove(strValue);
                } else {
                    if (m_dic_value_check.ContainsKey(strKey)) {
                        if (!m_dic_value_check[strKey](strValue)) {
                            return false;
                        }
                    }
                    m_dic_attr_has_set[strKey] = strValue;
                }
            } else {
                if (m_dic_value_check.ContainsKey(strKey)) {
                    if (!m_dic_value_check[strKey](strValue)) {
                        return false;
                    }
                }
                m_dic_attr_has_set.Add(strKey, strValue);
            }
            return true;
        }

        public bool SetDefault(string strKey, string strValue) {
            if (m_dic_attr_default.ContainsKey(strKey)) {
                if (string.IsNullOrEmpty(strValue)) {
                    m_dic_attr_default.Remove(strValue);
                } else {
                    if (m_dic_value_check.ContainsKey(strKey)) {
                        if (!m_dic_value_check[strKey](strValue)) {
                            return false;
                        }
                    }
                    m_dic_attr_default[strKey] = strValue;
                }
            } else {
                if (m_dic_value_check.ContainsKey(strKey)) {
                    if (!m_dic_value_check[strKey](strValue)) {
                        return false;
                    }
                }
                m_dic_attr_default.Add(strKey, strValue);
            }
            return true;
        }

        public string Get(string strKey) {
            string str = this[strKey];
            if (str != null) {
                return str;
            }
            return this.GetDefault(strKey);
        }

        public string GetDefault(string strKey) {
            if (m_dic_attr_default.ContainsKey(strKey)) {
                return m_dic_attr_default[strKey];
            }
            return SvgAttributes.GetStaticDefault(strKey);
        }

        public static bool SetCheckCallBack(string strName, SvgAttributeValueCheckCallBack cb) {
            return SvgAttributes.AddCheckCallBack(strName, cb, true);
        }

        public static bool AddCheckCallBack(string strName, SvgAttributeValueCheckCallBack cb, bool isOverrid) {
            if (!m_dic_value_check.ContainsKey(strName)) {
                m_dic_value_check.Add(strName, cb);
                return true;
            }
            if (isOverrid) {
                m_dic_value_check[strName] = cb;
                return true;
            }
            return false;
        }

        public string[] GetHasSetKeys() {
            return m_dic_attr_has_set.Keys.ToArray();
        }

        // [interface] ========================================================
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            foreach (var v in m_dic_attr_has_set) {
                yield return v;
            }
            //foreach (var v in m_dic_attr_inherit) {
            //    yield return v;
            //}
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
