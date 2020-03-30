using System;
using System.Collections.Generic;
using System.Text;

namespace NewsBotProject
{
    public class News
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string ImageLink { get; set; }
        public string Header { get; set; }
        public string ShortStory { get; set; }
        public string FullStory { get; set; }
        public string Date { get; set; }
        public string Category { get; set; } = "World";
        public string Fields { get; set; }
        public string Keyword { get; set; }
    }
}
