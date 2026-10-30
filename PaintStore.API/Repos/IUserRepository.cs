using PaintStore.Models.Methods;

namespace PaintStore.API.Repos;

public interface IUserRepository
{
    Task<List<User>> GetPagedAsync(int pageNumber, int pageSize);

    Task<User?> GetByIdAsync (int id);

    Task<List<User>> GetByNameAsync (string userName);

    Task<List<User>> GetByEmailAsync (string email);

    Task<User> AddAsync (User user);

    Task UpdateAsync (User user);

    Task DeleteAsync (User user);
}
