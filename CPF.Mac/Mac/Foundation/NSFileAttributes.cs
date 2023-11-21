namespace CPF.Mac.Foundation
{
	public class NSFileAttributes
	{
		public bool? AppendOnly
		{
			get;
			set;
		}

		public bool? Busy
		{
			get;
			set;
		}

		public bool? FileExtensionHidden
		{
			get;
			set;
		}

		public NSDate CreationDate
		{
			get;
			set;
		}

		public string OwnerAccountName
		{
			get;
			set;
		}

		public uint? DeviceIdentifier
		{
			get;
			set;
		}

		public uint? FileGroupOwnerAccountID
		{
			get;
			set;
		}

		public bool? Immutable
		{
			get;
			set;
		}

		public NSDate ModificationDate
		{
			get;
			set;
		}

		public uint? FileOwnerAccountID
		{
			get;
			set;
		}

		public uint? HfsTypeCode
		{
			get;
			set;
		}

		public uint? PosixPermissions
		{
			get;
			set;
		}

		public uint? FileReferenceCount
		{
			get;
			set;
		}

		public uint? FileSystemFileNumber
		{
			get;
			set;
		}

		public ulong? FileSize
		{
			get;
			set;
		}

		public NSFileType? FileType
		{
			get;
			set;
		}

		internal NSDictionary ToDictionary()
		{
			NSMutableDictionary nSMutableDictionary = new NSMutableDictionary();
			if (AppendOnly.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromBoolean(AppendOnly.Value), NSFileManager.AppendOnly);
			}
			if (Busy.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromBoolean(Busy.Value), NSFileManager.Busy);
			}
			if (CreationDate != null)
			{
				nSMutableDictionary.SetObject(CreationDate, NSFileManager.CreationDate);
			}
			if (ModificationDate != null)
			{
				nSMutableDictionary.SetObject(ModificationDate, NSFileManager.ModificationDate);
			}
			if (OwnerAccountName != null)
			{
				nSMutableDictionary.SetObject(new NSString(OwnerAccountName), NSFileManager.OwnerAccountName);
			}
			if (DeviceIdentifier.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(DeviceIdentifier.Value), NSFileManager.DeviceIdentifier);
			}
			if (FileExtensionHidden.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromBoolean(FileExtensionHidden.Value), NSFileManager.ExtensionHidden);
			}
			if (FileGroupOwnerAccountID.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(FileGroupOwnerAccountID.Value), NSFileManager.GroupOwnerAccountID);
			}
			if (FileOwnerAccountID.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(FileOwnerAccountID.Value), NSFileManager.OwnerAccountID);
			}
			if (HfsTypeCode.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(HfsTypeCode.Value), NSFileManager.HfsTypeCode);
			}
			if (PosixPermissions.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(PosixPermissions.Value), NSFileManager.PosixPermissions);
			}
			if (FileReferenceCount.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(FileReferenceCount.Value), NSFileManager.ReferenceCount);
			}
			if (FileSystemFileNumber.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt32(FileSystemFileNumber.Value), NSFileManager.SystemFileNumber);
			}
			if (FileSize.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromUInt64(FileSize.Value), NSFileManager.Size);
			}
			if (Immutable.HasValue)
			{
				nSMutableDictionary.SetObject(NSNumber.FromBoolean(Immutable.Value), NSFileManager.Immutable);
			}
			if (FileType.HasValue)
			{
				NSString nSString = null;
				switch (FileType.Value)
				{
				case NSFileType.Directory:
					nSString = NSFileManager.TypeDirectory;
					break;
				case NSFileType.Regular:
					nSString = NSFileManager.TypeRegular;
					break;
				case NSFileType.SymbolicLink:
					nSString = NSFileManager.TypeSymbolicLink;
					break;
				case NSFileType.Socket:
					nSString = NSFileManager.TypeSocket;
					break;
				case NSFileType.CharacterSpecial:
					nSString = NSFileManager.TypeCharacterSpecial;
					break;
				case NSFileType.BlockSpecial:
					nSString = NSFileManager.TypeBlockSpecial;
					break;
				default:
					nSString = NSFileManager.TypeUnknown;
					break;
				}
				nSMutableDictionary.SetObject(nSString, NSFileManager.NSFileType);
			}
			return nSMutableDictionary;
		}

		internal static bool fetch(NSDictionary dict, NSString key, ref bool b)
		{
			NSNumber nSNumber = dict.ObjectForKey(key) as NSNumber;
			if (nSNumber == null)
			{
				return false;
			}
			b = nSNumber.BoolValue;
			return true;
		}

		internal static bool fetch(NSDictionary dict, NSString key, ref uint b)
		{
			NSNumber nSNumber = dict.ObjectForKey(key) as NSNumber;
			if (nSNumber == null)
			{
				return false;
			}
			b = nSNumber.UInt32Value;
			return true;
		}

		internal static bool fetch(NSDictionary dict, NSString key, ref ulong b)
		{
			NSNumber nSNumber = dict.ObjectForKey(key) as NSNumber;
			if (nSNumber == null)
			{
				return false;
			}
			b = nSNumber.UInt64Value;
			return true;
		}

		public static NSFileAttributes FromDict(NSDictionary dict)
		{
			if (dict == null)
			{
				return null;
			}
			NSFileAttributes nSFileAttributes = new NSFileAttributes();
			bool b = false;
			if (fetch(dict, NSFileManager.AppendOnly, ref b))
			{
				nSFileAttributes.AppendOnly = b;
			}
			if (fetch(dict, NSFileManager.Busy, ref b))
			{
				nSFileAttributes.Busy = b;
			}
			if (fetch(dict, NSFileManager.Immutable, ref b))
			{
				nSFileAttributes.Immutable = b;
			}
			if (fetch(dict, NSFileManager.ExtensionHidden, ref b))
			{
				nSFileAttributes.FileExtensionHidden = b;
			}
			NSDate nSDate = dict.ObjectForKey(NSFileManager.CreationDate) as NSDate;
			if (nSDate != null)
			{
				nSFileAttributes.CreationDate = nSDate;
			}
			nSDate = (dict.ObjectForKey(NSFileManager.ModificationDate) as NSDate);
			if (nSDate != null)
			{
				nSFileAttributes.ModificationDate = nSDate;
			}
			NSString nSString = dict.ObjectForKey(NSFileManager.OwnerAccountName) as NSString;
			if (nSString != null)
			{
				nSFileAttributes.OwnerAccountName = nSString.ToString();
			}
			uint b2 = 0u;
			if (fetch(dict, NSFileManager.DeviceIdentifier, ref b2))
			{
				nSFileAttributes.DeviceIdentifier = b2;
			}
			if (fetch(dict, NSFileManager.GroupOwnerAccountID, ref b2))
			{
				nSFileAttributes.FileGroupOwnerAccountID = b2;
			}
			if (fetch(dict, NSFileManager.OwnerAccountID, ref b2))
			{
				nSFileAttributes.FileOwnerAccountID = b2;
			}
			if (fetch(dict, NSFileManager.HfsTypeCode, ref b2))
			{
				nSFileAttributes.HfsTypeCode = b2;
			}
			if (fetch(dict, NSFileManager.PosixPermissions, ref b2))
			{
				nSFileAttributes.PosixPermissions = b2;
			}
			if (fetch(dict, NSFileManager.ReferenceCount, ref b2))
			{
				nSFileAttributes.FileReferenceCount = b2;
			}
			if (fetch(dict, NSFileManager.SystemFileNumber, ref b2))
			{
				nSFileAttributes.FileSystemFileNumber = b2;
			}
			ulong b3 = 0uL;
			if (fetch(dict, NSFileManager.Size, ref b3))
			{
				nSFileAttributes.FileSize = b3;
			}
			return nSFileAttributes;
		}
	}
}
