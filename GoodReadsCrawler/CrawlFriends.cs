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
    class CrawlFriends
    {

        public static frmMain form;
        private static Crawler c = null;

        public static void Run(frmMain parentForm)
        {
            form = parentForm;

            IsolatedStorageModule.Setup(false);

            c = new Crawler(new Uri("http://www.goodreads.com/friend/user/542037-annalisa"),
                new HtmlDocumentProcessor(), // Process html
                new FriendDumperStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 2;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();            
        }

    }



    internal class FriendDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        private static Regex regBook = new Regex(@"/book/show/(\d+).*");
        private static Regex regAuthor = new Regex(@"/author/show/(\d+).*");

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            
            var s = propertyBag["HtmlDoc"].Value;

                
            
        }

        #endregion

    }
}
