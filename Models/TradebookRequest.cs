using System;

namespace RemitJet.ExchangeData.Models
{
	public class TradebookRequest
	{
		public string ApiToken { get; set; }
		public string ExchangeMarketCD { get; set; }
		public string ExchangeMarketRef { get; set; }

		public TradebookRequest ()
		{
		}
	}
}

