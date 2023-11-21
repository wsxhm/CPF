using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.ComponentModel;

namespace CPF.Mac.CoreImage
{
	[Register("CIFilter", true)]
	public class CIFilter : NSObject
	{
		private static readonly IntPtr selInputKeysHandle = Selector.GetHandle("inputKeys");

		private static readonly IntPtr selOutputKeysHandle = Selector.GetHandle("outputKeys");

		private static readonly IntPtr selAttributesHandle = Selector.GetHandle("attributes");

		private static readonly IntPtr selNameHandle = Selector.GetHandle("name");

		private static readonly IntPtr selSetDefaultsHandle = Selector.GetHandle("setDefaults");

		private static readonly IntPtr selFilterWithName_Handle = Selector.GetHandle("filterWithName:");

		private static readonly IntPtr selFilterNamesInCategory_Handle = Selector.GetHandle("filterNamesInCategory:");

		private static readonly IntPtr selFilterNamesInCategories_Handle = Selector.GetHandle("filterNamesInCategories:");

		private static readonly IntPtr selApplyArgumentsOptions_Handle = Selector.GetHandle("apply:arguments:options:");

		private static readonly IntPtr selRegisterFilterNameConstructorClassAttributes_Handle = Selector.GetHandle("registerFilterName:constructor:classAttributes:");

		private static readonly IntPtr selLocalizedNameForFilterName_Handle = Selector.GetHandle("localizedNameForFilterName:");

		private static readonly IntPtr selLocalizedNameForCategory_Handle = Selector.GetHandle("localizedNameForCategory:");

		private static readonly IntPtr selLocalizedDescriptionForFilterName_Handle = Selector.GetHandle("localizedDescriptionForFilterName:");

		private static readonly IntPtr selLocalizedReferenceDocumentationForFilterName_Handle = Selector.GetHandle("localizedReferenceDocumentationForFilterName:");

		private static readonly IntPtr selSetValueForKey_Handle = Selector.GetHandle("setValue:forKey:");

		private static readonly IntPtr selValueForKey_Handle = Selector.GetHandle("valueForKey:");

		private static readonly IntPtr class_ptr = Class.GetHandle("CIFilter");

		private object __mt_Attributes_var;

		public NSObject this[NSString key]
		{
			get
			{
				return ValueForKey(key);
			}
			set
			{
				SetValueForKey(value, key);
			}
		}

		public override IntPtr ClassHandle => class_ptr;

