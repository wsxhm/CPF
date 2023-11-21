using CPF.Mac.CoreFoundation;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFontFeatureSelectors
	{
		public NSDictionary Dictionary
		{
			get;
			private set;
		}

		[Advice("Use one of descendant classes")]
		public NSNumber Identifier
		{
			get
			{
				return (NSNumber)Dictionary[CTFontFeatureSelectorKey.Identifier];
			}
			set
			{
				Adapter.SetValue(Dictionary, CTFontFeatureSelectorKey.Identifier, value);
			}
		}

		protected int FeatureWeak => (int)(NSNumber)Dictionary[CTFontFeatureSelectorKey.Identifier];

		public string Name
		{
			get
			{
				return Adapter.GetStringValue(Dictionary, CTFontFeatureSelectorKey.Name);
			}
			set
			{
				Adapter.SetValue(Dictionary, CTFontFeatureSelectorKey.Name, value);
			}
		}

		public bool Default
		{
			get
			{
				return CFDictionary.GetBooleanValue(Dictionary.Handle, CTFontFeatureSelectorKey.Default.Handle);
			}
			set
			{
				CFMutableDictionary.SetValue(Dictionary.Handle, CTFontFeatureSelectorKey.Default.Handle, value);
			}
		}

		public bool Setting
		{
			get
			{
				return CFDictionary.GetBooleanValue(Dictionary.Handle, CTFontFeatureSelectorKey.Setting.Handle);
			}
			set
			{
				CFMutableDictionary.SetValue(Dictionary.Handle, CTFontFeatureSelectorKey.Setting.Handle, value);
			}
		}

		public CTFontFeatureSelectors()
			: this(new NSMutableDictionary())
		{
		}

		public CTFontFeatureSelectors(NSDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			Dictionary = dictionary;
		}

		internal static CTFontFeatureSelectors Create(FontFeatureGroup featureGroup, NSDictionary dictionary)
		{
			switch (featureGroup)
			{
			case FontFeatureGroup.AllTypographicFeatures:
				return new CTFontFeatureAllTypographicFeatures(dictionary);
			case FontFeatureGroup.Ligatures:
				return new CTFontFeatureLigatures(dictionary);
			case FontFeatureGroup.CursiveConnection:
				return new CTFontFeatureCursiveConnection(dictionary);
			case FontFeatureGroup.LetterCase:
				return new CTFontFeatureLetterCase(dictionary);
			case FontFeatureGroup.VerticalSubstitution:
				return new CTFontFeatureVerticalSubstitutionConnection(dictionary);
			case FontFeatureGroup.LinguisticRearrangement:
				return new CTFontFeatureLinguisticRearrangementConnection(dictionary);
			case FontFeatureGroup.NumberSpacing:
				return new CTFontFeatureNumberSpacing(dictionary);
			case FontFeatureGroup.SmartSwash:
				return new CTFontFeatureSmartSwash(dictionary);
			case FontFeatureGroup.Diacritics:
				return new CTFontFeatureDiacritics(dictionary);
			case FontFeatureGroup.VerticalPosition:
				return new CTFontFeatureVerticalPosition(dictionary);
			case FontFeatureGroup.Fractions:
				return new CTFontFeatureFractions(dictionary);
			case FontFeatureGroup.OverlappingCharacters:
				return new CTFontFeatureOverlappingCharacters(dictionary);
			case FontFeatureGroup.TypographicExtras:
				return new CTFontFeatureTypographicExtras(dictionary);
			case FontFeatureGroup.MathematicalExtras:
				return new CTFontFeatureMathematicalExtras(dictionary);
			case FontFeatureGroup.OrnamentSets:
				return new CTFontFeatureOrnamentSets(dictionary);
			case FontFeatureGroup.CharacterAlternatives:
				return new CTFontFeatureCharacterAlternatives(dictionary);
			case FontFeatureGroup.DesignComplexity:
				return new CTFontFeatureDesignComplexity(dictionary);
			case FontFeatureGroup.StyleOptions:
				return new CTFontFeatureStyleOptions(dictionary);
			case FontFeatureGroup.CharacterShape:
				return new CTFontFeatureCharacterShape(dictionary);
			case FontFeatureGroup.NumberCase:
				return new CTFontFeatureNumberCase(dictionary);
			case FontFeatureGroup.TextSpacing:
				return new CTFontFeatureTextSpacing(dictionary);
			case FontFeatureGroup.Transliteration:
				return new CTFontFeatureTransliteration(dictionary);
			case FontFeatureGroup.Annotation:
				return new CTFontFeatureAnnotation(dictionary);
			case FontFeatureGroup.KanaSpacing:
				return new CTFontFeatureKanaSpacing(dictionary);
			case FontFeatureGroup.IdeographicSpacing:
				return new CTFontFeatureIdeographicSpacing(dictionary);
			case FontFeatureGroup.UnicodeDecomposition:
				return new CTFontFeatureUnicodeDecomposition(dictionary);
			case FontFeatureGroup.RubyKana:
				return new CTFontFeatureRubyKana(dictionary);
			case FontFeatureGroup.CJKSymbolAlternatives:
				return new CTFontFeatureCJKSymbolAlternatives(dictionary);
			case FontFeatureGroup.IdeographicAlternatives:
				return new CTFontFeatureIdeographicAlternatives(dictionary);
			case FontFeatureGroup.CJKVerticalRomanPlacement:
				return new CTFontFeatureCJKVerticalRomanPlacement(dictionary);
			case FontFeatureGroup.ItalicCJKRoman:
				return new CTFontFeatureItalicCJKRoman(dictionary);
			case FontFeatureGroup.CaseSensitiveLayout:
				return new CTFontFeatureCaseSensitiveLayout(dictionary);
			case FontFeatureGroup.AlternateKana:
				return new CTFontFeatureAlternateKana(dictionary);
			case FontFeatureGroup.StylisticAlternatives:
				return new CTFontFeatureStylisticAlternatives(dictionary);
			case FontFeatureGroup.ContextualAlternates:
				return new CTFontFeatureContextualAlternates(dictionary);
			case FontFeatureGroup.LowerCase:
				return new CTFontFeatureLowerCase(dictionary);
			case FontFeatureGroup.UpperCase:
				return new CTFontFeatureUpperCase(dictionary);
			case FontFeatureGroup.CJKRomanSpacing:
				return new CTFontFeatureCJKRomanSpacing(dictionary);
			default:
				return new CTFontFeatureSelectors(dictionary);
			}
		}
	}
}
