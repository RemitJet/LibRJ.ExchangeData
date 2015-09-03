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
	public class Taurus : IGetQuoteApi, ITradebookApi, IOrderbookApi
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

		public Taurus ()
		{
			this.QuoteApiUri = new Uri ("http://api.taurusexchange.com/ticker");
			this.TradebookApiUri = new Uri ("http://api.taurusexchange.com/transactions");
			this.OrderbookApiUri = new Uri ("http://api.taurusexchange.com/order_book");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("book", request.ExchangeMarketRef);

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				DailyHigh = Decimal.Parse(jsonData["high"].Value<string>()),
				DailyLow = Decimal.Parse(jsonData["low"].Value<string>()),
				DailyVolume = Decimal.Parse(jsonData["volume"].Value<string>()),
				LastBid = Decimal.Parse(jsonData["bid"].Value<string>()),
				LastAsk = Decimal.Parse(jsonData["ask"].Value<string>()),
				LastTrade = Decimal.Parse(jsonData["last"].Value<string>()),
				ReportedTime = Utilities.GetDateTimeFromUnixTimestamp (jsonData["timestamp"].Value<long> ()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}

		public async Task<TradebookResponse> GetTradebook(TradebookRequest request)
		{
			var response = new TradebookResponse ();
			var client = this.ApiClient ();

			client.QueryString.Add ("book", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.TradebookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawTrade in jsonData
				select new Trade () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					TradeRef = rawTrade ["tid"].Value<string> (),
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

			client.QueryString.Add ("book", request.ExchangeMarketRef);
			string rawData = await client.DownloadStringTaskAsync (this.OrderbookApiUri);
			var jsonData = JToken.Parse (rawData);

			response.AddRange (
				from rawOrder in jsonData["bids"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Price = Decimal.Parse (rawOrder [0].Value<string> ()),
					Amount = Decimal.Parse (rawOrder [1].Value<string> ()),
					OrderType = OrderType.BidOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			response.AddRange (
				from rawOrder in jsonData["asks"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Price = Decimal.Parse (rawOrder[0].Value<string> ()),
					Amount = Decimal.Parse (rawOrder[1].Value<string> ()),
					OrderType = OrderType.AskOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}
	}
}

