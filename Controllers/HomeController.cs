using System.Diagnostics;
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
    private string filepath = "Catalog.json";

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

    public IActionResult BookForm()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddBook(Book book)
    {
        try
        {
            WriteJson(book);
            return Json(new {success = true, title = "Success", message = "Book Successfully Added!"});
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
    public List<Book> ReadJson()
    {
        try
        {
            string BooksJson = System.IO.File.ReadAllText(filepath);
            List<Book> Books = JsonSerializer.Deserialize<List<Book>>(BooksJson);

            //Writes them to console for testing purposes - does not effect the site working
            foreach (var Book in Books)
            {
                Console.WriteLine(Book.ToString());
            }
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
