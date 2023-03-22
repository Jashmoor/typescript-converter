using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TypescriptClassConverter.Web.Models;

namespace TypescriptClassConverter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public TestResponse PostRequest(TestRequest request)
        {
            return new TestResponse()
            {
                ResponseDate = DateTime.Now,
                ResponseId = ""
            };
        }

        [HttpGet]
        [Route("multi")]
        public async Task<TestResponse> Multi(TestOtherRequest req)
        {
            return new TestResponse()
            {
                ResponseDate = DateTime.Now,
                ResponseId = ""
            };
        }

        [HttpGet]
        [Route("empty")]
        public async Task<EmptyResponse> TestEmpty(EmptyRequest request)
        {
            return new EmptyResponse()
            {
                Tuple = ("", 0)
            };
        }
    }
}
