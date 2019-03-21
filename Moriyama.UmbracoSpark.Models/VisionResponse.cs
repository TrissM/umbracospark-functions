using System.Collections.Generic;
using System.Security.Policy;

namespace Moriyama.UmbracoSpark.Models
{
    public class Detail
    {
        public List<object> celebrities { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public double score { get; set; }
        public Detail detail { get; set; }
    }

    public class Color
    {
        public string dominantColorForeground { get; set; }
        public string dominantColorBackground { get; set; }
        public List<string> dominantColors { get; set; }
        public string accentColor { get; set; }
        public bool isBwImg { get; set; }
        public bool isBWImg { get; set; }
    }

    public class Caption
    {
        public string text { get; set; }
        public double confidence { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public List<Caption> captions { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    public class VisionApiResponse
    {
        public int ContentId { get; set; }
        public List<Category> categories { get; set; }
        public Color color { get; set; }
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }
}
