using System;
	namespace SAaD; 
		class Program 
		{ 
			static void Main() 
			{ 
				//create a new library object
                Library myLibrary = new Library("My Library");

                // add books to the library ie make new book objects and add
                Book book1 = new Book("My Library", "The Great Gatsby", "978-0-7432-7356-5", true);
                Book book2 = new Book("My Library", "To Kill a Mockingbird", "978-0-06-112008-4", true);
                myLibrary.addBook(book1);
                myLibrary.addBook(book2);

                myLibrary.listAllBooks();

                //add members to the library ie make new member objects and add
                Member member1 = new Member("My Library", "M001", "Alice");
                Member member2 = new Member("My Library", "M002", "Bob");
                myLibrary.registerMember(member1);
                myLibrary.registerMember(member2);

                myLibrary.listAllMembers();

                //borrow a book
                // take in the isbn of the book and the member object and call the borrowBook method in the library class
                    // check if available is true, if so, set available to false and print a message that the book has been borrowed
                
                // return a book
                //prevent borrowing if the book is not available
			} 
		}
        class Library
        {
            public string name { get; set; }
            public Library(string name)
            {
                this.name = name;
            }
            public List<Book> allBooks { get; set; } = new List<Book>();
            public List<Member> allMembers { get; set; } = new List<Member>();
            public void addBook(Book book)
            {
                //add book to the library
                this.allBooks.Add(book);
                Console.WriteLine($"Book '{book.title}' has been added to {name}");
            }
            public void registerMember(Member member)
            {
                //register a new member
                // create a list of members in the library class and add the member to the list
                this.allMembers.Add(member);
                Console.WriteLine($"Member '{member.name}' has been registered to {name}");

            }
            public void borrowBook(string isbn, Member member)
            {
                //borrow a book from the library
                //add to a borrowed book dictionary containing the book and the member who borrowed it ?
                if (this.allBooks.Any(b => b.isbn == isbn && b.available))
                {
                    Book bookToBorrow = this.allBooks.First(b => b.isbn == isbn);
                    bookToBorrow.available = false;
                    Console.WriteLine($"Book '{bookToBorrow.title}' has been borrowed by {member.name}");
                }
                else
                {
                    //Console.WriteLine($"Book with ISBN '{isbn}' is not available for borrowing.");
                    Console.WriteLine($"{this.allBooks.First(b => b.isbn == isbn).title} is not available for borrowing.");
                }
            }
            public void returnBook(string isbn)
            {
                //return a book to the library
            }
            public void listAllBooks()
            {
                //list all available books in the library
                foreach (Book book in this.allBooks)
                {
                    Console.WriteLine($"Title: {book.title}, ISBN: {book.isbn}, Available: {book.available}");
                }
            }
            public void listAllMembers()
            {
                //list all registered members in the library
                foreach (Member member in this.allMembers)
                {
                    Console.WriteLine($"Member ID: {member.memberId}, Name: {member.name}");
                }
            }
        }
        class Book : Library
        {
            public string title { get; set; }
            public string isbn { get; set; }
            public bool available { get; set; }
            public string libraryName { get; set; }
            public Book( string libraryName, string title, string isbn, bool available) : base(libraryName)
            {
                this.libraryName = libraryName;
                this.title = title;
                this.isbn = isbn;
                this.available = available;
            }
            public void borrow()
            {
                //borrow a book from the library
                
            }
            public void returnBook()
            {
                //return a book to the library
            }
}
        class Member : Library
        {
            public string libraryName { get; set; }
            public string memberId { get; set; }
            public string name { get; set; } //change to ...
            public Member(string libraryName, string memberId, string name) : base(libraryName)
            {
                this.libraryName = libraryName;
                this.memberId = memberId;
                this.name = name;
            }
        
        }