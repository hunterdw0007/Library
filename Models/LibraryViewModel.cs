using System;
using System.Collections.Generic;
using System.Web;

namespace Library.Models
{
    public class Account
    {

    }

    public class Librarian : Account
    {

    }

    public class Customer : Account
    {

    }

    public class Cart{

    }

    public class BookInformation
    {
        public string? vcTitle { get; set; }
        public string? vcAuthor { get; set; }
        public string? vcISBN { get; set; }
        public DateTime dtPublishedDate { get; set; }
        public int iQuantityAvailable { get; set; }
    }

    public class Book : BookInformation
    {
        public Guid uID { get; set; }
        public DateTime dtDueDate { get; set; }
        public string? vcStatus { get; set; }

        public override string ToString()
        {
            return "ID:\t\t" 
            + this.uID 
            + "\nTitle:\t\t" 
            + this.vcTitle
            + "\nAuthor:\t\t" 
            + this.vcAuthor
            + "\nISBN:\t\t" 
            + this.vcISBN
            + "\nPub Date:\t" 
            + this.dtPublishedDate
            + "\nQuantity:\t" 
            + this.iQuantityAvailable
            + "\nDue Date:\t" 
            + this.dtDueDate
            + "\nStatus:\t\t" 
            + this.vcStatus;
        }
    }
}