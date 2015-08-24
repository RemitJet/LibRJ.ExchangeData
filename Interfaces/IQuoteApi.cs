using System;
using System.Threading.Tasks;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData.Interfaces
{
	public interface IGetQuoteApi
	{
		/// <summary>
		/// Gets or sets the URI to the exchange's quote API.
		/// </summary>
		/// <value>URI to the exchange's quote API.</value>
		Uri QuoteApiUri { get; set; }

		/// <summary>
		/// Contacts the exchange and returns a new quote.
		/// </summary>
		/// <returns>The quote.</returns>
		/// <param name="request">Parameters for the request (ie. currency pair, etc)</param>
		Task<QuoteResponse> GetQuote (QuoteRequest request);
	}
}

