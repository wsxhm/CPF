using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using CPF.Drawing;
using CPF;

namespace ST.Library.UI.STTextBox
{
    partial class STTextBox
    {
        public class Core
        {
            private Dictionary<ITextStyleMonitor, TextStyleRange> m_dic_cache_style;

            public int FontHeight { get { return this.ITextBoxRender.GetFontHeight(); } }
            public int LineHeight { get { return this.ITextBoxRender.GetFontHeight() + this.TextBox.GetIntYSize(this.TextBox.LineSpacing); } }
            public ITextBoundary IGraphemeSplitter { get; internal set; }
            public ITextBoundary IWordSplitter { get; internal set; }
            public ISTTextBoxRender ITextBoxRender { get; internal set; }
            public IEmojiRender IEmojiRender { get; internal set; }
            public ITextView ITextView { get; internal set; }
            public ITextHistory ITextHistory { get; internal set; }
            public ITextStyleMonitor[] ITextStyleMonitors { get; internal set; }

            public Rect ViewRectangle { get; internal set; }

            public STTextBox TextBox { get; private set; }
            public TextManager TextManager { get; private set; }
            public STTextBoxCaretInfo Caret { get; private set; }
            public STTextBoxScrollInfo Scroll { get; private set; }
            public STTextBoxSelectionInfo Selection { get; private set; }

            internal Core(STTextBox textBox) {
                if (textBox == null) {
                    throw new ArgumentNullException("textBox");
                }
                this.TextBox = textBox;
                m_dic_cache_style = new Dictionary<ITextStyleMonitor, TextStyleRange>();
                //GraphemeSplitter.CreateArrayCache();
                //WordSplitter.CreateArrayCache();
                this.IGraphemeSplitter = new GraphemeSplitter();
                this.IWordSplitter = new WordSplitter();
                this.ITextBoxRender = new STTextBoxGDIPRender();
                this.ITextHistory = new TextHistory(10);
                this.ITextView = new NoWrapTextView();

                this.TextManager = new TextManager();
                this.Caret = new STTextBoxCaretInfo();
                this.Scroll = new STTextBoxScrollInfo();
                this.Selection = new STTextBoxSelectionInfo();
                var textManager = this.TextManager;
                textManager.TextStartChange += (s, e) => {
                    this.ITextBoxRender.BeginPaint();
                    this.ITextView.OnTextStartChange(this.ITextBoxRender, e);
                };
                textManager.LineAdded += (s, e) => this.ITextView.OnLineAdded(this.ITextBoxRender, e);
                textManager.LineChanged += (s, e) => this.ITextView.OnLineChanged(this.ITextBoxRender, e);
                textManager.LineRemoved += (s, e) => this.ITextView.OnLineRemoved(this.ITextBoxRender, e);
                textManager.LineCountChanged += (s, e) => {
                    this.ITextView.OnLineCountChanged(this.ITextBoxRender, e);
                    this.ITextView.OnCalcTextRectangle();
                };
                textManager.TextChanged += (s, e) => {
                    m_dic_cache_style.Clear();
                    if (this.ITextStyleMonitors != null) {
                        foreach (var m in this.ITextStyleMonitors) {
                            m.OnTextChanged(textManager, e.TextHistoryRecord);
                        }
                    }
                    this.ITextView.OnTextChanged(this.ITextBoxRender, e);
                    this.ITextBoxRender.EndPaint();
                    this.ITextView.OnCalcScroll(this.Scroll);
                    this.TextBox.Invalidate();
                };

                this.ITextBoxRender.BindControl(textBox);
                this.ITextView.Init(this);
                this.Selection.SelectionChanged += (s, e) => {
                    if (this.ITextStyleMonitors == null || this.ITextStyleMonitors.Length == 0) {
                        return;
                    }
                    m_dic_cache_style.Clear();
                    foreach (var m in this.ITextStyleMonitors) {
                        m.OnSelectionChanged(textManager, this.Selection.StartIndex, this.Selection.Length);
                    }
                };
            }

            public ISTTextBoxRender SetTextBoxRender(ISTTextBoxRender render) {
                //TODO: move out
                var old = this.ITextBoxRender;
                if (old == render) return old;
                old.UnbindControl();
                render.BindControl(this.TextBox);
                this.ITextBoxRender = render;
                return old;
            }

