
using Microsoft.AspNetCore.Mvc;

using PaintStore.API.Dtos;

using PaintStore.API.Services;



namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request) 
    {



        var user = await _userService.CreateAsync(request);

        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user.ToDto());


    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTO>> GetUser(int id) 
    {
        //假设从数据库获取了一个用户实体
        var user = await _userService.GetByIdAsync(id);

        //将user转换为UserDTO并返回

        return Ok(user.ToDto());

    }







    [HttpGet]  
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {


        var users = await _userService.GetPagedAsync(pageNumber, pageSize);
        return Ok(users.Select(u => u.ToDto()));
    }



    [HttpGet("username/{userName}")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserByName (string userName)
    {
        var users =  await _userService.GetByNameAsync(userName);
        return Ok(users.Select(u => u.ToDto()));
    }

    [HttpGet("usermail/{userMail}")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserByEmail (string userMail)
    {
        var users = await _userService.GetByEmailAsync(userMail);
        return Ok(users.Select(u => u.ToDto()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(int id, [FromBody] UserUpdateRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);
        return Ok(user.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();   // 204
    }
    




}
