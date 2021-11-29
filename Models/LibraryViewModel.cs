using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;

namespace Library.Models
{
    public class Account
    {
        public Guid uID { get; set; }
        public Boolean bIsLocked { get; set; }
        public string? vcFirstName { get; set; }
        public string? vcMiddleMaidenName { get; set; }
        public string? vcLastName { get; set; }
        public string? vcEmailAddress { get; set; }
        //This holds the hash of the password not the password itself
        public byte[]? vcPassword { get; set; }
        public string? vcAddress { get; set; }
    }

    public class Librarian : Account
    {
        public int iEmployeeID { get; set; }
    }

    public class Customer : Account
    {
        public int iCustomerID { get; set; }
    }

    public class Cart
    {
        public List<Book> Books { get; set; }
        public static int MAX_ITEM_LIMIT { get; } = 10;
        public Account Customer { get; set; }
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
        public Guid uCheckedOutBy { get; set; }

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
            + this.vcStatus
            + "\nChecked Out By:\t"
            + this.uCheckedOutBy;
        }
    }
}