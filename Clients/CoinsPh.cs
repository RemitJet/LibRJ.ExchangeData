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
	public class CoinsPh : IGetQuoteApi
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

		public CoinsPh ()
		{
			this.QuoteApiUri = new Uri ("https://coins.ph/api/v1/quote");
		}

		private void CheckResponse(JToken jsonData)
		{
			if (jsonData["success"] != null && !jsonData["success"].Value<bool>()) {
				if (jsonData ["message"] != null) {
					throw new ExchangeDataException (jsonData["message"].Value<string>());
				} else {
					throw new ExchangeDataException ("Did not receive success flag from exchange response.");
				}
			}
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("currencypair", request.ExchangeMarketRef);

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			this.CheckResponse (jsonData);

			var quoteData = jsonData["quote"];

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				LastBid = Decimal.Parse(quoteData["bid"].Value<string>()),
				LastAsk = Decimal.Parse(quoteData["ask"].Value<string>()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}
	}
}

