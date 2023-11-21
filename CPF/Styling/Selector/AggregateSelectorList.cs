using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    /// <summary>
    /// ,
    /// </summary>
    public class AggregateSelectorList : SelectorList
    {
        public readonly string Delimiter;

        public AggregateSelectorList(string delimiter)
        {
            if (delimiter.Length > 1)
            {
                throw new ArgumentException("Expected single character delimiter or empty string", "delimiter");
            }

            Delimiter = delimiter;
        }

        public override bool Select(UIElement element)
        {
            if (Delimiter == "")
            {
                if (this.Length != 2)
                {
                    throw new Exception("只能支持两层选择器" + ToString());
                }
                if (this[0].Select(element))
                {
                    return this[1].Select(element);
                }
            }
            else if (Delimiter == ",")
            {
                foreach (var item in this)
                {
                    if (item.Select(element))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var builder = new StringBuilder();

            foreach (var selector in Selectors)
            {
                builder.Append(selector.ToString(friendlyFormat, indentation + 1));
                builder.Append(Delimiter);
            }

            if (Delimiter.Length <= 0)
            {
                return builder.ToString();
            }

            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }
    }
}
