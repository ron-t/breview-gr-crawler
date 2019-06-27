using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlFavouriteAuthors
    {
        public static frmMain form;
        public static string baseUri = "";

        public static int maxPage = 1;

        public static int count = 0;

        public static void Run(frmMain parentForm, User user)
        {
            form = parentForm;
            count = 0;
            maxPage = 1;

            //use in-memory storage

            baseUri = string.Format("http://www.goodreads.com/user/{0}/favorite_authors", user.id);

            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new CrawlFavouriteAuthors_DumperStep(user));

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsAndPagesToSkip;

            c.BeforeDownload += new EventHandler<NCrawler.Events.BeforeDownloadEventArgs>(c_BeforeDownload);

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();
        }

        //anonymous event handler to delay before each page visit/crawl
        static void c_BeforeDownload(object sender, NCrawler.Events.BeforeDownloadEventArgs e)
        {
            //CrawlFavouriteAuthors.form.appendLineToLog("sleeping between 2 and 5 seconds " + e.CrawlStep.ToString());
            System.Threading.Thread.Sleep(new Random().Next(2000, 5000));
        }
    }



    internal class CrawlFavouriteAuthors_DumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public CrawlFavouriteAuthors_DumperStep(User user)
        {
            this.User = user;
        }

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }

            HtmlNode doc = htmlDoc.DocumentNode;

            lock (this)
            {
                //there are 30 authors per page.
                //easier method is to get the count from the title
                if (propertyBag.ResponseUri.OriginalString == CrawlFavouriteAuthors.baseUri)
                {
                    var node = doc.SelectSingleNode("//title");

                    if (node != null)
                    {
                        string s = node.InnerText.Trim();
                        User.numFavouriteAuthors = Convert.ToInt32(CrawlUtil.extractNumberFromString(s.Substring(s.IndexOf(" authors"))));
                    }
                    else
                    {
                        User.numFavouriteAuthors = 0;
                    }
                }

                CrawlFavouriteAuthors.form.appendLineToLog(User.userIdString + ":: updated to have " + User.numFavouriteAuthors + " favourite authors.");
            }
        }
    }

        #endregion
}