            public ITextStyleMonitor[] SetTextStyleMonitors(params ITextStyleMonitor[] monitors) {
                var old = this.ITextStyleMonitors;
                string strText = this.TextManager.GetText();
                List<ITextStyleMonitor> lst = new List<ITextStyleMonitor>();
                foreach (var m in monitors) {
                    if (m == null) {
                        continue;
                    }
                    lst.Add(m);
                    m.Init(strText);
                }
                this.ITextStyleMonitors = lst.ToArray();
                m_dic_cache_style.Clear();
                return old;
            }

            public bool IsEmoji(string strChar) {
                if (this.IEmojiRender == null) return false;
                return this.IEmojiRender.IsEmoji(strChar);
            }

            public int GetStringWidth(string strText, TextStyle textStyle, int nLeftWidth) {
                if (this.IsEmoji(strText)) {
                    return this.FontHeight;
                }
                if (strText.Length > 1 && strText[strText.Length - 1] >= '\uFE00' && strText[strText.Length - 1] <= '\uFE0F') {
                    strText = strText.Substring(0, strText.Length - 1);
                }
                return this.ITextBoxRender.GetStringWidth(strText, textStyle, nLeftWidth) + this.TextBox.GetIntXSize(this.TextBox.CharSpacing);
            }

            public TextStyle GetStyleFromCharIndex(int nIndex) {
                TextStyle style = new TextStyle();
                if (this.ITextStyleMonitors == null || this.ITextStyleMonitors.Length == 0) {
                    style.ForeColor = (this.TextBox.Foreground as SolidColorFill).Color; 
                    return style;
                }
                var range = TextStyleRange.Empty;
                foreach (var m in this.ITextStyleMonitors) {
                    if (m_dic_cache_style.ContainsKey(m)) {
                        range = m_dic_cache_style[m];
                        if (nIndex >= range.Index && nIndex < range.Index + range.Length) {
                            if (range.Style.RejectMix) return range.Style;
                            style.Mix(range.Style);
                            continue;
                        }
                    }
                    range = m.GetStyleFromCharIndex(nIndex);
                    if (range == TextStyleRange.Empty) continue;
                    if (m_dic_cache_style.ContainsKey(m)) {
                        m_dic_cache_style[m] = range;
                    } else {
                        m_dic_cache_style.Add(m, range);
                    }
                    if (range.Style.RejectMix) return range.Style;
                    style.Mix(range.Style);
                }
                if (style.ForeColor.A == 0) {
                    style.ForeColor = (this.TextBox.Foreground as SolidColorFill).Color;
                }
                return style;
            }
        }

        public ISTTextBoxRender SetTextBoxRender(ISTTextBoxRender render) {
            if (render == null) {
                throw new ArgumentNullException("render");
            }
            return m_core.SetTextBoxRender(render);
        }

        public ITextBoundary SetGraphemeSplitter(ITextBoundary textBoundary) {
            if (textBoundary == null) {
                throw new ArgumentNullException("textBoundary");
            }
            if (m_core.IGraphemeSplitter == textBoundary) {
                return textBoundary;
            }
            var old = m_core.IGraphemeSplitter;
            m_core.IGraphemeSplitter = textBoundary;
            return old;
        }

        public ITextBoundary SetWordSplitter(ITextBoundary textBoundary) {
            if (textBoundary == null) {
                throw new ArgumentNullException("textBoundary");
            }
            if (m_core.IWordSplitter == textBoundary) {
                return textBoundary;
            }
            var old = m_core.IWordSplitter;
            m_core.IWordSplitter = textBoundary;
            return old;
        }

        public IEmojiRender SetEmojiRender(IEmojiRender emojiRender) {
            if (emojiRender == null) {
                throw new ArgumentNullException("emojiRender");
            }
            if (m_core.IEmojiRender == emojiRender) {
                return emojiRender;
            }
            var old = m_core.IEmojiRender;
            m_core.IEmojiRender = emojiRender;
            return old;
        }

        public ITextHistory SetTextHistory(ITextHistory textHistory) {
            if (m_core.ITextHistory == textHistory) {
                return textHistory;
            }
            var old = m_core.ITextHistory;
            m_core.ITextHistory = textHistory;
            return old;
        }

        public ITextView SetTextView(ITextView textView) {
            if (textView == null) {
                throw new ArgumentNullException("textView");
            }
            if (m_core.ITextView == textView) {
                return textView;
            }
            var old = m_core.ITextView;
            m_core.ITextView = textView;
            textView.Init(m_core);
            return old;
        }

        public ITextStyleMonitor[] SetTextStyleMonitors(params ITextStyleMonitor[] monitors) {
            return m_core.SetTextStyleMonitors(monitors);
        }
    }
}
