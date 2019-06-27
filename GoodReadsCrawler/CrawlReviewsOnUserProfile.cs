using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlReviewsOnUserProfile
    {
        public static frmMain form;
        public static string baseUri = "";

        public static int count = 0; //temp

        public static void Run(frmMain parentForm, User user)
        {
            count = 0;

            form = parentForm;
            //use in-memory storage

            baseUri = "http://www.goodreads.com/review/list/" + user.userIdString + "?format=html&page=1&per_page=100&shelf=read";

            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new UserReviewDumperStep(user));

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsAndPagesToSkip;

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();
        }
    }



    internal class UserReviewDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public UserReviewDumperStep(User user)
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
            string temp2 = CrawlReviewsOnUserProfile.baseUri.Substring(0, 54 + User.userIdString.Length);

            //on page 1, add other pages for user
            if (propertyBag.ResponseUri.OriginalString == CrawlReviewsOnUserProfile.baseUri)
            {
                var node = doc.SelectSingleNode(".//div[@id='reviewPagination']");

                if (node != null)
                {
                    try
                    {
                        var x = node.LastChild.PreviousSibling.PreviousSibling;
                        int maxPage = Int32.Parse(x.InnerText.Trim());

                        string uri;
                        for (int i = 2; i <= maxPage; i++)
                        {
                            uri = "http://www.goodreads.com/review/list/" + User.userIdString + "?format=html&page=" + i + "&per_page=100&shelf=read";
                            crawler.AddStep(new Uri(uri), 0);

                            CrawlReviewsOnUserProfile.form.appendLineToLog(uri);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

            }

            lock (this)
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                foreach (var reviewNode in doc.SelectNodes(".//tr[@class='bookalike review']"))
                {
                    CrawlReviewsOnUserProfile.count++;

                    string reviewIdString = reviewNode.GetAttributeValue("id", "");

                    if (reviewIdString == "")
                    {
                        return;
                    }

                    int reviewId = Int32.Parse(reviewIdString.Split('_').GetValue(1).ToString());

                    Review review = CrawlUtil.getReview(context, reviewId);

                    //create and process the REVIEW if it doesn't already exist
                    if (review == null) 
                    {
                        HtmlNode node;
                        review = new Review();

                        review.id = reviewId;

                        //REVIEW.rating
                        node = reviewNode.SelectSingleNode(".//td[@class='field rating']//img");
                        if (node != null)
                        {
                            string ratingString = node.GetAttributeValue("alt", "0");
                            short rating = short.Parse(ratingString.Substring(0, 1));

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
                    }
                }

                CrawlReviewsOnUserProfile.form.appendLineToLog(User.userIdString + ":: " + CrawlReviewsOnUserProfile.count + " reviews crawled");
            }
        }

    }

        #endregion
}




