using System;
using System.Text.RegularExpressions;
using NCrawler;
using NCrawler.Interfaces;
using HtmlAgilityPack;
using NCrawler.HtmlProcessor;
namespace GoodReadsCrawler
{
    class CrawlUserProfile
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



    internal class UserProfileDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public User User { get; set; }
        //public string UserIdString { get; set; }

        public UserProfileDumperStep(User user)
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
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();
                HtmlNodeCollection nodes;
                HtmlNode node;

                //update user's status changes: private (default 0), author (default 0), or username
                var url = propertyBag.ResponseUri.AbsoluteUri;
                if (url != CrawlUserProfile.baseUri) //user is author or has changed useridstring
                {
                    if (url == "http://www.goodreads.com/") //this means the user's profile no longer exists
                    {
                        CrawlUserProfile.form.appendLineToLog(string.Format("User {0} no longer exists."
                                                                        , User.userIdString
                                                                   ));
                        User.IsPrivate = true;
                    }

                    if (url.Contains("/author/"))
                    {
                        User.IsAuthor = true;
                        User.authorUrl = url;
                    }
                    
                    if (url.Contains("/show/")) //both users and authors can change usernames
                    {
                        //when users change their username it can be extracted from the new url.
                        if (User.IsAuthor != true)
                        {
                            User.userIdString = CrawlUtil.extractUserIdStringFromUrl(url);
                        }
                        else //if an author has changed username, then this cannot be detected from the authorurl (to which we are redirected)
                        {
                            //search for the new username using the user's id. e.g. for links to /user/show/37868189
                            node = doc.SelectSingleNode(".//a[starts-with(@href, '/user/show/" + User.id + "')]");
                            if (node != null)
                            {
                                var IdStringToExtract = node.GetAttributeValue("href", "");
                                User.userIdString = CrawlUtil.extractUserIdStringFromUrl(IdStringToExtract);
                            }
                        }
                    }

                    
                    CrawlUserProfile.form.appendLineToLog(string.Format("User {0} updated with status IsAuthor = {1}| {2}; IsPrivate = {3}"
                                                                        , User.userIdString
                                                                        , User.IsAuthor
                                                                        , User.authorUrl
                                                                        , User.IsPrivate
                                                                    ));
                }

               

                //name
                node = doc.SelectSingleNode("//h1//text()[normalize-space()]");
                if (node != null)
                {
                    User.name = CrawlUtil.extractNameFromString(node.InnerText);
                    //inner text for user: "\n  (Tori-Smexybooks) smexys_sidekick’s Profile\n"
                    //         for author: "Derek White"
                }

