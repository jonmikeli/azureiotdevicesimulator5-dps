using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace IoT.Simulator.API.DeviceManagement.API.Controllers.IoT.V1
{
    [ApiVersion("1.0")]
    [Route("api/exceptions")]
    [Route("api/v{version:apiVersion}/exceptions")]
    [ApiController]
    public class ExceptionControllerV1 : Controller
    {
        private ILogger<ExceptionControllerV1> _logger;

#pragma warning disable CS1591
        public ExceptionControllerV1(ILogger<ExceptionControllerV1> logger)
        {
            _logger = logger;
        }
#pragma warning restore CS1591

        #region Excetpions
        /// <summary>
        /// Endpoint allowing to raise an exception and test how they are handled.
        /// </summary>
        /// <param name="message" type="string">String with the content of the exception.</param>
        /// <returns>Exception</returns>
        [HttpGet("{message}")]
        [ProducesErrorResponseType(typeof(Exception))]
        public async Task RaiseException(string message)
        {
            _logger.LogDebug($"ExceptionControllerV1::RaiseException::{message}");

            throw new Exception(message);
        }
        #endregion
    }
}