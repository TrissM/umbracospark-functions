using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;

namespace Moriyama.UmbracoSpark.Functions
{
    public static class EmailBlogPostFunction
    {
        [FunctionName("EmailBlogPostFunction")]
        public static void Run([ServiceBusTrigger("umbraco.publish", "sub.blog.email", Connection = "ServiceBusConnectionSetting")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            UmbracoContent umbracoContent = JsonConvert.DeserializeObject<UmbracoContent>(mySbMsg);

            if (umbracoContent.ContentType != "blogpost")
            {
                return;
            }

            // Do the email stuff.
            string emailMessage = $"{umbracoContent.Name} was published by {umbracoContent.Author}";
        }

    }
}
