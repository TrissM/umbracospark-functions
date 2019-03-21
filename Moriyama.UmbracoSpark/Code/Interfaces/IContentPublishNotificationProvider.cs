using Umbraco.Core.Models;

namespace Moriyama.UmbracoSpark.Code.Interfaces
{
    public interface IContentPublishNotificationProvider
    {
        void NotifyPublish(IContent content);
    }
}