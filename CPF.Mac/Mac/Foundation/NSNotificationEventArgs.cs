using System;

namespace CPF.Mac.Foundation
{
	public class NSNotificationEventArgs : EventArgs
	{
		public NSNotification Notification
		{
			get;
			private set;
		}

		public NSNotificationEventArgs(NSNotification notification)
		{
			Notification = notification;
		}
	}
}
