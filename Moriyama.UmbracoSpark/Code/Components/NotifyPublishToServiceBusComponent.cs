using Moriyama.UmbracoSpark.Code.Interfaces;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Moriyama.UmbracoSpark.Code.Components
{
    public class NotifyPublishToServiceBusComponent : IComponent
    {
        private readonly IContentPublishNotificationProvider _contentPublishNotificationProvider;

        public NotifyPublishToServiceBusComponent(IContentPublishNotificationProvider contentPublishNotificationProvider)
        {
            this._contentPublishNotificationProvider = contentPublishNotificationProvider;
        }

        public void Initialize()
        {
            ContentService.Published += ContentServicePublished;
        }

        public void Terminate()
        {
           
        }

        private void ContentServicePublished(IContentService sender, ContentPublishedEventArgs e)
        {
            foreach (IContent content in e.PublishedEntities)
            {
                this._contentPublishNotificationProvider.NotifyPublish(content);
            }
        }
    }
}