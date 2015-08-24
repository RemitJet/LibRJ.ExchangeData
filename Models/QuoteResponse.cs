using System;

namespace RemitJet.ExchangeData.Models
{
	public class QuoteResponse
	{
		public string ExchangeMarketCD { get; set; }
		public decimal DailyOpen { get; set; }
		public decimal DailyClose { get; set; }
		public decimal DailyHigh { get; set; }
		public decimal DailyLow { get; set; }
		public decimal DailyAverage { get; set; }
		public decimal DailyVolume { get; set; }
		public decimal WeeklyVolume { get; set; }
		public decimal LastTrade { get; set; }
		public decimal LastBid { get; set; }
		public decimal LastAsk { get; set; }
		public DateTimeOffset SeenTime { get; set; }
		public DateTimeOffset ReportedTime { get; set; }
		public byte[] RawData  { get; set; }

		public QuoteResponse ()
		{
		}
	}
}