using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using AngleSharp.Dom;

namespace ParseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            String address = "https://docs.google.com/forms/d/e/1FAIpQLSdj2X4LWjJQW13kSOU6BUbNy7xiS3fllzYdx8nDfzY_PVYjFg/viewform";
            HttpResponseMessage responce = client.GetAsync(address).Result;
            var parser = new HtmlParser();
            IHtmlDocument doc = parser.ParseDocument(responce.Content.ReadAsStringAsync().Result);
            Console.WriteLine(doc.GetElementsByClassName("freebirdFormviewerViewNumberedItemContainer").Length);
            List<IElement> list = doc.GetElementsByClassName("freebirdFormviewerViewNumberedItemContainer").SelectMany(x=>x.Children).Where(x=>x.HasAttribute("data-params")).ToList();
            foreach(IElement e in list)
            {
                Console.WriteLine(e.GetAttribute("data-params"));
            }
        }
    }
}
