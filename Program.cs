using System;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			//var api = new RebitPh (); // ApiToken = "yTvzB1pALESPyxb_yNkKrAsyuD42b5wo"
			var api = new Coinage();

			var task = api.GetQuote (new QuoteRequest());
			//var task = api.GetTradebook(new TradebookRequest() { ExchangeMarketRef = "BTCCAD" });
			//var task = api.GetOrderbook(new OrderbookRequest() { ExchangeMarketRef = "BTCCAD" });

			task.Wait ();
		}
	}
}
