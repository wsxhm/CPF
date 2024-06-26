﻿using System;
using System.Collections.Generic;
using System.Linq;
using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class KeyframesRule : RuleSet, IRuleContainer
    {
        private readonly List<RuleSet> _ruleSets;
        private string _identifier;
        private string _ruleName;

        public KeyframesRule(string ruleName = null)
        {
            _ruleName = ruleName ?? "keyframes";
            _ruleSets = new List<RuleSet>();
            RuleType = RuleType.Keyframes;
        }

        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        //TODO change to "keyframes"
        public List<RuleSet> Declarations
        {
            get { return _ruleSets; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var join = friendlyFormat ? "".NewLineIndent(true, indentation) : "";

            var declarationList = _ruleSets.Select(d => d.ToString(friendlyFormat, indentation + 1));
#if BUILD_FOR_UNITY
			var declarations = string.Join(join, declarationList.ToArray());
#else
            var declarations = string.Join(join, declarationList);
#endif

            return ("@" + _ruleName + " " + _identifier + "{").NewLineIndent(friendlyFormat, indentation) +
                declarations.NewLineIndent(friendlyFormat, indentation) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
