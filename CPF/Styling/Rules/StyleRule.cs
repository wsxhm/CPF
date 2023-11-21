using System;
using CPF.Styling;
using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class StyleRule : RuleSet, ISupportsSelector, ISupportsDeclarations
    {
        private string _value;
        private BaseSelector _selector;
        private readonly StyleDeclaration _declarations;

        private readonly int _line;

        public StyleRule(int line) : this(new StyleDeclaration())
        {
            _line = line;
        }
        public StyleRule() : this( new StyleDeclaration())
        {}

        public StyleRule(StyleDeclaration declarations) 
        {
            RuleType = RuleType.Style;
            _declarations = declarations;
        }

        public BaseSelector Selector
        {
            get { return _selector; }
            set
            {
                _selector = value;
                _value = value.ToString();
            }
        }

        public string Value
        {
            get { return _value; }
            //set
            //{
            //    _selector = Parser.ParseSelector(value);
            //    _value = value;
            //}
        }
        /// <summary>
        /// 样式声明  Width: 150;
        /// </summary>
        public StyleDeclaration Declarations
        {
            get { return _declarations; }
        }
        /// <summary>
        /// 行号
        /// </summary>
        public int Line
        {
            get { return _line; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return _value.NewLineIndent(friendlyFormat, indentation) +
                "{" +
                _declarations.ToString(friendlyFormat, indentation) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
