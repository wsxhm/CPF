using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Xml;

namespace ST.Library.Drawing.SvgRender
{
    partial class SvgDocument
    {
        private static Type m_type_svg_element = typeof(SvgElement);
        private static Regex m_reg_style_width = new Regex(@"\bwidth\s*:\s*(\.\d+|\d+(?:\.\d+))");
        private static Regex m_reg_style_height = new Regex(@"\bheight\s*:\s*(\.\d+|\d+(?:\.\d+))");
        private static Regex m_reg_number = new Regex(@"(-?(\.\d+|\d(?:\.\d+)?)+)");
        private static Dictionary<string, Type> m_dic_target_type = new Dictionary<string, Type>();

        static SvgDocument() {
            var asm = Assembly.GetExecutingAssembly();
            SvgDocument.RegisterType(asm);
        }

        internal static SvgElement CreateElement(string strTargetName) {
            if (!m_dic_target_type.ContainsKey(strTargetName)) {
                return null;
            }
            var obj = Activator.CreateInstance(m_dic_target_type[strTargetName]);
            return (SvgElement)obj;
        }

        public static string[] GetTargetNames() {
            return m_dic_target_type.Keys.ToArray();
        }

        public static bool UnRegisterType(string strTargetName) {
            if (!m_dic_target_type.ContainsKey(strTargetName)) {
                return false;
            }
            m_dic_target_type.Remove(strTargetName);
            return true;
        }

        public static bool RegisterType(string strTargetName, Type type) {
            return SvgDocument.RegisterType(strTargetName, type, true);
        }

        public static bool RegisterType(string strTargetName, Type type, bool bOverride) {
            if (type.IsAbstract) {
                throw new ArgumentException("Can not retister a abstract class!");
            }
            if (!(type.IsSubclassOf(m_type_svg_element))) {
                throw new ArgumentException("The type is not a SvgElement!");
            }
            if (!m_dic_target_type.ContainsKey(strTargetName)) {
                m_dic_target_type.Add(strTargetName, type);
                return true;
            }
            if (!bOverride) {
                return false;
            }
            m_dic_target_type[strTargetName] = type;
            return true;
        }

        public static int RegisterType() {
            var asm = Assembly.GetCallingAssembly();
            return SvgDocument.RegisterType(asm, true);
        }

        public static int RegisterType(bool bOverride) {
            var asm = Assembly.GetCallingAssembly();
            return SvgDocument.RegisterType(asm, bOverride);
        }

        public static int RegisterType(string strFile) {
            return SvgDocument.RegisterType(strFile, true);
        }

        public static int RegisterType(string strFile, bool bOverride) {
            var asm = Assembly.LoadFrom(Path.GetFullPath(strFile));
            return SvgDocument.RegisterType(asm, bOverride);
        }

        public static int RegisterType(Assembly asm) {
            return SvgDocument.RegisterType(asm, true);
        }

        public static int RegisterType(Assembly asm, bool bOverride) {
            int nCounter = 0;
            var types = asm.GetTypes();
            foreach (var t in types) {
                if (t.IsAbstract) {
                    continue;
                }
                var attrs = t.GetCustomAttributes(false);
                foreach (var a in attrs) {
                    if (!(a is SvgElementAttribute)) {
                        continue;
                    }
                    if (!t.IsSubclassOf(m_type_svg_element)) {
                        continue;
                    }
                    if (SvgDocument.RegisterType(((SvgElementAttribute)a).TargetName, t, bOverride)) {
                        nCounter++;
                    }
                }
            }
            return nCounter;
        }
        //========================================================
        public static SvgDocument FromXmlFile(string strFile) {
            return SvgDocument.FromXml(File.ReadAllText(strFile));
        }

        public static SvgDocument FromXml(string strXml) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(strXml);
            return SvgDocument.FromXmlDocument(xml);
        }

        public static SvgDocument FromXmlDocument(XmlDocument xml) {
            // TODO: comment element
            var doc = xml.DocumentElement;
            if (doc == null || doc.Name != "svg") {
                throw new ArgumentException("Invalid svg document");
            }
            SvgDocument svg = new SvgDocument();
            svg.InitElement(svg, null, doc);
            return svg;
        }

    }
}
