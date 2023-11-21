using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF.Input
{
    public struct KeyGesture : IEquatable<KeyGesture>
    {
        public KeyGesture(Keys key, InputModifiers modifiers = InputModifiers.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public bool Equals(KeyGesture other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key && Modifiers == other.Modifiers;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is KeyGesture && Equals((KeyGesture)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Key * 397) ^ (int)Modifiers;
            }
        }

        public static bool operator ==(KeyGesture left, KeyGesture right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(KeyGesture left, KeyGesture right)
        {
            return !Equals(left, right);
        }

        public Keys Key { get; set; }

        public InputModifiers Modifiers { get; set; }


        static readonly Dictionary<string, Keys> KeySynonyms = new Dictionary<string, Keys>
        {
            {"+", Keys.OemPlus },
            {"-", Keys.OemMinus},
            {".", Keys.OemPeriod }
        };

        //TODO: Move that to external key parser
        static Keys ParseKey(string key)
        {
            Keys rv;
            if (KeySynonyms.TryGetValue(key.ToLower(), out rv))
                return rv;
            return (Keys)Enum.Parse(typeof(Keys), key, true);
        }

        static InputModifiers ParseModifier(string modifier)
        {
            if (modifier.Equals("ctrl", StringComparison.OrdinalIgnoreCase))
                return InputModifiers.Control;
            return (InputModifiers)Enum.Parse(typeof(InputModifiers), modifier, true);
        }

        public static KeyGesture Parse(string gesture)
        {
            //string.Split can't be used here because "Ctrl++" is a perfectly valid key gesture

            var parts = new List<string>();

            var cstart = 0;
            for (var c = 0; c <= gesture.Length; c++)
            {
                var ch = c == gesture.Length ? '\0' : gesture[c];
                if (c == gesture.Length || (ch == '+' && cstart != c))
                {
                    parts.Add(gesture.Substring(cstart, c - cstart));
                    cstart = c + 1;
                }
            }
            for (var c = 0; c < parts.Count; c++)
                parts[c] = parts[c].Trim();

            var rv = new KeyGesture();

            for (var c = 0; c < parts.Count; c++)
            {
                if (c == parts.Count - 1)
                    rv.Key = ParseKey(parts[c]);
                else
                    rv.Modifiers |= ParseModifier(parts[c]);
            }
            return rv;
        }

        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var flag in Enum.GetValues(typeof(InputModifiers)).Cast<InputModifiers>())
            {
                if (Modifiers.HasFlag(flag) && flag != InputModifiers.None)
                    parts.Add(flag.ToString());
            }
            parts.Add(Key.ToString());
            return string.Join(" + ", parts);
        }

        public bool Matches(KeyEventArgs keyEvent) => ResolveNumPadOperationKey(keyEvent.Key) == Key && keyEvent.Modifiers == Modifiers;

        private Keys ResolveNumPadOperationKey(Keys key)
        {
            switch (key)
            {
                case Keys.Add:
                    return Keys.OemPlus;
                case Keys.Subtract:
                    return Keys.OemMinus;
                case Keys.Decimal:
                    return Keys.OemPeriod;
                default:
                    return key;
            }
        }
    }
}
