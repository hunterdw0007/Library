﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Text.Json;
using System;
using System.IO;
using System.Text;

namespace Library.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //This variable holds the path to the file which holds the book catalog
    private readonly string filepath = "Catalog.json";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var vm = new CatalogViewModel()
        {
            Books = ReadJson()
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult AddBook()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddBook(string Title, string Author, string ISBN)
    {
        try
        {
            //Validation
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

            //Create a book
            Book book = new Book()
            {
                uID = Guid.NewGuid(),
                vcTitle = Title,
                vcAuthor = Author,
                vcISBN = ISBN
            };
            WriteJson(book);
            return Json(new { success = true, title = "Success", message = "Book Successfully Added!" });
        }
        catch (Exception)
        {
            return Json(new { success = false, title = "Failure", message = "Something went wrong when adding the book. Check the input again." });
        }
    }

    [HttpGet]
    public IActionResult DeleteBook(){
        var vm = new CatalogViewModel()
        {
            Books = ReadJson()
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
            List<Book> Books = ReadJson();

            Books = Books.Where(m => m.uID != bookID).ToList();

            string JsonCatalog = JsonSerializer.Serialize(Books);
            System.IO.File.WriteAllText(filepath, JsonCatalog);
            return Json(new{success = true, title = "Successfully Deleted", message = "The Book has been deleted."});
        }
        catch (Exception)
        {
            return Json(new{success = false, title = "Error", message = "Something went wrong when deleting the book."});
        }
    }

    /*
        The next two methods take care of reading and writing to the Json that stores the book catalog.
        It's pretty scuffed but it works for our purposes.
    */
    public List<Book> ReadJson()
    {
        try
        {
            string BooksJson = System.IO.File.ReadAllText(filepath);
            List<Book> Books = JsonSerializer.Deserialize<List<Book>>(BooksJson) ?? new List<Book>();

            //Writes them to console for testing purposes
            /* foreach (var Book in Books)
            {
                Console.WriteLine(Book.ToString());
            } */

            return Books;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void WriteJson(Book NewBook)
    {
        try
        {
            //Getting the inital catalog to add the new books to it
            IEnumerable<Book> catalog = ReadJson();

            //Combine the new books with the existing ones but exclude duplicates
            IEnumerable<Book> Books = catalog.Append(NewBook);

            string JsonCatalog = JsonSerializer.Serialize(Books);
            System.IO.File.WriteAllText(filepath, JsonCatalog);

        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
