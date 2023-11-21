
// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public abstract class Term
    {
        public static readonly InheritTerm Inherit = new InheritTerm();

        public virtual string GetValue()
        {
            return this.ToString();
        }
    }
}
