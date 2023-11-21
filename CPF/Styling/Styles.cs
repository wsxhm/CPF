using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Styling
{
    public class Styles : Collection<Style>
    {
        internal Dictionary<string, List<Style>> typeNameStyles = new Dictionary<string, List<Style>>();
        internal Dictionary<string, List<Style>> nameStyles = new Dictionary<string, List<Style>>();
        internal Dictionary<string, List<Style>> classStyles = new Dictionary<string, List<Style>>();
        internal List<Style> other = new List<Style>();

        bool isChanged;
        protected override void OnCollectionChanged(CollectionChangedEventArgs<Style> e)
        {
            isChanged = true;
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// 更新索引，提高检索速度
        /// </summary>
        internal void Update()
        {
            if (!isChanged)
            {
                return;
            }
            isChanged = false;
            typeNameStyles.Clear();
            nameStyles.Clear();
            classStyles.Clear();
            other.Clear();

            foreach (var item in this)
            {
                if (item.Selector is TypeNameSelector typeName)
                {
                    if (!typeNameStyles.TryGetValue(typeName.TypeName, out var list))
                    {
                        list = new List<Style>();
                        typeNameStyles.Add(typeName.TypeName, list);
                    }
                    list.Add(item);
                }
                else if (item.Selector is NameSelector nameSelector)
                {
                    if (!nameStyles.TryGetValue(nameSelector.ElementName, out var list))
                    {
                        list = new List<Style>();
                        nameStyles.Add(nameSelector.ElementName, list);
                    }
                    list.Add(item);
                }
                else if (item.Selector is ClassSelector classSelector)
                {
                    if (!classStyles.TryGetValue(classSelector.ClassName, out var list))
                    {
                        list = new List<Style>();
                        classStyles.Add(classSelector.ClassName, list);
                    }
                    list.Add(item);
                }
                else
                {
                    other.Add(item);
                }
            }
        }
    }
}
