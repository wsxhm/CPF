using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ST.Library.UI.STTextBox;
using ST.Library.Drawing.SvgRender;

using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CPF.Drawing;

namespace ST.Library.Drawing
{
    public class EmojiRender : IEmojiRender, IDisposable
    {
        private Dictionary<string, string> m_dic_xml = new Dictionary<string, string>();
        private Dictionary<string, CacheInfo> m_dic_cache = new Dictionary<string, CacheInfo>();

        private struct CacheInfo
        {
            public int Size;
            public Image Image;
            public Image SelectedImage;
        }

        //private ImageAttributes m_img_attr_normal;
        //private ImageAttributes m_img_attr_selected;

        public EmojiRender(string strFile) {
            /*ColorMatrix m = new ColorMatrix(new float[][]{
                    new float[]{1, 0, 0, 0, 0},
                    new float[]{0, 1, 0, 0, 0},
                    new float[]{0, 0, 1, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1},
                });
            m_img_attr_normal = new ImageAttributes();
            m_img_attr_normal.SetColorMatrix(m);
            m[3, 3] = 0.5F;
            m_img_attr_selected = new ImageAttributes();
            m_img_attr_selected.SetColorMatrix(m);*/
            this.InitSvgFromPackage(strFile);
        }

        public bool IsEmoji(string strChar) {
            return this.GetEmojiString(strChar) != null;
        }

        private string GetEmojiString(string strText) {
            if (m_dic_xml.ContainsKey(strText)) {
                return strText;
            }
            // http://www.unicode.org/charts/PDF/UFE00.pdf
            // FE00 - FE0F is the Variation Selectors
            // FE0E -> text variation selector
            // FE0F -> emoji variation selector
            if (strText.IndexOf('\uFE0F') == -1) {
                return null;
            }
            strText = strText.Replace("\uFE0F", "");// strText.Substring(0, strText.Length - 1);
            if (m_dic_xml.ContainsKey(strText)) {
                return strText;
            }
            return null;
        }

        public void DrawEmoji(ISTTextBoxRender render, string strChar, int nX, int nY, int nWidth, bool bSelected) {
            strChar = this.GetEmojiString(strChar);
            CacheInfo ci;
            if (string.IsNullOrEmpty(strChar)) {
                strChar = "none";
            }
            if (m_dic_cache.ContainsKey(strChar)) {
                ci = m_dic_cache[strChar];
                if (ci.Size != nWidth) {
                    ci.Image.Dispose();
                    ci.Image = this.GetEmojiImage(strChar, nWidth);
                    m_dic_cache[strChar] = ci;
                }
            } else {
                ci.Size = nWidth;
                ci.Image = this.GetEmojiImage(strChar, nWidth);
                ci.SelectedImage = this.SetAlpha((Bitmap)ci.Image.Clone());
                m_dic_cache.Add(strChar, ci);
            }
            Rect rect = new Rect(nX, nY, nWidth, nWidth);
            render.DrawImage(bSelected ? ci.SelectedImage : ci.Image, rect);
            //if (!bSelected) {
            //    render.DrawImage(ci.Image, rect);
            //} else {
            //    using (Bitmap bmp = (Bitmap)ci.Image.Clone()) {
            //        var bmpData = bmp.LockBits(new Rectangle(0, 0, nWidth, nWidth), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            //        byte[] byClr = new byte[bmpData.Stride * bmpData.Height];
            //        Marshal.Copy(bmpData.Scan0, byClr, 0, byClr.Length);
            //        for (int x = 0; x < bmp.Width; x++) {
            //            for (int y = 0; y < bmp.Height; y++) {
            //                int nIndex = y * bmpData.Stride + x * 4 + 3;
            //                byClr[nIndex] = (byte)(byClr[nIndex] >> 1);
            //            }
            //        }
            //        Marshal.Copy(byClr, 0, bmpData.Scan0, byClr.Length);
            //        bmp.UnlockBits(bmpData);
            //        render.DrawImage(bmp, rect);
            //    }
            //}
        }

