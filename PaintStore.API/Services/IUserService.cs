using PaintStore.Models.Methods;
namespace PaintStore.API.Services;
using PaintStore.API.Dtos; 

public interface IUserService
{
    Task<List<User>> GetPagedAsync(int pageNumber, int pageSize);

    Task<User> GetByIdAsync (int id);

    Task<List<User>> GetByNameAsync (string userName);

    Task<List<User>> GetByEmailAsync (string email);

    Task<User> CreateAsync(UserCreateRequest request);
}
