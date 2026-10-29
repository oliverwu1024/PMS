using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using PaintStore.Models.Methods;


namespace PaintStore.API.Repos;


public class UserRepository: IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository (AppDbContext context)
    {
        _context = context;
    }
    

    public async Task<List<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Users.OrderBy(u=>u.UserId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<User?> GetByIdAsync (int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u =>u.UserId ==id);
    }

    public async Task<List<User>> GetByNameAsync (string userName)
    {
        return await _context.Users.Where(u =>u.UserName ==userName).ToListAsync();
    }

    public async Task<List<User>> GetByEmailAsync (string email)
    {
        return await _context.Users.Where(u =>u.UserEmail ==email).ToListAsync();
    }

    public async Task<User> AddAsync (User user)
    {
        _context.Users.Add(user);          
        await _context.SaveChangesAsync();   
        return user;                      

    }
}




