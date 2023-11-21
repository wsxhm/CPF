using System;

namespace CPF.Mac.Foundation
{
	public class UNCDidActivateNotificationEventArgs : EventArgs
	{
		public NSUserNotification Notification
		{
			get;
			set;
		}

		public UNCDidActivateNotificationEventArgs(NSUserNotification notification)
		{
			Notification = notification;
		}
	}
}
