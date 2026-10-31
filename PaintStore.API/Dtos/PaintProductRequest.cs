using System.ComponentModel.DataAnnotations;
using PaintStore.Models.Enums;

namespace PaintStore.API.Dtos;

public class PaintProductRequest
{

    [Required]
    [StringLength(50)]
    public string Name {get;set;}

    [Required]
    [EnumDataType(typeof(Brand))]
    public Brand? BrandName {get;set;}

    public PaintType Type {get;set;}

    [Required]
    public string Color {get;set;}

    [Range(1, 1000)]
    public int SizeInLiters {get;set;}

    [Range(0.01, 100000)]
    public decimal Price {get;set;}

    [Range(0, int.MaxValue)]
    public int Stock {get; set;}

    public int SeriesId {get;set;}

}
