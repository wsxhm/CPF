using System.Linq;
using CPF.Styling;
using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class MediaRule : ConditionalRule, ISupportsMedia
    {
        private readonly MediaTypeList _media;

        public MediaRule() 
        {
            _media = new MediaTypeList();
            RuleType = RuleType.Media;
        }

        public override string Condition
        {
            get { return _media.MediaType; }
            set { _media.MediaType = value; }
        }

        public MediaTypeList Media
        {
            get { return _media; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var join = friendlyFormat ? "".NewLineIndent(true, indentation + 1) : "";

            var declarationList = RuleSets.Select(d => d.ToString(friendlyFormat, indentation + 1).TrimFirstLine());
#if BUILD_FOR_UNITY
			var declarations = string.Join(join, declarationList.ToArray());
#else
            var declarations = string.Join(join, declarationList);
#endif

            return ("@media " + _media.MediaType + "{").NewLineIndent(friendlyFormat, indentation) +
                declarations.TrimFirstLine().NewLineIndent(friendlyFormat, indentation + 1) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
