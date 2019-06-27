namespace GoodReadsCrawler
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCrawlList = new System.Windows.Forms.Button();
            this.btnCrawlReviews = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnUpdateBookShow = new System.Windows.Forms.Button();
            this.bgwUpdateIsbn = new System.ComponentModel.BackgroundWorker();
            this.btnTestAsync = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.nudBooksOrUsersToProcess = new System.Windows.Forms.NumericUpDown();
            this.lblToProcess = new System.Windows.Forms.Label();
            this.btnGetReview = new System.Windows.Forms.Button();
            this.ttipBtnUpdateISBN = new System.Windows.Forms.ToolTip(this.components);
            this.lblCrawlListPages = new System.Windows.Forms.Label();
            this.nudCrawlListFrom = new System.Windows.Forms.NumericUpDown();
            this.nudCrawlListTo = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudReviewsToProcess = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudInstanceNum = new System.Windows.Forms.NumericUpDown();
            this.nudNumOfInstances = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGetUserProfileReviews = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOnlyInList = new System.Windows.Forms.CheckBox();
            this.txtBookOrUserList = new System.Windows.Forms.TextBox();
            this.chkFollowScroll = new System.Windows.Forms.CheckBox();
            this.lblInstanceWarning = new System.Windows.Forms.Label();
            this.tabCommands = new System.Windows.Forms.TabControl();
            this.tabPageUserCommands = new System.Windows.Forms.TabPage();
            this.btnUpdateUserFavAuthorsFromList = new System.Windows.Forms.Button();
            this.btnUpdateUserListVotesFromList = new System.Windows.Forms.Button();
            this.btnUpdateUserShelvesFromList = new System.Windows.Forms.Button();
            this.btnUpdateUserGroupsFromList = new System.Windows.Forms.Button();
            this.btnUpdateUserFromList = new System.Windows.Forms.Button();
            this.btnGetUserReviewsFromList = new System.Windows.Forms.Button();
            this.tabPageBookCommands = new System.Windows.Forms.TabPage();
            this.btnUpdateBooksReviewCount = new System.Windows.Forms.Button();
            this.chkOnlyUnprocessed = new System.Windows.Forms.CheckBox();
            this.chkLatestPages = new System.Windows.Forms.CheckBox();
            this.chkAddFriends = new System.Windows.Forms.CheckBox();
            this.chkAddGenres = new System.Windows.Forms.CheckBox();
            this.chkAddChallenges = new System.Windows.Forms.CheckBox();
            this.chkAddActivities = new System.Windows.Forms.CheckBox();
            this.nudLatestReviewPages = new System.Windows.Forms.NumericUpDown();
            this.chkGetReviewsSinceLastCrawl = new System.Windows.Forms.CheckBox();
            this.statusStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBooksOrUsersToProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrawlListFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrawlListTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReviewsToProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstanceNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfInstances)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabCommands.SuspendLayout();
            this.tabPageUserCommands.SuspendLayout();
            this.tabPageBookCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLatestReviewPages)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCrawlList
            // 
            this.btnCrawlList.Enabled = false;
            this.btnCrawlList.Location = new System.Drawing.Point(6, 3);
            this.btnCrawlList.Name = "btnCrawlList";
            this.btnCrawlList.Size = new System.Drawing.Size(110, 36);
            this.btnCrawlList.TabIndex = 1;
            this.btnCrawlList.Text = "Crawl List (get reviewIDs)";
            this.btnCrawlList.UseVisualStyleBackColor = true;
            this.btnCrawlList.Click += new System.EventHandler(this.btnCrawlList_Click);
            // 
            // btnCrawlReviews
            // 
            this.btnCrawlReviews.Enabled = false;
            this.btnCrawlReviews.Location = new System.Drawing.Point(6, 153);
            this.btnCrawlReviews.Name = "btnCrawlReviews";
            this.btnCrawlReviews.Size = new System.Drawing.Size(110, 36);
            this.btnCrawlReviews.TabIndex = 1;
            this.btnCrawlReviews.Text = "Crawl review IDs (from IFrame)";
            this.btnCrawlReviews.UseVisualStyleBackColor = true;
            this.btnCrawlReviews.Click += new System.EventHandler(this.btnCrawlReviews_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 400);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(824, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(14, 17);
            this.lblStatus.Text = ":)";
            // 
            // btnUpdateBookShow
            // 
            this.btnUpdateBookShow.Location = new System.Drawing.Point(6, 39);
            this.btnUpdateBookShow.Name = "btnUpdateBookShow";
            this.btnUpdateBookShow.Size = new System.Drawing.Size(110, 54);
            this.btnUpdateBookShow.TabIndex = 3;
            this.btnUpdateBookShow.Text = "Update details of books (1-by-1 /book/show/)";
            this.btnUpdateBookShow.UseVisualStyleBackColor = true;
            this.btnUpdateBookShow.Click += new System.EventHandler(this.btnUpdateBookShow_Click);
            this.btnUpdateBookShow.MouseHover += new System.EventHandler(this.btnUpdateISBN_MouseHover);
            // 
            // bgwUpdateIsbn
            // 
            this.bgwUpdateIsbn.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdateIsbn_DoWork);
            this.bgwUpdateIsbn.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdateIsbn_RunWorkerCompleted);
            // 
            // btnTestAsync
            // 
            this.btnTestAsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTestAsync.Location = new System.Drawing.Point(4, 367);
            this.btnTestAsync.Name = "btnTestAsync";
            this.btnTestAsync.Size = new System.Drawing.Size(110, 23);
            this.btnTestAsync.TabIndex = 4;
            this.btnTestAsync.Text = "Test async call";
            this.btnTestAsync.UseVisualStyleBackColor = true;
            this.btnTestAsync.Click += new System.EventHandler(this.btnTestAsync_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(332, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 385);
            this.tabControl1.TabIndex = 5;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(472, 359);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.CausesValidation = false;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(466, 353);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(472, 359);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "TODO-visualiser";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // nudBooksOrUsersToProcess
            // 
            this.nudBooksOrUsersToProcess.Location = new System.Drawing.Point(144, 224);
            this.nudBooksOrUsersToProcess.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBooksOrUsersToProcess.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBooksOrUsersToProcess.Name = "nudBooksOrUsersToProcess";
            this.nudBooksOrUsersToProcess.Size = new System.Drawing.Size(123, 20);
            this.nudBooksOrUsersToProcess.TabIndex = 6;
            this.nudBooksOrUsersToProcess.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblToProcess
            // 
            this.lblToProcess.AutoSize = true;
            this.lblToProcess.Location = new System.Drawing.Point(141, 208);
            this.lblToProcess.Name = "lblToProcess";
            this.lblToProcess.Size = new System.Drawing.Size(89, 13);
            this.lblToProcess.TabIndex = 7;
            this.lblToProcess.Text = "Books to process";
            // 
            // btnGetReview
            // 
            this.btnGetReview.Enabled = false;
            this.btnGetReview.Location = new System.Drawing.Point(6, 189);
            this.btnGetReview.Name = "btnGetReview";
            this.btnGetReview.Size = new System.Drawing.Size(110, 66);
            this.btnGetReview.TabIndex = 8;
            this.btnGetReview.Text = "Update review details from review ID crawl";
            this.btnGetReview.UseVisualStyleBackColor = true;
            this.btnGetReview.Click += new System.EventHandler(this.btnGetReview_Click);
            // 
            // lblCrawlListPages
            // 
            this.lblCrawlListPages.AutoSize = true;
            this.lblCrawlListPages.Location = new System.Drawing.Point(142, 14);
            this.lblCrawlListPages.Name = "lblCrawlListPages";
            this.lblCrawlListPages.Size = new System.Drawing.Size(167, 13);
            this.lblCrawlListPages.TabIndex = 9;
            this.lblCrawlListPages.Text = "From pages (for review from book)";
            // 
            // nudCrawlListFrom
            // 
            this.nudCrawlListFrom.Enabled = false;
            this.nudCrawlListFrom.Location = new System.Drawing.Point(145, 30);
            this.nudCrawlListFrom.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudCrawlListFrom.Name = "nudCrawlListFrom";
            this.nudCrawlListFrom.Size = new System.Drawing.Size(50, 20);
            this.nudCrawlListFrom.TabIndex = 10;
            this.nudCrawlListFrom.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // nudCrawlListTo
            // 
            this.nudCrawlListTo.Enabled = false;
            this.nudCrawlListTo.Location = new System.Drawing.Point(219, 30);
            this.nudCrawlListTo.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudCrawlListTo.Name = "nudCrawlListTo";
            this.nudCrawlListTo.Size = new System.Drawing.Size(50, 20);
            this.nudCrawlListTo.TabIndex = 2;
            this.nudCrawlListTo.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "to";
            // 
            // nudReviewsToProcess
            // 
            this.nudReviewsToProcess.Enabled = false;
            this.nudReviewsToProcess.Location = new System.Drawing.Point(146, 265);
            this.nudReviewsToProcess.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.nudReviewsToProcess.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudReviewsToProcess.Name = "nudReviewsToProcess";
            this.nudReviewsToProcess.Size = new System.Drawing.Size(123, 20);
            this.nudReviewsToProcess.TabIndex = 13;
            this.nudReviewsToProcess.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 249);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Reviews to process (&&books)";
            // 
            // nudInstanceNum
            // 
            this.nudInstanceNum.Location = new System.Drawing.Point(2, 22);
            this.nudInstanceNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudInstanceNum.Name = "nudInstanceNum";
            this.nudInstanceNum.Size = new System.Drawing.Size(50, 20);
            this.nudInstanceNum.TabIndex = 15;
            this.nudInstanceNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudInstanceNum.ValueChanged += new System.EventHandler(this.nudInstanceNum_ValueChanged);
            // 
            // nudNumOfInstances
            // 
            this.nudNumOfInstances.Location = new System.Drawing.Point(71, 22);
            this.nudNumOfInstances.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumOfInstances.Name = "nudNumOfInstances";
            this.nudNumOfInstances.Size = new System.Drawing.Size(50, 20);
            this.nudNumOfInstances.TabIndex = 16;
            this.nudNumOfInstances.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumOfInstances.ValueChanged += new System.EventHandler(this.nudNumOfInstances_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "of";
            // 
            // btnGetUserProfileReviews
            // 
            this.btnGetUserProfileReviews.Enabled = false;
            this.btnGetUserProfileReviews.Location = new System.Drawing.Point(6, 255);
            this.btnGetUserProfileReviews.Name = "btnGetUserProfileReviews";
            this.btnGetUserProfileReviews.Size = new System.Drawing.Size(110, 61);
            this.btnGetUserProfileReviews.TabIndex = 19;
            this.btnGetUserProfileReviews.Text = "Get user\'s reviews from review content crawl";
            this.btnGetUserProfileReviews.UseVisualStyleBackColor = true;
            this.btnGetUserProfileReviews.Click += new System.EventHandler(this.btnGetUserProfileReviews_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudNumOfInstances);
            this.groupBox1.Controls.Add(this.nudInstanceNum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(144, 296);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(123, 65);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instance numbers";
            // 
            // chkOnlyInList
            // 
            this.chkOnlyInList.AutoSize = true;
            this.chkOnlyInList.Checked = true;
            this.chkOnlyInList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlyInList.Location = new System.Drawing.Point(145, 95);
            this.chkOnlyInList.Name = "chkOnlyInList";
            this.chkOnlyInList.Size = new System.Drawing.Size(64, 17);
            this.chkOnlyInList.TabIndex = 21;
            this.chkOnlyInList.Text = "From list";
            this.chkOnlyInList.UseVisualStyleBackColor = true;
            this.chkOnlyInList.CheckedChanged += new System.EventHandler(this.chkOnlyBooksInList_CheckedChanged);
            // 
            // txtBookOrUserList
            // 
            this.txtBookOrUserList.Location = new System.Drawing.Point(144, 114);
            this.txtBookOrUserList.Name = "txtBookOrUserList";
            this.txtBookOrUserList.Size = new System.Drawing.Size(125, 20);
            this.txtBookOrUserList.TabIndex = 22;
            this.txtBookOrUserList.Text = "list.txt";
            // 
            // chkFollowScroll
            // 
            this.chkFollowScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFollowScroll.AutoSize = true;
            this.chkFollowScroll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFollowScroll.Location = new System.Drawing.Point(237, 380);
            this.chkFollowScroll.Name = "chkFollowScroll";
            this.chkFollowScroll.Size = new System.Drawing.Size(89, 17);
            this.chkFollowScroll.TabIndex = 23;
            this.chkFollowScroll.Text = "Follow scroll?";
            this.chkFollowScroll.UseVisualStyleBackColor = true;
            // 
            // lblInstanceWarning
            // 
            this.lblInstanceWarning.AutoSize = true;
            this.lblInstanceWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstanceWarning.ForeColor = System.Drawing.Color.Red;
            this.lblInstanceWarning.Location = new System.Drawing.Point(142, 343);
            this.lblInstanceWarning.Name = "lblInstanceWarning";
            this.lblInstanceWarning.Size = new System.Drawing.Size(701, 13);
            this.lblInstanceWarning.TabIndex = 24;
            this.lblInstanceWarning.Text = "Instance numbers only apply to review details crawl. (Books list ALSO applies to " +
    "review details crawl so use complete list.)";
            // 
            // tabCommands
            // 
            this.tabCommands.Controls.Add(this.tabPageBookCommands);
            this.tabCommands.Controls.Add(this.tabPageUserCommands);
            this.tabCommands.Location = new System.Drawing.Point(4, 14);
            this.tabCommands.Name = "tabCommands";
            this.tabCommands.SelectedIndex = 0;
            this.tabCommands.Size = new System.Drawing.Size(131, 347);
            this.tabCommands.TabIndex = 26;
            this.tabCommands.SelectedIndexChanged += new System.EventHandler(this.tabCommands_SelectedIndexChanged);
            // 
            // tabPageUserCommands
            // 
            this.tabPageUserCommands.Controls.Add(this.btnUpdateUserFavAuthorsFromList);
            this.tabPageUserCommands.Controls.Add(this.btnUpdateUserListVotesFromList);
            this.tabPageUserCommands.Controls.Add(this.btnUpdateUserShelvesFromList);
            this.tabPageUserCommands.Controls.Add(this.btnUpdateUserGroupsFromList);
            this.tabPageUserCommands.Controls.Add(this.btnUpdateUserFromList);
            this.tabPageUserCommands.Controls.Add(this.btnGetUserReviewsFromList);
            this.tabPageUserCommands.Location = new System.Drawing.Point(4, 22);
            this.tabPageUserCommands.Name = "tabPageUserCommands";
            this.tabPageUserCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUserCommands.Size = new System.Drawing.Size(123, 321);
            this.tabPageUserCommands.TabIndex = 1;
            this.tabPageUserCommands.Text = "User crawl";
            this.tabPageUserCommands.UseVisualStyleBackColor = true;
            // 
            // btnUpdateUserFavAuthorsFromList
            // 
            this.btnUpdateUserFavAuthorsFromList.Location = new System.Drawing.Point(7, 159);
            this.btnUpdateUserFavAuthorsFromList.Name = "btnUpdateUserFavAuthorsFromList";
            this.btnUpdateUserFavAuthorsFromList.Size = new System.Drawing.Size(110, 38);
            this.btnUpdateUserFavAuthorsFromList.TabIndex = 6;
            this.btnUpdateUserFavAuthorsFromList.Text = "Update users\' favourite authors";
            this.btnUpdateUserFavAuthorsFromList.UseVisualStyleBackColor = true;
            this.btnUpdateUserFavAuthorsFromList.Click += new System.EventHandler(this.btnUpdateUserFavAuthorsFromList_Click);
            // 
            // btnUpdateUserListVotesFromList
            // 
            this.btnUpdateUserListVotesFromList.Enabled = false;
            this.btnUpdateUserListVotesFromList.Location = new System.Drawing.Point(7, 121);
            this.btnUpdateUserListVotesFromList.Name = "btnUpdateUserListVotesFromList";
            this.btnUpdateUserListVotesFromList.Size = new System.Drawing.Size(110, 38);
            this.btnUpdateUserListVotesFromList.TabIndex = 5;
            this.btnUpdateUserListVotesFromList.Text = "Update users\' voted lists";
            this.btnUpdateUserListVotesFromList.UseVisualStyleBackColor = true;
            this.btnUpdateUserListVotesFromList.Click += new System.EventHandler(this.btnUpdateUsersListVotesFromList_Click);
            // 
            // btnUpdateUserShelvesFromList
            // 
            this.btnUpdateUserShelvesFromList.Location = new System.Drawing.Point(7, 83);
            this.btnUpdateUserShelvesFromList.Name = "btnUpdateUserShelvesFromList";
            this.btnUpdateUserShelvesFromList.Size = new System.Drawing.Size(110, 38);
            this.btnUpdateUserShelvesFromList.TabIndex = 4;
            this.btnUpdateUserShelvesFromList.Text = "Update users\' shelves (API)";
            this.btnUpdateUserShelvesFromList.UseVisualStyleBackColor = true;
            this.btnUpdateUserShelvesFromList.Click += new System.EventHandler(this.btnUpdateUserShelvesFromList_Click);
            // 
            // btnUpdateUserGroupsFromList
            // 
            this.btnUpdateUserGroupsFromList.Location = new System.Drawing.Point(7, 45);
            this.btnUpdateUserGroupsFromList.Name = "btnUpdateUserGroupsFromList";
            this.btnUpdateUserGroupsFromList.Size = new System.Drawing.Size(110, 38);
            this.btnUpdateUserGroupsFromList.TabIndex = 3;
            this.btnUpdateUserGroupsFromList.Text = "Update users\' groups (API)";
            this.btnUpdateUserGroupsFromList.UseVisualStyleBackColor = true;
            this.btnUpdateUserGroupsFromList.Click += new System.EventHandler(this.btnUpdateUserGroupsFromList_Click);
            // 
            // btnUpdateUserFromList
            // 
            this.btnUpdateUserFromList.Location = new System.Drawing.Point(7, 7);
            this.btnUpdateUserFromList.Name = "btnUpdateUserFromList";
            this.btnUpdateUserFromList.Size = new System.Drawing.Size(110, 38);
            this.btnUpdateUserFromList.TabIndex = 1;
            this.btnUpdateUserFromList.Text = "Update users\' details (and ticked)";
            this.btnUpdateUserFromList.UseVisualStyleBackColor = true;
            this.btnUpdateUserFromList.Click += new System.EventHandler(this.btnUpdateUserFromList_Click);
            // 
            // btnGetUserReviewsFromList
            // 
            this.btnGetUserReviewsFromList.Location = new System.Drawing.Point(6, 277);
            this.btnGetUserReviewsFromList.Name = "btnGetUserReviewsFromList";
            this.btnGetUserReviewsFromList.Size = new System.Drawing.Size(110, 38);
            this.btnGetUserReviewsFromList.TabIndex = 0;
            this.btnGetUserReviewsFromList.Text = "Get users\' reviews";
            this.btnGetUserReviewsFromList.UseVisualStyleBackColor = true;
            this.btnGetUserReviewsFromList.Click += new System.EventHandler(this.btnGetUserReviewsFromList_Click);
            // 
            // tabPageBookCommands
            // 
            this.tabPageBookCommands.Controls.Add(this.btnUpdateBooksReviewCount);
            this.tabPageBookCommands.Controls.Add(this.btnCrawlList);
            this.tabPageBookCommands.Controls.Add(this.btnCrawlReviews);
            this.tabPageBookCommands.Controls.Add(this.btnUpdateBookShow);
            this.tabPageBookCommands.Controls.Add(this.btnGetReview);
            this.tabPageBookCommands.Controls.Add(this.btnGetUserProfileReviews);
            this.tabPageBookCommands.Location = new System.Drawing.Point(4, 22);
            this.tabPageBookCommands.Name = "tabPageBookCommands";
            this.tabPageBookCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBookCommands.Size = new System.Drawing.Size(123, 321);
            this.tabPageBookCommands.TabIndex = 0;
            this.tabPageBookCommands.Text = "Book crawl";
            this.tabPageBookCommands.UseVisualStyleBackColor = true;
            // 
            // btnUpdateBooksReviewCount
            // 
            this.btnUpdateBooksReviewCount.Location = new System.Drawing.Point(6, 93);
            this.btnUpdateBooksReviewCount.Name = "btnUpdateBooksReviewCount";
            this.btnUpdateBooksReviewCount.Size = new System.Drawing.Size(110, 59);
            this.btnUpdateBooksReviewCount.TabIndex = 20;
            this.btnUpdateBooksReviewCount.Text = "Update details of books (batch /book/review_count/)";
            this.btnUpdateBooksReviewCount.UseVisualStyleBackColor = true;
            this.btnUpdateBooksReviewCount.Click += new System.EventHandler(this.btnUpdateBookReviewCount_Click);
            // 
            // chkOnlyUnprocessed
            // 
            this.chkOnlyUnprocessed.AutoSize = true;
            this.chkOnlyUnprocessed.Location = new System.Drawing.Point(210, 95);
            this.chkOnlyUnprocessed.Name = "chkOnlyUnprocessed";
            this.chkOnlyUnprocessed.Size = new System.Drawing.Size(119, 17);
            this.chkOnlyUnprocessed.TabIndex = 27;
            this.chkOnlyUnprocessed.Text = "Only unproc\'d users";
            this.chkOnlyUnprocessed.UseVisualStyleBackColor = true;
            // 
            // chkLatestPages
            // 
            this.chkLatestPages.AutoSize = true;
            this.chkLatestPages.Enabled = false;
            this.chkLatestPages.Location = new System.Drawing.Point(197, 55);
            this.chkLatestPages.Name = "chkLatestPages";
            this.chkLatestPages.Size = new System.Drawing.Size(134, 17);
            this.chkLatestPages.TabIndex = 28;
            this.chkLatestPages.Text = "inc latest review pages";
            this.chkLatestPages.UseVisualStyleBackColor = true;
            this.chkLatestPages.CheckedChanged += new System.EventHandler(this.chkLatestPage_CheckedChanged);
            // 
            // chkAddFriends
            // 
            this.chkAddFriends.AutoSize = true;
            this.chkAddFriends.Checked = true;
            this.chkAddFriends.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddFriends.Location = new System.Drawing.Point(145, 137);
            this.chkAddFriends.Name = "chkAddFriends";
            this.chkAddFriends.Size = new System.Drawing.Size(107, 17);
            this.chkAddFriends.TabIndex = 29;
            this.chkAddFriends.Text = "(Re)add Friends?";
            this.chkAddFriends.UseVisualStyleBackColor = true;
            this.chkAddFriends.CheckedChanged += new System.EventHandler(this.chkAddFriends_CheckedChanged);
            // 
            // chkAddGenres
            // 
            this.chkAddGenres.AutoSize = true;
            this.chkAddGenres.Enabled = false;
            this.chkAddGenres.Location = new System.Drawing.Point(145, 154);
            this.chkAddGenres.Name = "chkAddGenres";
            this.chkAddGenres.Size = new System.Drawing.Size(107, 17);
            this.chkAddGenres.TabIndex = 30;
            this.chkAddGenres.Text = "(Re)add Genres?";
            this.chkAddGenres.UseVisualStyleBackColor = true;
            this.chkAddGenres.CheckedChanged += new System.EventHandler(this.chkAddGenres_CheckedChanged);
            // 
            // chkAddChallenges
            // 
            this.chkAddChallenges.AutoSize = true;
            this.chkAddChallenges.Checked = true;
            this.chkAddChallenges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddChallenges.Location = new System.Drawing.Point(145, 171);
            this.chkAddChallenges.Name = "chkAddChallenges";
            this.chkAddChallenges.Size = new System.Drawing.Size(125, 17);
            this.chkAddChallenges.TabIndex = 31;
            this.chkAddChallenges.Text = "(Re)add Challenges?";
            this.chkAddChallenges.UseVisualStyleBackColor = true;
            this.chkAddChallenges.CheckedChanged += new System.EventHandler(this.chkAddChallenges_CheckedChanged);
            // 
            // chkAddActivities
            // 
            this.chkAddActivities.AutoSize = true;
            this.chkAddActivities.Enabled = false;
            this.chkAddActivities.Location = new System.Drawing.Point(145, 188);
            this.chkAddActivities.Name = "chkAddActivities";
            this.chkAddActivities.Size = new System.Drawing.Size(115, 17);
            this.chkAddActivities.TabIndex = 32;
            this.chkAddActivities.Text = "(Re)add Activities?";
            this.chkAddActivities.UseVisualStyleBackColor = true;
            this.chkAddActivities.CheckedChanged += new System.EventHandler(this.chkAddActivities_CheckedChanged);
            // 
            // nudLatestReviewPages
            // 
            this.nudLatestReviewPages.Enabled = false;
            this.nudLatestReviewPages.Location = new System.Drawing.Point(145, 54);
            this.nudLatestReviewPages.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLatestReviewPages.Name = "nudLatestReviewPages";
            this.nudLatestReviewPages.Size = new System.Drawing.Size(50, 20);
            this.nudLatestReviewPages.TabIndex = 33;
            this.nudLatestReviewPages.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkGetReviewsSinceLastCrawl
            // 
            this.chkGetReviewsSinceLastCrawl.AutoSize = true;
            this.chkGetReviewsSinceLastCrawl.Checked = true;
            this.chkGetReviewsSinceLastCrawl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGetReviewsSinceLastCrawl.Location = new System.Drawing.Point(197, 72);
            this.chkGetReviewsSinceLastCrawl.Name = "chkGetReviewsSinceLastCrawl";
            this.chkGetReviewsSinceLastCrawl.Size = new System.Drawing.Size(159, 17);
            this.chkGetReviewsSinceLastCrawl.TabIndex = 34;
            this.chkGetReviewsSinceLastCrawl.Text = "only reviews since last crawl";
            this.chkGetReviewsSinceLastCrawl.UseVisualStyleBackColor = true;
            this.chkGetReviewsSinceLastCrawl.CheckedChanged += new System.EventHandler(this.chkGetReviewsSinceLastCrawl_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 422);
            this.Controls.Add(this.chkGetReviewsSinceLastCrawl);
            this.Controls.Add(this.nudLatestReviewPages);
            this.Controls.Add(this.chkAddActivities);
            this.Controls.Add(this.chkAddChallenges);
            this.Controls.Add(this.chkAddGenres);
            this.Controls.Add(this.chkAddFriends);
            this.Controls.Add(this.chkLatestPages);
            this.Controls.Add(this.chkOnlyUnprocessed);
            this.Controls.Add(this.tabCommands);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkFollowScroll);
            this.Controls.Add(this.btnTestAsync);
            this.Controls.Add(this.txtBookOrUserList);
            this.Controls.Add(this.chkOnlyInList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudReviewsToProcess);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudCrawlListTo);
            this.Controls.Add(this.nudCrawlListFrom);
            this.Controls.Add(this.lblCrawlListPages);
            this.Controls.Add(this.lblToProcess);
            this.Controls.Add(this.nudBooksOrUsersToProcess);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.lblInstanceWarning);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMain";
            this.Text = "GoodReads.com Crawler";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudBooksOrUsersToProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrawlListFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCrawlListTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReviewsToProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstanceNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfInstances)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabCommands.ResumeLayout(false);
            this.tabPageUserCommands.ResumeLayout(false);
            this.tabPageBookCommands.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLatestReviewPages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCrawlList;
        private System.Windows.Forms.Button btnCrawlReviews;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnUpdateBookShow;
        private System.ComponentModel.BackgroundWorker bgwUpdateIsbn;
        private System.Windows.Forms.Button btnTestAsync;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.NumericUpDown nudBooksOrUsersToProcess;
        private System.Windows.Forms.Label lblToProcess;
        private System.Windows.Forms.Button btnGetReview;
        private System.Windows.Forms.ToolTip ttipBtnUpdateISBN;
        private System.Windows.Forms.Label lblCrawlListPages;
        private System.Windows.Forms.NumericUpDown nudCrawlListFrom;
        private System.Windows.Forms.NumericUpDown nudCrawlListTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudReviewsToProcess;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudInstanceNum;
        private System.Windows.Forms.NumericUpDown nudNumOfInstances;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGetUserProfileReviews;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkOnlyInList;
        private System.Windows.Forms.TextBox txtBookOrUserList;
        private System.Windows.Forms.CheckBox chkFollowScroll;
        private System.Windows.Forms.Label lblInstanceWarning;
        private System.Windows.Forms.TabControl tabCommands;
        private System.Windows.Forms.TabPage tabPageBookCommands;
        private System.Windows.Forms.TabPage tabPageUserCommands;
        private System.Windows.Forms.Button btnGetUserReviewsFromList;
        private System.Windows.Forms.CheckBox chkOnlyUnprocessed;
        private System.Windows.Forms.Button btnUpdateUserFromList;
        private System.Windows.Forms.CheckBox chkLatestPages;
        private System.Windows.Forms.CheckBox chkAddFriends;
        private System.Windows.Forms.CheckBox chkAddGenres;
        private System.Windows.Forms.CheckBox chkAddChallenges;
        private System.Windows.Forms.CheckBox chkAddActivities;
        private System.Windows.Forms.Button btnUpdateUserGroupsFromList;
        private System.Windows.Forms.Button btnUpdateUserShelvesFromList;
        private System.Windows.Forms.Button btnUpdateUserFavAuthorsFromList;
        private System.Windows.Forms.Button btnUpdateUserListVotesFromList;
        private System.Windows.Forms.NumericUpDown nudLatestReviewPages;
        private System.Windows.Forms.CheckBox chkGetReviewsSinceLastCrawl;
        private System.Windows.Forms.Button btnUpdateBooksReviewCount;
    }
}

