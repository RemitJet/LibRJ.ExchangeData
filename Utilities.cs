using System;

namespace RemitJet.ExchangeData
{
	public static class Utilities
	{
		public static DateTime GetDateTimeFromUnixTimestamp(long unixTimestamp)
		{
			var returnTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			returnTime = returnTime.AddSeconds(unixTimestamp);
			return returnTime;
		}
	}
}

