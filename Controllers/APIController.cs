using ExchangeRateAPI.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IApiKeysRepository _apiKeysRepository;

        public APIController(IApiKeysRepository keysRepository)
        {
            this._apiKeysRepository = keysRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            bool result;
            string key = _apiKeysRepository.GenerateNewApiKey(out result);

            if (result)
                return Ok(key);
            else
                return BadRequest();
        }
    }
}
