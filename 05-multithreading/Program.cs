using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2
{
    class Program
    {
        static void Main(string[] args)
        {
            const int N = 3;
            RSSParser parser = new RSSParser("rss_list.txt", "processed_articles.txt", N);

        }
    }
}

