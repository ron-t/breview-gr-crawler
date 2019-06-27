using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlListAndVotes
    {
        public static frmMain form;
        public static string baseUri = "";

        public static int count = 0;

        public static void Run(frmMain parentForm, User user)
        {
            form = parentForm;
            count = 0;

            //use in-memory storage

            baseUri = "http://www.goodreads.com/list/user_votes/" + user.userIdString;

            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new CrawlListAndVotes_DumperStep(user));

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
            //CrawlListAndVotes.form.appendLineToLog("sleeping between 2 and 10 seconds " + e.CrawlStep.ToString());
            System.Threading.Thread.Sleep(new Random().Next(2000, 10000));
        }
    }



    internal class CrawlListAndVotes_DumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public CrawlListAndVotes_DumperStep(User user)
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

            //on page 1, add other pages for user
            if (propertyBag.ResponseUri.OriginalString == CrawlListAndVotes.baseUri)
            {
/*
 *  
<div>
 <span class="previous_page disabled">&laquo; previous</span>
 <em class="current">1</em>
 <a rel="next" href="/list/user_votes/1045275-natasha?page=2">2</a>
 <a href="/list/user_votes/1045275-natasha?page=3">3</a> 
 * <a href="/list/user_votes/1045275-natasha?page=4">4</a> 
 * <a href="/list/user_votes/1045275-natasha?page=5">5</a> 
 * <a class="next_page" rel="next" href="/list/user_votes/1045275-natasha?page=2">next &raquo;</a>
</div>
 */
                var node = doc.SelectSingleNode(".//a[@class='next_page' and @rel='next']");

                if (node != null)
                {
                    try
                    {
                        var x = node.PreviousSibling.PreviousSibling;
                        int maxPage = Int32.Parse(x.InnerText.Trim());

                        string uri;
                        for (int i = 2; i <= maxPage; i++)
                        {
                            uri = "http://www.goodreads.com/list/user_votes/" + User.userIdString +"?page=" + i;
                            crawler.AddStep(new Uri(uri), 0);

                            CrawlListAndVotes.form.appendLineToLog(uri);
                        }
                    }
                    catch (Exception ex)
                    {
                        CrawlListAndVotes.form.appendLineToLog(ex.Message);
                    }
                }
            }

            lock (this)
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                foreach (var listNode in doc.SelectNodes(".//div[@class='cell']"))
                {
                    List l = null;
                    string title = null;
                    var titleNode = listNode.SelectSingleNode(".//a[@class='listTitle']");
                    if (titleNode != null)
                    {
                        title = titleNode.InnerText.Trim();
                    }

                    if (title != null)
                    {
                        l = CrawlUtil.createOrGetList(context, title);
                    }
                    else
                    {
                        continue;
                    }
/*
    296 books
    &mdash;
    994 voters
*/
                    var statsNode = listNode.SelectSingleNode(".//div[@class='listFullDetails']");
                    if (statsNode != null)
                    {
                        string s = statsNode.InnerText.Replace("\n", "").Trim();
                        l.numBooks = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));

                        s = s.Substring(s.IndexOf("books"));
                        l.numVoters = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                    }

                    User u = CrawlUtil.getUser(context, User.id);
                    u.Lists.Add(l);

                    try
                    {
                        context.SaveChanges();
                        CrawlListAndVotes.count++;
                    }
                    catch (Exception ex)
                    {
                        User.Lists.Remove(l);
                        //this just prints out to check an inner exception which is a dupe PK error
                        //CrawlListAndVotes.form.appendLineToLog(ex.Message);
                    }
                }

                CrawlListAndVotes.form.appendLineToLog(User.userIdString + ":: " + CrawlListAndVotes.count + " lists added");
            }
        }

    }

        #endregion
}




