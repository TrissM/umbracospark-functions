using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;

namespace Moriyama.UmbracoSpark.Functions
{
    public static class LogPublishFunction
    {
        [FunctionName("LogPublishFunction")]
        public static void Run([ServiceBusTrigger("umbraco.publish", "sub.log", 
            Connection = "ServiceBusConnectionSetting")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            UmbracoContent umbracoContent = JsonConvert.DeserializeObject<UmbracoContent>(mySbMsg);

            log.LogInformation($"{umbracoContent.Name} was published by {umbracoContent.Author}");
        }
    }
}
