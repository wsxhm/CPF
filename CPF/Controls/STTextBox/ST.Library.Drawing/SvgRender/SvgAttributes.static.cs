using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ST.Library.Drawing.SvgRender
{
    public delegate bool SvgAttributeValueCheckCallBack(string strValue);

    public partial class SvgAttributes
    {
        private static Regex m_reg_number = new Regex(@"([+-]?(?:\.\d+|\d+(?:\.\d+)?)(?:[eE]-\d+)?)");
        private static Regex m_reg_angle = new Regex(@"([+-]?(?:\.\d+|\d+(?:\.\d+)?)(?:[eE]-\d+)?)(deg|grad|rad)?");
        private static Regex m_reg_size = new Regex(@"([+-]?(?:\.\d+|\d+(?:\.\d+)?)(?:[eE]-\d+)?)(em|ex|px|in|cm|mm|pt|pc|%)?");
        private static Regex m_reg_transform = new Regex(@"(rotate|scale|scaleX|scaleY|skew|skewX|skewY|translate|translateX|translateY|matrix)\((.*?)\)");
        private static Dictionary<string, Color> m_dic_color = new Dictionary<string, Color>();
        private static Dictionary<string, SvgAttributeValueCheckCallBack> m_dic_value_check = new Dictionary<string, SvgAttributeValueCheckCallBack>();
        private static Dictionary<string, string> m_dic_static_default = new Dictionary<string, string>();

        static SvgAttributes() {
            SvgAttributes.InitColorTable();
            SvgAttributes.InitDefaultCallBack();
            SvgAttributes.InitDefaultValue();
        }

        private static void InitColorTable() {
            m_dic_color.Add("aliceblue", Color.FromArgb(240, 248, 255));
            m_dic_color.Add("antiquewhite", Color.FromArgb(250, 235, 215));
            m_dic_color.Add("antiquewhite1", Color.FromArgb(255, 239, 219));
            m_dic_color.Add("antiquewhite2", Color.FromArgb(238, 223, 204));
            m_dic_color.Add("antiquewhite3", Color.FromArgb(205, 192, 176));
            m_dic_color.Add("antiquewhite4", Color.FromArgb(139, 131, 120));
            m_dic_color.Add("aquamarine", Color.FromArgb(127, 255, 212));
            m_dic_color.Add("aquamarine1", Color.FromArgb(127, 255, 212));
            m_dic_color.Add("aquamarine2", Color.FromArgb(118, 238, 198));
            m_dic_color.Add("aquamarine3", Color.FromArgb(102, 205, 170));
            m_dic_color.Add("aquamarine4", Color.FromArgb(69, 139, 116));
            m_dic_color.Add("azure", Color.FromArgb(240, 255, 255));
            m_dic_color.Add("azure1", Color.FromArgb(240, 255, 255));
            m_dic_color.Add("azure2", Color.FromArgb(224, 238, 238));
            m_dic_color.Add("azure3", Color.FromArgb(193, 205, 205));
            m_dic_color.Add("azure4", Color.FromArgb(131, 139, 139));
            m_dic_color.Add("beige", Color.FromArgb(245, 245, 220));
            m_dic_color.Add("bisque", Color.FromArgb(255, 228, 196));
            m_dic_color.Add("bisque1", Color.FromArgb(255, 228, 196));
            m_dic_color.Add("bisque2", Color.FromArgb(238, 213, 183));
            m_dic_color.Add("bisque3", Color.FromArgb(205, 183, 158));
            m_dic_color.Add("bisque4", Color.FromArgb(139, 125, 107));
            m_dic_color.Add("black", Color.FromArgb(0, 0, 0));
            m_dic_color.Add("blanchedalmond", Color.FromArgb(255, 235, 205));
            m_dic_color.Add("blue", Color.FromArgb(0, 0, 255));
            m_dic_color.Add("blue1", Color.FromArgb(0, 0, 255));
            m_dic_color.Add("blue2", Color.FromArgb(0, 0, 238));
            m_dic_color.Add("blue3", Color.FromArgb(0, 0, 205));
            m_dic_color.Add("blue4", Color.FromArgb(0, 0, 139));
            m_dic_color.Add("blueviolet", Color.FromArgb(138, 43, 226));
            m_dic_color.Add("brown", Color.FromArgb(165, 42, 42));
            m_dic_color.Add("brown1", Color.FromArgb(255, 64, 64));
            m_dic_color.Add("brown2", Color.FromArgb(238, 59, 59));
            m_dic_color.Add("brown3", Color.FromArgb(205, 51, 51));
            m_dic_color.Add("brown4", Color.FromArgb(139, 35, 35));
            m_dic_color.Add("burlywood", Color.FromArgb(222, 184, 135));
            m_dic_color.Add("burlywood1", Color.FromArgb(255, 211, 155));
            m_dic_color.Add("burlywood2", Color.FromArgb(238, 197, 145));
            m_dic_color.Add("burlywood3", Color.FromArgb(205, 170, 125));
            m_dic_color.Add("burlywood4", Color.FromArgb(139, 115, 85));
            m_dic_color.Add("cadetblue", Color.FromArgb(95, 158, 160));
            m_dic_color.Add("cadetblue1", Color.FromArgb(152, 245, 255));
            m_dic_color.Add("cadetblue2", Color.FromArgb(142, 229, 238));
            m_dic_color.Add("cadetblue3", Color.FromArgb(122, 197, 205));
            m_dic_color.Add("cadetblue4", Color.FromArgb(83, 134, 139));
            m_dic_color.Add("chartreuse", Color.FromArgb(127, 255, 0));
            m_dic_color.Add("chartreuse1", Color.FromArgb(127, 255, 0));
            m_dic_color.Add("chartreuse2", Color.FromArgb(118, 238, 0));
            m_dic_color.Add("chartreuse3", Color.FromArgb(102, 205, 0));
            m_dic_color.Add("chartreuse4", Color.FromArgb(69, 139, 0));
            m_dic_color.Add("chocolate", Color.FromArgb(210, 105, 30));
            m_dic_color.Add("chocolate1", Color.FromArgb(255, 127, 36));
            m_dic_color.Add("chocolate2", Color.FromArgb(238, 118, 33));
            m_dic_color.Add("chocolate3", Color.FromArgb(205, 102, 29));
            m_dic_color.Add("chocolate4", Color.FromArgb(139, 69, 19));
            m_dic_color.Add("coral", Color.FromArgb(255, 127, 80));
            m_dic_color.Add("coral1", Color.FromArgb(255, 114, 86));
            m_dic_color.Add("coral2", Color.FromArgb(238, 106, 80));
            m_dic_color.Add("coral3", Color.FromArgb(205, 91, 69));
            m_dic_color.Add("coral4", Color.FromArgb(139, 62, 47));
            m_dic_color.Add("cornflowerblue", Color.FromArgb(100, 149, 237));
            m_dic_color.Add("cornsilk", Color.FromArgb(255, 248, 220));
            m_dic_color.Add("cornsilk1", Color.FromArgb(255, 248, 220));
            m_dic_color.Add("cornsilk2", Color.FromArgb(238, 232, 205));
            m_dic_color.Add("cornsilk3", Color.FromArgb(205, 200, 177));
            m_dic_color.Add("cornsilk4", Color.FromArgb(139, 136, 120));
            m_dic_color.Add("cyan", Color.FromArgb(0, 255, 255));
            m_dic_color.Add("cyan1", Color.FromArgb(0, 255, 255));
            m_dic_color.Add("cyan2", Color.FromArgb(0, 238, 238));
            m_dic_color.Add("cyan3", Color.FromArgb(0, 205, 205));
            m_dic_color.Add("cyan4", Color.FromArgb(0, 139, 139));
            m_dic_color.Add("darkblue", Color.FromArgb(0, 0, 139));
            m_dic_color.Add("darkcyan", Color.FromArgb(0, 139, 139));
            m_dic_color.Add("darkgoldenrod", Color.FromArgb(184, 134, 11));
            m_dic_color.Add("darkgoldenrod1", Color.FromArgb(255, 185, 15));
            m_dic_color.Add("darkgoldenrod2", Color.FromArgb(238, 173, 14));
            m_dic_color.Add("darkgoldenrod3", Color.FromArgb(205, 149, 12));
            m_dic_color.Add("darkgoldenrod4", Color.FromArgb(139, 101, 8));
            m_dic_color.Add("darkgreen", Color.FromArgb(0, 100, 0));
            m_dic_color.Add("darkgrey", Color.FromArgb(169, 169, 169));
            m_dic_color.Add("darkkhaki", Color.FromArgb(189, 183, 107));
            m_dic_color.Add("darkmagenta", Color.FromArgb(139, 0, 139));
            m_dic_color.Add("darkolivegreen", Color.FromArgb(85, 107, 47));
            m_dic_color.Add("darkolivegreen1", Color.FromArgb(202, 255, 112));
            m_dic_color.Add("darkolivegreen2", Color.FromArgb(188, 238, 104));
            m_dic_color.Add("darkolivegreen3", Color.FromArgb(162, 205, 90));
            m_dic_color.Add("darkolivegreen4", Color.FromArgb(110, 139, 61));
            m_dic_color.Add("darkorange", Color.FromArgb(255, 140, 0));
            m_dic_color.Add("darkorange1", Color.FromArgb(255, 127, 0));
            m_dic_color.Add("darkorange2", Color.FromArgb(238, 118, 0));
            m_dic_color.Add("darkorange3", Color.FromArgb(205, 102, 0));
            m_dic_color.Add("darkorange4", Color.FromArgb(139, 69, 0));
            m_dic_color.Add("darkorchid", Color.FromArgb(153, 50, 204));
            m_dic_color.Add("darkorchid1", Color.FromArgb(191, 62, 255));
            m_dic_color.Add("darkorchid2", Color.FromArgb(178, 58, 238));
            m_dic_color.Add("darkorchid3", Color.FromArgb(154, 50, 205));
            m_dic_color.Add("darkorchid4", Color.FromArgb(104, 34, 139));
            m_dic_color.Add("darkred", Color.FromArgb(139, 0, 0));
            m_dic_color.Add("darksalmon", Color.FromArgb(233, 150, 122));
            m_dic_color.Add("darkseagreen", Color.FromArgb(143, 188, 143));
            m_dic_color.Add("darkseagreen1", Color.FromArgb(193, 255, 193));
            m_dic_color.Add("darkseagreen2", Color.FromArgb(180, 238, 180));
            m_dic_color.Add("darkseagreen3", Color.FromArgb(155, 205, 155));
            m_dic_color.Add("darkseagreen4", Color.FromArgb(105, 139, 105));
            m_dic_color.Add("darkslateblue", Color.FromArgb(72, 61, 139));
            m_dic_color.Add("darkslategray", Color.FromArgb(47, 79, 79));
            m_dic_color.Add("darkslategray1", Color.FromArgb(151, 255, 255));
            m_dic_color.Add("darkslategray2", Color.FromArgb(141, 238, 238));
            m_dic_color.Add("darkslategray3", Color.FromArgb(121, 205, 205));
            m_dic_color.Add("darkslategray4", Color.FromArgb(82, 139, 139));
            m_dic_color.Add("darkturquoise", Color.FromArgb(0, 206, 209));
            m_dic_color.Add("darkviolet", Color.FromArgb(148, 0, 211));
            m_dic_color.Add("deeppink", Color.FromArgb(255, 20, 147));
            m_dic_color.Add("deeppink1", Color.FromArgb(255, 20, 147));
            m_dic_color.Add("deeppink2", Color.FromArgb(238, 18, 137));
            m_dic_color.Add("deeppink3", Color.FromArgb(205, 16, 118));
            m_dic_color.Add("deeppink4", Color.FromArgb(139, 10, 80));
            m_dic_color.Add("deepskyblue", Color.FromArgb(0, 191, 255));
            m_dic_color.Add("deepskyblue1", Color.FromArgb(0, 191, 255));
            m_dic_color.Add("deepskyblue2", Color.FromArgb(0, 178, 238));
            m_dic_color.Add("deepskyblue3", Color.FromArgb(0, 154, 205));
            m_dic_color.Add("deepskyblue4", Color.FromArgb(0, 104, 139));
            m_dic_color.Add("dimgrey", Color.FromArgb(105, 105, 105));
            m_dic_color.Add("dodgerblue", Color.FromArgb(30, 144, 255));
            m_dic_color.Add("dodgerblue1", Color.FromArgb(30, 144, 255));
            m_dic_color.Add("dodgerblue2", Color.FromArgb(28, 134, 238));
            m_dic_color.Add("dodgerblue3", Color.FromArgb(24, 116, 205));
            m_dic_color.Add("dodgerblue4", Color.FromArgb(16, 78, 139));
            m_dic_color.Add("firebrick", Color.FromArgb(178, 34, 34));
            m_dic_color.Add("firebrick1", Color.FromArgb(255, 48, 48));
            m_dic_color.Add("firebrick2", Color.FromArgb(238, 44, 44));
            m_dic_color.Add("firebrick3", Color.FromArgb(205, 38, 38));
            m_dic_color.Add("firebrick4", Color.FromArgb(139, 26, 26));
            m_dic_color.Add("floralwhite", Color.FromArgb(255, 250, 240));
            m_dic_color.Add("forestgreen", Color.FromArgb(34, 139, 34));
            m_dic_color.Add("gainsboro", Color.FromArgb(220, 220, 220));
            m_dic_color.Add("ghostwhite", Color.FromArgb(248, 248, 255));
            m_dic_color.Add("gold", Color.FromArgb(255, 215, 0));
            m_dic_color.Add("gold1", Color.FromArgb(255, 215, 0));
            m_dic_color.Add("gold2", Color.FromArgb(238, 201, 0));
            m_dic_color.Add("gold3", Color.FromArgb(205, 173, 0));
            m_dic_color.Add("gold4", Color.FromArgb(139, 117, 0));
            m_dic_color.Add("goldenrod1", Color.FromArgb(255, 193, 37));
            m_dic_color.Add("goldenrod2", Color.FromArgb(238, 180, 34));
            m_dic_color.Add("goldenrod3", Color.FromArgb(205, 155, 29));
            m_dic_color.Add("goldenrod4", Color.FromArgb(139, 105, 20));
            m_dic_color.Add("green", Color.FromArgb(0, 255, 0));
            m_dic_color.Add("green1", Color.FromArgb(0, 255, 0));
            m_dic_color.Add("green2", Color.FromArgb(0, 238, 0));
            m_dic_color.Add("green3", Color.FromArgb(0, 205, 0));
            m_dic_color.Add("green4", Color.FromArgb(0, 139, 0));
            m_dic_color.Add("greenyellow", Color.FromArgb(173, 255, 47));
            m_dic_color.Add("grey", Color.FromArgb(190, 190, 190));
            m_dic_color.Add("honeydew", Color.FromArgb(240, 255, 240));
            m_dic_color.Add("honeydew1", Color.FromArgb(240, 255, 240));
            m_dic_color.Add("honeydew2", Color.FromArgb(224, 238, 224));
            m_dic_color.Add("honeydew3", Color.FromArgb(193, 205, 193));
            m_dic_color.Add("honeydew4", Color.FromArgb(131, 139, 131));
            m_dic_color.Add("hotpink", Color.FromArgb(255, 105, 180));
            m_dic_color.Add("hotpink1", Color.FromArgb(255, 110, 180));
            m_dic_color.Add("hotpink2", Color.FromArgb(238, 106, 167));
            m_dic_color.Add("hotpink3", Color.FromArgb(205, 96, 144));
            m_dic_color.Add("hotpink4", Color.FromArgb(139, 58, 98));
            m_dic_color.Add("indianred", Color.FromArgb(205, 92, 92));
            m_dic_color.Add("indianred1", Color.FromArgb(255, 106, 106));
            m_dic_color.Add("indianred2", Color.FromArgb(238, 99, 99));
            m_dic_color.Add("indianred3", Color.FromArgb(205, 85, 85));
            m_dic_color.Add("indianred4", Color.FromArgb(139, 58, 58));
            m_dic_color.Add("ivory", Color.FromArgb(255, 255, 240));
            m_dic_color.Add("ivory1", Color.FromArgb(255, 255, 240));
            m_dic_color.Add("ivory2", Color.FromArgb(238, 238, 224));
            m_dic_color.Add("ivory3", Color.FromArgb(205, 205, 193));
            m_dic_color.Add("ivory4", Color.FromArgb(139, 139, 131));
            m_dic_color.Add("khaki1", Color.FromArgb(255, 246, 143));
            m_dic_color.Add("khaki2", Color.FromArgb(238, 230, 133));
            m_dic_color.Add("khaki3", Color.FromArgb(205, 198, 115));
            m_dic_color.Add("khaki4", Color.FromArgb(139, 134, 78));
            m_dic_color.Add("lavenderblush", Color.FromArgb(255, 240, 245));
            m_dic_color.Add("lavenderblush1", Color.FromArgb(255, 240, 245));
            m_dic_color.Add("lavenderblush2", Color.FromArgb(238, 224, 229));
            m_dic_color.Add("lavenderblush3", Color.FromArgb(205, 193, 197));
            m_dic_color.Add("lavenderblush4", Color.FromArgb(139, 131, 134));
            m_dic_color.Add("lawngreen", Color.FromArgb(124, 252, 0));
            m_dic_color.Add("lemonchiffon", Color.FromArgb(255, 250, 205));
            m_dic_color.Add("lemonchiffon1", Color.FromArgb(255, 250, 205));
            m_dic_color.Add("lemonchiffon2", Color.FromArgb(238, 233, 191));
            m_dic_color.Add("lemonchiffon3", Color.FromArgb(205, 201, 165));
            m_dic_color.Add("lemonchiffon4", Color.FromArgb(139, 137, 112));
            m_dic_color.Add("lightblue", Color.FromArgb(173, 216, 230));
            m_dic_color.Add("lightblue1", Color.FromArgb(191, 239, 255));
            m_dic_color.Add("lightblue2", Color.FromArgb(178, 223, 238));
            m_dic_color.Add("lightblue3", Color.FromArgb(154, 192, 205));
            m_dic_color.Add("lightblue4", Color.FromArgb(104, 131, 139));
            m_dic_color.Add("lightcoral", Color.FromArgb(240, 128, 128));
            m_dic_color.Add("lightcyan", Color.FromArgb(224, 255, 255));
            m_dic_color.Add("lightcyan1", Color.FromArgb(224, 255, 255));
            m_dic_color.Add("lightcyan2", Color.FromArgb(209, 238, 238));
            m_dic_color.Add("lightcyan3", Color.FromArgb(180, 205, 205));
            m_dic_color.Add("lightcyan4", Color.FromArgb(122, 139, 139));
            m_dic_color.Add("lightgoldenrod", Color.FromArgb(238, 221, 130));
            m_dic_color.Add("lightgoldenrod1", Color.FromArgb(255, 236, 139));
            m_dic_color.Add("lightgoldenrod2", Color.FromArgb(238, 220, 130));
            m_dic_color.Add("lightgoldenrod3", Color.FromArgb(205, 190, 112));
            m_dic_color.Add("lightgoldenrod4", Color.FromArgb(139, 129, 76));
            m_dic_color.Add("lightgray", Color.FromArgb(211, 211, 211));
            m_dic_color.Add("lightgreen", Color.FromArgb(144, 238, 144));
            m_dic_color.Add("lightpink", Color.FromArgb(255, 182, 193));
            m_dic_color.Add("lightpink1", Color.FromArgb(255, 174, 185));
            m_dic_color.Add("lightpink2", Color.FromArgb(238, 162, 173));
            m_dic_color.Add("lightpink3", Color.FromArgb(205, 140, 149));
            m_dic_color.Add("lightpink4", Color.FromArgb(139, 95, 101));
            m_dic_color.Add("lightsalmon", Color.FromArgb(255, 160, 122));
            m_dic_color.Add("lightsalmon1", Color.FromArgb(255, 160, 122));
            m_dic_color.Add("lightsalmon2", Color.FromArgb(238, 149, 114));
            m_dic_color.Add("lightsalmon3", Color.FromArgb(205, 129, 98));
            m_dic_color.Add("lightsalmon4", Color.FromArgb(139, 87, 66));
            m_dic_color.Add("lightseagreen", Color.FromArgb(32, 178, 170));
            m_dic_color.Add("lightskyblue", Color.FromArgb(135, 206, 250));
            m_dic_color.Add("lightskyblue1", Color.FromArgb(176, 226, 255));
            m_dic_color.Add("lightskyblue2", Color.FromArgb(164, 211, 238));
            m_dic_color.Add("lightskyblue3", Color.FromArgb(141, 182, 205));
            m_dic_color.Add("lightskyblue4", Color.FromArgb(96, 123, 139));
            m_dic_color.Add("lightslateblue", Color.FromArgb(132, 112, 255));
            m_dic_color.Add("lightslategray", Color.FromArgb(119, 136, 153));
            m_dic_color.Add("lightsteelblue", Color.FromArgb(176, 196, 222));
            m_dic_color.Add("lightsteelblue1", Color.FromArgb(202, 225, 255));
            m_dic_color.Add("lightsteelblue2", Color.FromArgb(188, 210, 238));
            m_dic_color.Add("lightsteelblue3", Color.FromArgb(162, 181, 205));
            m_dic_color.Add("lightsteelblue4", Color.FromArgb(110, 123, 139));
            m_dic_color.Add("lightyellow", Color.FromArgb(255, 255, 224));
            m_dic_color.Add("lightyellow1", Color.FromArgb(255, 255, 224));
            m_dic_color.Add("lightyellow2", Color.FromArgb(238, 238, 209));
            m_dic_color.Add("lightyellow3", Color.FromArgb(205, 205, 180));
            m_dic_color.Add("lightyellow4", Color.FromArgb(139, 139, 122));
            m_dic_color.Add("limegreen", Color.FromArgb(50, 205, 50));
            m_dic_color.Add("linen", Color.FromArgb(250, 240, 230));
            m_dic_color.Add("ltgoldenrodyello", Color.FromArgb(250, 250, 210));
            m_dic_color.Add("magenta", Color.FromArgb(255, 0, 255));
            m_dic_color.Add("magenta1", Color.FromArgb(255, 0, 255));
            m_dic_color.Add("magenta2", Color.FromArgb(238, 0, 238));
            m_dic_color.Add("magenta3", Color.FromArgb(205, 0, 205));
            m_dic_color.Add("magenta4", Color.FromArgb(139, 0, 139));
            m_dic_color.Add("maroon", Color.FromArgb(176, 48, 96));
            m_dic_color.Add("maroon1", Color.FromArgb(255, 52, 179));
            m_dic_color.Add("maroon2", Color.FromArgb(238, 48, 167));
            m_dic_color.Add("maroon3", Color.FromArgb(205, 41, 144));
            m_dic_color.Add("maroon4", Color.FromArgb(139, 28, 98));
            m_dic_color.Add("medspringgreen", Color.FromArgb(0, 250, 154));
            m_dic_color.Add("mediumaquamarine", Color.FromArgb(102, 205, 170));
            m_dic_color.Add("mediumblue", Color.FromArgb(0, 0, 205));
            m_dic_color.Add("mediumorchid", Color.FromArgb(186, 85, 211));
            m_dic_color.Add("mediumorchid1", Color.FromArgb(224, 102, 255));
            m_dic_color.Add("mediumorchid2", Color.FromArgb(209, 95, 238));
            m_dic_color.Add("mediumorchid3", Color.FromArgb(180, 82, 205));
            m_dic_color.Add("mediumorchid4", Color.FromArgb(122, 55, 139));
            m_dic_color.Add("mediumpurple", Color.FromArgb(147, 112, 219));
            m_dic_color.Add("mediumpurple1", Color.FromArgb(171, 130, 255));
            m_dic_color.Add("mediumpurple2", Color.FromArgb(159, 121, 238));
            m_dic_color.Add("mediumpurple3", Color.FromArgb(137, 104, 205));
            m_dic_color.Add("mediumpurple4", Color.FromArgb(93, 71, 139));
            m_dic_color.Add("mediumseagreen", Color.FromArgb(60, 179, 113));
            m_dic_color.Add("mediumslateblue", Color.FromArgb(123, 104, 238));
            m_dic_color.Add("mediumturquoise", Color.FromArgb(72, 209, 204));
            m_dic_color.Add("mediumvioletred", Color.FromArgb(199, 21, 133));
            m_dic_color.Add("midnightblue", Color.FromArgb(25, 25, 112));
            m_dic_color.Add("mintcream", Color.FromArgb(245, 255, 250));
            m_dic_color.Add("mistyrose", Color.FromArgb(255, 228, 225));
            m_dic_color.Add("mistyrose1", Color.FromArgb(255, 228, 225));
            m_dic_color.Add("mistyrose2", Color.FromArgb(238, 213, 210));
            m_dic_color.Add("mistyrose3", Color.FromArgb(205, 183, 181));
            m_dic_color.Add("mistyrose4", Color.FromArgb(139, 125, 123));
            m_dic_color.Add("moccasin", Color.FromArgb(255, 228, 181));
            m_dic_color.Add("navajowhite", Color.FromArgb(255, 222, 173));
            m_dic_color.Add("navajowhite1", Color.FromArgb(255, 222, 173));
            m_dic_color.Add("navajowhite2", Color.FromArgb(238, 207, 161));
            m_dic_color.Add("navajowhite3", Color.FromArgb(205, 179, 139));
            m_dic_color.Add("navajowhite4", Color.FromArgb(139, 121, 94));
            m_dic_color.Add("navyblue", Color.FromArgb(0, 0, 128));
            m_dic_color.Add("oldlace", Color.FromArgb(253, 245, 230));
            m_dic_color.Add("olivedrab", Color.FromArgb(107, 142, 35));
            m_dic_color.Add("olivedrab1", Color.FromArgb(192, 255, 62));
            m_dic_color.Add("olivedrab2", Color.FromArgb(179, 238, 58));
            m_dic_color.Add("olivedrab3", Color.FromArgb(154, 205, 50));
            m_dic_color.Add("olivedrab4", Color.FromArgb(105, 139, 34));
            m_dic_color.Add("orange", Color.FromArgb(255, 165, 0));
            m_dic_color.Add("orange1", Color.FromArgb(255, 165, 0));
            m_dic_color.Add("orange2", Color.FromArgb(238, 154, 0));
            m_dic_color.Add("orange3", Color.FromArgb(205, 133, 0));
            m_dic_color.Add("orange4", Color.FromArgb(139, 90, 0));
            m_dic_color.Add("orangered", Color.FromArgb(255, 69, 0));
            m_dic_color.Add("orangered1", Color.FromArgb(255, 69, 0));
            m_dic_color.Add("orangered2", Color.FromArgb(238, 64, 0));
            m_dic_color.Add("orangered3", Color.FromArgb(205, 55, 0));
            m_dic_color.Add("orangered4", Color.FromArgb(139, 37, 0));
            m_dic_color.Add("orchid", Color.FromArgb(218, 112, 214));
            m_dic_color.Add("orchid1", Color.FromArgb(255, 131, 250));
            m_dic_color.Add("orchid2", Color.FromArgb(238, 122, 233));
            m_dic_color.Add("orchid3", Color.FromArgb(205, 105, 201));
            m_dic_color.Add("orchid4", Color.FromArgb(139, 71, 137));
            m_dic_color.Add("palegoldenrod", Color.FromArgb(238, 232, 170));
            m_dic_color.Add("palegreen", Color.FromArgb(152, 251, 152));
            m_dic_color.Add("palegreen1", Color.FromArgb(154, 255, 154));
            m_dic_color.Add("palegreen2", Color.FromArgb(144, 238, 144));
            m_dic_color.Add("palegreen3", Color.FromArgb(124, 205, 124));
            m_dic_color.Add("palegreen4", Color.FromArgb(84, 139, 84));
            m_dic_color.Add("paleturquoise", Color.FromArgb(175, 238, 238));
            m_dic_color.Add("paleturquoise1", Color.FromArgb(187, 255, 255));
            m_dic_color.Add("paleturquoise2", Color.FromArgb(174, 238, 238));
            m_dic_color.Add("paleturquoise3", Color.FromArgb(150, 205, 205));
            m_dic_color.Add("paleturquoise4", Color.FromArgb(102, 139, 139));
            m_dic_color.Add("palevioletred", Color.FromArgb(219, 112, 147));
            m_dic_color.Add("palevioletred1", Color.FromArgb(255, 130, 171));
            m_dic_color.Add("palevioletred2", Color.FromArgb(238, 121, 159));
            m_dic_color.Add("palevioletred3", Color.FromArgb(205, 104, 137));
            m_dic_color.Add("palevioletred4", Color.FromArgb(139, 71, 93));
            m_dic_color.Add("papayawhip", Color.FromArgb(255, 239, 213));
            m_dic_color.Add("peachpuff", Color.FromArgb(255, 218, 185));
            m_dic_color.Add("peachpuff1", Color.FromArgb(255, 218, 185));
            m_dic_color.Add("peachpuff2", Color.FromArgb(238, 203, 173));
            m_dic_color.Add("peachpuff3", Color.FromArgb(205, 175, 149));
            m_dic_color.Add("peachpuff4", Color.FromArgb(139, 119, 101));
            m_dic_color.Add("peru", Color.FromArgb(205, 133, 63));
            m_dic_color.Add("pink", Color.FromArgb(255, 192, 203));
            m_dic_color.Add("pink1", Color.FromArgb(255, 181, 197));
            m_dic_color.Add("pink2", Color.FromArgb(238, 169, 184));
            m_dic_color.Add("pink3", Color.FromArgb(205, 145, 158));
            m_dic_color.Add("pink4", Color.FromArgb(139, 99, 108));
            m_dic_color.Add("plum", Color.FromArgb(221, 160, 221));
            m_dic_color.Add("plum1", Color.FromArgb(255, 187, 255));
            m_dic_color.Add("plum2", Color.FromArgb(238, 174, 238));
            m_dic_color.Add("plum3", Color.FromArgb(205, 150, 205));
            m_dic_color.Add("plum4", Color.FromArgb(139, 102, 139));
            m_dic_color.Add("powderblue", Color.FromArgb(176, 224, 230));
            m_dic_color.Add("purple", Color.FromArgb(160, 32, 240));
            m_dic_color.Add("purple1", Color.FromArgb(155, 48, 255));
            m_dic_color.Add("purple2", Color.FromArgb(145, 44, 238));
            m_dic_color.Add("purple3", Color.FromArgb(125, 38, 205));
            m_dic_color.Add("purple4", Color.FromArgb(85, 26, 139));
            m_dic_color.Add("red", Color.FromArgb(255, 0, 0));
            m_dic_color.Add("red1", Color.FromArgb(255, 0, 0));
            m_dic_color.Add("red2", Color.FromArgb(238, 0, 0));
            m_dic_color.Add("red3", Color.FromArgb(205, 0, 0));
            m_dic_color.Add("red4", Color.FromArgb(139, 0, 0));
            m_dic_color.Add("rosybrown", Color.FromArgb(188, 143, 143));
            m_dic_color.Add("rosybrown1", Color.FromArgb(255, 193, 193));
            m_dic_color.Add("rosybrown2", Color.FromArgb(238, 180, 180));
            m_dic_color.Add("rosybrown3", Color.FromArgb(205, 155, 155));
            m_dic_color.Add("rosybrown4", Color.FromArgb(139, 105, 105));
            m_dic_color.Add("royalblue", Color.FromArgb(65, 105, 225));
            m_dic_color.Add("royalblue1", Color.FromArgb(72, 118, 255));
            m_dic_color.Add("royalblue2", Color.FromArgb(67, 110, 238));
            m_dic_color.Add("royalblue3", Color.FromArgb(58, 95, 205));
            m_dic_color.Add("royalblue4", Color.FromArgb(39, 64, 139));
            m_dic_color.Add("saddlebrown", Color.FromArgb(139, 69, 19));
            m_dic_color.Add("salmon", Color.FromArgb(250, 128, 114));
            m_dic_color.Add("salmon1", Color.FromArgb(255, 140, 105));
            m_dic_color.Add("salmon2", Color.FromArgb(238, 130, 98));
            m_dic_color.Add("salmon3", Color.FromArgb(205, 112, 84));
            m_dic_color.Add("salmon4", Color.FromArgb(139, 76, 57));
            m_dic_color.Add("sandybrown", Color.FromArgb(244, 164, 96));
            m_dic_color.Add("seagreen", Color.FromArgb(46, 139, 87));
            m_dic_color.Add("seagreen1", Color.FromArgb(84, 255, 159));
            m_dic_color.Add("seagreen2", Color.FromArgb(78, 238, 148));
            m_dic_color.Add("seagreen3", Color.FromArgb(67, 205, 128));
            m_dic_color.Add("seagreen4", Color.FromArgb(46, 139, 87));
            m_dic_color.Add("seashell", Color.FromArgb(255, 245, 238));
            m_dic_color.Add("seashell1", Color.FromArgb(255, 245, 238));
            m_dic_color.Add("seashell2", Color.FromArgb(238, 229, 222));
            m_dic_color.Add("seashell3", Color.FromArgb(205, 197, 191));
            m_dic_color.Add("seashell4", Color.FromArgb(139, 134, 130));
            m_dic_color.Add("sienna", Color.FromArgb(160, 82, 45));
            m_dic_color.Add("sienna1", Color.FromArgb(255, 130, 71));
            m_dic_color.Add("sienna2", Color.FromArgb(238, 121, 66));
            m_dic_color.Add("sienna3", Color.FromArgb(205, 104, 57));
            m_dic_color.Add("sienna4", Color.FromArgb(139, 71, 38));
            m_dic_color.Add("skyblue", Color.FromArgb(135, 206, 235));
            m_dic_color.Add("skyblue1", Color.FromArgb(135, 206, 255));
            m_dic_color.Add("skyblue2", Color.FromArgb(126, 192, 238));
            m_dic_color.Add("skyblue3", Color.FromArgb(108, 166, 205));
            m_dic_color.Add("skyblue4", Color.FromArgb(74, 112, 139));
            m_dic_color.Add("slateblue", Color.FromArgb(106, 90, 205));
            m_dic_color.Add("slateblue1", Color.FromArgb(131, 111, 255));
            m_dic_color.Add("slateblue2", Color.FromArgb(122, 103, 238));
            m_dic_color.Add("slateblue3", Color.FromArgb(105, 89, 205));
            m_dic_color.Add("slateblue4", Color.FromArgb(71, 60, 139));
            m_dic_color.Add("slategray1", Color.FromArgb(198, 226, 255));
            m_dic_color.Add("slategray2", Color.FromArgb(185, 211, 238));
            m_dic_color.Add("slategray3", Color.FromArgb(159, 182, 205));
            m_dic_color.Add("slategray4", Color.FromArgb(108, 123, 139));
            m_dic_color.Add("slategrey", Color.FromArgb(112, 128, 144));
            m_dic_color.Add("snow", Color.FromArgb(255, 250, 250));
            m_dic_color.Add("snow1", Color.FromArgb(255, 250, 250));
            m_dic_color.Add("snow2", Color.FromArgb(238, 233, 233));
            m_dic_color.Add("snow3", Color.FromArgb(205, 201, 201));
            m_dic_color.Add("snow4", Color.FromArgb(139, 137, 137));
            m_dic_color.Add("springgreen", Color.FromArgb(0, 255, 127));
            m_dic_color.Add("springgreen1", Color.FromArgb(0, 255, 127));
            m_dic_color.Add("springgreen2", Color.FromArgb(0, 238, 118));
            m_dic_color.Add("springgreen3", Color.FromArgb(0, 205, 102));
            m_dic_color.Add("springgreen4", Color.FromArgb(0, 139, 69));
            m_dic_color.Add("steelblue", Color.FromArgb(70, 130, 180));
            m_dic_color.Add("steelblue1", Color.FromArgb(99, 184, 255));
            m_dic_color.Add("steelblue2", Color.FromArgb(92, 172, 238));
            m_dic_color.Add("steelblue3", Color.FromArgb(79, 148, 205));
            m_dic_color.Add("steelblue4", Color.FromArgb(54, 100, 139));
            m_dic_color.Add("tan", Color.FromArgb(210, 180, 140));
            m_dic_color.Add("tan1", Color.FromArgb(255, 165, 79));
            m_dic_color.Add("tan2", Color.FromArgb(238, 154, 73));
            m_dic_color.Add("tan3", Color.FromArgb(205, 133, 63));
            m_dic_color.Add("tan4", Color.FromArgb(139, 90, 43));
            m_dic_color.Add("thistle", Color.FromArgb(216, 191, 216));
            m_dic_color.Add("thistle1", Color.FromArgb(255, 225, 255));
            m_dic_color.Add("thistle2", Color.FromArgb(238, 210, 238));
            m_dic_color.Add("thistle3", Color.FromArgb(205, 181, 205));
            m_dic_color.Add("thistle4", Color.FromArgb(139, 123, 139));
            m_dic_color.Add("tomato", Color.FromArgb(255, 99, 71));
            m_dic_color.Add("tomato1", Color.FromArgb(255, 99, 71));
            m_dic_color.Add("tomato2", Color.FromArgb(238, 92, 66));
            m_dic_color.Add("tomato3", Color.FromArgb(205, 79, 57));
            m_dic_color.Add("tomato4", Color.FromArgb(139, 54, 38));
            m_dic_color.Add("turquoise", Color.FromArgb(64, 224, 208));
            m_dic_color.Add("turquoise1", Color.FromArgb(0, 245, 255));
            m_dic_color.Add("turquoise2", Color.FromArgb(0, 229, 238));
            m_dic_color.Add("turquoise3", Color.FromArgb(0, 197, 205));
            m_dic_color.Add("turquoise4", Color.FromArgb(0, 134, 139));
            m_dic_color.Add("violet", Color.FromArgb(238, 130, 238));
            m_dic_color.Add("violetred", Color.FromArgb(208, 32, 144));
            m_dic_color.Add("violetred1", Color.FromArgb(255, 62, 150));
            m_dic_color.Add("violetred2", Color.FromArgb(238, 58, 140));
            m_dic_color.Add("violetred3", Color.FromArgb(205, 50, 120));
            m_dic_color.Add("violetred4", Color.FromArgb(139, 34, 82));
            m_dic_color.Add("wheat", Color.FromArgb(245, 222, 179));
            m_dic_color.Add("wheat1", Color.FromArgb(255, 231, 186));
            m_dic_color.Add("wheat2", Color.FromArgb(238, 216, 174));
            m_dic_color.Add("wheat3", Color.FromArgb(205, 186, 150));
            m_dic_color.Add("wheat4", Color.FromArgb(139, 126, 102));
            m_dic_color.Add("white", Color.FromArgb(255, 255, 255));
            m_dic_color.Add("whitesmoke", Color.FromArgb(245, 245, 245));
            m_dic_color.Add("yellow", Color.FromArgb(255, 255, 0));
            m_dic_color.Add("yellow1", Color.FromArgb(255, 255, 0));
            m_dic_color.Add("yellow2", Color.FromArgb(238, 238, 0));
            m_dic_color.Add("yellow3", Color.FromArgb(205, 205, 0));
            m_dic_color.Add("yellow4", Color.FromArgb(139, 139, 0));
            m_dic_color.Add("yellowgreen", Color.FromArgb(154, 205, 50));
            m_dic_color.Add("goldenrod", Color.FromArgb(218, 165, 32));
            m_dic_color.Add("gray81", Color.FromArgb(207, 207, 207));
            m_dic_color.Add("gray91", Color.FromArgb(232, 232, 232));
            m_dic_color.Add("grey11", Color.FromArgb(28, 28, 28));
            m_dic_color.Add("grey21", Color.FromArgb(54, 54, 54));
            m_dic_color.Add("grey31", Color.FromArgb(79, 79, 79));
            m_dic_color.Add("grey41", Color.FromArgb(105, 105, 105));
            m_dic_color.Add("grey51", Color.FromArgb(130, 130, 130));
            m_dic_color.Add("grey61", Color.FromArgb(156, 156, 156));
            m_dic_color.Add("grey71", Color.FromArgb(181, 181, 181));
            m_dic_color.Add("lavender", Color.FromArgb(230, 230, 250));
            foreach (var v in Enum.GetNames(typeof(KnownColor))) {
                string strKey = v.ToLower();
                if (m_dic_color.ContainsKey(strKey)) {
                    m_dic_color[strKey] = Color.FromName(v);
                } else {
                    m_dic_color.Add(strKey, Color.FromName(v));
                }
            }
            m_dic_color.Add("none", Color.Transparent);
        }

        private static void InitDefaultCallBack() {
            SvgAttributes.SetCheckCallBack("x", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("y", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("x1", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("y1", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("x2", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("y2", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("cx", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("cy", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("rx", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("ry", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("left", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("top", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("right", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("bottom", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("width", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("height", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("stroke-width", SvgAttributes.CheckIsSize);
            SvgAttributes.SetCheckCallBack("opacity", SvgAttributes.CheckIsFloat);
            SvgAttributes.SetCheckCallBack("stroke-opacity", SvgAttributes.CheckIsFloat);
            SvgAttributes.SetCheckCallBack("fill-opacity", SvgAttributes.CheckIsFloat);
            SvgAttributes.SetCheckCallBack("stroke", SvgAttributes.CheckIsColor);
            SvgAttributes.SetCheckCallBack("fill", SvgAttributes.CheckIsColor);
            SvgAttributes.SetCheckCallBack("transform", SvgAttributes.CheckIsTransform);
        }

        private static void InitDefaultValue() {
            SvgAttributes.SetStaticDefault("font-size", "16");
            SvgAttributes.SetStaticDefault("stroke-width", "1");
            SvgAttributes.SetStaticDefault("stroke-opacity", "1");
            SvgAttributes.SetStaticDefault("stroke-miterlimit", "10");
            SvgAttributes.SetStaticDefault("stroke-dashoffset", "0");
            SvgAttributes.SetStaticDefault("stroke-linecap", "butt");
            SvgAttributes.SetStaticDefault("stroke-linejoin", "miter");
            SvgAttributes.SetStaticDefault("fill", "black");
            SvgAttributes.SetStaticDefault("fill-opacity", "1");
            SvgAttributes.SetStaticDefault("fill-rule", "nonzero");
        }

        public static bool SetStaticDefault(string strKey, string strValue) {
            if (m_dic_static_default.ContainsKey(strKey)) {
                if (string.IsNullOrEmpty(strValue)) {
                    m_dic_static_default.Remove(strKey);
                } else {
                    if (m_dic_value_check.ContainsKey(strKey)) {
                        if (!m_dic_value_check[strKey](strValue)) {
                            return false;
                        }
                    }
                    m_dic_static_default[strKey] = strValue;
                }
                return true;
            } else {
                if (m_dic_value_check.ContainsKey(strKey)) {
                    if (!m_dic_value_check[strKey](strValue)) {
                        return false;
                    }
                }
                m_dic_static_default.Add(strKey, strValue);
                return true;
            }
        }

        public static string GetStaticDefault(string strKey) {
            if (m_dic_static_default.ContainsKey(strKey)) {
                return m_dic_static_default[strKey];
            }
            return null;
        }

        public static bool CheckIsHex(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            bool bFlag = false;
            foreach (var v in strText) {
                bFlag |= ('0' <= v && v <= '9');
                bFlag |= ('a' <= v && v <= 'f');
                bFlag |= ('A' <= v && v <= 'F');
                if (!bFlag) {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckIsInt(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            int nIndex = 0;
            if (strText[0] == '-' || strText[0] == '+') {
                nIndex++;
            }
            if (nIndex >= strText.Length) {
                return false;
            }
            for (int i = nIndex; i < strText.Length; i++) {
                if (strText[i] < '0' || strText[i] > '9') {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckIsFloat(string strText) {
            if (string.IsNullOrEmpty(strText)) {
                return false;
            }
            return Regex.IsMatch(strText, @"^[+-]?(\.\d+|\d+(\.\d+)?)([eE]-\d+)?$");
        }

        public static bool CheckIsSize(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            return Regex.IsMatch(strText, @"^[+-]?(\.\d+|\d+(?:\.\d+)?)(em|ex|px|in|cm|mm|pt|pc|%)?$");
        }

        public static bool CheckIsAngle(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            return Regex.IsMatch(strText, @"^[+-]?(\.\d+|\d+(?:\.\d+)?)([eE]-\d+)?(deg|grad|rad)?$");
        }

        public static bool CheckIsColor(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            strText = Regex.Replace(strText, @"\s+", "");
            strText = strText.Trim().ToLower();
            if (m_dic_color.ContainsKey(strText)) {
                return true;
            }
            if (Regex.IsMatch(strText, @"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")) {
                return true;
            }
            if (Regex.IsMatch(strText, @"^rgb\((\d{1,3},){2}\d{1,3}\)$")) {
                return true;
            }
            if (Regex.IsMatch(strText, @"^rgba\((\d{1,3},){3}(\d?\.\d+|\d)\)$")) {
                return true;
            }
            if (Regex.IsMatch(strText, @"^hsl\(\d{1,3}(,(?:\.\d+|\d+(?:\.\d+)?){2})\)$")) {
                return true;
            }
            if (Regex.IsMatch(strText, @"^hsla\(\d{1,3}(,(?:\.\d+|\d+(?:\.\d+)?){2}),(\.\d+|\d(?:\.\d+)?)\)$")) {
                return true;
            }
            if (Regex.IsMatch(strText, @"url\(#.+\)")) {
                return true;
            }
            return false;
        }

        public static bool CheckIsTransform(string strText) {
            if (string.IsNullOrEmpty(strText)) return false;
            var ms = Regex.Matches(strText, @"(?:(rotate|scale|scaleX|scaleY|skew|skewX|skewY|translate|translateX|translateY|matrix)\((.*?)\)[,\s]*)");
            if (ms.Count == 0) {
                return false;
            }
            foreach (Match m in ms) {
                string[] strs = Regex.Split(m.Groups[2].Value.Trim(), @"[,\s]+");
                switch (m.Groups[1].Value) {
                    case "skew":
                    case "skewX":
                    case "skewY":
                    case "rotate":
                        foreach (var v in strs) {
                            if (!SvgAttributes.CheckIsAngle(v)) {
                                return false;
                            }
                        }
                        break;
                    case "scale":
                    case "scaleX":
                    case "scaleY":
                        foreach (var v in strs) {
                            if (!SvgAttributes.CheckIsFloat(v)) {
                                return false;
                            }
                        }
                        break;
                    case "translate":
                    case "translateX":
                    case "translateY":
                        foreach (var v in strs) {
                            if (!SvgAttributes.CheckIsSize(v)) {
                                return false;
                            }
                        }
                        break;
                    case "matrix":
                        foreach (var v in strs) {
                            if (!SvgAttributes.CheckIsFloat(v)) {
                                return false;
                            }
                        }
                        break;
                }
            }
            return true;
        }
        // ==============================================================================================
        public static string GetString(SvgElement ele, string strName) {
            return SvgAttributes.GetString(ele, strName, true);
        }

        public static string GetString(SvgElement ele, string strName, bool inherit) {
            if (ele == null) return null;
            string strText = ele.Attributes[strName];
            if (!string.IsNullOrEmpty(strText)) {
                return strText;
            }
            if (inherit) {
                strText = SvgAttributes.GetString(ele.CurrentParent, strName, true);
            }
            if (strText == null) {
                strText = ele.Attributes.GetDefault(strName);
            }
            return strText;
        }

        public static float GetFloat(SvgElement ele, string strName) {
            string strText = ele.Attributes.GetDefault(strName);
            float f = 0;
            if (!string.IsNullOrEmpty(strText) && SvgAttributes.CheckIsFloat(strText)) {
                f = _F(strText);
            }
            return SvgAttributes.GetFloat(ele, strName, true, f);
        }

        public static float GetFloat(SvgElement ele, string strName, bool inherit, float defaultValue) {
            if (ele == null) return defaultValue;
            string str = ele.Attributes[strName];
            if (string.IsNullOrEmpty(str) || !SvgAttributes.CheckIsFloat(str)) {
                if (!inherit) return defaultValue;
                return SvgAttributes.GetFloat(ele.CurrentParent, strName, true, defaultValue);
            }
            return float.Parse(str);
        }

        public static Color GetColor(SvgElement ele, string strName) {
            string strText = ele.Attributes.GetDefault(strName);
            bool bError = false;
            Color clr = SvgAttributes.GetColorFromText(strText, out bError);
            return SvgAttributes.GetColor(ele, strName, true, clr);
        }

        public static Color GetColor(SvgElement ele, string strName, bool inherit, Color defaultValue) {
            if (ele == null) return defaultValue;
            string strText = ele.Attributes[strName];
            if (string.IsNullOrEmpty(strText)) {
                if (!inherit) return defaultValue;
                return SvgAttributes.GetColor(ele.CurrentParent, strName, true, defaultValue);
            }
            bool bError = false;
            Color clr = SvgAttributes.GetColorFromText(strText, out bError);
            if (bError && inherit) {
                return SvgAttributes.GetColor(ele.CurrentParent, strName, true, defaultValue);
            }
            return clr;
        }

        public static Color GetColorFromText(string strText, out bool bError) {
            bError = false;
            if (string.IsNullOrEmpty(strText)) {
                bError = true;
                return Color.Transparent;
            }
            int nR = 0, nG = 0, nB = 0, nA = 255;
            strText = strText.ToLower();
            if (m_dic_color.ContainsKey(strText)) {
                return m_dic_color[strText];
            }
            if (strText[0] == '#') {
                if (!SvgAttributes.CheckIsHex(strText.Substring(1))) {
                    bError = true;
                    return Color.Transparent;
                }
                switch (strText.Length) {
                    case 4:
                    case 5:
                        nR = Convert.ToInt32(strText[1].ToString() + strText[1], 16);
                        nG = Convert.ToInt32(strText[2].ToString() + strText[2], 16);
                        nB = Convert.ToInt32(strText[3].ToString() + strText[3], 16);
                        if (strText.Length == 5) {
                            nA = Convert.ToInt32(strText[4].ToString() + strText[4], 16);
                        }
                        break;
                    case 7:
                    case 9:
                        nR = Convert.ToInt32(strText.Substring(1, 2), 16);
                        nG = Convert.ToInt32(strText.Substring(3, 2), 16);
                        nB = Convert.ToInt32(strText.Substring(5, 2), 16);
                        if (strText.Length == 9) {
                            nA = Convert.ToInt32(strText.Substring(7, 2), 16);
                        }
                        break;
                    default:
                        bError = true;
                        return Color.Transparent;
                }
                return Color.FromArgb(nA, nR, nG, nB);
            }
            if (strText.StartsWith("rgb(")) {
                int nLeft = strText.IndexOf('(');
                int nRight = strText.IndexOf(')');
                string[] strs = strText.Substring(nLeft + 1, nRight - nLeft - 1).Split(',');
                if (strs.Length < 3) {
                    bError = true;
                    return Color.Transparent;
                }
                nR = (int)SvgAttributes.GetColorChannel(strs[0]);
                nG = (int)SvgAttributes.GetColorChannel(strs[1]);
                nB = (int)SvgAttributes.GetColorChannel(strs[2]);
                if (strs.Length == 4 && SvgAttributes.CheckIsFloat(strs[3])) {
                    nA = (int)(255 * _F(strs[3]));
                }
                return Color.FromArgb(nA, nR, nG, nB);
            }
            if (strText.StartsWith("hsl(")) {
                int nLeft = strText.IndexOf('(');
                int nRight = strText.IndexOf(')');
                string[] strs = strText.Substring(nLeft + 1, nRight - nLeft - 1).Split(',');
                if (strs.Length < 3) {
                    bError = true;
                    return Color.Transparent;
                }
                if (strs.Length == 4 && SvgAttributes.CheckIsFloat(strs[3])) {
                    nA = (int)(255 * _F(strs[3]));
                }
                float fH = (strs[0].Length > 1 && strs[1][strs[0].Length - 1] == '%') ? _F(strs[0].Substring(0, strs[0].Length - 1)) : 0;
                float fS = (strs[1].Length > 1 && strs[1][strs[1].Length - 1] == '%') ? _F(strs[1].Substring(0, strs[1].Length - 1)) : 0;
                float fL = (strs[2].Length > 1 && strs[1][strs[2].Length - 1] == '%') ? _F(strs[2].Substring(0, strs[2].Length - 1)) : 0;
                return SvgAttributes.GetColorFromHSL(
                    SvgAttributes.CheckRange(fH, 0, 360) / 360,
                    SvgAttributes.CheckRange(fS, 0, 100) / 100,
                    SvgAttributes.CheckRange(fS, 0, 100) / 100,
                    SvgAttributes.CheckRange(nA, 0, 255));
            }
            return Color.Transparent;
        }

        public static Color GetColorFromHSL(float h, float s, float l, int nAlpha) {
            float r, g, b;

            if (s == 0) {
                r = g = b = l; // achromatic
            } else {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = SvgAttributes.HueToRgb(p, q, h + 1 / 3);
                g = SvgAttributes.HueToRgb(p, q, h);
                b = SvgAttributes.HueToRgb(p, q, h - 1 / 3);
            }
            return Color.FromArgb(nAlpha, (int)r, (int)g, (int)b);
        }

        public static float GetAngleFromText(string strText) {
            Match m = m_reg_angle.Match(strText);
            if (!m.Success) {
                return 0;
            }
            // 90deg = 100grad = 0.25turn ≈ 1.570796326794897rad
            float f = float.Parse(m.Groups[1].Value);
            switch (m.Groups[1].Value) {
                case "grad":
                    return f * 90 / 100;
                case "rad":
                    return (float)(f * 90 / (Math.PI / 2));
            }
            return f;
        }

        public static float GetSize(SvgElement ele, string strName) {
            string strText = ele.Attributes.GetDefault(strName);
            bool bError = false;
            float f = SvgAttributes.GetSizeFromText(ele, strText, out bError);
            return SvgAttributes.GetSize(ele, strName, true, f);
        }

        public static float GetSize(SvgElement ele, string strName, bool inherit, float defaultValue) {
            if (ele == null) return defaultValue;
            string strText = ele.Attributes[strName];
            if (string.IsNullOrEmpty(strText)) {
                if (!inherit) return defaultValue;
                return SvgAttributes.GetSize(ele.CurrentParent, strName, true, defaultValue);
            }
            // number ("em" | "ex" | "px" | "in" | "cm" | "mm" | "pt" | "pc" | "%")?
            Match m = m_reg_size.Match(strText);
            if (!m.Success) {
                return defaultValue;
            }
            float fSize = float.Parse(m.Groups[1].Value);
            switch (m.Groups[2].Value) {
                case "%":
                    return fSize / 100 * SvgAttributes.GetSize(ele.CurrentParent, strName, inherit, defaultValue);
                case "em":
                    return fSize * SvgAttributes.GetFontSize(ele);
                case "ex":
                    return fSize * SvgAttributes.GetFontSize(ele) * 0.45F;
                case "in":
                    return fSize * 96;
                case "cm":
                    return fSize * 96 / 2.54F;
                case "mm":
                    return fSize * 96 / 2.54F / 10;
                case "pt":
                    return fSize * 1 / 72;
                case "pc":
                    return 12 * fSize * 1 / 72;
                default:    // px
                    return fSize;
            }
        }

        public static float GetSizeFromText(SvgElement ele, string strText, out bool bError) {
            bError = false;
            if (string.IsNullOrEmpty(strText)) {
                bError = true;
                return 0;
            }
            // number ("em" | "ex" | "px" | "in" | "cm" | "mm" | "pt" | "pc" | "%")?
            Match m = m_reg_size.Match(strText);
            if (!m.Success) {
                bError = true;
                return 0;
            }
            float fSize = float.Parse(m.Groups[1].Value);
            switch (m.Groups[2].Value) {
                case "%":
                    return fSize / 100 * SvgAttributes.GetSize(ele, "width", true, 0);
                case "em":
                    return fSize * SvgAttributes.GetFontSize(ele);
                case "ex":
                    return fSize * SvgAttributes.GetFontSize(ele) * 0.45F;
                case "in":
                    return fSize * 96;
                case "cm":
                    return fSize * 96 / 2.54F;
                case "mm":
                    return fSize * 96 / 2.54F / 10;
                case "pt":
                    return fSize * 1 / 72;
                case "pc":
                    return 12 * fSize * 1 / 72;
                default:    // px
                    return fSize;
            }
        }

        public static float[] GetSizeArray(SvgElement ele, string strName, bool inherit) {
            if (ele == null) return null;
            string strText = ele.Attributes[strName];
            if (string.IsNullOrEmpty(strText)) {
                if (!inherit) return null;
                return SvgAttributes.GetSizeArray(ele.CurrentParent, strName, inherit);
            }
            var ms = m_reg_size.Matches(strText);
            float[] arr = new float[ms.Count];
            for (int i = 0; i < arr.Length; i++) {
                var m = ms[i];
                float fSize = float.Parse(m.Groups[1].Value);
                switch (m.Groups[2].Value) {
                    case "%":
                        arr[i] = fSize / 100 * SvgAttributes.GetFontSize(ele.CurrentParent);
                        break;
                    case "em":
                        arr[i] = fSize * SvgAttributes.GetFontSize(ele.CurrentParent);
                        break;
                    case "ex":  // the x-height
                        arr[i] = fSize * SvgAttributes.GetFontSize(ele.CurrentParent) * 0.45F;
                        break;
                    case "in":  // 1in = ppi(dpi), 96 is the normal dpi, and SVG is vector graphics. Use regular DPI directly
                        arr[i] = fSize * 96;
                        break;
                    case "cm":  // 1in = 2.54cm
                        arr[i] = fSize * 96 / 2.54F;
                        break;
                    case "mm":
                        arr[i] = fSize * 96 / 2.54F / 10;
                        break;
                    case "pt":  // point
                        arr[i] = fSize * 1 / 72;
                        break;
                    case "pc":  // 1pc = 12pt
                        arr[i] = 12 * fSize * 1 / 72;
                        break;
                    default:    // px
                        arr[i] = fSize;
                        break;
                }
            }
            return arr;
        }

        public static float GetFontSize(SvgElement ele) {
            if (ele == null) return 16; // The browser default font size is 16px;
            string strText = ele.Attributes["font-size"];
            if (string.IsNullOrEmpty(strText)) {    // not set,inherit from parent
                return SvgAttributes.GetFontSize(ele.CurrentParent);
            }
            // number ("em" | "ex" | "px" | "in" | "cm" | "mm" | "pt" | "pc" | "%")?
            Match m = m_reg_size.Match(strText);
            if (!m.Success) {   // The error value, search from parent to inherit
                return SvgAttributes.GetFontSize(ele.CurrentParent);
            }
            float fSize = float.Parse(m.Groups[1].Value);
            switch (m.Groups[2].Value) {
                case "%":
                    return fSize / 100 * SvgAttributes.GetFontSize(ele.CurrentParent);
                case "em":
                    return fSize * SvgAttributes.GetFontSize(ele.CurrentParent);
                case "ex":  // the x-height
                    return fSize * SvgAttributes.GetFontSize(ele.CurrentParent) * 0.45F;
                case "in":  // 1in = ppi(dpi), 96 is the normal dpi, and SVG is vector graphics. Use regular DPI directly
                    return fSize * 96;
                case "cm":  // 1in = 2.54cm
                    return fSize * 96 / 2.54F;
                case "mm":
                    return fSize * 96 / 2.54F / 10;
                case "pt":  // point
                    return fSize * 1 / 72;
                case "pc":  // 1pc = 12pt
                    return 12 * fSize * 1 / 72;
                default:    // px
                    return fSize;
            }
        }

        public static float GetOpacity(SvgElement ele, string strName, bool inherit) {
            float f = 1;
            if (ele == null) {
                return f;
            }
            string strText = ele.Attributes[strName];
            if (SvgAttributes.CheckIsFloat(strText)) {
                f = _F(strText, 0, 1);
            }
            if (!inherit) return f;
            return f * SvgAttributes.GetOpacity(ele.CurrentParent, strName, true);
        }

        public static string GetEnum(SvgElement ele, string strName, bool inherit, string[] strValues, string defaultValue) {
            if (ele == null) return defaultValue;
            string strText = ele.Attributes[strName];
            if (strValues.Contains(strText)) {
                return strText;
            }
            if (!inherit) return defaultValue;
            return SvgAttributes.GetEnum(ele.CurrentParent, strName, true, strValues, defaultValue);
        }

        public static Matrix GetTransform(SvgElement ele, string strName) {
            return SvgAttributes.GetTransform(ele, strName, true);
        }

        public static Matrix GetTransform(SvgElement ele, string strName, bool inherit) {
            Matrix m = new Matrix();
            return SvgAttributes.GetTransformPrivate(ele, strName, inherit, m);
        }

        private static Matrix GetTransformPrivate(SvgElement ele, string strName, bool inherit, Matrix defaultValue) {
            if (ele == null) {
                return defaultValue;
            }
            string strValue = ele.Attributes[strName];
            if (string.IsNullOrEmpty(strValue)) {
                if (!inherit) return defaultValue;
                return SvgAttributes.GetTransformPrivate(ele.CurrentParent, strName, true, defaultValue);
            }
            bool bError = false;
            Matrix matrix = new Matrix();
            foreach (Match m in m_reg_transform.Matches(strValue)) {
                var strArgs = Regex.Split(m.Groups[2].Value, @"[,\s*]");// m.Groups[2].Value.Split(',');
                if (strArgs.Length < 1) {
                    continue;
                }
                switch (m.Groups[1].Value) {
                    case "rotate":
                        matrix.Rotate(SvgAttributes.GetAngleFromText(strArgs[0].Trim()));
                        break;
                    case "scale":
                        if (strArgs.Length == 1) {
                            matrix.Scale(_F(strArgs[0].Trim()), _F(strArgs[0].Trim()));
                        } else {
                            matrix.Scale(_F(strArgs[0].Trim()), _F(strArgs[1].Trim()));
                        }
                        break;
                    case "scaleX":
                        matrix.Scale(_F(strArgs[0].Trim()), 1);
                        break;
                    case "scaleY":
                        matrix.Scale(1, _F(strArgs[0].Trim()));
                        break;
                    case "skew":
                        //hmbb 这个cpf没有
                        /*if (strArgs.Length == 1) {
                            matrix.Shear(
                                (float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[0].Trim()) / 180 * Math.PI),
                                (float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[0].Trim()) / 180 * Math.PI));
                        } else {
                            matrix.Shear(
                                (float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[0].Trim()) / 180 * Math.PI),
                                (float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[1].Trim()) / 180 * Math.PI));
                        }*/
                        break;
                    case "skewX":
                        //matrix.Shear((float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[0].Trim()) / 180 * Math.PI), 0);
                        break;
                    case "skewY":
                        //matrix.Shear(0, (float)Math.Tan(SvgAttributes.GetAngleFromText(strArgs[0].Trim()) / 180 * Math.PI));
                        break;
                    case "translate":
                        if (strArgs.Length == 1) {
                            matrix.Translate(
                                SvgAttributes.GetSizeFromText(ele, strArgs[0].Trim(), out bError),
                                SvgAttributes.GetSizeFromText(ele, strArgs[0].Trim(), out bError));
                        } else {
                            matrix.Translate(
                                SvgAttributes.GetSizeFromText(ele, strArgs[0].Trim(), out bError),
                                SvgAttributes.GetSizeFromText(ele, strArgs[1].Trim(), out bError));
                        }
                        break;
                    case "translateX":
                        matrix.Translate(SvgAttributes.GetSizeFromText(ele, strArgs[0].Trim(), out bError), 0);
                        break;
                    case "translateY":
                        matrix.Translate(0, SvgAttributes.GetSizeFromText(ele, strArgs[0].Trim(), out bError));
                        break;
                    case "matrix":
                        if (strArgs.Length != 6) {
                            break;
                        }
                        Matrix m_temp = new Matrix(
                            _F(strArgs[0].Trim()),
                            _F(strArgs[1].Trim()),
                            _F(strArgs[2].Trim()),
                            _F(strArgs[3].Trim()),
                            _F(strArgs[4].Trim()),
                            _F(strArgs[5].Trim()));
                        matrix = Matrix.Multiply(matrix,m_temp);
                        break;
                }
            }
            if (!inherit) return matrix;
            var ret = SvgAttributes.GetTransformPrivate(ele.CurrentParent, strName, true, defaultValue);
            ret = Matrix.Multiply(ret,matrix);
            //matrix.Dispose();
            return ret;
        }
        //==========================================================
        private static float GetColorChannel(string strNum) {
            if (strNum.Length == 0) {
                return 0;
            }
            if (strNum[strNum.Length - 1] != '%') {
                if (!SvgAttributes.CheckIsFloat(strNum)) {
                    return 0;
                }
                return float.Parse(strNum);
            }
            if (strNum.Length < 2) {
                return 0;
            }
            strNum = strNum.Substring(0, strNum.Length - 1);
            if (!SvgAttributes.CheckIsFloat(strNum)) {
                return 0;
            }
            return 255 * (SvgAttributes.CheckRange(float.Parse(strNum), 0, 100) % 100);
        }

        private static float HueToRgb(float p, float q, float t) {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) {
                p = p + (q - p) * 6 * t;
            } else if (t < 1 / 2) {
                p = q;
            } else if (t < 2 / 3) {
                p = p + (q - p) * (2 / 3 - t) * 6;
            }
            return SvgAttributes.CheckRange(p, 0, 255);
        }

        private static int CheckRange(int nNum, int nMin, int nMax) {
            if (nNum < nMin) {
                nNum = nMin;
            } else if (nNum > nMax) {
                nNum = nMax;
            }
            return nNum;
        }

        private static float CheckRange(float fNum, float fMin, float fMax) {
            if (fNum < fMin) {
                fNum = fMin;
            } else if (fNum > fMax) {
                fNum = fMax;
            }
            return fNum;
        }

        private static float _F(string strText) {
            if (!SvgAttributes.CheckIsFloat(strText)) {
                return 0;
            }
            return float.Parse(strText);
        }

        private static float _F(string strText, float fMin, float fMax) {
            return SvgAttributes.CheckRange(_F(strText), fMin, fMax);
        }
    }
}
