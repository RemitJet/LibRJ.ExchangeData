using System;
using System.Runtime.Serialization;

namespace RemitJet.ExchangeData
{
	public class ExchangeDataException : Exception
	{
		public ExchangeDataException() : base() {}
		public ExchangeDataException(string message) : base(message) {}
		public ExchangeDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public ExchangeDataException(string message, Exception innerException) : base(message, innerException) { }
			
	}
}

