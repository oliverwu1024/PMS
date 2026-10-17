namespace PaintStore.Models.Methods;

public class Publisher
{
    public int Id {get; set;}

    public string Name {get;set;}

    //Navigation Property
    public ICollection<Book> Books {get;set;} = new List<Book>();
}
