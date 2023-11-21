using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF;

namespace ConsoleApp1
{
    public class NodeData : CPF.CpfObject
    {
        public NodeData()
        {
            Nodes = new Collection<NodeData>();
            Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        private void Nodes_CollectionChanged(object sender, CollectionChangedEventArgs<NodeData> e)
        {
            if (e.Action == CollectionChangedAction.Add || e.Action == CollectionChangedAction.Replace)
            {
                e.NewItem.Parent = this;
            }
        }

        public string Text
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public Collection<NodeData> Nodes
        {
            get { return GetValue<Collection<NodeData>>(); }
            set { SetValue(value); }
        }

        public bool IsChecked
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public NodeData Parent { get; set; }
    }

    public class TestData : INotifyPropertyChanged
    {
        public string Category { get; set; }

        private float _number = 0;
        public float Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Number"));
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
