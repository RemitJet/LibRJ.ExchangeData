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
	public class WesternUnion : IGetQuoteApi
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

		public WesternUnion ()
		{
			this.QuoteApiUri = new Uri ("http://onlinefx.westernunion.ca/currency-calculator/json.aspx");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			client.QueryString.Add ("amount", "1");
			client.QueryString.Add ("curr", request.ExchangeMarketRef.Substring(0, 3));
			client.QueryString.Add ("refCurr", request.ExchangeMarketRef.Substring(3, 3));

			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				LastTrade = Decimal.Parse(jsonData["rate"].Value<string>()),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}
	}
}

