using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulIntegration.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsulController : ControllerBase
    {
        private readonly ILogger<ConsulController> _logger;
        private readonly IConfiguration _configuration;  

        public ConsulController(ILogger<ConsulController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getvalues")]
        public IEnumerable<string> GetValues()
        {
            List<string> values = new List<string>() { _configuration["key1"], _configuration["key2"], _configuration["key3"] };
            return values;
        }
    }
}
