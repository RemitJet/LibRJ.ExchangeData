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
	public class Kraken : IGetQuoteApi, ITradebookApi, IOrderbookApi
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
		public Uri QuoteApiUri_OHLC { get; set; } 
		public Uri TradebookApiUri { get; set; }
		public Uri OrderbookApiUri { get; set; } 

		public Kraken ()
		{
			this.QuoteApiUri = new Uri ("https://api.kraken.com/0/public/Ticker");
			this.QuoteApiUri_OHLC = new Uri ("https://api.kraken.com/0/public/OHLC");
			this.TradebookApiUri = new Uri ("https://api.kraken.com/0/public/Trades");
			this.OrderbookApiUri = new Uri ("https://api.kraken.com/0/public/Depth");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("pair", request.ExchangeMarketRef);

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);
			var quoteData = jsonData["result"][request.ExchangeMarketRef];

			rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri_OHLC);
			jsonData = JToken.Parse (rawData);
			var ohlcData = jsonData["result"][request.ExchangeMarketRef];

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				DailyOpen = Decimal.Parse(ohlcData[1].Value<string>()),
				DailyHigh = Decimal.Parse(ohlcData[2].Value<string>()),
				DailyLow = Decimal.Parse(ohlcData[3].Value<string>()),
				DailyClose = Decimal.Parse(ohlcData[4].Value<string>()),
				DailyVolume = Decimal.Parse(ohlcData[6].Value<string>()),
				LastBid = Decimal.Parse(quoteData["b"][0].Value<string>()),
				LastAsk = Decimal.Parse(quoteData["a"][0].Value<string>()),
				LastTrade = Decimal.Parse(quoteData["c"][0].Value<string>()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}

		public async Task<TradebookResponse> GetTradebook(TradebookRequest request)
		{
			var response = new TradebookResponse ();
			var client = this.ApiClient ();

			client.QueryString.Add ("pair", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.TradebookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawTrade in jsonData["result"][request.ExchangeMarketRef]
				select new Trade () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Price = Decimal.Parse (rawTrade[0].Value<string> ()),
					Amount = Decimal.Parse (rawTrade[1].Value<string> ()),
					RawData = Encoding.ASCII.GetBytes (rawTrade.ToString ()),
					ReportedTime = Utilities.GetDateTimeFromUnixTimestamp (rawTrade[2].Value<long> ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}

		public async Task<OrderbookResponse> GetOrderbook(OrderbookRequest request)
		{
			var response = new OrderbookResponse ();
			var client = this.ApiClient ();

			client.QueryString.Add ("pair", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.OrderbookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawOrder in jsonData["result"][request.ExchangeMarketRef]["bids"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Price = Decimal.Parse (rawOrder[0].Value<string> ()),
					Amount = Decimal.Parse (rawOrder[1].Value<string> ()),
					OrderType = OrderType.BidOrder,
					ReportedTime = Utilities.GetDateTimeFromUnixTimestamp (rawOrder[2].Value<long> ()),
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			response.AddRange (
				from rawOrder in jsonData["result"][request.ExchangeMarketRef]["asks"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Price = Decimal.Parse (rawOrder[0].Value<string> ()),
					Amount = Decimal.Parse (rawOrder[1].Value<string> ()),
					OrderType = OrderType.AskOrder,
					ReportedTime = Utilities.GetDateTimeFromUnixTimestamp (rawOrder[2].Value<long> ()),
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}
	}
}