		public virtual string[] InputKeys
		{
			[Export("inputKeys")]
			get
			{
				if (IsDirectBinding)
				{
					return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selInputKeysHandle));
				}
				return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selInputKeysHandle));
			}
		}

		public virtual string[] OutputKeys
		{
			[Export("outputKeys")]
			get
			{
				if (IsDirectBinding)
				{
					return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selOutputKeysHandle));
				}
				return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selOutputKeysHandle));
			}
		}

		public virtual NSDictionary Attributes
		{
			[Export("attributes")]
			get
			{
				return (NSDictionary)(__mt_Attributes_var = ((!IsDirectBinding) ? ((NSDictionary)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selAttributesHandle))) : ((NSDictionary)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(base.Handle, selAttributesHandle)))));
			}
		}

		public virtual string Name
		{
			[Export("name")]
			get
			{
				if (IsDirectBinding)
				{
					return NSString.FromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selNameHandle));
				}
				return NSString.FromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selNameHandle));
			}
		}

		internal CIFilter(string name)
			: base(CreateFilter(name))
		{
		}

		public static string[] FilterNamesInCategories(params string[] categories)
		{
			return _FilterNamesInCategories(categories);
		}

		internal NSObject ValueForKey(string key)
		{
			using (NSString key2 = new NSString(key))
			{
				return ValueForKey(key2);
			}
		}

		internal void SetValue(string key, NSObject value)
		{
			using (NSString key2 = new NSString(key))
			{
				SetValueForKey(value, key2);
			}
		}

		internal static IntPtr CreateFilter(string name)
		{
			using (NSString nSString = new NSString(name))
			{
				return Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selFilterWithName_Handle, nSString.Handle);
			}
		}

		internal void SetFloat(string key, float value)
		{
			using (NSString key2 = new NSString(key))
			{
				SetValueForKey(new NSNumber(value), key2);
			}
		}

		internal float GetFloat(string key)
		{
			using (NSString key2 = new NSString(key))
			{
				NSObject nSObject = ValueForKey(key2);
				if (nSObject is NSNumber)
				{
					return (nSObject as NSNumber).FloatValue;
				}
				return 0f;
			}
		}

		internal CIVector GetVector(string key)
		{
			return ValueForKey(key) as CIVector;
		}

		internal CIColor GetColor(string key)
		{
			return ValueForKey(key) as CIColor;
		}

		internal CIImage GetInputImage()
		{
			return ValueForKey(CIFilterInputKey.Image) as CIImage;
		}

		internal void SetInputImage(CIImage value)
		{
			SetValueForKey(value, CIFilterInputKey.Image);
		}

		internal CIImage GetBackgroundImage()
		{
			return GetImage("inputBackgroundImage");
		}

		internal CIImage GetImage(string key)
		{
			using (NSString key2 = new NSString(key))
			{
				return ValueForKey(key2) as CIImage;
			}
		}

		internal void SetBackgroundImage(CIImage value)
		{
			SetImage("inputBackgroundImage", value);
		}

		internal void SetImage(string key, CIImage value)
		{
			using (NSString key2 = new NSString(key))
			{
				SetValueForKey(value, key2);
			}
		}

		internal static string GetFilterName(IntPtr filterHandle)
		{
			return NSString.FromHandle(Messaging.IntPtr_objc_msgSend(filterHandle, selNameHandle));
		}

		internal static CIFilter FromName(string filterName, IntPtr handle)
		{
			switch (filterName)
			{
			case "CIAdditionCompositing":
				return new CIAdditionCompositing(handle);
			case "CIAffineTransform":
				return new CIAffineTransform(handle);
			case "CICheckerboardGenerator":
				return new CICheckerboardGenerator(handle);
			case "CIColorBlendMode":
				return new CIColorBlendMode(handle);
			case "CIColorBurnBlendMode":
				return new CIColorBurnBlendMode(handle);
			case "CIColorControls":
				return new CIColorControls(handle);
			case "CIColorCube":
				return new CIColorCube(handle);
			case "CIColorDodgeBlendMode":
				return new CIColorDodgeBlendMode(handle);
			case "CIColorInvert":
				return new CIColorInvert(handle);
			case "CIColorMatrix":
				return new CIColorMatrix(handle);
			case "CIColorMonochrome":
				return new CIColorMonochrome(handle);
			case "CIConstantColorGenerator":
				return new CIConstantColorGenerator(handle);
			case "CICrop":
				return new CICrop(handle);
			case "CIDarkenBlendMode":
				return new CIDarkenBlendMode(handle);
			case "CIDifferenceBlendMode":
				return new CIDifferenceBlendMode(handle);
			case "CIExclusionBlendMode":
				return new CIExclusionBlendMode(handle);
			case "CIExposureAdjust":
				return new CIExposureAdjust(handle);
			case "CIFalseColor":
				return new CIFalseColor(handle);
			case "CIGammaAdjust":
				return new CIGammaAdjust(handle);
			case "CIGaussianGradient":
				return new CIGaussianGradient(handle);
			case "CIHardLightBlendMode":
				return new CIHardLightBlendMode(handle);
			case "CIHighlightShadowAdjust":
				return new CIHighlightShadowAdjust(handle);
			case "CIHueAdjust":
				return new CIHueAdjust(handle);
			case "CIHueBlendMode":
				return new CIHueBlendMode(handle);
			case "CILightenBlendMode":
				return new CILightenBlendMode(handle);
			case "CILinearGradient":
				return new CILinearGradient(handle);
			case "CILuminosityBlendMode":
				return new CILuminosityBlendMode(handle);
			case "CIMaximumCompositing":
				return new CIMaximumCompositing(handle);
			case "CIMinimumCompositing":
				return new CIMinimumCompositing(handle);
			case "CIMultiplyBlendMode":
				return new CIMultiplyBlendMode(handle);
			case "CIMultiplyCompositing":
				return new CIMultiplyCompositing(handle);
			case "CIOverlayBlendMode":
				return new CIOverlayBlendMode(handle);
			case "CIRadialGradient":
				return new CIRadialGradient(handle);
			case "CISaturationBlendMode":
				return new CISaturationBlendMode(handle);
			case "CIScreenBlendMode":
				return new CIScreenBlendMode(handle);
			case "CISepiaTone":
				return new CISepiaTone(handle);
			case "CISoftLightBlendMode":
				return new CISoftLightBlendMode(handle);
			case "CISourceAtopCompositing":
				return new CISourceAtopCompositing(handle);
			case "CISourceInCompositing":
				return new CISourceInCompositing(handle);
			case "CISourceOutCompositing":
				return new CISourceOutCompositing(handle);
			case "CISourceOverCompositing":
				return new CISourceOverCompositing(handle);
			case "CIStraightenFilter":
				return new CIStraightenFilter(handle);
			case "CIStripesGenerator":
				return new CIStripesGenerator(handle);
			case "CITemperatureAndTint":
				return new CITemperatureAndTint(handle);
			case "CIToneCurve":
				return new CIToneCurve(handle);
			case "CIVibrance":
				return new CIVibrance(handle);
			case "CIWhitePointAdjust":
				return new CIWhitePointAdjust(handle);
			case "CIFaceBalance":
				return new CIFaceBalance(handle);
			case "CIAffineClamp":
				return new CIAffineClamp(handle);
			case "CIAffineTile":
				return new CIAffineTile(handle);
			case "CIBlendWithMask":
				return new CIBlendWithMask(handle);
			case "CIBarsSwipeTransition":
				return new CIBarsSwipeTransition(handle);
			case "CICopyMachineTransition":
				return new CICopyMachineTransition(handle);
			case "CIDisintegrateWithMaskTransition":
				return new CIDisintegrateWithMaskTransition(handle);
			case "CIDissolveTransition":
				return new CIDissolveTransition(handle);
			case "CIFlashTransition":
				return new CIFlashTransition(handle);
			case "CIModTransition":
				return new CIModTransition(handle);
			case "CISwipeTransition":
				return new CISwipeTransition(handle);
			case "CIBloom":
				return new CIBloom(handle);
			case "CICircularScreen":
				return new CICircularScreen(handle);
			case "CIDotScreen":
				return new CIDotScreen(handle);
			case "CIHatchedScreen":
				return new CIHatchedScreen(handle);
			case "CILineScreen":
				return new CILineScreen(handle);
			case "CIColorMap":
				return new CIColorMap(handle);
			case "CIColorPosterize":
				return new CIColorPosterize(handle);
			case "CIEightfoldReflectedTile":
				return new CIEightfoldReflectedTile(handle);
			case "CIFourfoldReflectedTile":
				return new CIFourfoldReflectedTile(handle);
			case "CIFourfoldRotatedTile":
				return new CIFourfoldRotatedTile(handle);
			case "CIFourfoldTranslatedTile":
				return new CIFourfoldTranslatedTile(handle);
			case "CISixfoldReflectedTile":
				return new CISixfoldReflectedTile(handle);
			case "CISixfoldRotatedTile":
				return new CISixfoldRotatedTile(handle);
			case "CITwelvefoldReflectedTile":
				return new CITwelvefoldReflectedTile(handle);
			case "CIGaussianBlur":
				return new CIGaussianBlur(handle);
			case "CIGloom":
				return new CIGloom(handle);
			case "CIHoleDistortion":
				return new CIHoleDistortion(handle);
			case "CIPinchDistortion":
				return new CIPinchDistortion(handle);
			case "CITwirlDistortion":
				return new CITwirlDistortion(handle);
			case "CIVortexDistortion":
				return new CIVortexDistortion(handle);
			case "CILanczosScaleTransform":
				return new CILanczosScaleTransform(handle);
			case "CIMaskToAlpha":
				return new CIMaskToAlpha(handle);
			case "CIMaximumComponent":
				return new CIMaximumComponent(handle);
			case "CIMinimumComponent":
				return new CIMinimumComponent(handle);
			case "CIPerspectiveTile":
				return new CIPerspectiveTile(handle);
			case "CIPerspectiveTransform":
				return new CIPerspectiveTransform(handle);
			case "CIPixellate":
				return new CIPixellate(handle);
			case "CIRandomGenerator":
				return new CIRandomGenerator(handle);
			case "CISharpenLuminance":
				return new CISharpenLuminance(handle);
			case "CIStarShineGenerator":
				return new CIStarShineGenerator(handle);
			case "CIUnsharpMask":
				return new CIUnsharpMask(handle);
			case "CICircleSplashDistortion":
				return new CICircleSplashDistortion(handle);
			case "CIDepthOfField":
				return new CIDepthOfField(handle);
			case "CIPageCurlTransition":
				return new CIPageCurlTransition(handle);
			case "CIRippleTransition":
				return new CIRippleTransition(handle);
			default:
				throw new NotImplementedException($"Unknown filter type returned: `{filterName}', returning a default CIFilter");
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Export("initWithCoder:")]
		public CIFilter(NSCoder coder)
			: base(NSObjectFlag.Empty)
		{
			if (IsDirectBinding)
			{
				base.Handle = Messaging.IntPtr_objc_msgSend_IntPtr(base.Handle, Selector.InitWithCoder, coder.Handle);
			}
			else
			{
				base.Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr(base.SuperHandle, Selector.InitWithCoder, coder.Handle);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public CIFilter(NSObjectFlag t)
			: base(t)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public CIFilter(IntPtr handle)
			: base(handle)
		{
		}

		[Export("setDefaults")]
		public virtual void SetDefaults()
		{
			if (IsDirectBinding)
			{
				Messaging.void_objc_msgSend(base.Handle, selSetDefaultsHandle);
			}
			else
			{
				Messaging.void_objc_msgSendSuper(base.SuperHandle, selSetDefaultsHandle);
			}
		}

		[Export("filterWithName:")]
		public static CIFilter FromName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			IntPtr intPtr = NSString.CreateNative(name);
			CIFilter result = (CIFilter)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selFilterWithName_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("filterNamesInCategory:")]
		public static string[] FilterNamesInCategory(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			IntPtr intPtr = NSString.CreateNative(category);
			string[] result = NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selFilterNamesInCategory_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("filterNamesInCategories:")]
		internal static string[] _FilterNamesInCategories(string[] categories)
		{
			if (categories == null)
			{
				throw new ArgumentNullException("categories");
			}
			NSArray nSArray = NSArray.FromStrings(categories);
			string[] result = NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selFilterNamesInCategories_Handle, nSArray.Handle));
			nSArray.Dispose();
			return result;
		}

		[Export("apply:arguments:options:")]
		public virtual CIImage Apply(CIKernel k, NSArray args, NSDictionary options)
		{
			if (k == null)
			{
				throw new ArgumentNullException("k");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (IsDirectBinding)
			{
				return (CIImage)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr(base.Handle, selApplyArgumentsOptions_Handle, k.Handle, args.Handle, options.Handle));
			}
			return (CIImage)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr(base.SuperHandle, selApplyArgumentsOptions_Handle, k.Handle, args.Handle, options.Handle));
		}

		[Export("registerFilterName:constructor:classAttributes:")]
		public static void RegisterFilterName(string name, NSObject constructorObject, NSDictionary classAttributes)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (constructorObject == null)
			{
				throw new ArgumentNullException("constructorObject");
			}
			if (classAttributes == null)
			{
				throw new ArgumentNullException("classAttributes");
			}
			IntPtr intPtr = NSString.CreateNative(name);
			Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr(class_ptr, selRegisterFilterNameConstructorClassAttributes_Handle, intPtr, constructorObject.Handle, classAttributes.Handle);
			NSString.ReleaseNative(intPtr);
		}

		[Export("localizedNameForFilterName:")]
		public static string FilterLocalizedName(string filterName)
		{
			if (filterName == null)
			{
				throw new ArgumentNullException("filterName");
			}
			IntPtr intPtr = NSString.CreateNative(filterName);
			string result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selLocalizedNameForFilterName_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("localizedNameForCategory:")]
		public static string CategoryLocalizedName(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			IntPtr intPtr = NSString.CreateNative(category);
			string result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selLocalizedNameForCategory_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("localizedDescriptionForFilterName:")]
		public static string FilterLocalizedDescription(string filterName)
		{
			if (filterName == null)
			{
				throw new ArgumentNullException("filterName");
			}
			IntPtr intPtr = NSString.CreateNative(filterName);
			string result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selLocalizedDescriptionForFilterName_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("localizedReferenceDocumentationForFilterName:")]
		public static NSUrl FilterLocalizedReferenceDocumentation(string filterName)
		{
			if (filterName == null)
			{
				throw new ArgumentNullException("filterName");
			}
			IntPtr intPtr = NSString.CreateNative(filterName);
			NSUrl result = (NSUrl)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selLocalizedReferenceDocumentationForFilterName_Handle, intPtr));
			NSString.ReleaseNative(intPtr);
			return result;
		}

		[Export("setValue:forKey:")]
		internal new virtual void SetValueForKey(NSObject value, NSString key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (IsDirectBinding)
			{
				Messaging.void_objc_msgSend_IntPtr_IntPtr(base.Handle, selSetValueForKey_Handle, value?.Handle ?? IntPtr.Zero, key.Handle);
			}
			else
			{
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr(base.SuperHandle, selSetValueForKey_Handle, value?.Handle ?? IntPtr.Zero, key.Handle);
			}
		}

		[Export("valueForKey:")]
		internal new virtual NSObject ValueForKey(NSString key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (IsDirectBinding)
			{
				return Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend_IntPtr(base.Handle, selValueForKey_Handle, key.Handle));
			}
			return Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper_IntPtr(base.SuperHandle, selValueForKey_Handle, key.Handle));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (base.Handle == IntPtr.Zero)
			{
				__mt_Attributes_var = null;
			}
		}
	}
}
