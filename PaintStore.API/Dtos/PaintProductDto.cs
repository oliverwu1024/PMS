namespace PaintStore.API.Dtos;

//This is a response dto for a paint product
// only the fields the client needs, enums sent as text, specification flattened
public class PaintProductDto
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Brand {get;set;} = "";
    public string Type {get;set;} = "";
    public string Color {get;set;} = "";
    public int SizeInLiters {get;set;}
    public decimal Price {get;set;}
}
