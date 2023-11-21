using CPF.Styling.Extensions;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class Property
    {
        private Term _term;
        private bool _important;
        private int _line;
        
        public Property(string name, int line)
        {
            Name = name;
            _line = line;
        }

        public string Name { get; private set; }

        public Term Term
        {
            get { return _term; }
            set { _term = value; }
        }

        //internal EffectiveValueEntry Value;

        public bool Important
        {
            get { return _important; }
            set { _important = value; }
        }

        public int Line { get { return _line; } }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool friendlyFormat, int indentation = 0)
        { 
            var value = Name + ":" + _term;

            if (_important)
            {
                value += " !important";
            }

            return value.Indent(friendlyFormat, indentation);
        }
    }
}
