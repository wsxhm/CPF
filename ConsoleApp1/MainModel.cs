using CPF;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;

namespace ConsoleApp1
{
    class MainModel : CpfObject
    {
        [PropertyMetadata("默认值")]
        public string Test
        {
            get
            {
                return (string)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public void Click()
        {
            Test += "test";
        }

        public MainModel()
        {
            Items = new Collection<(string, string)>();
            TestItems = new Collection<(string, int)>();

            for (int i = 0; i < 500; i++)
            {
                TestItems.Add((i.ToString(), i));
            }


            Nodes = new Collection<Node>();
            for (int i = 0; i < 100; i++)
            {
                Node node = new Node { Text = "node" + i };
                for (int j = 0; j < 100; j++)
                {
                    node.Nodes.Add(new Node { Text = "子节点" + j });
                }
                Nodes.Add(node);
            }
            TestItems1 = new Collection<string>();
            for (int i = 0; i < 100; i++)
            {
                TestItems1.Add(i.ToString());
            }

            //UIElements = new Collection<UIElement>();
            //UIElements.Add(new CPF.Controls.Button { Content="2131" });
            //UIElements.Add(new CPF.Controls.Button { Content="sdaf" });
        }

        public Collection<(string, string)> Items
        {
            get
            {
                return (Collection<(string, string)>)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        public Collection<(string, int)> TestItems
        {
            get
            {
                return (Collection<(string, int)>)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        public Collection<string> TestItems1
        {
            get
            {
                return (Collection<string>)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        //[Binding(Select = "#combobox", Convert = "convert")]
        public int SelectValue
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        [PropertyMetadata(true)]
        public bool TestBool
        {
            get
            {
                return (bool)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public Collection<Node> Nodes
        {
            get { return GetValue<Collection<Node>>(); }
            set { SetValue(value); }
        }
        public UIElementCollection UIElements
        {
            get { return GetValue<UIElementCollection>(); }
            set { SetValue(value); }
        }

        public void AddItem()
        {
            Items.Add(("test" + Items.Count, Items.Count.ToString()));
        }
        [PropertyMetadata(typeof(GridLength), "200")]
        public GridLength ColumnWidth
        {
            get { return GetValue<GridLength>(); }
            set { SetValue(value); }
        }
    }
    public class Node
    {
        public string Text { get; set; }

        public Collection<Node> Nodes { get; set; } = new Collection<Node>();
    }
}
