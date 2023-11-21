using System.Collections.Generic;
using CPF.Styling;
using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class KeyframeRule : RuleSet, ISupportsDeclarations
    {
        private List<string> _values { get; set; }
        public KeyframeRule()
        {
            Declarations = new StyleDeclaration();
            RuleType = RuleType.Keyframe;
            _values = new List<string>();
        }

        internal float Timeline;
        public void AddValue(string value)
        {
            _values.Add(value);
            if (value == "from")
            {
                Timeline = 0;
            }
            else if (value == "to")
            {
                Timeline = 1;
            }
            else
            {
                try
                {
                    var v = value.TrimEnd('%');
                    var vv = float.Parse(v);
                    Timeline = vv / 100;
                }
                catch (System.Exception)
                {
                    throw new System.Exception("动画格式不对" + value);
                }
            }
        }

        public StyleDeclaration Declarations { get; private set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Empty.Indent(friendlyFormat, indentation) +
#if BUILD_FOR_UNITY
                string.Join(",", _values.ToArray()) +
#else
                string.Join(",", _values) +
#endif
                "{" +
                Declarations.ToString(friendlyFormat, indentation) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
