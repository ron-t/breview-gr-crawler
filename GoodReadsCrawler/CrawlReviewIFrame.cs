using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NCrawler;
using NCrawler.HtmlProcessor;
using NCrawler.Interfaces;
using NCrawler.IsolatedStorageServices;
using NCrawler.Services;
using System.Linq;
using System.Collections.Generic;

namespace GoodReadsCrawler
{
    class CrawlReviewIFrame
    {
        public static frmMain form;
        private static Crawler c = null;

        public static string baseUri = "";
        public static Book currentBook = null;
        public static List<int> existingReviewIds = null;

        public static void Run(frmMain parentForm, Book book)
        {
            form = parentForm;
            IsolatedStorageModule.Setup(false);

            currentBook = book;

            existingReviewIds = CrawlUtil.getNewContext().Reviews.Where(r => r.bookId == currentBook.id).Select(r => r.id).ToList();

            baseUri = "http://www.goodreads.com/api/reviews_widget_iframe?did=DEVELOPER_ID&amp;format=html&amp;isbn=" + book.isbn + "&amp;links=660&amp;min_rating=&amp;review_back=fff&amp;stars=000&amp;text=000";

            c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new ReviewIFrameDumperStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1; //** 2012-09-03 changed this from 2 to 1 in hopes that it'll fix the unknown (seemingly) random crashes.
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();
        }


    }



    internal class ReviewIFrameDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        private static Regex regReview = new Regex(@"review/show/(\d+).*");

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }
            int maxPage = CrawlUtil.getMaxReviewIFramePageNumber(htmlDoc);

            //add other review pages if at base (uri) page
            if (propertyBag.ResponseUri.ToString() == CrawlReviewIFrame.baseUri && maxPage != -1)
            {
                int maxPageToCrawl = maxPage;

                string uri = "";
                //if (maxPage > 10)   commenting this out means to crawl all review pages.
                //{
                //    maxPageToCrawl = 10;
                //}

                for (int i = 2; i <= maxPageToCrawl; i++)
                {
                    uri = "http://www.goodreads.com/api/reviews_widget_iframe?did=DEVELOPER_ID&amp;format=html&amp;isbn=" + CrawlReviewIFrame.currentBook.isbn + "&amp;links=660&amp;min_rating=&amp;page=" + i + "&amp;review_back=fff&amp;stars=000&amp;text=000";
                    crawler.AddStep(new Uri(uri), 0);
                }


                CrawlReviewIFrame.form.appendLineToLog("Crawling " + maxPageToCrawl + " pages of reviews for " + CrawlReviewIFrame.currentBook.getShortTitle());
            }

            //only process review iframe pages
            if (!propertyBag.ResponseUri.OriginalString.StartsWith(CrawlReviewIFrame.baseUri.Substring(0, 100)))
            {
                return;
            }

            lock (this)
            {
                string currentPage = "0";
                var currentPageNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='current']");
                if (currentPageNode != null)
                {
                    currentPage = currentPageNode.InnerText.Trim();
                }

                var reviews = htmlDoc.DocumentNode.SelectNodes("//*[@itemtype='http://schema.org/Review']");

                if (reviews == null || reviews.Count == 0)
                {
                    return;
                }

                

                //**do stuff to handle dupes properly
                //                           -_-
                //current method just saves each review one by one and ignores all errors when trying to save.
                //this also means all reviews are attempted to be saved again no matter what :(
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                foreach (var r in reviews)
                {
                    string reviewUrl;
                    int reviewId = -1;
                    Match match;
                    var reviewLinkNode = r.SelectSingleNode(".//div[@class='gr_review_text']/link[@itemprop='url']");
                    DateTime publishDate = DateTime.MinValue;
                    short starRating = 0;

                    Review toAdd = new Review();
                    if (reviewLinkNode != null)
                    {
                        reviewUrl = reviewLinkNode.GetAttributeValue("href", "null");
                        match = regReview.Match(reviewUrl);

                        if (Int32.TryParse(match.Groups[1].Value, out reviewId))
                        {
                            if (CrawlReviewIFrame.existingReviewIds.Contains(reviewId))
                            {
                                continue;
                            }

                            var node = r.SelectSingleNode(".//span[@class='gr_review_date']");
                            if (node != null)
                            {
                                DateTime.TryParse(node.InnerText, out publishDate);
                            }

                            node = r.SelectSingleNode(".//span[@class='gr_rating']");
                            if (node != null)
                            {
                                starRating = CrawlUtil.countStarsFromString(node.InnerText);
                            }

                            toAdd.id = reviewId;
                            toAdd.bookId = CrawlReviewIFrame.currentBook.id;
                            toAdd.publishDate = publishDate;
                            toAdd.starRating = starRating;
                            toAdd.foundOnPage = Int32.Parse(currentPage);
                            toAdd.maxPage = maxPage;

                            context.Reviews.AddObject(toAdd);
                        }

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            context.Reviews.Detach(toAdd);

                            CrawlReviewIFrame.form.appendLineToLog(ex.Message);
                            if (ex.InnerException != null)
                            {
                                CrawlReviewIFrame.form.appendLineToLog("\t" + ex.InnerException.Message);
                            }
                        }
                    }
                }

                CrawlReviewIFrame.form.appendLineToLog("Added " + reviews.Count + " on page " + currentPage + " of " + maxPage + " for " + CrawlReviewIFrame.currentBook.getShortTitle());
            }

        }

        #endregion
    }



}
