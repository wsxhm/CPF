using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public sealed class StyleSheet
    {
        private readonly Collection<RuleSet> _rules;

        public StyleSheet()
        {
            _rules = new Collection<RuleSet>();
            _rules.CollectionChanged += _rules_CollectionChanged;
            Errors = new List<StylesheetParseError>();
        }

        private void _rules_CollectionChanged(object sender, CollectionChangedEventArgs<RuleSet> e)
        {
            styleRules = null;
        }

        public Collection<RuleSet> Rules
        {
            get { return _rules; }
        }

        public string Url
        {
            get; internal set;
        }

        //public StyleSheet RemoveRule(int index)
        //{
        //    if (index >= 0 && index < _rules.Count)
        //    {
        //        _rules.RemoveAt(index);
        //    }

        //    return this;
        //}

        //public StyleSheet InsertRule(string rule, int index)
        //{
        //    if (index < 0 || index > _rules.Count)
        //    {
        //        return this;
        //    }

        //    var value = Parser.ParseRule(rule);
        //    _rules.Insert(index, value);

        //    return this;
        //}

        List<StyleRule> styleRules;
        public IEnumerable<StyleRule> StyleRules
        {
            get
            {
                if (styleRules == null)
                {
                    styleRules = new List<StyleRule>();
                    if (MediaDirectives.Count() > 0)
                    {
                        var keyword = "";
                        switch (CPF.Platform.Application.OperatingSystem)
                        {
                            case Platform.OperatingSystemType.Unknown:
                                break;
                            case Platform.OperatingSystemType.Windows:
                                keyword = "windows";
                                break;
                            case Platform.OperatingSystemType.Linux:
                                keyword = "linux";
                                break;
                            case Platform.OperatingSystemType.OSX:
                                keyword = "osx";
                                break;
                            case Platform.OperatingSystemType.Android:
                                keyword = "android";
                                break;
                            case Platform.OperatingSystemType.iOS:
                                keyword = "ios";
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            foreach (var item in MediaDirectives)
                            {
                                foreach (var type in item.Media)
                                {
                                    if (type.ToLower().Trim() == keyword)
                                    {
                                        styleRules.AddRange(item.RuleSets.Where(r => r is StyleRule).Cast<StyleRule>());
                                    }
                                    else if (CPF.Platform.Application.DesignMode && type.ToLower().Trim() == "designmode")
                                    {
                                        styleRules.AddRange(item.RuleSets.Where(r => r is StyleRule).Cast<StyleRule>());
                                    }
                                }
                            }
                        }
                    }
                    styleRules.AddRange(Rules.Where(r => r is StyleRule).Cast<StyleRule>());
                }
                return styleRules;
                //return Rules.Where(r => r is StyleRule).Cast<StyleRule>();
            }
        }


        //public IEnumerable<CharacterSetRule> CharsetDirectives
        //{
        //    get
        //    {
        //        return GetDirectives<CharacterSetRule>(RuleType.Charset);
        //    }
        //}

        //public IEnumerable<ImportRule> ImportDirectives
        //{
        //    get
        //    {
        //        return GetDirectives<ImportRule>(RuleType.Import);
        //    }
        //}

        public IEnumerable<FontFaceRule> FontFaceDirectives
        {
            get
            {
                return GetDirectives<FontFaceRule>(RuleType.FontFace);
            }
        }

        public IEnumerable<KeyframesRule> KeyframeDirectives
        {
            get
            {
                return GetDirectives<KeyframesRule>(RuleType.Keyframes);
            }
        }

        public IEnumerable<MediaRule> MediaDirectives
        {
            get
            {
                return GetDirectives<MediaRule>(RuleType.Media);

            }
        }

        //public IEnumerable<PageRule> PageDirectives
        //{
        //    get
        //    {
        //        return GetDirectives<PageRule>(RuleType.Page);

        //    }
        //}

        //public IEnumerable<SupportsRule> SupportsDirectives
        //{
        //    get
        //    {
        //        return GetDirectives<SupportsRule>(RuleType.Supports);
        //    }
        //}

        //public IEnumerable<NamespaceRule> NamespaceDirectives
        //{
        //    get
        //    {
        //        return GetDirectives<NamespaceRule>(RuleType.Namespace);
        //    }
        //}

        private IEnumerable<T> GetDirectives<T>(RuleType ruleType)
        {
            return Rules.Where(r => r.RuleType == ruleType).Cast<T>();
        }

        public List<StylesheetParseError> Errors { get; private set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool friendlyFormat, int indentation = 0)
        {
            var builder = new StringBuilder();

            foreach (var rule in _rules)
            {
                builder.Append(rule.ToString(friendlyFormat, indentation).TrimStart() + (friendlyFormat ? Environment.NewLine : ""));
            }

            return builder.TrimFirstLine().TrimLastLine().ToString();
        }
    }
}
