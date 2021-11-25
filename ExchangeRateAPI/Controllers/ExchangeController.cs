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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ExchangeRateAPI.Models.Responses;

namespace ExchangeRateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ILogger<ExchangeController> _logger;
        private readonly IEcbApiHandler _ecbApiHandler;
        private readonly IApiKeysRepository _apiKeysRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;

        public ExchangeController(ILogger<ExchangeController> logger, IEcbApiHandler ecbApiHandler, IApiKeysRepository keysRepository, IExchangeRatesRepository ratesRepository)
        {
            this._logger = logger;
            this._ecbApiHandler = ecbApiHandler;
            this._apiKeysRepository = keysRepository;
            this._exchangeRatesRepository = ratesRepository;
        }

        /// <summary>
        /// Get exchange rates
        /// </summary>
        /// <remarks>Returns set of exchange rates for given currencies and time period</remarks>
        /// <param name="exchangeRequest">Request model consisting of Dictionary(string, string) (where Key is Currency From and Value is Currency To), StartDate, EndDate and Valid API Key</param>
        /// <response code="200">Returns list of exchange rates for given currencies and time period</response>
        /// <response code="400">Incorrect input data</response>
        /// <response code="404">There is no data for received request</response>
        /// <response code="500">Server side error</response>
        [HttpGet()]
        [ProducesResponseType(typeof(Response<List<ExchangeResponseModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery]ExchangeRequestModel exchangeRequest)
        {
            try
            {
                List<ExchangeResponseModel> response = new List<ExchangeResponseModel>();

                #region [ DATE CHECK ]

                if (exchangeRequest.EndDate == null)
                    exchangeRequest.EndDate = exchangeRequest.StartDate;

                if (exchangeRequest.StartDate.Date > DateTime.Now.Date)
                {
                    _logger.LogInformation("Received StartDate from future.");
                    return NotFound(new ErrorResponse("Data not found", new ApiError { Key = nameof(exchangeRequest.StartDate), Message = "Given value refers to future"}));
                }

                if (exchangeRequest.StartDate > exchangeRequest.EndDate)
                {
                    _logger.LogInformation("Received StartDate greater than EndDate.");
                    return BadRequest(new ErrorResponse($"Data valiation failed", new ApiError { Key = nameof(exchangeRequest.StartDate), Message = $"{ nameof(exchangeRequest.StartDate) } is greater than { nameof(exchangeRequest.EndDate) }" }));
                }
                #endregion

                if (!_apiKeysRepository.CheckKey(exchangeRequest.ApiKey))
                {
                    _logger.LogInformation("Received invalid ApiKey");
                    return BadRequest(new ErrorResponse("Given API Key is expired or invalid"));
                }

                #region [ LOCAL DATA REQUEST ]

                var localData = _exchangeRatesRepository.SelectExchangeRates(exchangeRequest.CurrencyCodesDict,
                                                                             exchangeRequest.StartDate,
                                                                             exchangeRequest.EndDate.Value);

                if(localData.Count == (exchangeRequest.CurrencyCodesDict.Count * ((exchangeRequest.EndDate.Value - exchangeRequest.StartDate).Days + 1)))
                {
                    response = localData.MappExchangeRatesToResponseModelList();
                    _logger.LogInformation("Local database has requested data");
                    _logger.LogInformation("Exchange rates response sent.");
                    return Ok(new Response<List<ExchangeResponseModel>>(response));
                }

                #endregion
                
                // get data from ECB
                var listedData = await _ecbApiHandler.GetExchangeData(exchangeRequest.CurrencyCodesDict,
                                                                      exchangeRequest.StartDate,
                                                                      exchangeRequest.EndDate.Value);

                //fill local database with data
                _exchangeRatesRepository.FillExchangeRates(listedData, exchangeRequest.StartDate, exchangeRequest.EndDate.Value);


                response = listedData.Where(x => x.DateOfExchangeRate >= exchangeRequest.StartDate && 
                                                 x.DateOfExchangeRate <= exchangeRequest.EndDate)
                                     .ToList();

                if (response.Count < 1)
                {
                    _logger.LogInformation("Data not found for given request.");
                    return NotFound(new ErrorResponse("Data not found"));
                }

                _logger.LogInformation("Exchange rates response sent.");
                return Ok(new Response<List<ExchangeResponseModel>>(response));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new ErrorResponse("An exception occured during processing"));
            }
        }
    }
}
