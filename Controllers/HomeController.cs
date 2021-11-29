using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Text.Json;
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace Library.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //This variable holds the path to the file which holds the book catalog
    private readonly string catalogpath = "Catalog.json";
    private readonly string accountpath = "Account.json";

    //This variable holds the id of who is logged in
    static volatile public Account CurrentlyLoggedIn = new Account();

    //This variable holds the books that are in the cart for when they are going to be checked out
    static volatile public List<Book> CartContents = new List<Book>();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string email, string password)
    {
        List<Account> Accounts = ReadAccount();

        // This is incredibly scuffed but something about saving it to a JSON makes it necessary to convert them to ASCII strings to compare them
        if (Accounts.Any(m => m.vcEmailAddress == email && Encoding.ASCII.GetString(m.vcPassword) == Encoding.ASCII.GetString(SHA256.HashData(Encoding.ASCII.GetBytes(password)))))
        {
            CurrentlyLoggedIn = Accounts.FirstOrDefault(m => m.vcEmailAddress == email && Encoding.ASCII.GetString(m.vcPassword) == Encoding.ASCII.GetString(SHA256.HashData(Encoding.ASCII.GetBytes(password))));
            Console.WriteLine(CurrentlyLoggedIn.uID.ToString());
            return RedirectToAction("Catalog");
        }
        else
        {
            ViewBag.Error = true;
            return View("Index", ViewBag);
        }
    }

    [HttpGet]
    public IActionResult CreateAccount()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateAccount(string First, string Middle, string Last, string Email, string Address, string Password)
    {
        Account customer = new Customer()
        {
            uID = Guid.NewGuid(),
            bIsLocked = false,
            vcFirstName = First,
            vcMiddleMaidenName = Middle ?? "",
            vcLastName = Last,
            vcEmailAddress = Email,
            vcAddress = Address,
            vcPassword = SHA256.HashData(Encoding.ASCII.GetBytes(Password))
        };
        WriteAccount(customer);
        ViewBag.Created = true;
        return View("Index", ViewBag);
    }

    public IActionResult Catalog()
    {
        var vm = new CatalogViewModel()
        {
            Books = ReadCatalog().OrderBy(m => m.vcTitle).ToList()
        };
        CartContents = new List<Book>();
        return View(vm);
    }

    [HttpGet]
    public IActionResult AddBook()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddBook(string Title, string Author, string ISBN, string PublishedDate, int QuantityAvailable, string Status)
    {
        try
        {
            //Validation and Parsing Dates
            if (String.IsNullOrWhiteSpace(Title))
            {
                return Json(new { success = false, title = "Title Error", message = "Title is null or whitespace. Check the input again." });

            }
            if (String.IsNullOrWhiteSpace(Author))
            {
                return Json(new { success = false, title = "Author Error", message = "Author is null or whitespace. Check the input again." });

            }
            if (String.IsNullOrWhiteSpace(ISBN))
            {
                return Json(new { success = false, title = "ISBN Error", message = "ISBN is null or whitespace. Check the input again." });

            }
            DateTime PubDate = new DateTime();
            if (!DateTime.TryParseExact(PublishedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out PubDate))
            {
                return Json(new { success = false, title = "Published Date Error", message = "Published Date is Invalid. Check the input again." });

            }

            //Create a book
            Book book = new Book()
            {
                uID = Guid.NewGuid(),
                vcTitle = Title,
                vcAuthor = Author,
                vcISBN = ISBN,
                dtPublishedDate = PubDate,
                dtDueDate = new DateTime(),
                iQuantityAvailable = QuantityAvailable,
                vcStatus = Status
            };
            WriteCatalog(book);
            return Json(new { success = true, title = "Success", message = "Book Successfully Added!" });
        }
        catch (Exception)
        {
            return Json(new { success = false, title = "Failure", message = "Something went wrong when adding the book. Check the input again." });
        }
    }

    [HttpGet]
    public IActionResult DeleteBook()
    {
        var vm = new CatalogViewModel()
        {
            Books = ReadCatalog()
        };
        return View(vm);
    }

    /**
    * This method takes in a bookID, finds it in the Catalog, deletes it and rewrites the catalog.
    * This probably isn't the most efficient way but it's my way so whatever.s
    */
    [HttpPost]
    public IActionResult DeleteBook(Guid bookID)
    {
        try
        {
            List<Book> Books = ReadCatalog();

            Books = Books.Where(m => m.uID != bookID).ToList();

            string JsonCatalog = JsonSerializer.Serialize(Books);
            System.IO.File.WriteAllText(catalogpath, JsonCatalog);
            return Json(new { success = true, title = "Successfully Deleted", message = "The Book has been deleted." });
        }
        catch (Exception)
        {
            return Json(new { success = false, title = "Error", message = "Something went wrong when deleting the book." });
        }
    }

    [HttpGet]
    public IActionResult Cart(string cart)
    {
        try
        {
            List<Book> Books = ReadCatalog();
            List<Book> BooksInCart = new List<Book>();
            if (cart != null)
            {
                String[] cartGuids = cart.Split(',');

                Guid x;
                foreach (String item in cartGuids)
                {
                    Guid.TryParse(item, out x);
                    BooksInCart.Add(Books.FirstOrDefault(m => m.uID == x));
                }
            }
            Cart vm = new Cart()
            {
                Books = BooksInCart,
                Customer = CurrentlyLoggedIn
            };
            CartContents = BooksInCart;
            return View(vm);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    // This method handles checking out books and then redirects the user to their checked out books
    public IActionResult CheckOut()
    {
        try
        {
            List<Book> Catalog = ReadCatalog();
            foreach (Book book in CartContents)
            {
                book.uCheckedOutBy = CurrentlyLoggedIn.uID;
                book.vcStatus = "Checked Out";
                book.dtDueDate = DateTime.Now.AddDays(14);
                WriteCatalog(book);
            }

            return RedirectToAction("MyBooks");
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    [HttpGet]
    public IActionResult ReturnBook(){
        CatalogViewModel vm = new CatalogViewModel(){
            Books = ReadCatalog().Where(m=> m.uCheckedOutBy == CurrentlyLoggedIn.uID).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult ReturnBook(Guid BookID){
        try
        {
            List<Book> Books = ReadCatalog();

            Book book = Books.FirstOrDefault(m => m.uID == BookID);
            book.uCheckedOutBy = Guid.Empty;
            book.vcStatus = "Available";
            WriteCatalog(book);
            return Json(new { success = true, title = "Successfully Returned", message = "The Book has been returned." });
        }
        catch (Exception)
        {
            return Json(new { success = false, title = "Error", message = "Something went wrong when returning the book." });
        }
    }

    public IActionResult MyBooks()
    {
        Console.WriteLine(CurrentlyLoggedIn.uID.ToString());
        Cart vm = new Cart(){
            Books = ReadCatalog().Where(m => m.uCheckedOutBy == CurrentlyLoggedIn.uID).ToList(),
            Customer = CurrentlyLoggedIn
        };
        return View(vm);
    }

    /*
        The next two methods take care of reading and writing to the Json that stores the book catalog.
        It's pretty scuffed but it works for our purposes.
    */
    public List<Book> ReadCatalog()
    {
        try
        {
            string BooksJson = System.IO.File.ReadAllText(catalogpath);
            List<Book> Books = JsonSerializer.Deserialize<List<Book>>(BooksJson) ?? new List<Book>();

            return Books;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void WriteCatalog(Book NewBook)
    {
        try
        {
            //Getting the inital catalog to add the new books to it
            //This also checks whether the book we want already exists then it is removed from the catalog and overwritten
            IEnumerable<Book> catalog = ReadCatalog().Where(m => m.uID != NewBook.uID);

            IEnumerable<Book> Books = catalog.Append(NewBook);

            Console.WriteLine(NewBook.ToString());

            string JsonCatalog = JsonSerializer.Serialize(Books);
            System.IO.File.WriteAllText(catalogpath, JsonCatalog);

        }
        catch (Exception e)
        {
            throw e;
        }
    }

    /*
            The next two methods take care of reading and writing to the Json that stores the book catalog.
            It's pretty scuffed but it works for our purposes.
        */
    public List<Account> ReadAccount()
    {
        try
        {
            string AccountsJson = System.IO.File.ReadAllText(accountpath);
            List<Account> Accounts = JsonSerializer.Deserialize<List<Account>>(AccountsJson) ?? new List<Account>();

            return Accounts;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void WriteAccount(Account newAccount)
    {
        try
        {
            //Getting the inital accounts to add the new accounts to it
            IEnumerable<Account> accounts = ReadAccount();

            IEnumerable<Account> Accounts = accounts.Append(newAccount);

            Console.WriteLine(newAccount.ToString());

            string JsonAccount = JsonSerializer.Serialize(Accounts);
            System.IO.File.WriteAllText(accountpath, JsonAccount);

        }
        catch (Exception e)
        {
            throw e;
        }
    }


}
