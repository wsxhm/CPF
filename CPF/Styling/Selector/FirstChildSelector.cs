// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    internal sealed class FirstChildSelector : BaseSelector, IToString
    {
        FirstChildSelector()
        { }

        static FirstChildSelector _instance;

        public static FirstChildSelector Instance
        {
            get { return _instance ?? (_instance = new FirstChildSelector()); }
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return ":" + PseudoSelectorPrefix.PseudoFirstchild;
        }

        public override bool Select(UIElement element)
        {
            throw new System.NotImplementedException();
        }
    }
}