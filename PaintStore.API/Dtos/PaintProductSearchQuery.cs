using PaintStore.Models.Enums;

namespace PaintStore.API.Dtos;

public class PaintProductSearchQuery
{
    
    public string? Name {get;set;}
    public Brand? Brand {get;set;}
    public PaintType? Type {get;set;}
    public string? Color {get;set;}
    public decimal? MinPrice {get;set;}
    public decimal? MaxPrice {get;set;}

}
