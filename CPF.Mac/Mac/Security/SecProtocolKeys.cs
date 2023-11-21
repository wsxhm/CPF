using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Security
{
	internal static class SecProtocolKeys
	{
		private static IntPtr _AttrProtocolFTP;

		private static IntPtr _AttrProtocolFTPAccount;

		private static IntPtr _AttrProtocolHTTP;

		private static IntPtr _AttrProtocolIRC;

		private static IntPtr _AttrProtocolNNTP;

		private static IntPtr _AttrProtocolPOP3;

		private static IntPtr _AttrProtocolSMTP;

		private static IntPtr _AttrProtocolSOCKS;

		private static IntPtr _AttrProtocolIMAP;

		private static IntPtr _AttrProtocolLDAP;

		private static IntPtr _AttrProtocolAppleTalk;

		private static IntPtr _AttrProtocolAFP;

		private static IntPtr _AttrProtocolTelnet;

		private static IntPtr _AttrProtocolSSH;

		private static IntPtr _AttrProtocolFTPS;

		private static IntPtr _AttrProtocolHTTPS;

		private static IntPtr _AttrProtocolHTTPProxy;

		private static IntPtr _AttrProtocolHTTPSProxy;

		private static IntPtr _AttrProtocolFTPProxy;

		private static IntPtr _AttrProtocolSMB;

		private static IntPtr _AttrProtocolRTSP;

		private static IntPtr _AttrProtocolRTSPProxy;

		private static IntPtr _AttrProtocolDAAP;

		private static IntPtr _AttrProtocolEPPC;

		private static IntPtr _AttrProtocolIPP;

		private static IntPtr _AttrProtocolNNTPS;

		private static IntPtr _AttrProtocolLDAPS;

		private static IntPtr _AttrProtocolTelnetS;

		private static IntPtr _AttrProtocolIMAPS;

		private static IntPtr _AttrProtocolIRCS;

		private static IntPtr _AttrProtocolPOP3S;

		public static IntPtr AttrProtocolFTP
		{
			get
			{
				if (_AttrProtocolFTP == IntPtr.Zero)
				{
					_AttrProtocolFTP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolFTP");
				}
				return _AttrProtocolFTP;
			}
		}

		public static IntPtr AttrProtocolFTPAccount
		{
			get
			{
				if (_AttrProtocolFTPAccount == IntPtr.Zero)
				{
					_AttrProtocolFTPAccount = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolFTPAccount");
				}
				return _AttrProtocolFTPAccount;
			}
		}

		public static IntPtr AttrProtocolHTTP
		{
			get
			{
				if (_AttrProtocolHTTP == IntPtr.Zero)
				{
					_AttrProtocolHTTP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolHTTP");
				}
				return _AttrProtocolHTTP;
			}
		}

		public static IntPtr AttrProtocolIRC
		{
			get
			{
				if (_AttrProtocolIRC == IntPtr.Zero)
				{
					_AttrProtocolIRC = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolIRC");
				}
				return _AttrProtocolIRC;
			}
		}

		public static IntPtr AttrProtocolNNTP
		{
			get
			{
				if (_AttrProtocolNNTP == IntPtr.Zero)
				{
					_AttrProtocolNNTP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolNNTP");
				}
				return _AttrProtocolNNTP;
			}
		}

		public static IntPtr AttrProtocolPOP3
		{
			get
			{
				if (_AttrProtocolPOP3 == IntPtr.Zero)
				{
					_AttrProtocolPOP3 = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolPOP3");
				}
				return _AttrProtocolPOP3;
			}
		}

		public static IntPtr AttrProtocolSMTP
		{
			get
			{
				if (_AttrProtocolSMTP == IntPtr.Zero)
				{
					_AttrProtocolSMTP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolSMTP");
				}
				return _AttrProtocolSMTP;
			}
		}

		public static IntPtr AttrProtocolSOCKS
		{
			get
			{
				if (_AttrProtocolSOCKS == IntPtr.Zero)
				{
					_AttrProtocolSOCKS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolSOCKS");
				}
				return _AttrProtocolSOCKS;
			}
		}

		public static IntPtr AttrProtocolIMAP
		{
			get
			{
				if (_AttrProtocolIMAP == IntPtr.Zero)
				{
					_AttrProtocolIMAP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolIMAP");
				}
				return _AttrProtocolIMAP;
			}
		}

		public static IntPtr AttrProtocolLDAP
		{
			get
			{
				if (_AttrProtocolLDAP == IntPtr.Zero)
				{
					_AttrProtocolLDAP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolLDAP");
				}
				return _AttrProtocolLDAP;
			}
		}

		public static IntPtr AttrProtocolAppleTalk
		{
			get
			{
				if (_AttrProtocolAppleTalk == IntPtr.Zero)
				{
					_AttrProtocolAppleTalk = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolAppleTalk");
				}
				return _AttrProtocolAppleTalk;
			}
		}

		public static IntPtr AttrProtocolAFP
		{
			get
			{
				if (_AttrProtocolAFP == IntPtr.Zero)
				{
					_AttrProtocolAFP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolAFP");
				}
				return _AttrProtocolAFP;
			}
		}

		public static IntPtr AttrProtocolTelnet
		{
			get
			{
				if (_AttrProtocolTelnet == IntPtr.Zero)
				{
					_AttrProtocolTelnet = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolTelnet");
				}
				return _AttrProtocolTelnet;
			}
		}

		public static IntPtr AttrProtocolSSH
		{
			get
			{
				if (_AttrProtocolSSH == IntPtr.Zero)
				{
					_AttrProtocolSSH = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolSSH");
				}
				return _AttrProtocolSSH;
			}
		}

		public static IntPtr AttrProtocolFTPS
		{
			get
			{
				if (_AttrProtocolFTPS == IntPtr.Zero)
				{
					_AttrProtocolFTPS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolFTPS");
				}
				return _AttrProtocolFTPS;
			}
		}

		public static IntPtr AttrProtocolHTTPS
		{
			get
			{
				if (_AttrProtocolHTTPS == IntPtr.Zero)
				{
					_AttrProtocolHTTPS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolHTTPS");
				}
				return _AttrProtocolHTTPS;
			}
		}

		public static IntPtr AttrProtocolHTTPProxy
		{
			get
			{
				if (_AttrProtocolHTTPProxy == IntPtr.Zero)
				{
					_AttrProtocolHTTPProxy = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolHTTPProxy");
				}
				return _AttrProtocolHTTPProxy;
			}
		}

		public static IntPtr AttrProtocolHTTPSProxy
		{
			get
			{
				if (_AttrProtocolHTTPSProxy == IntPtr.Zero)
				{
					_AttrProtocolHTTPSProxy = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolHTTPSProxy");
				}
				return _AttrProtocolHTTPSProxy;
			}
		}

		public static IntPtr AttrProtocolFTPProxy
		{
			get
			{
				if (_AttrProtocolFTPProxy == IntPtr.Zero)
				{
					_AttrProtocolFTPProxy = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolFTPProxy");
				}
				return _AttrProtocolFTPProxy;
			}
		}

		public static IntPtr AttrProtocolSMB
		{
			get
			{
				if (_AttrProtocolSMB == IntPtr.Zero)
				{
					_AttrProtocolSMB = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolSMB");
				}
				return _AttrProtocolSMB;
			}
		}

		public static IntPtr AttrProtocolRTSP
		{
			get
			{
				if (_AttrProtocolRTSP == IntPtr.Zero)
				{
					_AttrProtocolRTSP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolRTSP");
				}
				return _AttrProtocolRTSP;
			}
		}

		public static IntPtr AttrProtocolRTSPProxy
		{
			get
			{
				if (_AttrProtocolRTSPProxy == IntPtr.Zero)
				{
					_AttrProtocolRTSPProxy = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolRTSPProxy");
				}
				return _AttrProtocolRTSPProxy;
			}
		}

		public static IntPtr AttrProtocolDAAP
		{
			get
			{
				if (_AttrProtocolDAAP == IntPtr.Zero)
				{
					_AttrProtocolDAAP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolDAAP");
				}
				return _AttrProtocolDAAP;
			}
		}

		public static IntPtr AttrProtocolEPPC
		{
			get
			{
				if (_AttrProtocolEPPC == IntPtr.Zero)
				{
					_AttrProtocolEPPC = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolEPPC");
				}
				return _AttrProtocolEPPC;
			}
		}

		public static IntPtr AttrProtocolIPP
		{
			get
			{
				if (_AttrProtocolIPP == IntPtr.Zero)
				{
					_AttrProtocolIPP = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolIPP");
				}
				return _AttrProtocolIPP;
			}
		}

		public static IntPtr AttrProtocolNNTPS
		{
			get
			{
				if (_AttrProtocolNNTPS == IntPtr.Zero)
				{
					_AttrProtocolNNTPS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolNNTPS");
				}
				return _AttrProtocolNNTPS;
			}
		}

		public static IntPtr AttrProtocolLDAPS
		{
			get
			{
				if (_AttrProtocolLDAPS == IntPtr.Zero)
				{
					_AttrProtocolLDAPS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolLDAPS");
				}
				return _AttrProtocolLDAPS;
			}
		}

		public static IntPtr AttrProtocolTelnetS
		{
			get
			{
				if (_AttrProtocolTelnetS == IntPtr.Zero)
				{
					_AttrProtocolTelnetS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolTelnetS");
				}
				return _AttrProtocolTelnetS;
			}
		}

		public static IntPtr AttrProtocolIMAPS
		{
			get
			{
				if (_AttrProtocolIMAPS == IntPtr.Zero)
				{
					_AttrProtocolIMAPS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolIMAPS");
				}
				return _AttrProtocolIMAPS;
			}
		}

		public static IntPtr AttrProtocolIRCS
		{
			get
			{
				if (_AttrProtocolIRCS == IntPtr.Zero)
				{
					_AttrProtocolIRCS = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolIRCS");
				}
				return _AttrProtocolIRCS;
			}
		}

		public static IntPtr AttrProtocolPOP3S
		{
			get
			{
				if (_AttrProtocolPOP3S == IntPtr.Zero)
				{
					_AttrProtocolPOP3S = Dlfcn.GetIntPtr(SecItem.securityLibrary, "kSecAttrProtocolPOP3S");
				}
				return _AttrProtocolPOP3S;
			}
		}

		public static IntPtr FromSecProtocol(SecProtocol protocol)
		{
			switch (protocol)
			{
			case SecProtocol.Ftp:
				return AttrProtocolFTP;
			case SecProtocol.FtpAccount:
				return AttrProtocolFTPAccount;
			case SecProtocol.Http:
				return AttrProtocolHTTP;
			case SecProtocol.Irc:
				return AttrProtocolIRC;
			case SecProtocol.Nntp:
				return AttrProtocolNNTP;
			case SecProtocol.Pop3:
				return AttrProtocolPOP3;
			case SecProtocol.Smtp:
				return AttrProtocolSMTP;
			case SecProtocol.Socks:
				return AttrProtocolSOCKS;
			case SecProtocol.Imap:
				return AttrProtocolIMAP;
			case SecProtocol.Ldap:
				return AttrProtocolLDAP;
			case SecProtocol.AppleTalk:
				return AttrProtocolAppleTalk;
			case SecProtocol.Afp:
				return AttrProtocolAFP;
			case SecProtocol.Telnet:
				return AttrProtocolTelnet;
			case SecProtocol.Ssh:
				return AttrProtocolSSH;
			case SecProtocol.Ftps:
				return AttrProtocolFTPS;
			case SecProtocol.Https:
				return AttrProtocolHTTPS;
			case SecProtocol.HttpProxy:
				return AttrProtocolHTTPProxy;
			case SecProtocol.HttpsProxy:
				return AttrProtocolHTTPSProxy;
			case SecProtocol.FtpProxy:
				return AttrProtocolFTPProxy;
			case SecProtocol.Smb:
				return AttrProtocolSMB;
			case SecProtocol.Rtsp:
				return AttrProtocolRTSP;
			case SecProtocol.RtspProxy:
				return AttrProtocolRTSPProxy;
			case SecProtocol.Daap:
				return AttrProtocolDAAP;
			case SecProtocol.Eppc:
				return AttrProtocolEPPC;
			case SecProtocol.Ipp:
				return AttrProtocolIPP;
			case SecProtocol.Nntps:
				return AttrProtocolNNTPS;
			case SecProtocol.Ldaps:
				return AttrProtocolLDAPS;
			case SecProtocol.Telnets:
				return AttrProtocolTelnetS;
			case SecProtocol.Imaps:
				return AttrProtocolIMAPS;
			case SecProtocol.Ircs:
				return AttrProtocolIRCS;
			case SecProtocol.Pop3s:
				return AttrProtocolPOP3S;
			default:
				throw new ArgumentException("protocol");
			}
		}

		public static SecProtocol ToSecProtocol(IntPtr handle)
		{
			if (handle == AttrProtocolFTP)
			{
				return SecProtocol.Ftp;
			}
			if (handle == AttrProtocolFTPAccount)
			{
				return SecProtocol.FtpAccount;
			}
			if (handle == AttrProtocolHTTP)
			{
				return SecProtocol.Http;
			}
			if (handle == AttrProtocolIRC)
			{
				return SecProtocol.Irc;
			}
			if (handle == AttrProtocolNNTP)
			{
				return SecProtocol.Nntp;
			}
			if (handle == AttrProtocolPOP3)
			{
				return SecProtocol.Pop3;
			}
			if (handle == AttrProtocolSMTP)
			{
				return SecProtocol.Smtp;
			}
			if (handle == AttrProtocolSOCKS)
			{
				return SecProtocol.Socks;
			}
			if (handle == AttrProtocolIMAP)
			{
				return SecProtocol.Imap;
			}
			if (handle == AttrProtocolLDAP)
			{
				return SecProtocol.Ldap;
			}
			if (handle == AttrProtocolAppleTalk)
			{
				return SecProtocol.AppleTalk;
			}
			if (handle == AttrProtocolAFP)
			{
				return SecProtocol.Afp;
			}
			if (handle == AttrProtocolTelnet)
			{
				return SecProtocol.Telnet;
			}
			if (handle == AttrProtocolSSH)
			{
				return SecProtocol.Ssh;
			}
			if (handle == AttrProtocolFTPS)
			{
				return SecProtocol.Ftps;
			}
			if (handle == AttrProtocolHTTPS)
			{
				return SecProtocol.Https;
			}
			if (handle == AttrProtocolHTTPProxy)
			{
				return SecProtocol.HttpProxy;
			}
			if (handle == AttrProtocolHTTPSProxy)
			{
				return SecProtocol.HttpsProxy;
			}
			if (handle == AttrProtocolFTPProxy)
			{
				return SecProtocol.FtpProxy;
			}
			if (handle == AttrProtocolSMB)
			{
				return SecProtocol.Smb;
			}
			if (handle == AttrProtocolRTSP)
			{
				return SecProtocol.Rtsp;
			}
			if (handle == AttrProtocolRTSPProxy)
			{
				return SecProtocol.RtspProxy;
			}
			if (handle == AttrProtocolDAAP)
			{
				return SecProtocol.Daap;
			}
			if (handle == AttrProtocolEPPC)
			{
				return SecProtocol.Eppc;
			}
			if (handle == AttrProtocolIPP)
			{
				return SecProtocol.Ipp;
			}
			if (handle == AttrProtocolNNTPS)
			{
				return SecProtocol.Nntps;
			}
			if (handle == AttrProtocolLDAPS)
			{
				return SecProtocol.Ldaps;
			}
			if (handle == AttrProtocolTelnetS)
			{
				return SecProtocol.Telnets;
			}
			if (handle == AttrProtocolIMAPS)
			{
				return SecProtocol.Imaps;
			}
			if (handle == AttrProtocolIRCS)
			{
				return SecProtocol.Ircs;
			}
			if (handle == AttrProtocolPOP3S)
			{
				return SecProtocol.Pop3s;
			}
			throw new ArgumentException("handle");
		}
	}
}
