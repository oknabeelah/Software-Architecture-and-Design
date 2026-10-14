using System;
	namespace SAaD; 
		class Program 
		{ 
			static void Main() 
			{ 
				//create a new library object
                Library myLibrary = new Library("My Library");

                // add books to the library ie make new book objects and add
                Book book1 = new Book("The Great Gatsby", "978-0-7432-7356-5", true);
                Book book2 = new Book("To Kill a Mockingbird", "978-0-06-112008-4", true);
                myLibrary.addBook(book1);
                myLibrary.addBook(book2);

                myLibrary.listAllBooks();

                //add members to the library ie make new member objects and add
                Member member1 = new Member("M001", "Alice");
                Member member2 = new Member("M002", "Bob");
                myLibrary.registerMember(member1);
                myLibrary.registerMember(member2);

                myLibrary.listAllMembers();

                //borrow a book
                myLibrary.borrowBook("978-0-7432-7356-5", member1);
                
                // return a book
                myLibrary.returnBook("978-0-7432-7356-5");

                
			} 
		}
        class Library
        {
            private string name;
            private List<Book> books;
            private List<Member> members;
            public Library(string name)
            {
                this.name = name;
                this.books = new List<Book>();
                this.members = new List<Member>();
            }
            public string getName() => name;
            public void addBook(Book book)
            {
                if (book != null && !books.Contains(book))
                {
                    books.Add(book);
                    Console.WriteLine($"Book '{book.getTitle()}' has been added to {name}");
                }
            }
           
            public void registerMember(Member member)
            {
                if (member != null && !members.Contains(member))
                {
                    members.Add(member);
                    Console.WriteLine($"Member '{member.getName()}' has been registered to {name}");
                }
            }
            public void borrowBook(string isbn, Member member)
            {
                if (!members.Contains(member))
                {
                    Console.WriteLine($"Member '{member.getName()}' is not registered in {name}");
                    return;
                }

                Book? book = books.Find(b => b.getIsbn() == isbn);
                if (book == null)
                {
                    Console.WriteLine($"Book with ISBN '{isbn}' not found in {name}");
                    return;
                }

                if (!book.isAvailable())
                {
                    Console.WriteLine($"Book '{book.getTitle()}' is not available for borrowing.");
                    return;
                }

                book.Borrow();
                Console.WriteLine($"Book '{book.getTitle()}' has been borrowed by {member.getName()}");
            }
            public void returnBook(string isbn)
            {
                //return a book to the library
                Book? book = books.Find(b => b.getIsbn() == isbn);
                if (book == null)
                {
                    Console.WriteLine($"Book with ISBN '{isbn}' not found in {name}");
                    return;
                }

                if (book.isAvailable())
                {
                    Console.WriteLine($"Book '{book.getTitle()}' is already available in the library.");
                    return;
                }

                book.returnBook();
                Console.WriteLine($"Book '{book.getTitle()}' has been returned to {name}");
            }
            public void listAllBooks()
            {
                //list all available books in the library
                foreach (Book book in books)
                {
                    Console.WriteLine($"{book}");
                }
            }
            public void listAllMembers()
            {
                //list all registered members in the library
                foreach (Member member in members)
                {
                    Console.WriteLine(member.ToString());
                }
            }
        }
        class Book
        {
            private string title { get; set; }
            private string isbn { get; set; }
            private bool available { get; set; }
            public Book( string title, string isbn, bool available)
            {
                this.title = title;
                this.isbn = isbn;
                this.available = available;
            }
            public string getTitle() => title;
            public string getIsbn() => isbn;
            public bool isAvailable() => available;
            public void Borrow()
            {
                //borrow a book from the library
                if (!available)
                {
                    //Console.WriteLine($"Book '{title}' is not available for borrowing.");
                    throw new InvalidOperationException($"Book '{title}' is not available for borrowing.");
                }
                available = false;
            }
            public void borrow() => Borrow();
            public void returnBook()
            {
                //return a book to the library
                if (available)
                {
                    //Console.WriteLine($"Book '{title}' is already available in the library.");
                    throw new InvalidOperationException($"Book '{title}' is already available in the library.");
                }
                available = true;
            }
            public override string ToString()
            {
                string status  = available ? "Yes" : "No";
                return $"Title: {title}, ISBN: {isbn}, Available: {available}";
            }
}
        class Member
        {
            private string memberId { get; set; }
            private string name { get; set; } //change to ...
            public Member(string memberId, string name)
            {
                this.memberId = memberId;
                this.name = name;
            }

            public string getMemberId() => memberId;
            public string getName() => name;

            public override string ToString()
            {
                return $"Member ID: {memberId}, Name: {name}";
            }
        
        }