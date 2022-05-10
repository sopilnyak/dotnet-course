using System.Net;
using System.Xml;
using System.Collections.Generic;

namespace Homework2
{
    class FileReader {
         public List<string> ReadUrl(string url) {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            XmlDocument xml = new XmlDocument();
            xml.Load(response.GetResponseStream());
            XmlNodeList xmlList = xml["rss"]["channel"].GetElementsByTagName("item");
            var listOfArticles = new List<string>();
            foreach (XmlNode i in xmlList)
            {
                if (i["link"].InnerText.Length > 0)
                {
                    listOfArticles.Add(i["link"].InnerText);
                }
            }
            return listOfArticles;
        }
    }
}
