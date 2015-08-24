using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemitJet.ExchangeData.Interfaces;
using RemitJet.ExchangeData.Models;
using System.Globalization;

namespace RemitJet.ExchangeData
{
	public class Coinage : IGetQuoteApi
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

		public Coinage ()
		{
			this.QuoteApiUri = new Uri ("https://api.coinage.ph/trade/ticker");
		}

		public async Task<QuoteResponse> GetQuote(QuoteRequest request)
		{
			var client = this.ApiClient();
			string rawData = await client.DownloadStringTaskAsync (this.QuoteApiUri);
			var jsonData = JToken.Parse (rawData);

			var quote = new QuoteResponse () {
				ExchangeMarketCD = request.ExchangeMarketCD,
				LastTrade = Decimal.Parse(jsonData["last"]["value"].Value<string>()),
				ReportedTime = DateTimeOffset.Parse(jsonData["last"]["timestamp"].Value<string>(),
													CultureInfo.InvariantCulture.DateTimeFormat,
													DateTimeStyles.AssumeUniversal),
				RawData = Encoding.ASCII.GetBytes (jsonData.ToString())
			};

			return quote;
		}
	}
}

