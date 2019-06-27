using System;
using NCrawler;
using NCrawler.HtmlProcessor;
using NCrawler.IsolatedStorageServices;
using NCrawler.Interfaces;
using System.Globalization;
using System.IO;
using NCrawler.Utils;
using NCrawler.Extensions;
using NCrawler.Services;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Configuration;

namespace GoodReadsCrawler
{
    class CrawlList
    {
        private static int _fromPage = 0;
        private static int _toPage = 0;

        public static int FromPage { get { return _fromPage; } }
        public static int ToPage { get { return _toPage; } }

        public static frmMain form;
        private static Crawler c = null;

        public static void Run(frmMain parentForm, int fromPage, int toPage)
        {
            form = parentForm;
            _fromPage = fromPage;
            _toPage = toPage;
            IsolatedStorageModule.Setup(false);

            c = new Crawler(new Uri("http://www.goodreads.com/list/show/1.Best_Books_Ever?page=" + FromPage),
                new HtmlDocumentProcessor(), // Process html
                new DumperStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 2;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();            
        }

    }



    internal class DumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        private static Regex regBook = new Regex(@"/book/show/(\d+).*");
        private static Regex regAuthor = new Regex(@"/author/show/(\d+).*");

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            //add pages only if at base uri page
            if (propertyBag.ResponseUri.ToString() == "http://www.goodreads.com/list/show/1.Best_Books_Ever?page=" + CrawlList.FromPage)
            {
                string uri = "";

                for (int i = CrawlList.FromPage + 1; i <= CrawlList.ToPage; i++)
                {
                    uri = "http://www.goodreads.com/list/show/1.Best_Books_Ever?page=" + i;
                    crawler.AddStep(new Uri(uri), 0);
                    CrawlList.form.appendLineToLog("also crawling " + uri);
                }
            }

            //only process list pages
            if (!propertyBag.ResponseUri.OriginalString.StartsWith("http://www.goodreads.com/list/show/1.Best_Books_Ever"))
            {
                return;
            }

            var s = propertyBag["HtmlDoc"].Value;

            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;

            if (htmlDoc != null)
            {
                lock (this)
                {
                    var books = htmlDoc.DocumentNode.SelectNodes("//tr[@itemtype='http://schema.org/Book\']");

                    if (books == null || books.Count == 0)
                    {
                        return;
                    }

                    GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();
                    foreach (var b in books)
                    {
                        string title = "null";
                        string authorName = "null";
                        var titleURLNode = b.SelectSingleNode(".//*[@class='bookTitle']");
                        var authorURLNode = b.SelectSingleNode(".//*[@class='authorName']");
                        string titleUrl = "null";
                        string authorUrl = "null";
                        Match match;
                        string bookId = "-1";
                        string authorId = "-1";
                        Book newBook = null;
                        Author author = null;


                        if (titleURLNode != null && authorURLNode != null)
                        {
                            titleUrl = titleURLNode.GetAttributeValue("href", "null");
                            match = regBook.Match(titleUrl);
                            bookId = match.Groups[1].Value;
                            title = titleURLNode.InnerText.Trim();

                            authorUrl = authorURLNode.GetAttributeValue("href", "null");
                            match = regAuthor.Match(authorUrl);
                            authorId = match.Groups[1].Value;
                            authorName = authorURLNode.InnerText.Trim();

                            author = CrawlUtil.createOrGetAuthor(context, Int32.Parse(authorId), authorName);
                            newBook = CrawlUtil.createOrGetBook(context, Int32.Parse(bookId), title);

                            newBook.Author = author;
                            //author.Book = newBook;
                        }

                        context.SaveChanges();
                    }

                    CrawlList.form.appendLineToLog("added/updated " + books.Count + " books and their authors");
                }
            }
        }

        #endregion

    }
}
