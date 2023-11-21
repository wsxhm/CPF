using System;

namespace CPF.Mac.CoreLocation
{
	public class CLLocationsUpdatedEventArgs : EventArgs
	{
		public CLLocation[] Locations
		{
			get;
			set;
		}

		public CLLocationsUpdatedEventArgs(CLLocation[] locations)
		{
			Locations = locations;
		}
	}
}