        public Image GetEmojiImage(string strChar, int nSize) {
            strChar = this.GetEmojiString(strChar);
            if (strChar == null) {
                return null;
            }
            Bitmap bmp = new Bitmap(nSize, nSize);
            Graphics g = Graphics.FromImage(bmp);
            using (SvgDocument svg = SvgDocument.FromXml(m_dic_xml[strChar]))
            {
                svg.Draw(g, new Rect(0, 0, nSize, nSize));
            }
            return bmp;
        }

        public Image SetAlpha(Bitmap bmp) {
            //确定图像的宽和高
            int height = bmp.Height;
            int width = bmp.Width;

            using (var l = bmp.Lock())
            {//l.DataPointer就是数据指针，一般不建议直接使用，因为不同平台，不同图形适配器，不同位图格式，数据格式不一样，那你就需要判断不同格式来遍历指针数据处理了
                for (int x = 0; x < width ; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        l.GetPixel(x, y, out byte a, out byte r, out byte g, out byte b);
                        var p = (byte)Math.Min(255, 0.7 * r + (0.2 * g) + (0.1 * b));
                        l.SetPixel(x, y, (byte)(a >> 1), r, g, b);
                    } // x
                } // y
            }
            return bmp;
        }

        private int InitSvgFromPackage(string strFile) {
            int nCounter = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(strFile, Encoding.UTF8)) {
                string strLine = string.Empty;
                while ((strLine = reader.ReadLine()) != null) {
                    int nIndex = strLine.IndexOf(',');
                    string strXml = strLine.Substring(nIndex + 1);
                    dic.Add(this.GetString(strLine.Substring(0, nIndex)), strXml);
                }
            }
            if (!dic.ContainsKey("none")) {
                dic.Add("none", "<svg viewBox='0,0,20,20'><rect stroke='red' fill='yellow' x='4' y='2' width='12' height='16'/><path stroke='red' fill='none' d='M8,8 v-2 h4 v4 h-2 v2'/><line stroke='red' x1='8' y1='14' x2='12' y2='14'/></svg>");
            }
            m_dic_xml = dic;
            return nCounter;
        }

        public string GetString(string strText) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strText.Length; i += 4) {
                sb.Append((char)(Convert.ToInt32(strText.Substring(i, 4), 16)));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Package all svg files to one file
        /// </summary>
        /// <param name="strSvgFilesPath">The svg path</param>
        /// <param name="strFileOut">The out file</param>
        /// <returns>Svg count</returns>
        public static int PackageSvgFiles(string strSvgFilesPath, string strFileOut) {
            int nCounter = 0;
            Regex reg_start = new Regex(@"^\s+|\s+$", RegexOptions.Multiline);
            using (StreamWriter writer = new StreamWriter(strFileOut, false, Encoding.UTF8)) {
                foreach (var v in Directory.GetFiles(strSvgFilesPath, "*.svg")) {
                    string strName = Path.GetFileNameWithoutExtension(v);
                    string strText = EmojiRender.FileNameToUnicode(strName);
                    string strXml = File.ReadAllText(v).Trim();
                    strXml = reg_start.Replace(strXml, "").Replace("\r", "").Replace("\n", "");
                    writer.WriteLine(strText + "," + strXml);
                    nCounter++;
                }
            }
            return nCounter;
        }

        private static string FileNameToUnicode(string strName) {
            //((strText[nIndex] & 0x03FF) << 10) + (strText[nIndex + 1] & 0x03FF) + 0x10000;
            strName.ToLower();
            if (strName[0] == 'u') {
                strName = strName.Substring(1);
            }
            string strRet = string.Empty;
            foreach (var v in strName.Split('-', '_')) {
                int number = Convert.ToInt32(v, 16);
                if (number < 0x10000) {
                    strRet += number.ToString("X4");
                } else {
                    number &= 0xFFFF;
                    int nH = (number >> 10) | 0xD800;       //D800-DBFF -> hight
                    int nL = (number & 0x03FF) | 0xDC00;    //DC00-DEFF -> low
                    strRet += nH.ToString("X4");
                    strRet += nL.ToString("X4");
                }
            }
            return strRet;
        }

        public void Dispose() {
            /*if (m_img_attr_normal != null) {
                m_img_attr_normal.Dispose();
            }
            if (m_img_attr_selected != null) {
                m_img_attr_selected.Dispose();
            }*/
        }
    }
}
