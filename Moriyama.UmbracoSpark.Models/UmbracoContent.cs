using System;

namespace Moriyama.UmbracoSpark.Models
{
    public class UmbracoContent
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Url { get; set; }

        public DateTime PublishDate { get; set; }

        public string ContentType { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }
    }
}