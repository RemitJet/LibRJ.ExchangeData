using System;
using System.Threading.Tasks;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData.Interfaces
{
	public interface ITradebookApi
	{
		/// <summary>
		/// Gets or sets the URI to the exchange's tradebook API.
		/// </summary>
		/// <value>URI to the exchange's tradebook API.</value>
		Uri TradebookApiUri { get; set; }

		/// <summary>
		/// Contacts the exchange and returns the latest trades.
		/// </summary>
		/// <returns>Collection of trades.</returns>
		/// <param name="request">Parameters for the request (ie. currency pair, etc)</param>
		Task<TradebookResponse> GetTradebook(TradebookRequest request);
	}
}

