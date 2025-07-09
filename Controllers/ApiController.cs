using Hangfire;
using Microsoft.AspNetCore.Mvc;
// using AwlrAziz.Helpers;
using AwlrAziz.Interfaces;
using AwlrAziz.Jobs;
using AwlrAziz.Models.Customs;
using Microsoft.AspNetCore.Http;
using AspNetCoreRateLimit;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace AwlrAziz.Controllers
{
    [Route("api/device")]
    public class ApiController : BaseController
    {
        private readonly ConcurrentDictionary<string, DateTime> lastRequestTimestamps = new ConcurrentDictionary<string, DateTime>();
        private readonly IClientPolicyStore _clientPolicyStore;

        public ApiController() { }

        [HttpGet("store")]
        public async Task Store(ExistingRequest request)
        {
            try
            {
                BackgroundJob.Enqueue<IExistingReadingJob>(task => task.Reading(request));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, ExistingRequest = request });
                throw;
            }
        }
    }
}