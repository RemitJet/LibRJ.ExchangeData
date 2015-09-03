using System;

namespace RemitJet.ExchangeData.Models
{
	public class OrderbookRequest
	{
		public string ApiToken { get; set; }
		public string ApiSecret { get; set; }
		public string ExchangeMarketCD { get; set; }
		public string ExchangeMarketRef { get; set; }

		public OrderbookRequest ()
		{
		}
	}
}

