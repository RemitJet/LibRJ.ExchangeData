using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemitJet.ExchangeData.Interfaces;
using RemitJet.ExchangeData.Models;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace RemitJet.ExchangeData.Clients
{
	public class Anx : IGetQuoteApi, ITradebookApi, IOrderbookApi
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

		public Anx ()
		{
			this.QuoteApiUri = new Uri ("https://anxpro.com/api/2/$$/money/ticker");
			this.TradebookApiUri = new Uri ("https://anxpro.com/api/2/$$/money/trade/fetch");
			this.OrderbookApiUri = new Uri ("https://anxpro.com/api/2/$$/money/depth/full");
		}

		public string GenerateRequestSignature(string secret, string apiPath, NameValueCollection queryString)
		{
			string formattedQueryString = String.Join ("&",
				from name in queryString.AllKeys
				select String.Concat (name, "=", WebUtility.UrlEncode (queryString[name]))
			);
			string message = String.Concat (apiPath, ((char)0).ToString(), formattedQueryString);
			byte[] rawMessage = Encoding.ASCII.GetBytes (message);
			byte[] decodedSecret = Convert.FromBase64String (secret);
			string encodedHash = null;

			using (var hmac = new HMACSHA512 (decodedSecret)) {
				byte[] hash = hmac.ComputeHash (rawMessage);
				encodedHash = Convert.ToBase64String (hash);
			}

			return encodedHash;
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			//client.QueryString.Add ("since", "1440701196");

			var endpointUri = new Uri (
				this.QuoteApiUri, 
				this.QuoteApiUri.AbsolutePath.Replace("$$", request.ExchangeMarketRef)
			);
			string signature = this.GenerateRequestSignature (request.ApiSecret, endpointUri.AbsolutePath,
															  client.QueryString);
			
			client.Headers.Add ("Rest-Key", request.ApiToken);
			client.Headers.Add ("Rest-Sign", signature);

			string rawData = await client.DownloadStringTaskAsync (endpointUri);
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

