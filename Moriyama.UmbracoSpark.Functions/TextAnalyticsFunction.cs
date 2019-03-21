using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;

namespace Moriyama.UmbracoSpark.Functions
{
    public static class TextAnalyticsFunction
    {
        [FunctionName("TextAnalyticsFunction")]
        public static void Run([ServiceBusTrigger("umbraco.publish", "sub.textanalytics", Connection = "ServiceBusConnectionSetting")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            UmbracoContent umbracoContent = JsonConvert.DeserializeObject<UmbracoContent>(mySbMsg);

            if (string.IsNullOrEmpty(umbracoContent.Text))
            {
                return;
            }

            

        }

    }
}
