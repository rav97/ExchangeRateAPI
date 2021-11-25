using ExchangeRateAPI.Models.Responses;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Controllers
{
    [ApiController]
    [Route("key")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;
        private readonly IApiKeysRepository _apiKeysRepository;

        public APIController(ILogger<APIController> logger, IApiKeysRepository keysRepository)
        {
            this._logger = logger;
            this._apiKeysRepository = keysRepository;
        }

        /// <summary>
        /// Get new API Key
        /// </summary>
        /// <remarks>Generates and returns new API Key valid for 6 months</remarks>
        /// <response code="200">Returns new API Key string valid for 6 months</response>
        /// <response code="500">Server side error prevented generation of new API Key</response>
        [HttpGet]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                bool result = false;
                string key = _apiKeysRepository.GenerateNewApiKey(out result);

                if (result)
                {
                    _logger.LogInformation("New API Key generated");
                    return Ok(new Response<string>(key));
                }
                else
                {
                    _logger.LogWarning("Error while generating or saving new API Key");
                    return StatusCode(500, new ErrorResponse("Error while generating or saving new API Key"));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new ErrorResponse("An exception occured during processing"));
            }
        }
    }
}
