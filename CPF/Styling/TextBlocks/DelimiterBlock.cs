﻿namespace CPF.Styling.TextBlocks
{
    internal class DelimiterBlock : CharacterBlock
    {
        internal DelimiterBlock()
        {
            GrammarSegment = GrammarSegment.Delimiter;
        }

        internal DelimiterBlock(char value) : base(value)
        {
            GrammarSegment = GrammarSegment.Delimiter;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
