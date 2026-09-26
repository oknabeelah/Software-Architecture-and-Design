using System;
namespace SAaD;

class Book
{
    String title;
    String author;
    String isbn;
    static void Main()
    {
        // Create a new book object
        Book book = new Book("1984", "George Orwell", "1234");
        //display info
        book.displayInfo();
        //get title and author
        book.getTitle();
        book.getAuthor();
        // print details
        book.displayInfo();
    }

    public Book(String title, String author, String isbn)
    {
        this.title = title;
        this.author = author;
        this.isbn = isbn;
    }
    public String getTitle()
    {
        return title;
    }
    public String getAuthor()
    {
        return author;
    }
    public String getIsbn()
    {
        return isbn;
    }
    public void displayInfo()
    {
        Console.WriteLine("Title: " + title);
        Console.WriteLine("Author: " + author);
        Console.WriteLine("ISBN: " + isbn);
    }

}