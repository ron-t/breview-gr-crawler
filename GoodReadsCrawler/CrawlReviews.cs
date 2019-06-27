using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlReviews
    {
        public static frmMain form;
        public static string baseUri = "";

        public static void Run(frmMain parentForm, int reviewId)
        {
            form = parentForm;
            //use in-memory storage

            baseUri = "http://www.goodreads.com/review/show/" + reviewId;

            Crawler c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new ReviewDumperStep(baseUri, reviewId));

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

            // Begin crawl
            c.Crawl();
        }
    }



    internal class ReviewDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        private static Regex regReview = new Regex(@"review/show/(\d+).*");

        public string BaseUri { get; set; }
        public int ReviewId { get; set; }
        public bool GetUser { get; set; }

        public ReviewDumperStep(string baseUri, int reviewId/*, bool getUser*/)
        {
            this.BaseUri = baseUri;
            this.ReviewId = reviewId;
            //this.GetUser = getUser;
        }

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }

            //only process the review page
            if (propertyBag.ResponseUri.OriginalString != CrawlReviews.baseUri)
            {
                return;
            }


            lock (this)
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                Review review = CrawlUtil.getReview(context, ReviewId);

                if (review == null)
                {
                    return; //this should never happen
                }

                //userId	int	Checked
                //userIdString	varchar(256)	Checked
                //reviewContent	varchar(MAX)	Checked
                //starRating	smallint	Checked
                //publishDate	date	Checked
                //recommendedFor	varchar(MAX)	Checked
                //numComments	int	Checked

                HtmlNode doc = htmlDoc.DocumentNode;

                int userId = -1;
                string userIdString = null;
                string userName = null;
                string reviewContent = null;
                short starRating = -1;
                DateTime publishDate;
                //string recommendedFor = null;
                int numComments = -1;

                var reviewAuthorNode = doc.SelectSingleNode("//a[@itemprop='author']");
                if (reviewAuthorNode != null)
                {
                    userIdString = reviewAuthorNode.GetAttributeValue("href", null);

                    userIdString = CrawlUtil.extractUserIdStringFromUrl(userIdString); //null if error
                    userId = CrawlUtil.extractUserIdFromString(userIdString); //-1 or 0 if error
                    userName = reviewAuthorNode.InnerText.Trim(); //empty if error

                    if (userIdString != null && userId > 0 && !String.IsNullOrEmpty(userName))
                    {
                        var user = CrawlUtil.createOrGetUser(context, userId, userIdString, userName);
                        review.User = user;
                        review.userIdString = userIdString;
                    }

                    //<a href="/user/show/52663-mer" class="userReview" itemprop="author">Mer</a>
                }

                var reviewContentNode = doc.SelectSingleNode("//div[@itemprop='reviewBody']");
                if (reviewContentNode != null)
                {
                    review.reviewContent = reviewContentNode.InnerText.Trim();
                }

                var starRatingNode = doc.SelectSingleNode("//div[@class='rating']/span[@class='value-title']");
                if (starRatingNode != null)
                {
                    short.TryParse(starRatingNode.GetAttributeValue("title", ""), out starRating);

                    review.starRating = starRating;
                    //<span class="value-title" title="5"></span>
                }

                var publishDateNode = doc.SelectSingleNode("//div[@class='right dtreviewed greyText smallText']/span[@class='value-title']");
                if (publishDateNode != null)
                {
                    DateTime.TryParse(publishDateNode.GetAttributeValue("title", ""), out publishDate);

                    review.publishDate = publishDate;
                    //<span class="value-title" title="2007-04-28"></span>
                }

                var recomendedForNode = doc.SelectSingleNode("//span[text()='Recommended for:']");
                if (recomendedForNode != null)
                {
                    review.recommendedFor = recomendedForNode.ParentNode.LastChild.InnerText.Trim();

                    /*
                     <div>
                        <span class="">Recommended for:</span>
                           enviornmentalists, nurturers, parents and children who want to discuss empathy and reciprocity
                     </div>
                     */
                }

                //var numCommentsNode = doc.SelectSingleNode("");
                //if (numCommentsNode != null)
                //{
                //    /*

                //     *  <h2 class="brownBackground">
                //     *  <div class="extraHeader">
                //     *      <a href="#comment_form" rel="nofollow">Post a comment &raquo;</a>
                //     *  </div>Comments
                //     *  <span class="smallText">
                //            (showing
                //            1-5
                //            of
                //            5)
                //         </span>
                //     * ...
                //     * </h2>
                //     */
                //}

                context.SaveChanges();
                CrawlReviews.form.appendLineToLog("Added review " + review.id + " by user " + review.User.name);

            }


        }

    }

        #endregion
}




