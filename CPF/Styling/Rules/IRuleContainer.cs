using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public interface IRuleContainer
    {
        List<RuleSet> Declarations { get; }
    }
}