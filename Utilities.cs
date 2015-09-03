using System;

namespace RemitJet.ExchangeData
{
	/// <summary>
	/// Miscellaneous bits of functionality that are used within this library.
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// Converts a UNIX timestamp (seconds since Jan-01-1970) to a DateTime object.
		/// </summary>
		/// <returns>The date time from unix timestamp.</returns>
		/// <param name="unixTimestamp">Integer representing a unix timestamp.</param>
		public static DateTime GetDateTimeFromUnixTimestamp(long unixTimestamp)
		{
			var returnTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			returnTime = returnTime.AddSeconds(unixTimestamp);
			return returnTime;
		}
	}
}

