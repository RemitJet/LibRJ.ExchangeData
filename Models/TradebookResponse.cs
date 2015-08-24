using System;
using System.Collections.Generic;

namespace RemitJet.ExchangeData.Models
{
	public class Trade
	{
		public string ExchangeMarketCD { get; set; }
		public string TradeRef { get; set; }
		public string SettlementCurrencyCD { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public DateTimeOffset FirstSeen { get; set; }
		public DateTimeOffset LastSeen { get; set; }
		public DateTimeOffset ReportedTime { get; set; }
		public byte[] RawData { get; set; }

		public Trade ()
		{
		}
	}

	public class TradebookResponse : List<Trade>
	{
	}
}

