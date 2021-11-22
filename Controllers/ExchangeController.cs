using ExchangeRateAPI.Database;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Services.Helpers.Interfaces;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using ExchangeRateAPI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IEcbApiHandler _ecbApiHandler;
        private readonly IApiKeysRepository _apiKeysRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;

        public ExchangeController(IEcbApiHandler ecbApiHandler, IApiKeysRepository keysRepository, IExchangeRatesRepository ratesRepository)
        {
            this._ecbApiHandler = ecbApiHandler;
            this._apiKeysRepository = keysRepository;
            this._exchangeRatesRepository = ratesRepository;
        }
        
        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery]ExchangeRequestModel exchangeRequest)
        {
            List<ExchangeResponseModel> response = new List<ExchangeResponseModel>();

            if (exchangeRequest.EndDate == null)
                exchangeRequest.EndDate = exchangeRequest.StartDate;

            if (exchangeRequest.StartDate.Date > DateTime.Now.Date)
                return BadRequest();

            if (exchangeRequest.StartDate > exchangeRequest.EndDate)
                return BadRequest();

            if (!_apiKeysRepository.CheckKey(exchangeRequest.ApiKey))
                return Forbid();


            var result = await _ecbApiHandler.GetExchangeData(exchangeRequest.CurrencyCodesDict, 
                                                              exchangeRequest.StartDate, 
                                                              exchangeRequest.EndDate);

            response = result.MappGenericDataToResponseModelList();
            return Ok(response);
        }
    }
}
