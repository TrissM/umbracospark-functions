using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Moriyama.UmbracoSpark.Functions
{
    public static class ImageProcessingFunction
    {
        [FunctionName("ImageProcessingFunction")]
        public async static Task Run([ServiceBusTrigger("umbraco.publish", 
                "sub.imageprocessing", 
                Connection = "ServiceBusConnectionSetting")]
            string mySbMsg, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            UmbracoContent umbracoContent = JsonConvert.DeserializeObject<UmbracoContent>(mySbMsg);

            if (string.IsNullOrEmpty(umbracoContent.Image))
            {
                return;
            }

            int id = umbracoContent.Id;
            string imageUrl = umbracoContent.Image;

            byte[] imageBytes;

            using (WebClient webClient = new WebClient())
            {
                imageBytes = webClient.DownloadData(imageUrl);
            }

            if (imageBytes.Length == 0)
            {
                return;
            }

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string visionApiEndPoint = config["VisionApiEndPoint"];
            string visionApiKey = config["VisionApiKey"];

            string responseString = await MakeAnalysisRequest(visionApiEndPoint, visionApiKey, imageBytes);

            VisionApiResponse response = JsonConvert.DeserializeObject<VisionApiResponse>(responseString);
            response.ContentId = id;
            string formattedResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

            log.LogInformation(formattedResponse);

            // Here we could ping back to Umbraco
            string umbracoEndPoint = config["UmbracoEndPoint"];
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(umbracoEndPoint + "/api/ImageInformationApi/PostImageInformation");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
               
                streamWriter.Write(JsonConvert.SerializeObject(response));
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        static async Task<string> MakeAnalysisRequest(string endpoint, string key, byte[] byteData)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                // Request parameters. A third optional parameter is "details".
                // The Analyze Image method returns information about the following
                // visual features:
                // Categories:  categorizes image content according to a
                //              taxonomy defined in documentation.
                // Description: describes the image content with a complete
                //              sentence in supported languages.
                // Color:       determines the accent color, dominant color, 
                //              and whether an image is black & white.
                string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                // Assemble the URI for the REST API method.
                string uri = endpoint + "?" + requestParameters;

                HttpResponseMessage response;

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                return contentString;

              
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
