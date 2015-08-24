using System;
using System.Threading.Tasks;
using RemitJet.ExchangeData.Models;

namespace RemitJet.ExchangeData.Interfaces
{
	public interface IOrderbookApi
	{
		/// <summary>
		/// Gets or sets the URI to the exchange's orderbook API.
		/// </summary>
		/// <value>URI to the exchange's orderbook API.</value>
		Uri OrderbookApiUri { get; set; }

		/// <summary>
		/// Contacts the exchange and returns the latest orders.
		/// </summary>
		/// <returns>Collection of orders.</returns>
		/// <param name="request">Parameters for the request (ie. currency pair, etc)</param>
		Task<OrderbookResponse> GetOrderbook(OrderbookRequest request);
	}
}

