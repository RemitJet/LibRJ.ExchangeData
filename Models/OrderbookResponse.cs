using System;
using System.Collections.Generic;

namespace RemitJet.ExchangeData.Models
{
	public enum OrderType
	{
		BidOrder = 2,
		AskOrder = 4
	}

	public class Order
	{
		public string ExchangeMarketCD { get; set; }
		public string OrderRef { get; set; }
		public OrderType OrderType { get; set; }
		public string SettlementCurrencyCD { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public DateTimeOffset FirstSeen { get; set; }
		public DateTimeOffset LastSeen { get; set; }
		public DateTimeOffset ReportedTime { get; set; }
		public byte[] RawData { get; set; }

		public Order ()
		{
		}
	}

	public class OrderbookResponse : List<Order>
	{
	}
}

