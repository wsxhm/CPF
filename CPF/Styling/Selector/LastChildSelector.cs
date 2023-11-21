// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    internal sealed class LastChildSelector : BaseSelector, IToString
    {
        LastChildSelector()
        { }

        static LastChildSelector _instance;

        public static LastChildSelector Instance
        {
            get { return _instance ?? (_instance = new LastChildSelector()); }
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return ":" + PseudoSelectorPrefix.PseudoLastchild;
        }

        public override bool Select(UIElement element)
        {
            throw new System.NotImplementedException();
        }
    }
}