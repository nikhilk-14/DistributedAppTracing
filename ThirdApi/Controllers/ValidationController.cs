using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThirdApi.Services;

namespace ThirdApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly ILogger<ValidationController> _logger;
        private readonly IValidationService _validationService;

        public ValidationController(ILogger<ValidationController> logger, IValidationService validationService)
        {
            _logger = logger;
            _validationService = validationService;
        }


        [HttpGet]
        [Route("email/{emailId}")]
        public async Task<bool> ValidateEmail([FromRoute] string emailId, [FromQuery] int userId)
        {
            var result = await _validationService.ValidateEmail(emailId, userId);
            return result;
        }
    }
}
