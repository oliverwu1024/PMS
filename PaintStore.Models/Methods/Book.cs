namespace PaintStore.Models.Methods;

public class Book
{
    public int BookId { get; set; }

    public string Author { get; set; }

    public DateTime CreatedAt {get; set;}

    public string Description {get; set;}

    public Publisher Publisher {get; set;}

    public int PublisherId {get; set;}

    public ICollection<BookCategory> BookCategories {get; set;} = new List<BookCategory> (); 


}
