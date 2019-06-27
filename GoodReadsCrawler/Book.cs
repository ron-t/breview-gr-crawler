using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace GoodReadsCrawler
{
    public partial class Book : EntityObject
    {
        public string getShortTitle()
        {
            string shortTitle = this.title.Length > 15 ? this.title.Substring(0, 15) + "..." : this.title;
            return shortTitle + "[" + this.id + "]";
        }
    }
}