                /*
                <div class="leftContainer">
                  <div class="leftAlignedImage" style="overflow: hidden; width: 110px;">
                    <a href="/photo/user/3255548-ah" rel="nofollow" title="AH">
                      <img alt="AH" src="http://d.gr-assets.com/users/1275413869p3/3255548.jpg" />
                    </a>
                    <div class="smallText">
                      <a href="/review/list/3255548?sort=rating&amp;view=reviews">874 ratings</a>
                      <a href="#" onclick="Element.toggle('ratingDistribution3255548');; new Ajax.Updater('ratingDistribution3255548', '/user/rating_distribution/3255548', {asynchronous:true, evalScripts:true, method:'get', onComplete:function(request){return false;}, parameters:'authenticity_token=' + encodeURIComponent('dlcB28CHfXju2vqnShahQlNLoL76d9c6QNZMYZI332g=')}); return false;">(3.82 avg)</a>
                      <div class="floatingBox" style="display:none; width: 400px;" id="ratingDistribution3255548"></div>
                      <br/>
                      <a href="/review/list/3255548?sort=review&amp;view=reviews">1218 reviews</a>
                      <br />
                      <a href="/photo/user/3255548-ah" rel="nofollow">more photos (3)</a>
                      <br/>
                      <br/>
                      #<a href="/user/best_reviewers?country=CA&amp;duration=a">10 best reviewers</a>
                      <br/>
                      #<a href="/user/top_reviewers?country=CA&amp;duration=a">36 top reviewers</a>

                    </div>
                  </div>
                </div>

                196 ratings
                (3.29 avg)
                115 reviews 

                #10 best reviewers
                #36 top reviewers
                */
                //numRatings
                //numReviews
                //avgRating
                //badges
                //EXCEPT badges, this code works for both user and author pages
                nodes = doc.SelectNodes(".//div[@class='leftContainer']//div[@class='smallText']//text()[normalize-space()]");
                if (nodes == null) 
                {
                    //private profiles still have this data but in a different HTML layout. If this 'different layout' is found
                    //then we assume this is a private profile.
                    User.IsPrivate = true;

                    CrawlUserProfile.form.appendLineToLog(string.Format("User {0} profile is deemed private."
                                                                        , User.userIdString
                                                                    ));
                    return;
                }
                if (nodes != null)
                {
                    int badgeNum = 1;

                    //to get numerical values and badges
                    foreach (var n in nodes)
                    {
                        string inner = n.InnerText.Trim(); ;

                        //if text begins with "//" then it's script text and we want to skip.
                        if (inner.StartsWith(@"//"))
                        {
                            continue;
                        }

                        //if text begins with "#" then it's a badge
                        if (inner.StartsWith("#"))// && n.NextSibling != null)
                        {
                            switch (badgeNum)
                            {
                                case 1:
                                    User.badge1 = inner;
                                    break;
                                case 2:
                                    User.badge2 = inner;
                                    break;
                                case 3:
                                    User.badge3 = inner;
                                    break;
                                case 4:
                                    User.badge4 = inner;
                                    break;
                                case 5:
                                    User.badge5 = inner;
                                    break;
                            }
                            badgeNum++;
                        }
                        else
                        {
                            decimal d = CrawlUtil.extractNumberFromString(inner);
                            if (d != -1)
                            {
                                if (inner.Contains("avg")) //author pages use "avg rating" which matches both "rating" and "avg" - so we match "avg" first
                                {
                                    User.avgRating = d;
                                }
                                else if (inner.Contains("rating"))
                                {
                                    User.numRatings = Convert.ToInt32(d);
                                }
                                else if (inner.Contains("review"))
                                {
                                    User.numReviews = Convert.ToInt32(d);
                                }
                            }
                        }
                    }
                }

                //to get the 'badges' for authors
                if(User.IsAuthor == true)
                {
                    nodes = doc.SelectNodes(".//div[@class='leftContainer']//div[@id='topListSection']//text()[normalize-space()]");
                    if(nodes != null)
                    {
                        int badgeNum = 1;
                        foreach (var n in nodes)
                        {
                            string inner = n.InnerText.Trim();

                            if (inner.StartsWith("#"))// && n.NextSibling != null)
                            {
                                switch (badgeNum)
                                {
                                    case 1:
                                        User.badge1 = inner;
                                        break;
                                    case 2:
                                        User.badge2 = inner;
                                        break;
                                    case 3:
                                        User.badge3 = inner;
                                        break;
                                    case 4:
                                        User.badge4 = inner;
                                        break;
                                    case 5:
                                        User.badge5 = inner;
                                        break;
                                }
                                badgeNum++;
                            }
                        }
                    }
                    
                }

                //<a href="/friend/user/104320-erin-beck" rel="nofollow">Erin’s Friends (4)</a>
                //<a rel="nofollow" href="/friend/user/3094317-tori-smexybooks-smexys-sidekick">(Tori-Smexybooks)’s Friends (1,505)</a>
                //numFriends
                //Works for both users and authors
                node = doc.SelectSingleNode(".//a[@href='/friend/user/" + User.userIdString + "']/text()");
                if (node != null)
                {
                    decimal d = CrawlUtil.extractNumberFromString(node.InnerText);

                    if (d != -1)
                        User.numFriends = Convert.ToInt32(d);
                }

