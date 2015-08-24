using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemitJet.ExchangeData.Interfaces;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData.Clients
{
	public class Cavirtex : IGetQuoteApi, ITradebookApi, IOrderbookApi
	{
		private Func<IApiClient> _apiClient;
		public Func<IApiClient> ApiClient
		{
			get {
				if (this._apiClient == null)
					this._apiClient = () => {
					return new ApiClient ();
				};
				return this._apiClient;
			}
			set {
				this._apiClient = value;
			}
		}

		public Uri QuoteApiUri { get; set; } 
		public Uri TradebookApiUri { get; set; }
		public Uri OrderbookApiUri { get; set; } 

		public Cavirtex ()
		{
			this.QuoteApiUri = new Uri ("https://cavirtex.com/api2/ticker.json");
			this.TradebookApiUri = new Uri ("https://cavirtex.com/api2/trades.json");
			this.OrderbookApiUri = new Uri ("https://cavirtex.com/api2/orderbook.json");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("currencypair", request.ExchangeMarketRef);

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quoteData = jsonData["ticker"][request.ExchangeMarketRef];

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				DailyVolume = Decimal.Parse(quoteData["volume"].Value<string>()),
				DailyHigh = Decimal.Parse(quoteData["high"].Value<string>()),
				DailyLow = Decimal.Parse(quoteData["low"].Value<string>()),
				LastBid = Decimal.Parse(quoteData["buy"].Value<string>()),
				LastAsk = Decimal.Parse(quoteData["sell"].Value<string>()),
				LastTrade = Decimal.Parse(quoteData["last"].Value<string>()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}

		public async Task<TradebookResponse> GetTradebook(TradebookRequest request)
		{
			var response = new TradebookResponse ();
			var client = this.ApiClient ();

			client.QueryString.Add ("currencypair", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.TradebookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawTrade in jsonData["trades"]
				select new Trade () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					TradeRef = rawTrade ["id"].Value<string> (),
					Price = Decimal.Parse (rawTrade ["price"].Value<string> ()),
					Amount = Decimal.Parse (rawTrade ["amount"].Value<string> ()),
					RawData = Encoding.ASCII.GetBytes (rawTrade.ToString ()),
					ReportedTime = Utilities.GetDateTimeFromUnixTimestamp (rawTrade ["date"].Value<long> ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}

		public async Task<OrderbookResponse> GetOrderbook(OrderbookRequest request)
		{
			var response = new OrderbookResponse ();
			var client = this.ApiClient ();

			client.QueryString.Add ("currencypair", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.OrderbookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawOrder in jsonData["orderbook"]["bids"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Amount = Decimal.Parse (rawOrder [1].Value<string> ()),
					Price = Decimal.Parse (rawOrder [0].Value<string> ()),
					OrderType = OrderType.BidOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			response.AddRange (
				from rawOrder in jsonData["orderbook"]["asks"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Amount = Decimal.Parse (rawOrder[1].Value<string> ()),
					Price = Decimal.Parse (rawOrder[0].Value<string> ()),
					OrderType = OrderType.AskOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}
	}
}

