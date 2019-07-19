using System;
using System.Globalization;

namespace System.Ex
{
	public static class DateTimeEx
	{
		public static readonly DateTime unixEpoch = new DateTime(1970, 1, 1);
		
		public static string ToLocalizedString(this DateTime d)
		{
			DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
			return d.ToString("g", dtfi);
		}
		
		public static TimeSpan ToUnixEpoch(this DateTime d)
		{
			return d.Subtract(unixEpoch);
		}
		
		
		/// <summary>
		/// Return the number of milliseconds since the Unix epoch (1 Jan., 1970 UTC) for a given DateTime value.
		/// </summary>
		/// <param name="dateTime">A UTC DateTime value not before epoch.</param>
		/// <returns>Number of whole milliseconds after epoch.</returns>
		/// <exception cref="ArgumentException">'dateTime' is before epoch.</exception>
		public static long ToUnixMs(this DateTime d)
		{
			if (d.CompareTo(unixEpoch) < 0)
				throw new ArgumentException("DateTime value may not be before the epoch", "dateTime");
			
			return (d.Ticks-unixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;
		}
		
		/// <summary>
		/// Create a DateTime value from the number of milliseconds since the Unix epoch (1 Jan., 1970 UTC).
		/// </summary>
		/// <param name="unixMs">Number of milliseconds since the epoch.</param>
		/// <returns>A UTC DateTime value</returns>
		public static DateTime UnixSecToDateTime(this Int32 unixSec)
		{
			return new DateTime(unixSec * TimeSpan.TicksPerSecond+unixEpoch.Ticks, DateTimeKind.Utc).ToLocalTime();
		}
		
		/// <summary>
		/// Create a DateTime value from the number of milliseconds since the Unix epoch (1 Jan., 1970 UTC).
		/// </summary>
		/// <param name="unixMs">Number of milliseconds since the epoch.</param>
		/// <returns>A UTC DateTime value</returns>
		public static DateTime UnixMilliToDateTime(this Int64 unixMilli)
		{
			return new DateTime(unixMilli * TimeSpan.TicksPerMillisecond+unixEpoch.Ticks, DateTimeKind.Utc).ToLocalTime();
		}
		
		public static bool IsPassed(this DateTime localTime)
		{
			return localTime < System.DateTime.Now;
		}
		
		public static TimeSpan GetRemains(this DateTime localTime)
		{
			return localTime - System.DateTime.Now;
		}
	}
}

