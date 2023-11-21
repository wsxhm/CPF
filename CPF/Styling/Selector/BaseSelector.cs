
// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public abstract class BaseSelector
    {
        public sealed override string ToString()
        {
            return ToString(false);
        }

        public abstract string ToString(bool friendlyFormat, int indentation = 0);
        /// <summary>
        /// 判断元素是否符合选择器标准
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract bool Select(UIElement element);
    }
}