                /* each friend's html
                <div class="left">
                  <div class="friendName">
                    <a href="/user/show/355607-t-k-kenyon" rel="acquaintance">T.K. Kenyon</a>
                  </div>
                  819 books
                  <span class="greyText">|</span>
                  2,392 friends
                </div>
                */
                //8 friends' summary details
                if (CrawlUserProfile.addFriends)
                {
                    nodes = doc.SelectNodes(".//div[@class='left']");

                    if (nodes != null)
                    {
                        foreach (var n in nodes) //each friend
                        {
                            Friendship f = new Friendship();
                            f.userId = this.User.id;

                            node = n.SelectSingleNode(".//div[@class='friendName']/a");
                            if (node != null)
                            {
                                string attr = node.GetAttributeValue("href", "");
                                f.friendIdString = CrawlUtil.extractUserIdStringFromUrl(attr);

                                f.rel = node.GetAttributeValue("rel", "");
                            }
                            else
                            {
                                continue;
                            }

                            node = n.SelectSingleNode("./text()[normalize-space()]"); //number of books
                            if (node != null)
                            {
                                int numBooks = Convert.ToInt32(CrawlUtil.extractNumberFromString(node.InnerText));

                                if (numBooks != -1)
                                    f.friendNumBooks = numBooks;

                                node = node.SelectSingleNode("following-sibling::text()"); //number of friends
                                if (node != null)
                                {
                                    int numFriends = Convert.ToInt32(CrawlUtil.extractNumberFromString(node.InnerText));

                                    if (numFriends != -1)
                                        f.friendNumFriends = numFriends;
                                }
                            }

                            try
                            {
                                context.Friendships.AddObject(f);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                context.Friendships.Detach(f);
                            }
                        }
                    }
                }

                //User.numFollowers
                //Users (non-authors) <a class="actionLink" rel="nofollow" href="/user/3094317-tori-smexybooks-smexys-sidekick/followers">319 people are following (Tori-Smexybooks)</a>
                //authors <a href="/author_followings?id=6458332&amp;method=get">Lucas Lyndes’s Followers (8)</a>

                if (User.IsAuthor == true)
                {
                    node = doc.SelectSingleNode(".//a[starts-with(@href, '/author_followings?')]/text()");
                }
                else
                {
                    node = doc.SelectSingleNode(".//a[@href='/user/" + User.userIdString + "/followers']/text()");
                }
                if (node != null)
                {
                    decimal d = CrawlUtil.extractNumberFromString(node.InnerText);

                    if (d != -1)
                        User.numFollowers = Convert.ToInt32(d);
                }


                //User.numUserIsFollowing - see [done]goodreads-numUserIsFollowing logic
                //N/A for authors
                if(User.IsAuthor == true)
                {
                    User.numUserIsFollowing = null;
                }
                else
                {
                    nodes = doc.SelectNodes(".//a[contains(text(),'is Following')]/../../../div[@class='bigBoxBody']//div/a");
                    if (nodes != null)
                    {
                        User.numUserIsFollowing = nodes.Count;
                    }
                }
                

                //see [done]goodreads-quiz logic.html
                //User.quizNumCorrect
                //User.quizNumQuestions
                //User.quizRank
                //User.quizRankOutOf
                //N/A for authors
                if(User.IsAuthor == true)
                {
                    User.quizNumCorrect = null;
                    User.quizNumQuestions  = null;
                    User.quizRank = null;
                    User.quizRankOutOf = null;
                }
                else
                {
                    nodes = doc.SelectNodes(".//div[@id='myQuizStats']//div[@class='infoBoxRowTitle' or @class='infoBoxRowItem']");
                    if (nodes != null)
                    {
                        string s = "";
                        foreach (var n in nodes)
                        {
                            if (n.InnerText.Contains("questions answered"))
                            {
                                s = n.NextSibling.NextSibling.InnerText;
                                User.quizNumQuestions = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                //CrawlUserProfile.form.appendLineToLog("answered: " + CrawlUtil.extractNumberFromString(s) + " (" + s + ")");
                            }

                            else if (n.InnerText.Contains("correct"))
                            {
                                s = n.NextSibling.NextSibling.InnerText;
                                User.quizNumCorrect = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                //CrawlUserProfile.form.appendLineToLog("correct: " + CrawlUtil.extractNumberFromString(s) + " (" + s + ")");
                            }

                            else if (n.InnerText.Contains("ranking"))
                            {
                                s = n.NextSibling.NextSibling.InnerText;
                                User.quizRank = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                //CrawlUserProfile.form.appendLineToLog("ranking: " + CrawlUtil.extractNumberFromString(s) + " (" + s + ")");

                                s = s.Substring(s.IndexOf("out of"));
                                User.quizRankOutOf = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                //CrawlUserProfile.form.appendLineToLog("of: " + CrawlUtil.extractNumberFromString(s));

                            }
                        }
                    }
                }
                

