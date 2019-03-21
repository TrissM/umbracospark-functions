using System.Net;
using System.Net.Http;
using System.Web.Http;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace Moriyama.UmbracoSpark.Controllers
{
    public class ImageInformationApiController : UmbracoApiController
    {
        private readonly IContentService _contentService;

        public ImageInformationApiController(IContentService contentService)
        {
            this._contentService = contentService;
        }

        [HttpPost]
        public object PostImageInformation([FromBody] VisionApiResponse data)
        {
            // Obviously authenticate these requests!!!!
            IContent content = this._contentService.GetById(data.ContentId);
            if (content.HasProperty("visionMetadata")) { 
            
                content.SetValue("visionMetadata", JsonConvert.SerializeObject(data, Formatting.Indented));
                this._contentService.Save(content);
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, data.requestId);
        }
    }
}