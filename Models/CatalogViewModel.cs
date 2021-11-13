using System;
using System.Collections.Generic;
using System.Web;

namespace Library.Models
{
    public class CatalogViewModel
    {
        public List<Book> Books { get; set; }
    }

    public class Book
    {
        public Guid uID { get; set; }
        public string vcTitle { get; set; }
        public string vcAuthor { get; set; }
        public string vcISBN { get; set; }

        public string ToString()
        {
            return this.vcTitle + " " + this.vcAuthor + " " + this.vcISBN;
        }
    }
}