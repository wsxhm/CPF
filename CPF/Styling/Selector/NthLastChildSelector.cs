using System;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    internal sealed class NthLastChildSelector : NthChildSelector, IToString
    {
        public override bool Select(UIElement element)
        {
            throw new NotImplementedException();
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return FormatSelector(PseudoSelectorPrefix.PseudoFunctionNthlastchild);
        }
    }
}