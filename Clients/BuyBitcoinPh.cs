using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RemitJet.ExchangeData.Interfaces;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData.Clients
{
	public class BuyBitcoinPh : IGetQuoteApi
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

		public BuyBitcoinPh ()
		{
			this.QuoteApiUri = new Uri ("https://api.buybitcoin.ph/rates");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				LastBid = Decimal.Parse(jsonData["buy_price"].Value<string>()),
				LastAsk = Decimal.Parse(jsonData["sell_price"].Value<string>()),
				LastTrade = Decimal.Parse(jsonData["remit_partner_price"].Value<string>()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}
	}
}

