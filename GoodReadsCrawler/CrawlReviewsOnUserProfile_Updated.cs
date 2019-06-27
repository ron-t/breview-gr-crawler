using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlReviewsOnUserProfile_Updated
    {
        public static frmMain form;
        public static string baseUri = "";

        //public static int minPage = 1;
        //public static int maxPage = 14;
        //public static int getHowManyLatestPages = 0;
        public static bool getReviewsSinceLastCrawl = false;

        public static int count = 0;

        public static void Run(frmMain parentForm, User user)
        {
            form = parentForm;
            count = 0;

            //use in-memory storage

            baseUri = "http://www.goodreads.com/review/list/" + user.userIdString + "?order=a&print=true&shelf=read&sort=date_added&view=reviews";
            
            //format as at 2015 11 30 (print view is always sorted by date added ascending with no option to change)
            //this means we have to add EARLIER pages (rather than later) to get new reviews.
            //http://www.goodreads.com/review/list/1-otis-chandler?page=3&print=true&shelf=read

            //old format as at 2013 09 12 (20 reviews per page; defaults to infinite scroll view; sorts differently)
            //http://www.goodreads.com/review/list/2581418-russ?order=a&print=true&shelf=read&sort=date_added&view=reviews
            //http://www.goodreads.com/review/list/2581418-russ?order=a&page=46&print=true&shelf=read&sort=date_added&view=reviews

            //old format (defaulted to print view)
            //http://www.goodreads.com/review/list/883303-michael?order=a&shelf=read&view=reviews
            //http://www.goodreads.com/review/list/883303-michael?order=a&page=2&shelf=read&view=reviews


            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new UserReview_Updated_DumperStep(user));

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
            //CrawlReviewsOnUserProfile_Updated.form.appendLineToLog("sleeping between 2 and 10 seconds " + e.CrawlStep.ToString());
            System.Threading.Thread.Sleep(new Random().Next(2000, 10000));
        }
    }



    internal class UserReview_Updated_DumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public UserReview_Updated_DumperStep(User user)
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

            string temp = propertyBag.ResponseUri.OriginalString;
            string temp2 = CrawlReviewsOnUserProfile_Updated.baseUri.Substring(0, 54 + User.userIdString.Length);

            //on page 1, add other pages for user
            if (propertyBag.ResponseUri.OriginalString == CrawlReviewsOnUserProfile_Updated.baseUri)
            {
                var node = doc.SelectSingleNode(".//div[@id='reviewPagination']");

                if (node != null)
                {
                    HtmlNode maxPageNode;
                    int maxPage = 0;

                    try
                    {
                        maxPageNode = node.LastChild.PreviousSibling.PreviousSibling;
                        maxPage = Int32.Parse(maxPageNode.InnerText.Trim());
                    }
                    catch (Exception)
                    {
                        CrawlReviewsOnUserProfile_Updated.form.appendLineToLog("Error getting maxPage on " + propertyBag.ResponseUri.OriginalString);
                        return;
                    }

                    //get new reviews since last crawl?
                    int pagesToCrawl = 0;
                    if (CrawlReviewsOnUserProfile_Updated.getReviewsSinceLastCrawl)
                    {
                        pagesToCrawl = maxPage - (User.Reviews.Count / 20); //int division results in truncation
                        if (pagesToCrawl < 1) { return; }

                        /**** TEMP to get pages 30 and above (for users with more than 600 reviews (after getting up to 600 only on a previous run))****/
                        //for (int i = 30; i <= maxPage; i++)
                        //{
                        //    String s= "http://www.goodreads.com/review/list/" + User.userIdString + "?order=a&page=" + i + "&print=true&shelf=read&sort=date_added&view=reviews";
                        //    crawler.AddStep(new Uri(s), 0);

                        //    CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(s);
                        //}
                        //return;

                        /*** Old logic pre 2015 11 30 ***
                        int startPage = (User.Reviews.Count / 20)+1;
                        string uri;
                        for (int i = startPage; i <= maxPage; i++)
                        {
                            uri = "http://www.goodreads.com/review/list/" + User.userIdString + "?order=a&page=" + i + "&print=true&shelf=read&sort=date_added&view=reviews";
                            crawler.AddStep(new Uri(uri), 0);

                            CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(uri);
                        }
                        return;
                         * *************/
                    }
                    else //crawl every page
                    {
                        pagesToCrawl = maxPage;
                    }

                    string uri;
                    for (int i = 2; i <= pagesToCrawl; i++)
                    {
                        //http://www.goodreads.com/review/list/1-otis-chandler?page=3&print=true&shelf=read
                        uri = "http://www.goodreads.com/review/list/" + User.userIdString + "?page=" + i + "&print=true&shelf=read";
                        crawler.AddStep(new Uri(uri), 0);

                        CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(uri);
                    }
                    //continue to with crawl on page 1 unless endPage is 0 (i.e. no pages need to be crawled)


                    //2015-11-30: getting X latest pages is now redudant since reviews are now always sorted by date added ascending.
                    //feature removed for 2015 update 5 crawl

                    //get reviews from specified pages; or latest X pages
                    //get user's latest X pages of reviews if user has more than (maxPage * 20) reviews; X determined by getHowManyLatestPages
                    //if (maxPage > CrawlReviewsOnUserProfile_Updated.maxPage)
                    //{
                    //    if (CrawlReviewsOnUserProfile_Updated.getHowManyLatestPages > 0)
                    //    {
                    //        int numLatestPages = Math.Min(CrawlReviewsOnUserProfile_Updated.getHowManyLatestPages, maxPage - CrawlReviewsOnUserProfile_Updated.maxPage);
                    //        for (int i = 0; i < numLatestPages; i++)
                    //        {
                    //            string uriLatest = "http://www.goodreads.com/review/list/" + User.userIdString + "?order=a&page=" + (maxPage - i) + "&print=true&shelf=read&sort=date_added&view=reviews";
                    //            crawler.AddStep(new Uri(uriLatest), 0);

                    //            CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(uriLatest);
                    //        }
                    //    }

                    //    maxPage = CrawlReviewsOnUserProfile_Updated.maxPage;
                    //}

                    //string u;
                    //for (int i = CrawlReviewsOnUserProfile_Updated.minPage; i <= maxPage; i++)
                    //{
                    //    u = "http://www.goodreads.com/review/list/" + User.userIdString + "?order=a&page=" + i + "&print=true&shelf=read&sort=date_added&view=reviews";
                    //    crawler.AddStep(new Uri(u), 0);

                    //    CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(u);
                    //}

                    //if don't want to include page 1 then don't crawl after adding other pages to crawl
                    //if (CrawlReviewsOnUserProfile_Updated.minPage > 1)
                    //{
                    //    return;
                    //}
                }
            }

            lock (this)
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                foreach (var reviewNode in doc.SelectNodes(".//tr[@class='bookalike review']"))
                {
                    string reviewIdString = reviewNode.GetAttributeValue("id", "");

                    if (reviewIdString == "")
                    {
                        return;
                    }

                    int reviewId = Int32.Parse(reviewIdString.Split('_').GetValue(1).ToString());

                    //Review review = CrawlUtil.createOrGetReview(context, reviewId);
                    Review review = CrawlUtil.getReview(context, reviewId);

                    if (review == null) //review is new
                    {
                        review = new Review();
                        review.id = reviewId;

                        context.Reviews.AddObject(review);
                    }
                    else //review already exists
                    {
                        continue;
                    }

                    HtmlNode node;

                    //REVIEW.rating
                    /*<td class="field rating">
                        <label>Reb's rating</label>
                        <div class="value">
                            <a class=" staticStars stars_4" title="really liked it">4 of 5 stars</a>
                        </div>
                    </td>*/
                    node = reviewNode.SelectSingleNode(".//td[@class='field rating']//a");
                    if (node != null)
                    {
                        string ratingClassString = node.GetAttributeValue("class", "0");
                        short rating = CrawlUtil.getRatingFromClassString(ratingClassString);

                        review.starRating = rating;
                    }

                    //REVIEW.publishdate
                    node = reviewNode.SelectSingleNode(".//td[@class='field date_added']//span");
                    if (node != null)
                    {
                        DateTime date;
                        DateTime.TryParse(node.InnerText, out date);

                        review.publishDate = date;
                    }

                    //USER
                    review.userId = User.id;
                    review.userIdString = User.userIdString;

                    //BOOK
                    node = reviewNode.SelectSingleNode(".//td[@class='field title']//a");
                    string bookUrl = node.GetAttributeValue("href", "");

                    int bookId = CrawlUtil.extractIdNumberFromUrl(bookUrl); //if bookUrl is null then bookId gets set to 0

                    Book book = CrawlUtil.getBook(context, bookId);

                    if (book == null)
                    {
                        book = new Book();
                        book.id = bookId;

                        string title = node.GetAttributeValue("title", "");
                        book.title = title;

                        node = reviewNode.SelectSingleNode(".//td[@class='field isbn']//div");
                        if (node != null)
                        {
                            book.isbn = node.InnerText.Trim();
                        }

                        //AUTHOR
                        node = reviewNode.SelectSingleNode(".//td[@class='field author']//a");
                        if (node != null)
                        {
                            string authorUrl = node.GetAttributeValue("href", "");

                            int authorId = CrawlUtil.extractIdNumberFromUrl(authorUrl); //if bookUrl is null then bookId gets set to 0

                            Author author = CrawlUtil.getAuthor(context, authorId);

                            if (author == null)
                            {
                                author = new Author();
                                author.id = authorId;

                                author.name = CrawlUtil.formatAuthorName(node.InnerText.Trim());

                                book.Author = author;
                            }
                        }

                    }

                    review.Book = book;

                    context.SaveChanges();


                    CrawlReviewsOnUserProfile_Updated.count++;
                }

                CrawlReviewsOnUserProfile_Updated.form.appendLineToLog(User.userIdString + ":: " + CrawlReviewsOnUserProfile_Updated.count + " reviews crawled");
            }
        }

    }

        #endregion
}




