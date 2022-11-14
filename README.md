# ExchangeRateAPI
ASP.NET core REST API to retrieve exchange rates for the given time periods. Source data are obtained from the European Central Bank.

## Features
- Generate new API Key
- Get exchange rates for given currencies and time period
- Logging all major events

## Tech
- .NET Core 3.1
- MS SQL Server
- Entity Framework Core
- Swashbuckle Swagger
- Newtonsoft.Json
- Serilog
- XUnit

## How to run

1. Clone repository
2. Prepare database:
    - Restore database ExchangeDB from backup file 'ExchangeRateAPI/Database/ExchangeDB.bak' in Microsoft SQL Server Management Studio
    - Create database manually using 'Database.sql' script (script does not contain API Key data)
3. Make sure you have installed all needed packages and dependencies
4. Replace connection strings in appsettings.json with connection string to your database
5. Build and run project ExchangeRateAPI


## Usage
### 1. Get new API Key

1. Send request to '/key' endpoint. For example:
```
https://localhost:44316/key
```
2. API generates new key based on request date and expiration date
3. API sends response. For example:
```
{
    "succeded":true,
    "message":"OK",
    "data":"MTI6MjA6MzU6NjcyLTIyLjA1LjI2"
}
```
4. "data" is your generated API Key.
5. API key is valid by default for 6 months
 
### 2. Get currency rates data

1. Prepare currency codes.
    - Prepare dictionary of 3-letter currency codes
    - For example if you want data about USD -> EUR and PLN -> EUR currency rates you need to prepare dictionary with structure: {"USD":"EUR","PLN":"EUR"}
    - in URL format: %7B%22USD%22%3A%22EUR%22%2C%22PLN%22%3A%22EUR%22%7D
    - right now API can't handle the same currency from if you want USD -> EUR and USD -> PLN you need to send two separate request, otherwise API will return only one of them 
2. Prepare start date.
    - start date specify from what date you want to get the exchange rates
    - date is presented in format yyyy-MM-dd
3. Prepare end date.
    - end date specyfy to what date you want to get the exchange rates
    - date is presented in format yyyy-MM-dd
    - end date is optional, if you dont specify it it means you want only exchange rates from StartDate
4. Prepare API key
    - use valid API Key you have got from /key endpoint
5. Send request. For example:
```
https://localhost:44316/Exchange?CurrencyCodes=%7B%22USD%22%3A%22EUR%22%2C%22PLN%22%3A%22EUR%22%7D&StartDate=2021-11-20&EndDate=2021-11-21&ApiKey=MTI6MjA6MzU6NjcyLTIyLjA1LjI2
```
6. API will try to gather all requested data and send response. For example:
```
{
  "succeded": true,
  "message": "OK",
  "data": [
    {
      "dateOfExchangeRate": "2021-11-20T00:00:00",
      "currencyFrom": "PLN",
      "currencyTo": "EUR",
      "exchangeRate": 4.6818
    },
    {
      "dateOfExchangeRate": "2021-11-21T00:00:00",
      "currencyFrom": "PLN",
      "currencyTo": "EUR",
      "exchangeRate": 4.6818
    },
    {
      "dateOfExchangeRate": "2021-11-20T00:00:00",
      "currencyFrom": "USD",
      "currencyTo": "EUR",
      "exchangeRate": 1.1271
    },
    {
      "dateOfExchangeRate": "2021-11-21T00:00:00",
      "currencyFrom": "USD",
      "currencyTo": "EUR",
      "exchangeRate": 1.1271
    }
  ]
}
```
7. If some of exchange rates for given currencies are unknown, API will return only known information
8. If you send request with date from future or unknown currencies you will get 404 Not Found response
