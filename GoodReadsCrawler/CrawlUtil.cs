using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NCrawler.Interfaces;
using NCrawler.Services;

namespace GoodReadsCrawler
{
    public class CrawlUtil
    {

        public static IFilter[] ExtensionsToSkip = new[]
			{
				(RegexFilter)new Regex(@"(\.jpg|\.css|\.js|\.gif|\.jpeg|\.png|\.ico)",
					RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
			};

        public static IFilter[] ExtensionsAndPagesToSkip = new IFilter[]
                {
                    (RegexFilter)new Regex(@"(\.jpg|\.css|\.js|\.gif|\.jpeg|\.png|\.ico)",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase),
                    (RegexFilter)new Regex(@"http://www.goodreads.com/(help|about|blog|api|jobs|author/program|advertisers)",RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                };

        public static GoodReadsCrawlerEntities getNewContext()
        {
            return new GoodReadsCrawlerEntities("metadata=res://*/GoodReadsModel.csdl|res://*/GoodReadsModel.ssdl|res://*/GoodReadsModel.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=xxx;Initial Catalog=GoodReadsCrawler;MultipleActiveResultSets=True\"");
        }
        

        public static int getMaxReviewIFramePageNumber(HtmlDocument htmlDoc)
        {
            var finalPageUrlNode = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@class='gr_pagination']//a[last()-1]");

            int pageNum;

            try
            {
                pageNum = Int32.Parse(finalPageUrlNode.InnerText.Trim());
            }
            catch (Exception e)
            {
                pageNum = -1;
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("getMaxReviewPageNumber() returning -1 as default/error.");
            }

            return pageNum;
        }

        public static int getMaxReviewUserPageNumber(HtmlDocument htmlDoc)
        {
            return -1;
        }

        public static Author createOrGetAuthor(GoodReadsCrawlerEntities context, int id, string name)
        {
            Author author = context.Authors.Where(a => a.id == id).FirstOrDefault();

            if (author == null)
            {
                Author newAuthor = new Author();
                newAuthor.id = id;
                newAuthor.name = name;

                author = newAuthor;
            }

            return author as Author;
        }

        public static Book createOrGetBook(GoodReadsCrawlerEntities context, int id, string title)
        {
            Book book = context.Books.Where(b => b.id == id).FirstOrDefault();

            if (book == null)
            {
                Book newBook = new Book();
                newBook.id = id;
                newBook.title = title;

                book = newBook;
                context.Books.AddObject(book);
            }

            return book as Book;
        }

        public static User createOrGetUser(GoodReadsCrawlerEntities context, int id, string userIdString, string name)
        {
            User user = context.Users.Where(u => u.id == id).FirstOrDefault();

            if (user == null)
            {
                User newUser = new User();
                newUser.id = id;
                newUser.userIdString = userIdString;
                newUser.name = name;

                user = newUser;
                context.Users.AddObject(user);
            }

            return user as User;
        }

        public static Review createOrGetReview(GoodReadsCrawlerEntities context, int id)
        {
            Review review = getReview(context, id);

            if (review == null)
            {
                review = new Review();
                review.id = id;

                context.Reviews.AddObject(review);
            }

            return review;
        }

        public static Group createOrGetGroup(GoodReadsCrawlerEntities context, int id, string title)
        {
            Group group = getGroup(context, id);

            if (group == null)
            {
                group = new Group();
                group.id = id;
                group.title = title;
            }

            return group;
        }

        public static List createOrGetList(GoodReadsCrawlerEntities context, string title)
        {
            List list = getList(context, title);

            if (list == null)
            {
                list = new List();
                list.title = title;

                context.Lists.AddObject(list);
            }

            return list;
        }

        public static ReadingChallenge createOrGetChallenge(GoodReadsCrawlerEntities context, int userid, string challenge)
        {
            ReadingChallenge ch = getChallenge(context, userid, challenge);

            if (ch == null)
            {
                ch = new ReadingChallenge();
                ch.userId = userid;
                ch.challenge = challenge;

                context.ReadingChallenges.AddObject(ch);
            }

            return ch;
        }
        

        private static List getList(GoodReadsCrawlerEntities context, string title)
        {
            return context.Lists.Where(l => l.title == title).FirstOrDefault();
        }

        public static Book getBook(GoodReadsCrawlerEntities context, int id)
        {
            return context.Books.Where(b => b.id == id).FirstOrDefault();
        }
        public static Book getBook(GoodReadsCrawlerEntities context, string isbn)
        {
            return context.Books.Where(b => b.isbn == isbn).FirstOrDefault();
        }

        public static Review getReview(GoodReadsCrawlerEntities context, int id)
        {
            return context.Reviews.Where(r => r.id == id).FirstOrDefault();
        }

        public static User getUser(GoodReadsCrawlerEntities context, int id)
        {
            return context.Users.Where(u => u.id == id).FirstOrDefault();
        }

        public static Author getAuthor(GoodReadsCrawlerEntities context, int id)
        {
            return context.Authors.Where(a => a.id == id).FirstOrDefault();
        }

        public static Group getGroup(GoodReadsCrawlerEntities context, int id)
        {
            return context.Groups.Where(g => g.id == id).FirstOrDefault();
        }

        public static ReadingChallenge getChallenge(GoodReadsCrawlerEntities context, int userid, string challenge)
        {
            return context.ReadingChallenges.Where(r => r.userId == userid && r.challenge == challenge).FirstOrDefault();
        }

        public static string extractUserIdStringFromUrl(string url)
        {
            //<a href="/user/show/52663-mer" class="userReview" itemprop="author">Mer</a>
            //...goodreads.com/author/show/1923475.Celine_Kiernan
            Regex userIdString = new Regex(@"/show/(.+)");
            Match m = userIdString.Match(url);

            return m != null && m.Groups[1] != null ? m.Groups[1].Value : null;
        }

        public static int extractIdNumberFromUrl(string url)
        {
            int id = -1;
            Regex idRegex = new Regex(@"(\d+)");
            Match m = idRegex.Match(url);

            if (m != null && m.Groups.Count > 1)
            {
                Int32.TryParse(m.Groups[1].Value, out id);
            }

            return id;
        }

        public static int extractUserIdFromString(string userIdString)
        {
            //52663-mer
            int id = -1;
            Regex userId = new Regex(@"(\d+)");
            Match m = userId.Match(userIdString);

            if (m != null)
            {
                Int32.TryParse(m.Value, out id);
            }

            return id;
        }

        internal static decimal extractNumberFromString(string s)
        {
            decimal d = -1;

            if (s != null)
            {
                Regex number = new Regex(@"([\,\.\d]+)");
                Match m = number.Match(s);

                if (m != null && m.Value != "")
                {
                    decimal.TryParse(m.Value, out d);
                }
            }

            return d;
        }

        internal static string extractNameFromString(string s)
        {
            if(string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            //note that the apostrophe character is ’ and not a regular ' (single quote)
            return s.Replace(@"\n", "").Replace(@"’", "'").Replace(@"'s profile","").Replace(@"'s Profile", "").Trim();
        }

        public static string formatAuthorName(string name)
        {
            string formattedName = "";
            string[] names = name.Split(',');

            if (names.Length == 2)
            {
                formattedName = names[1].Trim() + " " + names[0]; 
            }

            return formattedName;
        }

        public static short countStarsFromString(string s)
        { //★☆
            short count = 0;
            foreach (char c in s.Trim())
            {
                if (c == '★')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        internal static short getRatingFromClassString(string s)
        {
            //<a class=" staticStars stars_4" title="really liked it">4 of 5 stars</a>

            return short.Parse(s.TrimEnd().Last().ToString());
        }

        
    }
}
