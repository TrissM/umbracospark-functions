using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Moriyama.UmbracoSpark.Code.Interfaces;
using Moriyama.UmbracoSpark.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Moriyama.UmbracoSpark.Code.Services
{
    public class AzureServiceBusContentPublishNotificationService : IContentPublishNotificationProvider
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _topicName;

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUserService _userService;
      
        public AzureServiceBusContentPublishNotificationService(IUmbracoContextAccessor umbracoContextAccessor, IUserService userService)
        {
            this._serviceBusConnectionString = ConfigurationManager.AppSettings["ServiceBus:ConnectionString"];
            this._topicName = ConfigurationManager.AppSettings["ServiceBus:Topic"];

            this._umbracoContextAccessor = umbracoContextAccessor;
            this._userService = userService;         
        }

        public void NotifyPublish(IContent content)
        {          
            // Get the properties from the content
            string name = content.Name;
            DateTime? publishDate = content.PublishDate;
            string absoluteUrl = this._umbracoContextAccessor.UmbracoContext.UrlAbsolute(content.Id);

            // Get the author
            IUser author = this._userService.GetUserById(content.WriterId);
            string authorName = author.Name;
            
            // Build up a POCO - to put on the service bus
            UmbracoContent umbracoContent = new UmbracoContent();

            umbracoContent.Id = content.Id;
            umbracoContent.Name = name;
            umbracoContent.Url = absoluteUrl;
            umbracoContent.Author = authorName;
            umbracoContent.PublishDate = publishDate == null ? DateTime.Now : publishDate.Value;
            umbracoContent.ContentType = content.ContentType.Alias;

            if (content.HasProperty("excerpt"))
            {
                umbracoContent.Text = content.GetValue<string>("excerpt");
            }

            if (content.HasProperty("photo"))
            {
                IPublishedContent publishedContent = this._umbracoContextAccessor.UmbracoContext.ContentCache.GetById(content.Id);

                IPublishedContent image = publishedContent.Value<IPublishedContent>("photo");
                Uri baseUrl = this._umbracoContextAccessor.UmbracoContext.HttpContext.Request.Url;

                // nasty
                string imageUrl = baseUrl.Scheme + "://" + baseUrl.Host + image.Url;

                umbracoContent.Image = imageUrl;
            }

            // Serialise it
            string json = JsonConvert.SerializeObject(umbracoContent);

            // Send it
            TopicClient client = TopicClient.CreateFromConnectionString(_serviceBusConnectionString, _topicName);
            client.Send(new BrokeredMessage(json));
        }
    }
}