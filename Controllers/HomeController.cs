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
        //Testing Write/Read from Json
        /*
        Book Book = new Book {
            vcTitle = "Bible",
            vcAuthor = "God",
            vcISBN = "00000000000"
        };
        var Books = new List<Book>();
        Books.Add(Book);
        WriteJson(Books);
        */
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public List<Book> ReadJson()
    {

        string BooksJson = System.IO.File.ReadAllText(filepath);
        List<Book> Books = JsonSerializer.Deserialize<List<Book>>(BooksJson);
        foreach (var Book in Books)
        {
            Console.WriteLine(Book.ToString());
        }
        return Books;
    }

    public void WriteJson(List<Book> NewBooks)
    {
        try
        {

            //Getting the inital catalog to add the new books to it
            IEnumerable<Book> catalog = ReadJson();

            //Combine the new books with the existing ones but exclude duplicates
            IEnumerable<Book> Books = catalog.Union(NewBooks);

            if (Books != null)
            {
                string JsonCatalog = JsonSerializer.Serialize(Books);
                System.IO.File.WriteAllText(filepath, JsonCatalog);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
