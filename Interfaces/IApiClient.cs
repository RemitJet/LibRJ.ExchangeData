using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace RemitJet.ExchangeData.Interfaces
{
	public interface IApiClient
	{
		NameValueCollection QueryString { get; }
		WebHeaderCollection Headers { get; }

		Task<string> DownloadStringTaskAsync (Uri endpoint);
		void CancelAsync ();
	}
}

