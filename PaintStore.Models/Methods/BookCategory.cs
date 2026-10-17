namespace PaintStore.Models.Methods;

public class BookCategory
{
    public int BookCategoryId {get; set;}

    public string Name {get; set;}

    public ICollection<Book> Books {get; set;} =  new List<Book> ();
}
