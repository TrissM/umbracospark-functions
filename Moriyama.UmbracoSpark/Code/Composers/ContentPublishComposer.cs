using Moriyama.UmbracoSpark.Code.Components;
using Moriyama.UmbracoSpark.Code.Interfaces;
using Moriyama.UmbracoSpark.Code.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Moriyama.UmbracoSpark.Code.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentPublishComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {

            composition.Register<IContentPublishNotificationProvider, AzureServiceBusContentPublishNotificationService>(Lifetime.Singleton);
            composition.Components().Append<NotifyPublishToServiceBusComponent>();
        }
    }
}