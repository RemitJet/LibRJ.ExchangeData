RemitJet.ExchangeData
=====================

ExchangeData is a library to support collecting quote, tradebook and orderbook information from a number of different
data providers.  The purpose of this is to provide up-to-date information on the state of a partner and guide Remit Jet
when making purchasing decisions. Originally this was intended to be Bitcoin exchanges only but has grown to include 
"rebittance" providers and other financial service entities (such as Western Union, Bank of Canada, iRemit, etc).

## Supported Data Providers

| Type       | Name           | Supported feeds
| ---------- | -------------- | ---------------------------
| Exchange   | ANX            | Quote, Tradebook, Orderbook
| Exchange   | Cavirtex       | Quote, Tradebook, Orderbook
| Exchange   | Kraken         | Quote, Tradebook, Orderbook
| Exchange   | QuadrigaCX     | Quote, Tradebook, Orderbook
| Exchange   | Taurus         | Quote, Tradebook, Orderbook
| Rebittance | BuyBitcoin.ph  | Quote
| Rebittance | Coinage.ph     | Quote
| Rebittance | Coins.ph       | Quote
| Rebittance | Rebit.ph       | Quote
| Other      | Bank of Canada | Quote
| Other      | iRemit         | Quote (work in progress)
| Other      | Western Union  | Quote

## Upcoming features

Several data providers (ANX comes to mind, but likely others) support WebSockets and will push new data to us as they
become available.  We should extend this library to support streaming updates.
