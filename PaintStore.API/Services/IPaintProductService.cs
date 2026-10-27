using PaintStore.Models.Methods;


namespace PaintStore.API.Services;

public interface IPaintProductService
{
    Task<PaintProduct> GetPaintProductById(int id);
    
}
