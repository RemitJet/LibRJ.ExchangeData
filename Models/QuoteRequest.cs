using System;

namespace RemitJet.ExchangeData.Models
{
	public class QuoteRequest
	{
		public string ApiToken { get; set; }
		public string ExchangeMarketCD { get; set; }
		public string ExchangeMarketRef { get; set; }

		public QuoteRequest ()
		{
		}
	}
}

