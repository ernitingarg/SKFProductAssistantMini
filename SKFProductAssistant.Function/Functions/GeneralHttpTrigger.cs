using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace SKFProductAssistant.Function.Functions
{
    public class GeneralHttpTrigger
    {
        [FunctionName(nameof(HealthCheck))]
        public IActionResult HealthCheck(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "GET",
                Route = "healthcheck")]
            HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(new { Status = "alive" });
        }

        [FunctionName(nameof(GetInfo))]
        public IActionResult GetInfo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "GET",
                Route = "getinfo")]
            HttpRequest req,
            ILogger log)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string version = FileVersionInfo.GetVersionInfo(assembly.Location)
                .ProductVersion;

            return new OkObjectResult(new { Version = version });
        }
    }
}