                //User.ReadingChallenges - see [done]goodreads-challenge logic.html
                //CHECK if working for authors
                if (CrawlUserProfile.addChallenges)
                {
                    nodes = doc.SelectNodes(".//div[@class='challengePic']");

                    if (nodes != null)
                    {
                        foreach (var n in nodes)
                        {
                            var challengePic = n.SelectSingleNode(".//img");

                            if (challengePic != null)
                            {
                                string challenge = challengePic.GetAttributeValue("alt", "unknown");

                                if (challenge != "unknown")
                                {
                                    ReadingChallenge rc = CrawlUtil.createOrGetChallenge(context, User.id, challenge);
                                    try
                                    {
                                        var stats = n.NextSibling.NextSibling.SelectSingleNode(".//div[@class='bookMeta progressStats']");

                                        if (stats != null)
                                        {
                                            string s = stats.InnerText;
                                            rc.numBooksRead = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));

                                            s = s.Substring(s.IndexOf(" of "));
                                            rc.numBooksTarget = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                        }
                                        else
                                        {
                                            stats = n.NextSibling.NextSibling.SelectSingleNode(".//a[@class='challengeBooksRead']");

                                            if (stats != null)
                                            {
                                                string s = stats.InnerText;
                                                rc.numBooksRead = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));

                                                s = s.Substring(s.IndexOf(" of "));
                                                rc.numBooksTarget = Convert.ToInt32(CrawlUtil.extractNumberFromString(s));
                                            }
                                        }

                                        rc.lastUpdated = DateTime.Now;

                                        context.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        context.ReadingChallenges.Detach(rc);
                                    }
                                }
                            }
                        }
                    }
                }

                /*
                <a href="/librarian/user/255233-sonic" rel="nofollow">Goodreads librarian</a>
                */
                //User.isLibrarian
                node = doc.SelectSingleNode(".//a[@href='/librarian/user/" + User.userIdString + "']");

                if (node != null && node.InnerText == "Goodreads librarian")
                {
                    User.IsLibrarian = true;
                }
                else
                {
                    User.IsLibrarian = false;
                }

                //User.Genres - see [done]goodreads-genres (fav) logic.html
                //UNCHECKED for round 5 update
                if (CrawlUserProfile.addGenres)
                {
                    nodes = doc.SelectNodes(".//h2[contains(text(),'Favorite Genres')]/../../div[@class='bigBoxBody']//a");

                    if (nodes != null)
                    {
                        foreach (var n in nodes)
                        {
                            Genre g = new Genre();
                            g.name = n.InnerText.Trim();
                            g.userId = User.id;

                            try
                            {
                                context.AddToGenres(g);
                                context.SaveChanges();
                            }
                            catch (Exception)
                            {
                                context.Genres.Detach(g);
                            }
                        }
                    }
                }

                //User.Activities - see [done]goodreads-updates logic.html
                //UNCHECKED for round 5 update
                if (CrawlUserProfile.addActivities)
                {
                    nodes = doc.SelectNodes(".//table[@class='tableListReverse friendUpdates']/tr[@class='update' or @class='no_border']");

                    if (nodes != null)
                    {
                        foreach (var n in nodes)
                        {
                            if ("update" == n.GetAttributeValue("class", "unknown"))
                            {
                                Activity a = new Activity();
                                try
                                {
                                    a.userId = User.id;

                                    a.activityHTML = n.InnerHtml.Length > 8000 ? n.InnerHtml.Substring(0, 7999) : n.InnerHtml;

                                    var ts = n.SelectSingleNode("following-sibling::tr//a[@class='updatedTimestamp']");
                                    a.activityTimestampString = ts.InnerText;
                                    a.retrievedAt = DateTime.Now;

                                    context.AddToActivities(a);
                                    context.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    context.Activities.Detach(a);
                                }
                            }
                        }
                    }
                }

                //separate function/class User.numFavouriteAuthors
                //separate function/class User.Groups
                //separate function/class User.Lists
                //separate function/class User.numShelves
                
            }

            CrawlUserProfile.form.appendLineToLog(User.userIdString + ":: " + "details and ticked updated.");
        }
    }

}

        #endregion





