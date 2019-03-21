using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Moriyama.UmbracoSpark.Functions
{
    public static class StartupFunction
    {
        [FunctionName("StartupFunction")]
        public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}