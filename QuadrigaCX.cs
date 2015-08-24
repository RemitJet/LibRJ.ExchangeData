using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemitJet.ExchangeData.Interfaces;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData
{
	public class QuadrigaCX : IGetQuoteApi, ITradebookApi, IOrderbookApi
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

		public QuadrigaCX ()
		{
			this.QuoteApiUri = new Uri ("https://api.quadrigacx.com/v2/ticker");
			this.TradebookApiUri = new Uri ("https://api.quadrigacx.com/v2/transactions");
			this.OrderbookApiUri = new Uri ("https://api.quadrigacx.com/v2/order_book");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("book", request.ExchangeMarketRef);

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				DailyVolume = Decimal.Parse(jsonData["volume"].Value<string>()),
				DailyHigh = Decimal.Parse(jsonData["high"].Value<string>()),
				DailyLow = Decimal.Parse(jsonData["low"].Value<string>()),
				LastBid = Decimal.Parse(jsonData["bid"].Value<string>()),
				LastAsk = Decimal.Parse(jsonData["ask"].Value<string>())
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
				from rawOrder in jsonData ["bids"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Amount = Decimal.Parse (rawOrder ["amount"].Value<string> ()),
					Price = Decimal.Parse (rawOrder ["price"].Value<string> ()),
					OrderType = OrderType.BidOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			response.AddRange (
				from rawOrder in jsonData ["asks"]
				select new Order () {
					ExchangeMarketCD = request.ExchangeMarketCD,
					Amount = Decimal.Parse (rawOrder ["amount"].Value<string> ()),
					Price = Decimal.Parse (rawOrder ["price"].Value<string> ()),
					OrderType = OrderType.AskOrder,
					RawData = Encoding.ASCII.GetBytes (rawOrder.ToString ()),
					LastSeen = DateTimeOffset.UtcNow
				}
			);

			return response;
		}
	}
}