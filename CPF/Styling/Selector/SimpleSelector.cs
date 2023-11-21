using System;
using CPF.Styling;
// ReSharper disable once CheckNamespace

namespace CPF.Styling
{
    public sealed class SimpleSelector : BaseSelector
    {
        internal readonly string _code;
        internal static readonly SimpleSelector All = new SimpleSelector("*") { simpleSelectorType = SimpleSelectorType.All };

        public SimpleSelector(string selectorText)
        {
            _code = selectorText;
        }

        internal static SimpleSelector PseudoElement(string pseudoElement)
        {
            return new SimpleSelector("::" + pseudoElement) { simpleSelectorType = SimpleSelectorType.PseudoElement, text = pseudoElement };
        }

        internal static SimpleSelector PseudoClass(string pseudoClass)
        {
            return new SimpleSelector(":" + pseudoClass) { simpleSelectorType = SimpleSelectorType.PseudoClass, text = pseudoClass };
        }

        internal static SimpleSelector Class(string match)
        {
            return new SimpleSelector("." + match) { simpleSelectorType = SimpleSelectorType.Class, text = match };
        }

        internal static SimpleSelector Id(string match)
        {
            return new SimpleSelector("#" + match) { simpleSelectorType = SimpleSelectorType.Id, text = match };
        }

        internal static SimpleSelector AttributeUnmatched(string match)
        {
            return new SimpleSelector("[" + match + "]") { simpleSelectorType = SimpleSelectorType.AttributeUnmatched, text = match };
        }

        internal static SimpleSelector AttributeMatch(string match, string value)
        {
            var code = string.Format("[{0}=\"{1}\"]", match, GetValueAsString(value));
            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeMatch, text = match, value = value };
        }

        internal static SimpleSelector AttributeNegatedMatch(string match, string value)
        {
            var code = string.Format("[{0}!=\"{1}\"]", match, GetValueAsString(value));
            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeNegatedMatch, text = match, value = value };
        }

        internal static SimpleSelector AttributeSpaceSeparated(string match, string value)
        {
            var code = string.Format("[{0}~=\"{1}\"]", match, GetValueAsString(value));

            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeSpaceSeparated, text = match, value = value };
        }

        internal static SimpleSelector AttributeStartsWith(string match, string value)
        {
            var code = string.Format("[{0}^=\"{1}\"]", match, GetValueAsString(value));

            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeStartsWith, text = match, value = value };
        }

        internal static SimpleSelector AttributeEndsWith(string match, string value)
        {
            var code = string.Format("[{0}$=\"{1}\"]", match, GetValueAsString(value));

            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeEndsWith, text = match, value = value };
        }

        internal static SimpleSelector AttributeContains(string match, string value)
        {
            var code = string.Format("[{0}*=\"{1}\"]", match, GetValueAsString(value));

            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeContains, text = match, value = value };
        }

        internal static SimpleSelector AttributeDashSeparated(string match, string value)
        {
            var code = string.Format("[{0}|=\"{1}\"]", match, GetValueAsString(value));

            return new SimpleSelector(code) { simpleSelectorType = SimpleSelectorType.AttributeDashSeparated, text = match, value = value };
        }

        internal static SimpleSelector Type(string match)
        {
            return new SimpleSelector(match) { simpleSelectorType = SimpleSelectorType.Type, text = match };
        }

        private static string GetValueAsString(string value)
        {
            var containsSpace = false;

            for (var i = 0; i < value.Length; i++)
            {
                if (!value[i].IsSpaceCharacter())
                {
                    continue;
                }
                containsSpace = true;
                break;
            }

            if (!containsSpace)
            {
                return value;
            }

            if (value.IndexOf(Specification.SingleQuote) != -1)
            {
                return '"' + value + '"';
            }

            return "'" + value + "'";
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return _code;
        }

        public override bool Select(UIElement element)
        {
            switch (simpleSelectorType)
            {
                case SimpleSelectorType.All:
                    return true;
                case SimpleSelectorType.PseudoElement:
                    break;
                case SimpleSelectorType.PseudoClass:
                    break;
                case SimpleSelectorType.Class:
                    if (element.classes != null)
                    {
                        if (text.IndexOf('.') < 0)
                        {
                            return element.classes.Contains(text);
                        }
                        var temp = text.Split('.');
                        foreach (var item in temp)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (!element.classes.Contains(item))
                                {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                    break;
                case SimpleSelectorType.Id:
                    return element.Name == text;
                case SimpleSelectorType.AttributeUnmatched:
                    break;
                case SimpleSelectorType.AttributeMatch:
                    return element.HasProperty(text);
                case SimpleSelectorType.AttributeNegatedMatch:
                    return element.HasProperty(text);
                case SimpleSelectorType.AttributeSpaceSeparated:
                    break;
                case SimpleSelectorType.AttributeStartsWith:
                    break;
                case SimpleSelectorType.AttributeEndsWith:
                    break;
                case SimpleSelectorType.AttributeContains:
                    break;
                case SimpleSelectorType.AttributeDashSeparated:
                    break;
                case SimpleSelectorType.Type:
                    if (_code.Contains("."))
                    {
                        var temp = _code.Split('.');
                        if (element.Type.Name == temp[0])
                        {
                            if (element.classes != null && temp.Length > 1 && !string.IsNullOrWhiteSpace(temp[1]))
                            {
                                for (int i = 1; i < temp.Length; i++)
                                {
                                    if (!element.classes.Contains(temp[i]))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        return false;
                    }
                    else if (_code.Contains("#"))
                    {
                        var temp = _code.Split('#');
                        if (element.Type.Name == temp[0])
                        {
                            return element.Name == temp[1];
                        }
                        return false;
                    }
                    return element.Type.Name == text;
                default:
                    break;
            }
            return false;
        }

        SimpleSelectorType simpleSelectorType;
        public SimpleSelectorType SimpleSelectorType { get { return simpleSelectorType; } }
        internal string text;
        internal string value;
    }

    public enum SimpleSelectorType : byte
    {
        All,
        PseudoElement,
        PseudoClass,
        Class,
        Id,
        AttributeUnmatched,
        AttributeMatch,
        AttributeNegatedMatch,
        AttributeSpaceSeparated,
        AttributeStartsWith,
        AttributeEndsWith,
        AttributeContains,
        AttributeDashSeparated,
        Type,
    }
}
