using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NCrawler.Interfaces;
using NCrawler.Services;
using System.Linq;
using RestSharp;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace GoodReadsCrawler
{
    public partial class frmMain : Form
    {
        private const int BATCH_BY = 600;

        delegate void SetTextCallback(string text);

        public frmMain()
        {
            InitializeComponent();

            Application.ThreadException += new ThreadExceptionEventHandler(MyCommonExceptionHandlingMethod);
        }

        private static void MyCommonExceptionHandlingMethod(object sender, ThreadExceptionEventArgs t)
        {
            Console.Error.WriteLine("ERROR: " + t.Exception.Message);

            if (t.Exception.InnerException != null)
            {
                Console.Error.WriteLine("INNER EXCEPTION: " + t.Exception.InnerException.Message);
            }
            Console.Error.WriteLine(t.Exception.StackTrace);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtLog.ForeColor = Color.Blue;
            txtLog.BackColor = Color.White;

            lblInstanceWarning.MaximumSize = new Size(150, 0);
            lblInstanceWarning.AutoSize = true;

            nudInstanceNum.Height = 40;
            nudNumOfInstances.Height = 40;

            if (tabCommands.SelectedTab == tabPageBookCommands)
            {
                nudBooksOrUsersToProcess.Value = BATCH_BY;
            }
            tabCommands_SelectedIndexChanged(null, null);

            chkAddFriends_CheckedChanged(null, null);
            chkAddGenres_CheckedChanged(null, null);
            chkAddChallenges_CheckedChanged(null, null);
            chkAddActivities_CheckedChanged(null, null);

        }

        public void appendLineToLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(appendLineToLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtLog.AppendText("[" + System.DateTime.Now.ToString() + "] " + text + Environment.NewLine);
                if (chkFollowScroll.Checked)
                {
                    this.txtLog.ScrollToCaret();
                }


                Console.Out.WriteLine("[" + System.DateTime.Now.ToString() + "] " + text);
            }
        }


        private void btnCrawlList_Click(object sender, EventArgs e)
        {
            this.btnCrawlList.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                int fromPage = 0;
                int toPage = 0;

                try
                {
                    fromPage = Convert.ToInt32(nudCrawlListFrom.Value);
                    toPage = Convert.ToInt32(nudCrawlListTo.Value);

                    if (fromPage > toPage)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    appendLineToLog("Error with 'from' or 'to' page number(s).");
                    return;
                }
                appendLineToLog("Starting list crawl");

                ServicePointManager.MaxServicePoints = 999999;
                ServicePointManager.DefaultConnectionLimit = 999999;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.EnableDnsRoundRobin = true;

                CrawlList.Run(this, fromPage, toPage);
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("Finished list crawl");
                this.btnCrawlList.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }

        private void btnCrawlReviews_Click(object sender, EventArgs e)
        {
            this.btnCrawlReviews.Enabled = false;

            int numBooksToProcess = Convert.ToInt32(nudBooksOrUsersToProcess.Value);

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                try
                {
                    GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                    List<Book> toProcess;
                    if (chkOnlyInList.Checked)  //can refactor this with the one used in crawl reviews
                    {
                        try
                        {
                            var ids = File.ReadLines(txtBookOrUserList.Text).Select(l => Int32.Parse(l));

                            toProcess = context.Books.Where(b => !b.isProcessed && !string.IsNullOrEmpty(b.isbn) && ids.Contains(b.id)).Take(numBooksToProcess).ToList();
                        }
                        catch (Exception ex)
                        {
                            appendLineToLog(ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        toProcess = (from b in context.Books
                                     where !b.isProcessed && !string.IsNullOrEmpty(b.isbn)
                                     select b).Take(numBooksToProcess).ToList();
                    }

                    appendLineToLog("Starting reviews id crawl for " + toProcess.Count + " books");

                    var unprocessedBooks = toProcess; //don't even need this assignment -_-
                    foreach (var b in unprocessedBooks)
                    {

                        if (b.isbn == null || b.isbn == "0")
                        {
                            appendLineToLog("book with id " + b.id + " has no ISBN");
                        }
                        else
                        {
                            CrawlReviewIFrame.Run(this, b);
                        }

                        b.isProcessed = true;

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished review crawl");
                this.btnCrawlReviews.Enabled = true;
            };

            try
            {
                bgw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                appendLineToLog("ERROR: " + ex.Message);
                if (ex.InnerException != null)
                {
                    appendLineToLog("INNER EXCEPTION: " + ex.InnerException);
                }
                appendLineToLog(ex.StackTrace);
                this.btnCrawlReviews.Enabled = true;
            }

        }

        #region old book update method
        private void btnUpdateBookShow_Click(object sender, EventArgs e)
        {
            btnUpdateBookShow.Enabled = false;
            chkOnlyInList.Enabled = false;
            txtBookOrUserList.Enabled = false;

            appendLineToLog("starting book details update by /book/show (1-by-1)");

            Random r = new Random();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                int numBooksToProcess = Convert.ToInt32(nudBooksOrUsersToProcess.Value);

                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                List<Book> toProcess = new List<Book>();
                if (chkOnlyInList.Checked)  //can refactor this with the one used in crawl reviews
                {
                    try
                    {
                        toProcess = GetBooksToProcessFromIdList(context, numBooksToProcess);
                    }
                    catch (Exception ex)
                    {
                        appendLineToLog(ex.Message);
                        return;
                    }
                }
                else //random numBooksToProcess
                {
                    toProcess = GetBooksToProcess(context, numBooksToProcess);
                }

                var unprocessedBooks = toProcess;

                int numProcessed = 0;
                int numRelatedProcessed = 0;
                int totalToProcess = unprocessedBooks.Count;

                foreach (var b in unprocessedBooks)
                {
                    //since related books get updated (in the code below) some books in the unprocessedBooks
                    //list will already be updated. In such cases we should not process/update this book again.
                    if (b.lastUpdated != null)
                    {
                        continue;
                    }

                    //delay each API call to respect the 1 call per second limit
                    System.Threading.Thread.Sleep(new Random().Next(1000, 1500));

                    var client = new RestClient("http://www.goodreads.com");

                    var request = new RestRequest("book/show", Method.GET);
                    request.AddParameter("format", "xml");
                    request.AddParameter("key", "gBp76B4PVZ0NiEeGlwNUA");
                    request.AddParameter("id", b.id.ToString());


                    // execute the request
                    var response = client.Execute<BookShowResponse>(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        appendLineToLog(response.ErrorMessage + ":: book/show API call failed for " + b.getShortTitle());
                        totalToProcess--;
                        continue; //something failed so we skip it.
                        //manual retry is achieved when we try to update ISBNs again
                    }

                    BookShowResponse responseBook = response.Data as BookShowResponse;

                    Book bookFromDb = CrawlUtil.getBook(context, responseBook.id);
                    if (bookFromDb != null)
                    {
                        var now = DateTime.Now;

                        bookFromDb.isbn = responseBook.isbn;
                        bookFromDb.numRatings = responseBook.ratingscount;
                        bookFromDb.avgRating = responseBook.averagerating;
                        bookFromDb.numTextReviews = responseBook.textreviewscount;
                        bookFromDb.ratingDist = responseBook.ratingdist;

                        if (bookFromDb.authorId == null && responseBook.authors != null && responseBook.authors.Count > 0)
                        {
                            Author a = CrawlUtil.createOrGetAuthor(context, responseBook.authors[0].id, responseBook.authors[0].name);
                            bookFromDb.Author = a;
                        }

                        bookFromDb.lastUpdated = now;

                        context.SaveChanges();

                        if (responseBook.similarbooks.Count > 0)
                        {
                            foreach (BookShowResponse similarResponseBook in responseBook.similarbooks)
                            {
                                Book similarBookFromDb = CrawlUtil.getBook(context, similarResponseBook.id);
                                if (similarBookFromDb != null && similarBookFromDb.lastUpdated == null)
                                {
                                    similarBookFromDb.isbn = similarResponseBook.isbn;
                                    similarBookFromDb.numRatings = similarResponseBook.ratingscount;
                                    similarBookFromDb.avgRating = similarResponseBook.averagerating;

                                    if (similarBookFromDb.authorId == null && similarResponseBook.authors != null && similarResponseBook.authors.Count > 0)
                                    {
                                        Author a = CrawlUtil.createOrGetAuthor(context, similarResponseBook.authors[0].id, similarResponseBook.authors[0].name);
                                        similarBookFromDb.Author = a;
                                    }

                                    similarBookFromDb.lastUpdated = now;

                                    numRelatedProcessed++;
                                }
                            }
                            context.SaveChanges();
                        }


                        if (++numProcessed % 10 == 0 || numProcessed == totalToProcess)
                        {
                            appendLineToLog(numProcessed + " of " + totalToProcess + " details updated (and " + numRelatedProcessed + " related books)");
                        }

                        if (numProcessed > 50 && r.Next(1, 200) == 26) //0.5% chance of sleeping
                        {
                            //sleep 1 to 4 mins
                            int t = r.Next(60000, 240000);
                            appendLineToLog("going to sleep for " + (Convert.ToDouble(t) / 60d / 1000d) + " mins (" + t + " ms)");
                            System.Threading.Thread.Sleep(t);
                        }
                    }
                }

            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished book details update by /book/show (1-by-1)");
                btnUpdateBookShow.Enabled = true;
                chkOnlyInList.Enabled = true;
                txtBookOrUserList.Enabled = true;
            };

            bgw.RunWorkerAsync();

        }
        #endregion

        private void btnUpdateBookReviewCount_Click(object sender, EventArgs e)
        {
            btnUpdateBooksReviewCount.Enabled = false;
            chkOnlyInList.Enabled = false;
            txtBookOrUserList.Enabled = false;

            Random r = new Random();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("starting book details update by /book/review_counts (isbn batch)");

                int numBooksToProcess = Convert.ToInt32(nudBooksOrUsersToProcess.Value);

                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                List<Book> toProcess = new List<Book>();
                if (chkOnlyInList.Checked)  //can refactor this with the one used in crawl reviews
                {
                    try
                    {
                        toProcess = GetBooksToProcessByIsbnFromList(context, numBooksToProcess);
                    }
                    catch (Exception ex)
                    {
                        appendLineToLog(ex.Message);
                        return;
                    }
                }
                else //random numBooksToProcess
                {
                    toProcess = GetBooksToProcessByIsbn(context, numBooksToProcess);
                }

                string[] unprocessedIsbnBatch = new string[0];
                int numProcessed = 0;
                int totalToProcess = toProcess.Count;

                //batch IDs in batches - the API doc says the limit is 1000, but server url length is exceeded with so many isbns
                for (int i = 0; i < totalToProcess; i += BATCH_BY)
                {
                    unprocessedIsbnBatch = toProcess.Skip(i).Take(BATCH_BY).Select(b => b.isbn).ToArray<string>();

                    //https://www.goodreads.com/book/review_counts.json?isbns=0441172717,0141439602&key=gBp76B4PVZ0NiEeGlwNUA
                    var client = new RestClient("http://www.goodreads.com");

                    var request = new RestRequest("book/review_counts", Method.GET);
                    request.AddParameter("key", "gBp76B4PVZ0NiEeGlwNUA");
                    request.AddParameter("isbns", string.Join(",", unprocessedIsbnBatch.ToArray<string>()));


                    // execute the request
                    var response = client.Execute<BookReviewCountResponse>(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        appendLineToLog(response.ErrorMessage + ":: book/review_counts API call failed for batch starting with isbn " + unprocessedIsbnBatch[0]);
                        continue; //something failed so we skip it.
                    }

                    BookReviewCountResponse res = response.Data as BookReviewCountResponse;

                    if (res != null && res.books != null)
                    {
                        var now = DateTime.Now;

                        foreach (var bookReviewCount in res.books)
                        {
                            Book bookFromDb = CrawlUtil.getBook(context, bookReviewCount.isbn);

                            if (bookFromDb != null)
                            {
                                bookFromDb.numRatings = bookReviewCount.ratings_count;
                                bookFromDb.avgRating = bookReviewCount.average_rating;
                                bookFromDb.numTextReviews = bookReviewCount.text_reviews_count;

                                bookFromDb.lastUpdated = now;

                                context.SaveChanges();
                                numProcessed++;
                            }
                        }

                        appendLineToLog(numProcessed + " of " + totalToProcess + " Books updated");

                        //delay each API call to respect the 1 call per second limit
                        System.Threading.Thread.Sleep(new Random().Next(1000, 1500));

                    }
                    else //something is wrong with the response
                    {
                        appendLineToLog("API call book/review_counts received no data for batch starting with isbn " + unprocessedIsbnBatch[0]);
                    }
                }

            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished book details update by /book/review_counts (isbn batch)");
                btnUpdateBooksReviewCount.Enabled = true;
                chkOnlyInList.Enabled = true;
                txtBookOrUserList.Enabled = true;
            };

            bgw.RunWorkerAsync();

        }



        #region btnTestAsync

        private void btnTestAsync_Click(object sender, EventArgs e)
        {
            if (!bgwUpdateIsbn.IsBusy)
            {
                bgwUpdateIsbn.RunWorkerAsync();
                btnTestAsync.Enabled = false;
            }
            else
            {
                appendLineToLog("already running");
            }
        }

        private void bgwUpdateIsbn_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1500);
        }

        private void bgwUpdateIsbn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            appendLineToLog("finished async");
            btnTestAsync.Enabled = true;
        }
        #endregion


        private void btnUpdateISBN_MouseHover(object sender, EventArgs e)
        {
            //var num = from b in CrawlUtil.getNewContext().Books
            //          where b.isbn == null
            //          select b;

            //this.lblStatus.Text = "Update ISBN for " + num.Count() + " books";
        }

        private void btnGetReview_Click(object sender, EventArgs e)
        {
            if (nudInstanceNum.Value > nudNumOfInstances.Value) //this can't happen?
            {
                appendLineToLog("Error with instance numbers.");
                return;
            }

            this.btnGetReview.Enabled = false;
            nudInstanceNum.Enabled = false;
            nudNumOfInstances.Enabled = false;

            int numBooksToProcess = Convert.ToInt32(nudBooksOrUsersToProcess.Value);
            int numReviewsToProcess = Convert.ToInt32(nudReviewsToProcess.Value);
            int reviewsProcessed = 0;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting reviews crawl for " + numReviewsToProcess + " reviews");

                var ids = File.ReadLines(txtBookOrUserList.Text).Select(l => Int32.Parse(l));

                var books = context.Books.
                    Where(b => b.isProcessed
                        && !string.IsNullOrEmpty(b.isbn)
                        && ids.Contains(b.id)
                        && b.id % nudNumOfInstances.Value == (nudInstanceNum.Value - 1)
                    ).Take(numBooksToProcess).ToList();

                foreach (var book in books)
                {
                    var unprocessedReviews = (from r in context.Reviews
                                              where r.bookId == book.id
                                                && r.reviewContent == null //in future add && r.starRating == null
                                              select r).ToList();

                    foreach (var r in unprocessedReviews)
                    {
                        CrawlReviews.Run(this, r.id);

                        if (++reviewsProcessed >= numReviewsToProcess)
                        {
                            return;
                        }
                    }
                }


            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished get review");
                this.btnGetReview.Enabled = true;
                nudInstanceNum.Enabled = true;
                nudNumOfInstances.Enabled = true;
            };

            bgw.RunWorkerAsync();



        }

        private void nudInstanceNum_ValueChanged(object sender, EventArgs e)
        {
            if (nudNumOfInstances.Value < nudInstanceNum.Value)
                nudNumOfInstances.Value = nudInstanceNum.Value;
        }

        private void nudNumOfInstances_ValueChanged(object sender, EventArgs e)
        {
            if (nudNumOfInstances.Value < nudInstanceNum.Value)
                nudInstanceNum.Value = nudNumOfInstances.Value;
        }

        private void btnGetUserProfileReviews_Click(object sender, EventArgs e)
        {
            if (nudInstanceNum.Value > nudNumOfInstances.Value) //this can't happen?
            {
                appendLineToLog("Error with instance numbers.");
                return;
            }

            btnGetUserProfileReviews.Enabled = false;
            nudInstanceNum.Enabled = false;
            nudNumOfInstances.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user reviews crawl");

                var unprocessedUsers = (from u in context.Users
                                        where u.numRatings == null
                                          && u.id % nudNumOfInstances.Value == (nudInstanceNum.Value - 1)
                                        select u).ToList();

                DialogResult choice = MessageBox.Show("This only needs to be done once. Proceeding will get additional user reviews from " +
                unprocessedUsers.Count + " users." + Environment.NewLine + "Are you sure you wish you proceed?", "caption", MessageBoxButtons.YesNo);

                if (choice == DialogResult.No)
                {
                    return;
                }

                foreach (var u in unprocessedUsers)
                {
                    CrawlReviewsOnUserProfile.Run(this, u);
                    u.numRatings = CrawlReviewsOnUserProfile.count;
                    context.SaveChanges();
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user reviews crawl");
                btnGetUserProfileReviews.Enabled = true;
                nudInstanceNum.Enabled = true;
                nudNumOfInstances.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }

        private void chkOnlyBooksInList_CheckedChanged(object sender, EventArgs e)
        {

            txtBookOrUserList.Enabled = (chkOnlyInList.Checked);
        }

        private void tabCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCommands.SelectedTab == tabPageBookCommands)
            {
                txtBookOrUserList.Text = "booklist.txt";
                //nudReviewsToProcess.Enabled = true;
                lblToProcess.Text = "Books to process";
                chkOnlyUnprocessed.Text = "Only unproc'd books";

                chkAddFriends.Enabled = false;
                chkAddActivities.Enabled = false;
                chkAddChallenges.Enabled = false;
                chkAddGenres.Enabled = false;

                lblCrawlListPages.Text = "From pages (for review from book)";
            }
            else
            {
                txtBookOrUserList.Text = "userlist.txt";
                //nudReviewsToProcess.Enabled = false;
                lblToProcess.Text = "Users to process";
                chkOnlyUnprocessed.Text = "Only unproc'd users";

                chkAddFriends.Enabled = true;
                chkAddActivities.Enabled = true;
                chkAddChallenges.Enabled = true;
                chkAddGenres.Enabled = true;

                lblCrawlListPages.Text = "From pages (for user reviews)";
            }
        }

        private void btnUpdateUserFromList_Click(object sender, EventArgs e)
        {
            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            btnUpdateUserFromList.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user detail update crawl");

                List<User> toProcess = new List<User>();

                try
                {
                    toProcess = GetUsersToProcessProfileFromIdList(context, Convert.ToInt32(nudBooksOrUsersToProcess.Value));
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                foreach (var u in toProcess)
                {
                    //if (u.IsAuthor == true)
                    //continue; //need to handle these differently as they have different url
                    //http://www.goodreads.com/author/show/3360351.Ryan_Dilbert (stringid is 78100-ryan-dilbert)

                    //add friends, genres, challenges, etc. are handled in the checked events of their respective checkboxes
                    CrawlUserProfile.Run(this, u);
                    u.lastUpdated = DateTime.Now;

                    context.SaveChanges();
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user detail update crawl");
                btnUpdateUserFromList.Enabled = true;
            };

            bgw.RunWorkerAsync();

        }
        private void btnGetUserReviewsFromList_Click(object sender, EventArgs e)
        {
            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            btnGetUserReviewsFromList.Enabled = false;
            //nudCrawlListFrom.Enabled = false;
            //nudCrawlListTo.Enabled = false;


            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user review crawl");

                List<User> toProcess = new List<User>();

                try
                {
                    toProcess = GetUsersToProcessReviewsFromIdList(context, Convert.ToInt32(nudBooksOrUsersToProcess.Value));
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                foreach (var u in toProcess)
                {
                    CrawlReviewsOnUserProfile_Updated.getReviewsSinceLastCrawl = chkGetReviewsSinceLastCrawl.Checked;

                    //feature removed for 2015 update round 5.
                    //CrawlReviewsOnUserProfile_Updated.minPage = (int)nudCrawlListFrom.Value;
                    //CrawlReviewsOnUserProfile_Updated.maxPage = (int)nudCrawlListTo.Value;

                    //if (chkLatestPages.Checked)
                    //{
                    //    CrawlReviewsOnUserProfile_Updated.getHowManyLatestPages = (int)nudLatestReviewPages.Value;
                    //}
                    //else
                    //{
                    //    CrawlReviewsOnUserProfile_Updated.getHowManyLatestPages = 0;
                    //}

                    CrawlReviewsOnUserProfile_Updated.Run(this, u);
                    u.whenReviewsAdded = DateTime.Now;

                    context.SaveChanges();

                    System.Threading.Thread.Sleep(new Random().Next(0, 10000));
                }


            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user review crawl");
                btnGetUserReviewsFromList.Enabled = true;
                //nudCrawlListFrom.Enabled = true;
                //nudCrawlListTo.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }

        private void chkAddFriends_CheckedChanged(object sender, EventArgs e)
        {
            CrawlUserProfile.addFriends = chkAddFriends.Checked;
        }

        private void chkAddGenres_CheckedChanged(object sender, EventArgs e)
        {
            CrawlUserProfile.addGenres = chkAddGenres.Checked;
        }

        private void chkAddChallenges_CheckedChanged(object sender, EventArgs e)
        {
            CrawlUserProfile.addChallenges = chkAddChallenges.Checked;
        }

        private void chkAddActivities_CheckedChanged(object sender, EventArgs e)
        {
            CrawlUserProfile.addActivities = chkAddActivities.Checked;
        }

        private void btnUpdateUserGroupsFromList_Click(object sender, EventArgs e)
        {
            btnUpdateUserGroupsFromList.Enabled = false;
            chkOnlyInList.Enabled = false;
            txtBookOrUserList.Enabled = false;

            Random r = new Random();

            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user groups crawl");

                List<User> toProcess = new List<User>();
                int numUsersToProcess = Convert.ToInt32(nudBooksOrUsersToProcess.Value);

                try
                {
                    toProcess = GetUsersToProcessProfileFromIdList(context, numUsersToProcess);
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                int numProcessed = 0;
                int totalToProcess = toProcess.Count;

                foreach (var u in toProcess)
                {
                    //delay each API call to respect the 1 call per second limit
                    System.Threading.Thread.Sleep(new Random().Next(1000, 1500));

                    var client = new RestClient("http://www.goodreads.com");

                    var request = new RestRequest("group/list/" + u.id + ".xml", Method.GET);
                    //request.AddParameter("format", "xml");
                    request.AddParameter("key", "gBp76B4PVZ0NiEeGlwNUA");
                    //request.AddParameter("id", b.id.ToString());


                    // execute the request
                    var response = client.Execute<UserGroupsResponse>(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        appendLineToLog(response.ErrorMessage + ":: book/show API call failed for " + u.userIdString);
                        totalToProcess--;
                        continue; //something failed so we skip it.
                        //manual retry is achieved when we try to update user groups
                    }

                    UserGroupsResponse responseGroups = response.Data as UserGroupsResponse;

                    if (responseGroups != null && responseGroups.list.Count > 0)
                    {
                        foreach (var g in responseGroups.list)
                        {
                            Group groupFromDb = CrawlUtil.createOrGetGroup(context, g.id, g.title);

                            try
                            {
                                //if (groupFromDb.lastActivityAtString == null)
                                //{
                                var clientForGroup = new RestClient("http://www.goodreads.com");

                                var requestForGroup = new RestRequest("group/show/" + groupFromDb.id, Method.GET);
                                request.AddParameter("format", "xml");
                                request.AddParameter("key", "gBp76B4PVZ0NiEeGlwNUA");

                                var responseForGroup = clientForGroup.Execute<GroupResponse>(requestForGroup);

                                if (responseForGroup.ResponseStatus != ResponseStatus.Completed)
                                {
                                    appendLineToLog(responseForGroup.ErrorMessage + ":: group/show API call failed for " + u.userIdString);
                                    continue; //something failed so we skip it.
                                    //manual retry is achieved when we try to update user groups
                                }

                                GroupResponse responseGroup = responseForGroup.Data as GroupResponse;

                                groupFromDb.userCount = responseGroup.groupUsersCount;
                                groupFromDb.description = responseGroup.description;
                                groupFromDb.category = responseGroup.category;
                                groupFromDb.subcategory = responseGroup.subcategory;
                                groupFromDb.lastActivityAtString = responseGroup.lastActivityAt;

                                context.SaveChanges();
                                //}

                                u.Groups.Add(groupFromDb);

                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                u.Groups.Remove(groupFromDb);
                                //context.SaveChanges();

                                appendLineToLog("ERROR: " + ex.Message);
                                if (ex.InnerException != null)
                                {
                                    appendLineToLog("\t" + ex.InnerException.Message);
                                }
                            }
                        }
                    }

                    if (++numProcessed % 10 == 0 || numProcessed == totalToProcess)
                    {
                        appendLineToLog(numProcessed + " of " + totalToProcess + " users' groups updated");
                    }

                    if (numProcessed > 50 && r.Next(1, 100) == 26) //1% chance of sleeping
                    {
                        int t = r.Next(60000, 300000);
                        appendLineToLog("going to sleep for " + (Convert.ToDouble(t) / 60d / 1000d) + " mins (" + t + " ms)");
                        System.Threading.Thread.Sleep(t);
                    }
                }
            };


            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user groups update");
                btnUpdateUserGroupsFromList.Enabled = true;
                chkOnlyInList.Enabled = true;
                txtBookOrUserList.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }



        private List<User> GetUsersToProcessProfileFromIdList(GoodReadsCrawlerEntities context, int numUsersToProcess)
        {
            List<User> toProcess = new List<User>();

            var ids = ReadIdsFromFile();

            toProcess = context.Users.Where(u => (chkOnlyUnprocessed.Checked ? u.lastUpdated == null : true)
                                                && ids.Contains(u.id))
                                        .Take(numUsersToProcess)
                                        .ToList();
            return toProcess;
        }

        private List<User> GetUsersToProcessReviewsFromIdList(GoodReadsCrawlerEntities context, int numUsersToProcess)
        {
            List<User> toProcess = new List<User>();

            var ids = ReadIdsFromFile();

            toProcess = context.Users.Where(u => (chkOnlyUnprocessed.Checked ? u.whenReviewsAdded == null : true)
                                                && ids.Contains(u.id))
                                        .Take(numUsersToProcess)
                                        .ToList();
            return toProcess;
        }

        private List<Book> GetBooksToProcessByIsbnFromList(GoodReadsCrawlerEntities context, int numBooksToProcess)
        {
            var Isbns = File.ReadLines(txtBookOrUserList.Text);

            return (from isbn in Isbns
                    join b in context.Books on isbn equals b.isbn
                    where (chkOnlyUnprocessed.Checked ? b.lastUpdated == (DateTime?)null : true) //need this to get null comparison to work properly
                    select b
                ).Take(numBooksToProcess)
                .ToList();
        }

        private List<Book> GetBooksToProcessByIsbn(GoodReadsCrawlerEntities context, int numBooksToProcess)
        {
            return (from b in context.Books 
                    where (chkOnlyUnprocessed.Checked ? b.lastUpdated == (DateTime?)null : true) //need this to get null comparison to work properly
                    select b
                ).Take(numBooksToProcess)
                .ToList();
        }

        private List<Book> GetBooksToProcessFromIdList(GoodReadsCrawlerEntities context, int numBooksToProcess)
        {
            return GetBooksToProcessFromDb(context, numBooksToProcess, true);
        }

        private List<Book> GetBooksToProcess(GoodReadsCrawlerEntities context, int numBooksToProcess)
        {
            return GetBooksToProcessFromDb(context, numBooksToProcess, false);
        }

        private List<Book> GetBooksToProcessFromDb(GoodReadsCrawlerEntities context, int numBooksToProcess, bool fromList)
        {
            List<Book> result = new List<Book>(numBooksToProcess);

            if (fromList)
            {
                List<int> ids = ReadIdsFromFile();

                result = (from id in ids
                          join b in context.Books on id equals b.id
                          where (chkOnlyUnprocessed.Checked ? b.lastUpdated == (DateTime?)null : true) //need this to get null comparison to work properly
                          select b
                    ).Take(numBooksToProcess)
                    .ToList();
            }
            else
            {
                result = (from b in context.Books
                          where (chkOnlyUnprocessed.Checked ? b.lastUpdated == (DateTime?)null : true) //need this to get null comparison to work properly
                          select b
                    ).Take(numBooksToProcess)
                    .ToList();
            }

            return result;

            //original version
            //List<Book> toProcess = new List<Book>();
            //List<int> ids = new List<int>();

            //if (fromList)
            //{
            //    ids = ReadIdsFromFile();
            //}

            //toProcess = context.Books.Where(b => (chkOnlyUnprocessed.Checked ? b.lastUpdated == null : true)
            //                                       && fromList ? ids.Contains(b.id) : true)
            //                            .Take(numBooksToProcess)
            //                            .ToList();
            //return toProcess;
        }

        private List<int> ReadIdsFromFile()
        {
            var ids = File.ReadLines(txtBookOrUserList.Text).Select(delegate(string s)
            {
                int i = 0;
                int.TryParse(s, out i);
                return i;
            }).ToList<int>();

            return ids;
        }


        private void btnUpdateUserShelvesFromList_Click(object sender, EventArgs e)
        {
            btnUpdateUserShelvesFromList.Enabled = false;
            chkOnlyInList.Enabled = false;
            txtBookOrUserList.Enabled = false;

            Random r = new Random();

            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user shelves crawl");

                List<User> toProcess = new List<User>();

                try
                {
                    toProcess = GetUsersToProcessProfileFromIdList(context, Convert.ToInt32(nudBooksOrUsersToProcess.Value));
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                int numProcessed = 0;
                int totalToProcess = toProcess.Count;

                foreach (var u in toProcess)
                {
                    //delay each API call to respect the 1 call per second limit
                    System.Threading.Thread.Sleep(new Random().Next(1000, 1500));

                    var client = new RestClient("http://www.goodreads.com");

                    var request = new RestRequest("user/show/" + u.id + ".xml", Method.GET);
                    request.AddParameter("key", "gBp76B4PVZ0NiEeGlwNUA");

                    // execute the request
                    var response = client.Execute<UserResponse>(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        appendLineToLog(response.ErrorMessage + ":: user/show API call failed for " + u.userIdString);
                        totalToProcess--;
                        continue; //something failed so we skip it.
                        //manual retry is achieved when we try to update user groups
                    }

                    UserResponse responseUser = response.Data as UserResponse;

                    if (responseUser != null && responseUser.userShelves != null)
                    {
                        u.numShelves = responseUser.userShelves.Count;
                        u.lastUpdated = DateTime.Now;
                        context.SaveChanges();
                    }

                    if (++numProcessed % 10 == 0 || numProcessed == totalToProcess)
                    {
                        appendLineToLog(numProcessed + " of " + totalToProcess + " users' shelves updated");
                    }

                    if (numProcessed > 50 && r.Next(1, 100) == 86) //1% chance of sleeping
                    {
                        int t = r.Next(60000, 300000);
                        appendLineToLog("going to sleep for " + (Convert.ToDouble(t) / 60d / 1000d) + " mins (" + t + " ms)");
                        System.Threading.Thread.Sleep(t);
                    }
                }
            };


            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user shelves update");
                btnUpdateUserShelvesFromList.Enabled = true;
                chkOnlyInList.Enabled = true;
                txtBookOrUserList.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }


        private void btnUpdateUsersListVotesFromList_Click(object sender, EventArgs e)
        {
            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            btnUpdateUserListVotesFromList.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user lists update crawl");

                List<User> toProcess = new List<User>();

                try
                {
                    toProcess = toProcess = GetUsersToProcessProfileFromIdList(context, Convert.ToInt32(nudBooksOrUsersToProcess.Value));
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                foreach (var u in toProcess)
                {
                    CrawlListAndVotes.Run(this, u);
                    u.lastUpdated = DateTime.Now;

                    context.SaveChanges();

                    System.Threading.Thread.Sleep(new Random().Next(0, 20000));
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user lists update crawl");
                btnUpdateUserListVotesFromList.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }

        private void btnUpdateUserFavAuthorsFromList_Click(object sender, EventArgs e)
        {
            if (!chkOnlyInList.Checked)
            {
                MessageBox.Show("A user list must be specified.");
                return;
            }

            btnUpdateUserFavAuthorsFromList.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                GoodReadsCrawlerEntities context = CrawlUtil.getNewContext();

                appendLineToLog("Starting user favourite authors update crawl");

                List<User> toProcess = new List<User>();

                try
                {
                    toProcess = GetUsersToProcessProfileFromIdList(context, Convert.ToInt32(nudBooksOrUsersToProcess.Value));
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    return;
                }

                foreach (var u in toProcess)
                {
                    CrawlFavouriteAuthors.Run(this, u);
                    u.lastUpdated = DateTime.Now;

                    context.SaveChanges();

                    System.Threading.Thread.Sleep(new Random().Next(0, 20000));
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("finished user favourite authors update crawl");
                btnUpdateUserFavAuthorsFromList.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }


        internal class BookReviewCountResponse
        {
            public List<BookReviewCount> books { get; set; }
        }

        internal class BookReviewCount
        {
            public int id { get; set; }
            public string isbn { get; set; }
            public int ratings_count { get; set; }
            public int text_reviews_count { get; set; }
            public decimal average_rating { get; set; }
            /* the 'work' attributes include stats for all editions, not just for this isbn
            "id":53732,
            "isbn":"0441172717",
            "isbn13":"9780441172719",
            "ratings_count":7664,
            "reviews_count":12512,
            "text_reviews_count":684,
            "work_ratings_count":426967,
            "work_reviews_count":616430,
            "work_text_reviews_count":10127,
            "average_rating":"4.15"
             */
        }

        internal class BookShowResponse
        {
            public int id { get; set; }
            public string title { get; set; }
            public string isbn { get; set; }
            //public string isbn13 { get; set; }
            //public string imageurl { get; set; }
            //public string publicationyear { get; set; }
            public int ratingscount { get; set; }
            public int textreviewscount { get; set; }
            public decimal averagerating { get; set; }
            public string ratingdist { get; set; }
            public List<BookShowResponse> similarbooks { get; set; }
            public List<AuthorResponse> authors { get; set; }
        }

        internal class WorkElement //need to use something like this to get the rating counts for ALL editions of the book (the value usually displayed on the website)
        {

        }

        internal class AuthorResponse
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        internal class UserGroupsResponse
        {
            public int id { get; set; }
            public string userName { get; set; }
            public List<GroupResponse> list { get; set; } //must be named list which is the name for the list of groups in the xml response

            //alias for 'list'
            public List<GroupResponse> groups
            {
                get { return list; }
            }
        }

        internal class GroupResponse
        {
            public int id { get; set; }
            public int groupUsersCount { get; set; }
            public string title { get; set; }
            public string lastActivityAt { get; set; }
            public string description { get; set; }
            public string category { get; set; }
            public string subcategory { get; set; }
            /*
             * <id>220</id> 
                  <access>public</access> 
                  <users_count>29422</users_count> 
                - <title>
                - <![CDATA[ Goodreads Librarians Group
                  ]]> 
                  </title>
                - <image_url>
                - <![CDATA[ https://d202m5krfqbpi5.cloudfront.net/groups/1269147049p2/220.jpg
                  ]]> 
                  </image_url>
                - <last_activity_at>
                - <![CDATA[ Thu Jun 13 22:10:30 -0700 2013
                  ]]> 
                  </last_activity_at>
*/
        }

        internal class UserResponse
        {
            public int id { get; set; }
            public List<ShelfResponse> userShelves { get; set; }
        }

        internal class ShelfResponse
        {
            public string name { get; set; }
            /*
            <book_count type="integer">0</book_count>
            <description nil="true"/>
            <display_fields/>
            <exclusive_flag type="boolean">true</exclusive_flag>
            <featured type="boolean">true</featured>
            <id type="integer">36869962</id>
            <name>read</name>
            <order nil="true"/>
            <per_page type="integer" nil="true"/>
            <recommend_for type="boolean">false</recommend_for>
            <sort nil="true"/>
            <sticky type="boolean" nil="true"/>
            */
        }

        private void chkLatestPage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLatestPages.Checked)
            {
                nudLatestReviewPages.Enabled = true;
                chkGetReviewsSinceLastCrawl.Checked = false;
            }
            else
            {
                nudLatestReviewPages.Enabled = false;
            }
        }

        private void chkGetReviewsSinceLastCrawl_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetReviewsSinceLastCrawl.Checked)
            {
                chkLatestPages.Checked = false;
            }
        }


    }
}
