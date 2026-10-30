using PaintStore.API.Repos;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos; 
namespace PaintStore.API.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;


    public UserService (IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }



    public async Task<List<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            _logger.LogWarning("Invalid paging: page {PageNumber}, size {PageSize}", pageNumber, pageSize);
            throw new ArgumentException("Page number and page size must be greater than 0.");

        }  
        var users = await _userRepository.GetPagedAsync(pageNumber, pageSize);
        return users;
    }

    public async Task<User> GetByIdAsync (int id)
    {
        var users = await _userRepository.GetByIdAsync(id);

        if (users == null)
        {   
            _logger.LogWarning("User with ID {Id} not found", id); 
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        return users;
    }

    public async Task<List<User>> GetByNameAsync (string userName)
    {
        var users = await _userRepository.GetByNameAsync(userName);

        if(users.Count == 0)
        {
            _logger.LogWarning("No users found for username {UserName}", userName);
            throw new KeyNotFoundException("No users related to the user name is found");
        }

        return users;
        
    }

    public async Task<List<User>> GetByEmailAsync (string email)
    {
        var users = await _userRepository.GetByEmailAsync(email);

        if(users.Count == 0)
        {
            _logger.LogWarning("No users found for email {Email}", email);
            throw new KeyNotFoundException("No users related to the email is found");
        }

        return users;
        
    }

    public async Task<User> CreateAsync(UserCreateRequest request)
    {

        var user = new User(request.UserName, request.UserEmail, request.UserPhone, request.Age, userId: 0);
        user = await _userRepository.AddAsync(user);
        _logger.LogInformation("User {UserId} created", user.UserId);
        return user;
    }


    public async Task<User> UpdateAsync (int id, UserUpdateRequest request)
    {
        var user = await GetByIdAsync(id);

        user.UserName = request.UserName;
        user.UserEmail = request.UserEmail;
        user.UserPhone = request.UserPhone;
        user.Age= request.Age;

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("User {UserId} updated", id);
        return user;
    }

    public async Task DeleteAsync (int id)
    {
        var user = await GetByIdAsync(id);

        await _userRepository.DeleteAsync(user);

        _logger.LogInformation("User {UserId} deleted", id);
    }



}
