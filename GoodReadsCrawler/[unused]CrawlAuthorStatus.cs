using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlAuthorStatus
    {
        public static frmMain form;
        public static string baseUri = "";
        public static bool addFriends = false;
        public static bool addGenres = false;
        public static bool addChallenges = false;
        public static bool addActivities = false;

        public static void Run(frmMain parentForm, User user)
        {
            form = parentForm;

            //use in-memory storage

            baseUri = "http://www.goodreads.com/user/show/" + user.userIdString;

            //http://www.goodreads.com/user/show/104320-erin-beck
            //http://www.goodreads.com/author/show/3360351.Ryan_Dilbert


            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new UserProfileDumperStep(user));

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
            System.Threading.Thread.Sleep(new Random().Next(2000, 10000));
        }
    }



    internal class AuthorStatusDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public AuthorStatusDumperStep(User user)
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
                
            }

            CrawlAuthorStatus.form.appendLineToLog(string.Format("User {0} (id:{1}) updated. IsAuthor = {2}; {3}"));
            //0 username
            //1 user id
            //2 IsAuthor true false
            //3 url of profile
        }
    }

}

        #endregion





