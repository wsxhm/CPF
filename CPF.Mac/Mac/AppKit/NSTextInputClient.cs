using System;
using System.Collections.Generic;
using System.Text;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using CPF.Mac.CoreGraphics;

namespace CPF.Mac.AppKit
{
    //[BaseType(typeof(NSObject))]
    [Protocol]
    public interface NSTextInputClient
    {
        [Export("insertText:replacementRange:")]
        void InsertText(NSObject text, NSRange replacementRange);

        [Export("setMarkedText:selectedRange:replacementRange:")]
        void SetMarkedText(NSObject text, NSRange selectedRange, NSRange replacementRange);

        [Export("unmarkText")]
        void UnmarkText();

        [Export("selectedRange")]
        NSRange SelectedRange { get; }

        [Export("markedRange")]
        NSRange MarkedRange { get; }

        [Export("hasMarkedText")]
        bool HasMarkedText { get; }

        [Export("attributedSubstringForProposedRange:actualRange:")]
        NSAttributedString GetAttributedSubstring(NSRange proposedRange, out NSRange actualRange);

        [Export("validAttributesForMarkedText")]
        NSString[] ValidAttributesForMarkedText { get; }

        [Export("firstRectForCharacterRange:actualRange:")]
        CGRect GetFirstRect(NSRange characterRange, out NSRange actualRange);

        [Export("characterIndexForPoint:")]
        uint GetCharacterIndex(CGPoint point);

        //[Export("attributedString")]
        //NSAttributedString AttributedString { get; }

        [Export("fractionOfDistanceThroughGlyphForPoint:")]
        float GetFractionOfDistanceThroughGlyph(CGPoint point);

        [Export("baselineDeltaForCharacterAtIndex:")]
        float GetBaselineDelta(uint charIndex);

        [Export("windowLevel")]
        NSWindowLevel WindowLevel { get; }

        //[Export("drawsVerticallyForCharacterAtIndex:")]
        //bool DrawsVertically(uint charIndex);
    }
}
